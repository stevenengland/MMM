using System;
using System.Runtime.InteropServices;
using Mql.Telegram.IntegrationTests.Framework;
using Mql.Telegram.IntegrationTests.Helpers;
using StEn.MMM.Mql.Telegram;
using StEn.MMM.Mql.Telegram.Services.Telegram;
using Telegram.Bot;
using Xunit;
using Xunit.Abstractions;

namespace Mql.Telegram.IntegrationTests
{
	[Collection(Constants.TestCollections.GettingUpdates)]
	public class GetMeTests
	{
		private readonly ITestOutputHelper testOutputHelper;

		private readonly string initializationResult;

		public GetMeTests(ITestOutputHelper testOutputHelper)
		{
			this.testOutputHelper = testOutputHelper;
			this.initializationResult = Initialize(Secrets.BOT_API_KEY, 10);
		}

		[DllImport(Constants.AssemblyUnderTestName)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string Initialize(
			[MarshalAs(UnmanagedType.LPWStr)] string apiKey,
			int timeout);

		[DllImport(Constants.AssemblyUnderTestName)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string GetMe();

		[DllImport(Constants.AssemblyUnderTestName)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static extern string StartGetMe();

		[Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.GetMe)]
		[Fact]
		public void InitializationSucceeds()
		{
			var result = Initialize(Secrets.BOT_API_KEY, 10);
		}

		[Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.GetMe)]
		[Fact]
		public void GetMeReturnsBotUser()
		{
			var result = GetMe();
			this.testOutputHelper.WriteLine(result);
			Assert.Contains(result, "hallo");
		}

		[Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.GetMe)]
		[Fact]
		public void Test()
		{
			var result = DllExports.GetMe();
			Assert.Contains(result, "hallo");
		}
	}
}
