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
	public class DllExportsTests
	{
		private const string ApiKey = "1234567:4TT8bAc8GHUspu3ERYn-KGcvsvGB9u_n4ddy";

		public DllExportsTests()
		{
			Type staticType = typeof(DllExports);
			ConstructorInfo ci = staticType.TypeInitializer;
			object[] parameters = new object[0];
			ci.Invoke(null, parameters);
			ResponseFactory.IsDebugEnabled = true;
		}

		[Fact]
		public void InitializationSucceeds()
		{
			var exportResponse = DllExports.Initialize("1234567:4TT8bAc8GHUspu3ERYn-KGcvsvGB9u_n4ddy", 10);
			var response = JsonConvert.DeserializeObject<Response<Message<string>>>(exportResponse);
			Assert.IsType<Response<Message<string>>>(response);
		}

		[Fact]
		public void MissingInitializationLeedsToErrors()
		{
			var exportResponse = DllExports.GetMe();
			var responseError = JsonConvert.DeserializeObject<Response<Error>>(exportResponse);
			Assert.Equal(typeof(ApplicationException).Name, responseError.Content.ExceptionType);
		}

		[Fact]
		public void WrongApiKeyFormatReturnsInitError()
		{
			var exportResponse = DllExports.Initialize("test", 10);
			var responseError = JsonConvert.DeserializeObject<Response<Error>>(exportResponse);
			Assert.IsType<Response<Error>>(responseError);
			Assert.Equal(typeof(ArgumentException).Name, responseError.Content.ExceptionType);
		}

		[Fact]
		public void ZeroTimoutReturnsInitError()
		{
			var exportResponse = DllExports.Initialize("test", 0);
			var responseError = JsonConvert.DeserializeObject<Response<Error>>(exportResponse);
			Assert.IsType<Response<Error>>(responseError);
			Assert.Equal(typeof(ArgumentException).Name, responseError.Content.ExceptionType);
		}

		[Fact]
		public void EmptyApiKeyReturnsInitError()
		{
			var exportResponse = DllExports.Initialize(string.Empty, 10);
			var responseError = JsonConvert.DeserializeObject<Response<Error>>(exportResponse);
			Assert.IsType<Response<Error>>(responseError);
			Assert.Equal(typeof(ArgumentException).Name, responseError.Content.ExceptionType);
		}

		[Fact]
		public void TimeoutIsSet()
		{
			DllExports.Initialize(ApiKey, 10);
			DllExports.SetRequestTimeout(10);
			Assert.True(DllExports.Bot.RequestTimeout == 10);
		}

		[Fact]
		public void GetMeSucceeds()
		{
			var mock = new Mock<ITelegramBotMapper>();
			mock.Setup(x => x.GetMe()).Returns("ok");
			mock.Setup(x => x.StartGetMe()).Returns("ok");

			var dllExports = new DllExports(mock.Object);
			Assert.True(DllExports.GetMe() == "ok");
			Assert.True(DllExports.StartGetMe() == "ok");
		}

		[Fact]
		public void SendTextSucceeds()
		{
			var mock = new Mock<ITelegramBotMapper>();
			mock.Setup(x => x.SendText(It.IsAny<string>(), It.IsAny<string>())).Returns("ok");
			mock.Setup(x => x.StartSendText(It.IsAny<string>(), It.IsAny<string>())).Returns("ok");

			var dllExports = new DllExports(mock.Object);
			Assert.True(DllExports.SendText(string.Empty, string.Empty) == "ok");
			Assert.True(DllExports.StartSendText(string.Empty, string.Empty) == "ok");
		}
	}
}
