using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Reso.Sdk.Core.Custom;

namespace Reso.Sdk.Core.Extension
{
	public static class FilterExtension
	{
		public static void ConfigureFilter<TErrorHandlingFilter>(this IServiceCollection services) where TErrorHandlingFilter : IExceptionFilter
		{
			services.AddMvc(delegate(MvcOptions ops)
			{
				ops.ValueProviderFactories.Add(new SnakeCaseQueryValueProviderFactory());
			});
			services.AddControllers(delegate(MvcOptions options)
			{
				options.Filters.Add<TErrorHandlingFilter>();
			});
		}
	}
}
