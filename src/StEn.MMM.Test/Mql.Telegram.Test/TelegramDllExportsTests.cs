using System;
using System.Reflection;
using Moq;
using Newtonsoft.Json;
using StEn.MMM.Mql.Common.Services.InApi.Entities;
using StEn.MMM.Mql.Common.Services.InApi.Factories;
using StEn.MMM.Mql.Telegram;
using StEn.MMM.Mql.Telegram.Services.Telegram;
using Xunit;

namespace Mql.Telegram.Tests
{
	public class TelegramDllExportsTests
	{
		private const string ApiKey = "1234567:4TT8bAc8GHUspu3ERYn-KGcvsvGB9u_n4ddy";

		public TelegramDllExportsTests()
		{
			// https://colinmackay.scot/2007/06/16/unit-testing-a-static-class/
			Type staticType = typeof(TelegramDllExports);
			ConstructorInfo ci = staticType.TypeInitializer;
			object[] parameters = new object[0];
			ci.Invoke(null, parameters);

			TelegramDllExports.SetDebugOutput(true);
		}

		[Fact]
		public void InitializationSucceeds()
		{
			var exportResponse = TelegramDllExports.Initialize("1234567:4TT8bAc8GHUspu3ERYn-KGcvsvGB9u_n4ddy", 10);
			var response = JsonConvert.DeserializeObject<Response<string>>(exportResponse);
			Assert.IsType<Response<string>>(response);
		}

		[Fact]
		public void MissingInitializationLeedsToErrors()
		{
			var exportResponse = TelegramDllExports.GetMe();
			var responseError = JsonConvert.DeserializeObject<Response<Error>>(exportResponse);
			Assert.Equal(typeof(ApplicationException).Name, responseError.Content.ExceptionType);
		}

		[Fact]
		public void WrongApiKeyFormatReturnsInitError()
		{
			var exportResponse = TelegramDllExports.Initialize("test", 10);
			var responseError = JsonConvert.DeserializeObject<Response<Error>>(exportResponse);
			Assert.IsType<Response<Error>>(responseError);
			Assert.Equal(typeof(ArgumentException).Name, responseError.Content.ExceptionType);
		}

		[Fact]
		public void ZeroTimoutReturnsInitError()
		{
			var exportResponse = TelegramDllExports.Initialize("test", 0);
			var responseError = JsonConvert.DeserializeObject<Response<Error>>(exportResponse);
			Assert.IsType<Response<Error>>(responseError);
			Assert.Equal(typeof(ArgumentException).Name, responseError.Content.ExceptionType);
		}

		[Fact]
		public void EmptyApiKeyReturnsInitError()
		{
			var exportResponse = TelegramDllExports.Initialize(string.Empty, 10);
			var responseError = JsonConvert.DeserializeObject<Response<Error>>(exportResponse);
			Assert.IsType<Response<Error>>(responseError);
			Assert.Equal(typeof(ArgumentException).Name, responseError.Content.ExceptionType);
		}

		[Fact]
		public void TimeoutIsSet()
		{
			TelegramDllExports.Initialize(ApiKey, 10);
			Assert.True(TelegramDllExports.Bot.RequestTimeout == 10000);
			TelegramDllExports.SetRequestTimeout(20);
			Assert.True(TelegramDllExports.Bot.RequestTimeout == 20000);
		}

		[Fact]
		public void GetMeSucceeds()
		{
			var mock = new Mock<ITelegramBotMapper>();
			mock.Setup(x => x.GetMe()).Returns("ok");
			mock.Setup(x => x.StartGetMe()).Returns("ok");

			var dllExports = new TelegramDllExports(mock.Object);
			Assert.True(TelegramDllExports.GetMe() == "ok");
			Assert.True(TelegramDllExports.StartGetMe() == "ok");
		}

		[Fact]
		public void SendTextSucceeds()
		{
			var mock = new Mock<ITelegramBotMapper>();
			mock.Setup(x => x.SendText(It.IsAny<string>(), It.IsAny<string>())).Returns("ok");
			mock.Setup(x => x.StartSendText(It.IsAny<string>(), It.IsAny<string>())).Returns("ok");

			var dllExports = new TelegramDllExports(mock.Object);
			Assert.True(TelegramDllExports.SendText(string.Empty, string.Empty) == "ok");
			Assert.True(TelegramDllExports.StartSendText(string.Empty, string.Empty) == "ok");
		}
	}
}
