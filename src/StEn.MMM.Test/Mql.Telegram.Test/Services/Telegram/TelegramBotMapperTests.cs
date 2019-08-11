using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StEn.MMM.Mql.Common.Services.InApi.Entities;
using StEn.MMM.Mql.Common.Services.InApi.Factories;
using StEn.MMM.Mql.Telegram.Services.Telegram;
using Telegram.Bot;
using Xunit;

namespace Mql.Telegram.Tests.Services.Telegram
{
	public class TelegramBotMapperTests
	{
		// Fake key but correct format
		private const string ApiKey = "1234567:4TT8bAc8GHUspu3ERYn-KGcvsvGB9u_n4ddy";

		public TelegramBotMapperTests()
		{
			ResponseFactory.IsDebugEnabled = true;
		}

		[Fact]
		public void LongRunningTaskGetsCancelled()
		{
			var mapper = new TelegramBotMapper(new TelegramBotClient(ApiKey));
			var cts = new CancellationTokenSource(1000);

			var result = mapper.ProxyCall(this.LongRunningTaskAsync(cts.Token));
			var errorResponse = JsonConvert.DeserializeObject<Response<Error>>(result);
			Assert.Equal(typeof(OperationCanceledException).Name, errorResponse.Content.ExceptionType);
		}

		[Fact]
		public void LongRunningFireAndForgetTaskGetsCancelled()
		{
			var mapper = new TelegramBotMapper(new TelegramBotClient(ApiKey));
			var cts = new CancellationTokenSource(3000);

			var result = mapper.FireAndForgetProxyCall(this.LongRunningTaskAsync(cts.Token));
			var successResponse = JsonConvert.DeserializeObject<Response<Message<string>>>(result);
			Assert.True(successResponse.IsSuccess);
			Assert.False(string.IsNullOrWhiteSpace(successResponse.CorrelationKey));
		}

		[Fact]
		public void TaskExceptionReturnsError()
		{
			var mapper = new TelegramBotMapper(new TelegramBotClient(ApiKey));
			var result = mapper.ProxyCall(this.ThrowingTaskAsync());
			var errorResponse = JsonConvert.DeserializeObject<Response<Error>>(result);
			Assert.Equal(typeof(AccessViolationException).Name, errorResponse.Content.ExceptionType);
		}

		[Fact]
		public void FireAndForgetReturnsSuccessEvenIfTaskFails()
		{
			var mapper = new TelegramBotMapper(new TelegramBotClient(ApiKey));

			var result = mapper.FireAndForgetProxyCall(this.ThrowingTaskAsync());
			var successResponse = JsonConvert.DeserializeObject<Response<Message<string>>>(result);

			Assert.True(successResponse.IsSuccess);
		}

		[Fact]
		public void MessageStoreReturnsErrorIfCorrelationKeyIsNotFound()
		{
			var mapper = new TelegramBotMapper(new TelegramBotClient(ApiKey));

			var result = mapper.GetMessageByCorrelationId("testCorrelationId");
			var errorResponse = JsonConvert.DeserializeObject<Response<Error>>(result);
			Assert.Equal(typeof(KeyNotFoundException).Name, errorResponse.Content.ExceptionType);
		}

		[Fact]
		public void FireAndForgetSuccessAddsMessageToStore()
		{
			var mapper = new TelegramBotMapper(new TelegramBotClient(ApiKey));

			mapper.HandleFireAndForgetSuccess("successTest", "testCorrelationId");
			var result = mapper.GetMessageByCorrelationId("testCorrelationId");
			var successResponse = JsonConvert.DeserializeObject<Response<Message<string>>>(result);
			Assert.Equal("successTest", successResponse.Content.Payload);
		}

		[Fact]
		public void FireAndForgetErrorAddsMessageToStore()
		{
			var mapper = new TelegramBotMapper(new TelegramBotClient(ApiKey));

			mapper.HandleFireAndForgetError(new ArgumentException("something went wrong for testing purpose"), "testCorrelationId");
			var result = mapper.GetMessageByCorrelationId("testCorrelationId");
			var errorResponse = JsonConvert.DeserializeObject<Response<Error>>(result);
			Assert.Equal(typeof(ArgumentException).Name, errorResponse.Content.ExceptionType);
		}

		private async Task<string> LongRunningTaskAsync(CancellationToken ct)
		{
			for (int i = 0; i < 10; i++)
			{
				await Task.Delay(1000);
				ct.ThrowIfCancellationRequested();
			}

			return string.Empty;
		}

		private async Task<string> ThrowingTaskAsync()
		{
			throw new AccessViolationException("You are not allowed to be here.");
		}
	}
}
