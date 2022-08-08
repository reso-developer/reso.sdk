using System;

namespace Reso.Sdk.Core.Custom
{
	public class PropertyClass
	{
		public Type GetType(string str)
		{
			return Type.GetType("Reso.DataService.Models." + str);
		}
	}
}
