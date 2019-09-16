using System;
using System.Reflection;
using Moq;
using Newtonsoft.Json;
using StEn.MMM.Mql.Common.Services.InApi.Entities;
using StEn.MMM.Mql.Telegram;
using StEn.MMM.Mql.Telegram.Services.Telegram;
using Telegram.Bot.Types.Enums;
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
		public void DefaultValuesAreSet()
		{
			TelegramDllExports.Initialize(ApiKey, 10);

			TelegramDllExports.SetDefaultValue(nameof(TelegramDllExports.Bot.DisableNotifications),"true");
			TelegramDllExports.SetDefaultValue(nameof(TelegramDllExports.Bot.DisableWebPagePreview),"true");
			TelegramDllExports.SetDefaultValue(nameof(TelegramDllExports.Bot.ParseMode),"html");
			Assert.True(TelegramDllExports.Bot.DisableNotifications);
			Assert.True(TelegramDllExports.Bot.DisableWebPagePreview);
			Assert.True(TelegramDllExports.Bot.ParseMode == ParseMode.Html);
		}

		[Fact]
		public void SetDefaultValueInputsGetValidated()
		{
			var mock = new Mock<ITelegramBotMapper>();

			TelegramDllExports.Initialize(ApiKey, 10);
			TelegramDllExports.Bot = mock.Object;

			Assert.Contains(nameof(ArgumentException), TelegramDllExports.SetDefaultValue(string.Empty, "some text"));
			Assert.Contains(nameof(ArgumentException), TelegramDllExports.SetDefaultValue("some text", string.Empty));
		}

		[Fact]
		public void GetMeSucceeds()
		{
			var mock = new Mock<ITelegramBotMapper>();
			mock.Setup(x => x.GetMe()).Returns("ok");
			mock.Setup(x => x.StartGetMe()).Returns("ok");

			TelegramDllExports.Initialize(ApiKey, 10);
			TelegramDllExports.Bot = mock.Object;

			Assert.True(TelegramDllExports.GetMe() == "ok");
			Assert.True(TelegramDllExports.StartGetMe() == "ok");
		}

		[Fact]
		public void GetUpdatesSucceeds()
		{
			var mock = new Mock<ITelegramBotMapper>();
			mock.Setup(x => x.GetUpdates()).Returns("ok");
			mock.Setup(x => x.StartGetUpdates()).Returns("ok");

			TelegramDllExports.Initialize(ApiKey, 10);
			TelegramDllExports.Bot = mock.Object;

			Assert.True(TelegramDllExports.GetUpdates() == "ok");
			Assert.True(TelegramDllExports.StartGetUpdates() == "ok");
		}

		[Fact]
		public void SendPhotoSucceeds()
		{
			var mock = new Mock<ITelegramBotMapper>();
			mock.Setup(x => x.SendPhoto(It.IsAny<string>(), It.IsAny<string>())).Returns("ok");
			mock.Setup(x => x.StartSendPhoto(It.IsAny<string>(), It.IsAny<string>())).Returns("ok");

			TelegramDllExports.Initialize(ApiKey, 10);
			TelegramDllExports.Bot = mock.Object;

			Assert.True(TelegramDllExports.SendPhoto("some text", "assets/favicon-32x32.png") == "ok");
			Assert.True(TelegramDllExports.StartSendPhoto("some text", "assets/favicon-32x32.png") == "ok");
		}

		[Fact]
		public void SendPhotoInputsGetValidated()
		{
			var mock = new Mock<ITelegramBotMapper>();

			TelegramDllExports.Initialize(ApiKey, 10);
			TelegramDllExports.Bot = mock.Object;

			Assert.Contains(nameof(ArgumentException), TelegramDllExports.SendPhoto(string.Empty, "D:/PathToFile.png"));
			Assert.Contains(nameof(ArgumentException), TelegramDllExports.SendPhoto("-102264846545", string.Empty));
			Assert.Contains(nameof(ArgumentException), TelegramDllExports.StartSendPhoto(string.Empty, "D:/PathToFile.png"));
			Assert.Contains(nameof(ArgumentException), TelegramDllExports.StartSendPhoto("-102264846545", string.Empty));
		}

		[Fact]
		public void SendTextSucceeds()
		{
			var mock = new Mock<ITelegramBotMapper>();
			mock.Setup(x => x.SendText(It.IsAny<string>(), It.IsAny<string>())).Returns("ok");
			mock.Setup(x => x.StartSendText(It.IsAny<string>(), It.IsAny<string>())).Returns("ok");

			TelegramDllExports.Initialize(ApiKey, 10);
			TelegramDllExports.Bot = mock.Object;

			Assert.True(TelegramDllExports.SendText("some text", "some text") == "ok");
			Assert.True(TelegramDllExports.StartSendText("some text", "some text") == "ok");
		}

		[Fact]
		public void SendTextInputsGetValidated()
		{
			var mock = new Mock<ITelegramBotMapper>();

			TelegramDllExports.Initialize(ApiKey, 10);
			TelegramDllExports.Bot = mock.Object;

			Assert.Contains(nameof(ArgumentException), TelegramDllExports.SendText(string.Empty, "some text"));
			Assert.Contains(nameof(ArgumentException), TelegramDllExports.SendText("some text", string.Empty));
			Assert.Contains(nameof(ArgumentException), TelegramDllExports.StartSendText(string.Empty, "some text"));
			Assert.Contains(nameof(ArgumentException), TelegramDllExports.StartSendText("some text", string.Empty));
		}
	}
}
