using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using Reso.Sdk.Core.Attributes;
using Reso.Sdk.Core.Custom;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Reso.Sdk.Core.Extension
{
    public static class SwaggerExtension
    {
        public class AuthorizeCheckOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                bool flag = (context.MethodInfo.DeclaringType!.GetCustomAttributes(inherit: true).OfType<AuthorizeAttribute>().Any() || context.MethodInfo.GetCustomAttributes(inherit: true).OfType<AuthorizeAttribute>().Any()) && !context.MethodInfo.GetCustomAttributes(inherit: true).OfType<AllowAnonymousAttribute>().Any();
                CustomAttributeData customAttributeData = context.MethodInfo.CustomAttributes.FirstOrDefault((CustomAttributeData a) => a.AttributeType == typeof(HiddenParamsAttribute));
                if (customAttributeData != null)
                {
                    string[] array = ((string)customAttributeData.ConstructorArguments.FirstOrDefault().Value).Split(",");
                    string[] array2 = array;
                    foreach (string parameter in array2)
                    {
                        if (operation.Parameters.Any((OpenApiParameter a) => a.Name.ToSnakeCase() == parameter.ToSnakeCase()))
                        {
                            operation.Parameters.Remove(operation.Parameters.FirstOrDefault((OpenApiParameter a) => a.Name.ToSnakeCase() == parameter.ToSnakeCase()));
                        }
                    }
                }
                if (flag)
                {
                    operation.Security = new List<OpenApiSecurityRequirement>
                    {
                        new OpenApiSecurityRequirement { [new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        }] = new string[0] }
                    };
                }
            }
        }

        public class RemoveVersionParameterFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                OpenApiParameter openApiParameter = operation.Parameters.SingleOrDefault((OpenApiParameter p) => p.Name == "version");
                if (openApiParameter != null)
                {
                    operation.Parameters.Remove(openApiParameter);
                }
            }
        }

        public class ReplaceVersionWithExactValueInPathFilter : IDocumentFilter
        {
            public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
            {
                OpenApiPaths openApiPaths = new OpenApiPaths();
                IEnumerable<ApiDescription> source = context.ApiDescriptions.Where((ApiDescription w) => w.ActionDescriptor.EndpointMetadata.Any((object a) => a.GetType() == typeof(HiddenControllerAttribute)));
                foreach (KeyValuePair<string, OpenApiPathItem> path in swaggerDoc.Paths)
                {
                    if (!source.Select((ApiDescription s) => s.RelativePath.StartsWith("/") ? s.RelativePath : ("/" + s.RelativePath)).Contains(path.Key))
                    {
                        openApiPaths.Add(path.Key.Replace("v{version}", swaggerDoc.Info.Version), path.Value);
                    }
                }
                swaggerDoc.Paths = openApiPaths;
            }
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddVersionedApiExplorer(delegate (ApiExplorerOptions options)
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            services.AddApiVersioning(delegate (ApiVersioningOptions o)
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });
            services.AddSwaggerGen(delegate (SwaggerGenOptions c)
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "UniDeli API",
                    Version = "v1.0"
                });
                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "UniDeli API",
                    Version = "v2.0"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.\r\n                      Enter 'Bearer' [space] and then your token in the text input below.\r\n                      Example: 'Bearer iJIUzI1NiIsInR5cCI6IkpXVCGlzIElzc2'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.OperationFilter<AuthorizeCheckOperationFilter>(Array.Empty<object>());
                IEnumerable<string> enumerable = from w in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory)
                                                 where new FileInfo(w).Extension == ".xml"
                                                 select w;
                foreach (string item in enumerable)
                {
                    c.IncludeXmlComments(item);
                }
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[0]
                } });
                c.OperationFilter<RemoveVersionParameterFilter>(Array.Empty<object>());
                c.DocumentFilter<ReplaceVersionWithExactValueInPathFilter>(Array.Empty<object>());
                c.EnableAnnotations();
            });
            services.AddSwaggerGenNewtonsoftSupport();
            services.TryAddEnumerable(ServiceDescriptor.Transient<IApiDescriptionProvider, SnakeCaseQueryParametersApiDescriptionProvider>());
        }

        public static void ConfigureSwagger(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(delegate (SwaggerUIOptions c)
            {
                foreach (ApiVersionDescription apiVersionDescription in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint("/swagger/" + apiVersionDescription.GroupName + "/swagger.json", apiVersionDescription.GroupName.ToUpperInvariant());
                }
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
