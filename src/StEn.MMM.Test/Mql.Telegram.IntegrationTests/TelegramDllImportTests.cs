using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Mql.Telegram.IntegrationTests.Framework;
using Mql.Telegram.IntegrationTests.Helpers;
using Newtonsoft.Json;
using NUnit.Framework;
using StEn.MMM.Mql.Common.Services.InApi.Entities;
using StEn.MMM.Mql.Common.Services.InApi.Factories;
using StEn.MMM.Mql.Telegram;
using StEn.MMM.Mql.Telegram.Services.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Mql.Telegram.IntegrationTests
{
	[TestFixture]
	public class TelegramDllImportTests
	{
#if !DEBUG

		#region DllImport

		[DllImport(Constants.AssemblyUnderTestName)]
		public static extern void Test();

		[DllImport(Constants.AssemblyUnderTestName)]
		public static extern void SetDebugOutput([MarshalAs(UnmanagedType.Bool)] bool enabled);

		[DllImport(Constants.AssemblyUnderTestName)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		private static extern string Initialize(
			[MarshalAs(UnmanagedType.LPWStr)] string apiKey,
			int timeout);

		[DllImport(Constants.AssemblyUnderTestName)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		private static extern string GetMe();

		[DllImport(Constants.AssemblyUnderTestName)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		private static extern string StartGetMe();

		#endregion

		[SetUp]
		public void PerTestSetup()
		{
			// https://colinmackay.scot/2007/06/16/unit-testing-a-static-class/
			// Type staticType = typeof(DllExports);
			// ConstructorInfo ci = staticType.TypeInitializer;
			// object[] parameters = new object[0];
			// ci.Invoke(null, parameters);
		}

		[Test]
		public void TestCall()
		{
			Test();
		}

		[Test, Category(Constants.TelegramBotApiMethods.GetMe)]
		public void GetMeReturnsBotUser()
		{
			Initialize(Secrets.BOT_API_KEY, 100);
			SetDebugOutput(true);
			var result = GetMe();
			var successResponse = JsonConvert.DeserializeObject<Response<Message<User>>>(result);
			Assert.True(successResponse.Content.Payload.IsBot);
		}

#endif
	}
}
