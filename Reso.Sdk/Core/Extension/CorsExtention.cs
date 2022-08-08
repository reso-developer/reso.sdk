using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Reso.Sdk.Core.Extension
{
	public static class CorsExtention
	{
		public static void ConfigCors(this IServiceCollection services, string name)
		{
			services.AddCors(delegate(CorsOptions options)
			{
				options.AddPolicy(name, delegate(CorsPolicyBuilder builder)
				{
					builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
				});
			});
		}
	}
}
