using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Reso.Sdk.Core.Extension
{
	public static class ErrorHandlerExtension
	{
		public static void ConfigureErrorHandler(this IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				return;
			}
			app.UseExceptionHandler("/error");
			app.UseHsts();
		}
	}
}
