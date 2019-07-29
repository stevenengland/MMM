using System;
using System.Runtime.InteropServices;
using RGiesecke.DllExport;

namespace Mql.Telegram
{
	public class Class1
	{
		[DllExport("Add", CallingConvention = CallingConvention.StdCall)]
		public static int Add(int left, int right)
		{
			return left + right;
		}

		[DllExport("Sub", CallingConvention = CallingConvention.StdCall)]
		public static int Sub(int left, int right)
		{
			return left - right;
		}

		public static int Mult(int factor1, int factor2)
		{
			return factor1 * factor2;
		}
	}
}
