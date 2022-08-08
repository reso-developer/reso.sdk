using System;

namespace Reso.Sdk.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class HiddenParamsAttribute : Attribute
	{
		public string Params { get; set; }

		public HiddenParamsAttribute(string parameters)
		{
			Params = parameters;
		}
	}
}
