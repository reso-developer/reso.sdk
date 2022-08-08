using System;

namespace Reso.Sdk.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class ExcludeAttribute : Attribute
	{
		public string Field { get; set; }
	}
}
