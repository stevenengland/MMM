using System;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;
using StEn.MMM.Mql.Common.Services.InApi.Factories;
using StEn.MMM.Mql.Telegram;

namespace TmpNunitTests
{
	[TestFixture]
	public class Class1
	{
#if !DEBUG
		[DllImport("Mql_Telegram.dll")]
		public static extern void Test();

		[DllImport("Mql_Telegram.dll")]
		public static extern void ResetClass();

		[DllImport("Mql_Telegram.dll")]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string Initialize(
			[MarshalAs(UnmanagedType.LPWStr)] string apiKey,
			int timeout);

		[DllImport("Mql_Telegram.dll")]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string GetMe();

		[DllImport("Mql_Telegram.dll")]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string StartGetMe();

		[SetUp]
		public void PerTestSetup()
		{
			// https://colinmackay.scot/2007/06/16/unit-testing-a-static-class/
			//Type staticType = typeof(DllExports);
			//ConstructorInfo ci = staticType.TypeInitializer;
			//object[] parameters = new object[0];
			//ci.Invoke(null, parameters);
			// DllExports.ResetClass();
			// ResponseFactory.IsDebugEnabled = true;
		}

		[Test, Category("Unmanaged interfaces")]
		public void SendTextAsync()
		{
			// ResetClass();
			Test();
		}

		[Test, Category("Unmanaged interfaces")]
		public void InitializationSucceeds()
		{
			try
			{
				Test();
				var result = Initialize("1234567:4TT8bAc8GHUspu3ERYn-KGcvsvGB9u_n4ddy", 10);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
#endif
	}
}
