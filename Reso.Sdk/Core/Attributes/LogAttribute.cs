using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Reso.Sdk.Core.Attributes
{
    /// <summary>
    /// # Config log request and response attribute <br/>
    /// - Add "LogAttributeOrigins" into appsettings.json <br/>
    /// - Add into Startup.cs 
    /// app.Use((context, next) =>
    /// {
    ///     context.Request.EnableBuffering();
    ///     return next();
    /// }); <br/>
    /// - Put [Log] above the controller method
    /// <example>
    ///  With specific origin:
    ///     <code>
    ///          "LogAttributeOrigins":"origin1,origin2,..."
    ///     </code>
    /// With all origin:
    ///     <code>
    ///          "LogAttributeOrigins":"AllOrigins"
    ///     </code>
    /// </example>
    /// </summary>
    public class LogAttribute : Attribute, IAsyncActionFilter
    {
        private static readonly IConfiguration Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .Build();

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var origin = context.HttpContext.Request.Headers["Origin"].ToString() == ""
                ? "AllOrigins"
                : context.HttpContext.Request.Headers["Origin"].ToString();

            if (CheckLogOrigin(origin))
            {
                var request = await FormatRequest(context.HttpContext.Request);

                var executedContext = await next();

                string response = "";
                if (executedContext.Result is ObjectResult result)
                {
                    response = $"{executedContext.HttpContext.Response.StatusCode}: {JsonConvert.SerializeObject(result.Value)}";
                }

                await Utilities.LogUtils.SendLog(136,
                        "[RequestResponse] Origin: " + origin +
                        "\n||  Request: " + request +
                        "\n|| Response: " + response)
                    .ConfigureAwait(false);
            }
            else
            {
                await next();
            }
        }


        private async Task<string> FormatRequest(HttpRequest request)
        {
            request.Body.Position = 0;

            using var streamReader = new StreamReader(request.Body);
            string bodyContent = await streamReader.ReadToEndAsync();

            request.Body.Position = 0;

            return
                $"{request.Method} {request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyContent}";
        }

        private bool CheckLogOrigin(string origin)
        {
            var origins = Configuration.GetValue<string>("LogAttributeOrigins");
            if (origins != string.Empty)
            {
                List<string> listOrigins = origins.Split(',').ToList();
                if (listOrigins.Any(x => x.Equals(origin)) || listOrigins.Any(x => x.Equals("AllOrigins")))
                {
                    return true;
                }
            }

            return false;
        }
    }
}