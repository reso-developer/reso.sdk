using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Reso.Sdk.Core.Middlewares
{
    /// <summary>
    /// # Config log request and response middlewares <br/>
    /// - Add "LogMiddlewareOrigins" into appsettings.json <br/>
    /// - Add into Startup.cs
    /// app.UseMiddleware&lt;RequestResponseMiddleware&gt;(); 
    /// <example>
    ///  With specific origin:
    ///     <code>
    ///          "LogMiddlewareOrigins":"origin1,origin2,..."
    ///     </code>
    /// With all origin:
    ///     <code>
    ///          "LogMiddlewareOrigins":"AllOrigins"
    ///     </code>
    /// </example>
    /// </summary>
    public class RequestResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public RequestResponseMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            var origin = context.Request.Headers["Origin"].ToString() == ""
                ? "AllOrigins"
                : context.Request.Headers["Origin"].ToString();

            if (CheckLogOrigin(origin))
            {
                //First, get the incoming request
                var request = await FormatRequest(context.Request);

                //Copy a pointer to the original response body stream
                var originalBodyStream = context.Response.Body;

                //Create a new memory stream...
                using (var responseBody = new MemoryStream())
                {
                    //...and use that for the temporary response body
                    context.Response.Body = responseBody;

                    //Continue down the Middleware pipeline, eventually returning to this class
                    await _next(context);

                    //Format the response from the server
                    var response = await FormatResponse(context.Response);
                    
                    await Utilities.LogUtils.SendLog(136,
                            "[RequestResponse] Origin: " + origin +
                            "\n||  Request: " + request +
                            "\n|| Response: " + response)
                        .ConfigureAwait(false);

                    //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            else
            {
                await _next(context);
            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            //This line allows us to set the reader for the request back at the beginning of its stream.
            request.EnableBuffering();

            //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            //...Then we copy the entire request stream into the new buffer.
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            //We convert the byte[] into a string using UTF8 encoding...
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Position = 0;
            //..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()

            return $"{request.Method} {request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            //We need to read the response stream from the beginning...
            response.Body.Seek(0, SeekOrigin.Begin);

            //...and copy it into a string
            string text = await new StreamReader(response.Body).ReadToEndAsync();

            //We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);

            //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
            return $"{response.StatusCode}: {text}";
        }

        private bool CheckLogOrigin(string origin)
        {
            var origins = _configuration.GetValue<string>("LogMiddlewareOrigins");
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