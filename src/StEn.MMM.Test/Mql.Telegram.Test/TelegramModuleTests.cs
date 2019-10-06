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
	public class TelegramModuleTests
	{
		private const string ApiKey = "1234567:4TT8bAc8GHUspu3ERYn-KGcvsvGB9u_n4ddy";

		public TelegramModuleTests()
		{
			// https://colinmackay.scot/2007/06/16/unit-testing-a-static-class/
			Type staticType = typeof(TelegramModule);
			ConstructorInfo ci = staticType.TypeInitializer;
			object[] parameters = new object[0];
			ci.Invoke(null, parameters);

			TelegramModule.SetDebugOutput(true);
		}

		[Fact]
		public void InitializationSucceeds()
		{
			var exportResponse = TelegramModule.Initialize("1234567:4TT8bAc8GHUspu3ERYn-KGcvsvGB9u_n4ddy", 10);
			var response = JsonConvert.DeserializeObject<Response<string>>(exportResponse);
			Assert.IsType<Response<string>>(response);
		}

		[Fact]
		public void MissingInitializationLeedsToErrors()
		{
			var exportResponse = TelegramModule.GetMe();
			var responseError = JsonConvert.DeserializeObject<Response<Error>>(exportResponse);
			Assert.Equal(typeof(ApplicationException).Name, responseError.Content.ExceptionType);
		}

		[Fact]
		public void WrongApiKeyFormatReturnsInitError()
		{
			var exportResponse = TelegramModule.Initialize("test", 10);
			var responseError = JsonConvert.DeserializeObject<Response<Error>>(exportResponse);
			Assert.IsType<Response<Error>>(responseError);
			Assert.Equal(typeof(ArgumentException).Name, responseError.Content.ExceptionType);
		}

		[Fact]
		public void ZeroTimoutReturnsInitError()
		{
			var exportResponse = TelegramModule.Initialize("test", 0);
			var responseError = JsonConvert.DeserializeObject<Response<Error>>(exportResponse);
			Assert.IsType<Response<Error>>(responseError);
			Assert.Equal(typeof(ArgumentException).Name, responseError.Content.ExceptionType);
		}

		[Fact]
		public void EmptyApiKeyReturnsInitError()
		{
			var exportResponse = TelegramModule.Initialize(string.Empty, 10);
			var responseError = JsonConvert.DeserializeObject<Response<Error>>(exportResponse);
			Assert.IsType<Response<Error>>(responseError);
			Assert.Equal(typeof(ArgumentException).Name, responseError.Content.ExceptionType);
		}

		[Fact]
		public void TimeoutIsSet()
		{
			TelegramModule.Initialize(ApiKey, 10);
			Assert.True(TelegramModule.Bot.RequestTimeout == 10000);
			TelegramModule.SetRequestTimeout(20);
			Assert.True(TelegramModule.Bot.RequestTimeout == 20000);
		}

		[Fact]
		public void DefaultValuesAreSet()
		{
			TelegramModule.Initialize(ApiKey, 10);

			TelegramModule.SetDefaultValue(nameof(TelegramModule.Bot.DisableNotifications), "true");
			TelegramModule.SetDefaultValue(nameof(TelegramModule.Bot.DisableWebPagePreview), "true");
			TelegramModule.SetDefaultValue(nameof(TelegramModule.Bot.ParseMode), "html");
			Assert.True(TelegramModule.Bot.DisableNotifications);
			Assert.True(TelegramModule.Bot.DisableWebPagePreview);
			Assert.True(TelegramModule.Bot.ParseMode == ParseMode.Html);
		}

		[Fact]
		public void SetDefaultValueInputsGetValidated()
		{
			var mock = new Mock<ITelegramBotMapper>();

			TelegramModule.Initialize(ApiKey, 10);
			TelegramModule.Bot = mock.Object;

			Assert.Contains(nameof(ArgumentException), TelegramModule.SetDefaultValue(string.Empty, "some text"));
			Assert.Contains(nameof(ArgumentException), TelegramModule.SetDefaultValue("some text", string.Empty));
		}

		[Fact]
		public void GetMeSucceeds()
		{
			var mock = new Mock<ITelegramBotMapper>();
			mock.Setup(x => x.GetMe()).Returns("ok");
			mock.Setup(x => x.StartGetMe()).Returns("ok");

			TelegramModule.Initialize(ApiKey, 10);
			TelegramModule.Bot = mock.Object;

			Assert.True(TelegramModule.GetMe() == "ok");
			Assert.True(TelegramModule.StartGetMe() == "ok");
		}

		[Fact]
		public void GetUpdatesSucceeds()
		{
			var mock = new Mock<ITelegramBotMapper>();
			mock.Setup(x => x.GetUpdates()).Returns("ok");
			mock.Setup(x => x.StartGetUpdates()).Returns("ok");

			TelegramModule.Initialize(ApiKey, 10);
			TelegramModule.Bot = mock.Object;

			Assert.True(TelegramModule.GetUpdates() == "ok");
			Assert.True(TelegramModule.StartGetUpdates() == "ok");
		}

		[Fact]
		public void SendDocumentSucceeds()
		{
			var mock = new Mock<ITelegramBotMapper>();
			mock.Setup(x => x.SendDocument(It.IsAny<string>(), It.IsAny<string>())).Returns("ok");
			mock.Setup(x => x.StartSendDocument(It.IsAny<string>(), It.IsAny<string>())).Returns("ok");

			TelegramModule.Initialize(ApiKey, 10);
			TelegramModule.Bot = mock.Object;

			Assert.True(TelegramModule.SendDocument("some text", "assets/fake_log.txt") == "ok");
			Assert.True(TelegramModule.StartSendDocument("some text", "assets/fake_log.txt") == "ok");
		}

		[Fact]
		public void SendDocumentInputsGetValidated()
		{
			var mock = new Mock<ITelegramBotMapper>();

			TelegramModule.Initialize(ApiKey, 10);
			TelegramModule.Bot = mock.Object;

			Assert.Contains(nameof(ArgumentException), TelegramModule.SendDocument(string.Empty, "D:/PathToFile.png"));
			Assert.Contains(nameof(ArgumentException), TelegramModule.SendDocument("-102264846545", string.Empty));
			Assert.Contains(nameof(ArgumentException), TelegramModule.StartSendDocument(string.Empty, "D:/PathToFile.png"));
			Assert.Contains(nameof(ArgumentException), TelegramModule.StartSendDocument("-102264846545", string.Empty));
		}

		[Fact]
		public void SendPhotoSucceeds()
		{
			var mock = new Mock<ITelegramBotMapper>();
			mock.Setup(x => x.SendPhoto(It.IsAny<string>(), It.IsAny<string>())).Returns("ok");
			mock.Setup(x => x.StartSendPhoto(It.IsAny<string>(), It.IsAny<string>())).Returns("ok");

			TelegramModule.Initialize(ApiKey, 10);
			TelegramModule.Bot = mock.Object;

			Assert.True(TelegramModule.SendPhoto("some text", "assets/favicon-32x32.png") == "ok");
			Assert.True(TelegramModule.StartSendPhoto("some text", "assets/favicon-32x32.png") == "ok");
		}

		[Fact]
		public void SendPhotoInputsGetValidated()
		{
			var mock = new Mock<ITelegramBotMapper>();

			TelegramModule.Initialize(ApiKey, 10);
			TelegramModule.Bot = mock.Object;

			Assert.Contains(nameof(ArgumentException), TelegramModule.SendPhoto(string.Empty, "D:/PathToFile.png"));
			Assert.Contains(nameof(ArgumentException), TelegramModule.SendPhoto("-102264846545", string.Empty));
			Assert.Contains(nameof(ArgumentException), TelegramModule.StartSendPhoto(string.Empty, "D:/PathToFile.png"));
			Assert.Contains(nameof(ArgumentException), TelegramModule.StartSendPhoto("-102264846545", string.Empty));
		}

		[Fact]
		public void SendTextSucceeds()
		{
			var mock = new Mock<ITelegramBotMapper>();
			mock.Setup(x => x.SendText(It.IsAny<string>(), It.IsAny<string>())).Returns("ok");
			mock.Setup(x => x.StartSendText(It.IsAny<string>(), It.IsAny<string>())).Returns("ok");

			TelegramModule.Initialize(ApiKey, 10);
			TelegramModule.Bot = mock.Object;

			Assert.True(TelegramModule.SendText("some text", "some text") == "ok");
			Assert.True(TelegramModule.StartSendText("some text", "some text") == "ok");
		}

		[Fact]
		public void SendTextInputsGetValidated()
		{
			var mock = new Mock<ITelegramBotMapper>();

			TelegramModule.Initialize(ApiKey, 10);
			TelegramModule.Bot = mock.Object;

			Assert.Contains(nameof(ArgumentException), TelegramModule.SendText(string.Empty, "some text"));
			Assert.Contains(nameof(ArgumentException), TelegramModule.SendText("some text", string.Empty));
			Assert.Contains(nameof(ArgumentException), TelegramModule.StartSendText(string.Empty, "some text"));
			Assert.Contains(nameof(ArgumentException), TelegramModule.StartSendText("some text", string.Empty));
		}
	}
}
