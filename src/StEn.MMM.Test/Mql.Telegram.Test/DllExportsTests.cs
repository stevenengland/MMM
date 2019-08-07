using System;
using Newtonsoft.Json;
using StEn.MMM.Mql.Common.Services.InApi.Entities;
using StEn.MMM.Mql.Common.Services.InApi.Factories;
using StEn.MMM.Mql.Telegram;
using Xunit;

namespace Mql.Telegram.Tests
{
	public class DllExportsTests
	{
		public DllExportsTests()
		{
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
			var exportResponse = DllExports.Initialize("", 10);
			var responseError = JsonConvert.DeserializeObject<Response<Error>>(exportResponse);
			Assert.IsType<Response<Error>>(responseError);
			Assert.Equal(typeof(ArgumentException).Name, responseError.Content.ExceptionType);
		}
	}
}
