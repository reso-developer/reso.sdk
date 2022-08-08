using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Reso.Sdk.Core.Custom
{
    public class SnakeCaseQueryValueProviderFactory : IValueProviderFactory
    {
        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            SnakeCaseQueryValueProvider item = new SnakeCaseQueryValueProvider(BindingSource.Query, context.ActionContext.HttpContext.Request.Query, CultureInfo.CurrentCulture);
            context.ValueProviders.Add(item);
            return Task.CompletedTask;
        }
    }
}
