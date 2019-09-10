using System;
using System.Collections.Generic;
using System.Text;

namespace StEn.MMM.Mql.Common.Base.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.GenericParameter, AllowMultiple = false)]
	public class MqlParamDocAttribute : Attribute
	{
		public string ExampleValue { get; set; }
	}
}
