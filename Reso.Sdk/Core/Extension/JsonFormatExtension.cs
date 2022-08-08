using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Reso.Sdk.Core.Extension
{
	public static class JsonFormatExtension
	{
		public static void JsonFormatConfig(this IServiceCollection services)
		{
			services.AddControllers().AddNewtonsoftJson(delegate(MvcNewtonsoftJsonOptions options)
			{
				options.SerializerSettings.ContractResolver = new DefaultContractResolver
				{
					NamingStrategy = new SnakeCaseNamingStrategy()
				};
				options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Populate;
				options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				options.SerializerSettings.Formatting = Formatting.Indented;
			});
		}
	}
}
