using System;

namespace Reso.Sdk.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
	public class MultiAttribute : Attribute
	{
		public string Params { get; set; }

		public MultiAttribute(string parameters)
		{
			Params = parameters;
		}
	}
}
