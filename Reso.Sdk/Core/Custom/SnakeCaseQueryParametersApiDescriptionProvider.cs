using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Reso.Sdk.Core.Custom
{
	public class SnakeCaseQueryParametersApiDescriptionProvider : IApiDescriptionProvider
	{
		public int Order => 1;

		public void OnProvidersExecuted(ApiDescriptionProviderContext context)
		{
		}

		public void OnProvidersExecuting(ApiDescriptionProviderContext context)
		{
			foreach (ApiParameterDescription item in from x in context.Results.SelectMany((ApiDescription x) => x.ParameterDescriptions)
				where x.Source.Id == "Query" || x.Source.Id == "ModelBinding"
				select x)
			{
				item.Name = item.Name.ToSnakeCase();
			}
		}
	}
}
