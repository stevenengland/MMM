using System;
using System.Collections.Generic;
using System.Text;

namespace StEn.MMM.Mql.Common.Base.Attributes
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class MqlFuncDocAttribute : Attribute
	{
		public int Order { get; set; }
	}
}