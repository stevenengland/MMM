using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nito.AsyncEx.Synchronous;
using StEn.MMM.Mql.Common.Base.Extensions;
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
		private readonly ResponseFactory responseFactory;

		public TelegramBotMapperTests()
		{
			this.responseFactory = new ResponseFactory
			{
				IsDebugEnabled = true,
			};
		}

		[Fact]
		public void LongRunningTaskGetsCancelled()
		{
			var mapper = new TelegramBotMapper(new TelegramBotClient(ApiKey), this.responseFactory);
			var cts = new CancellationTokenSource(1000);

			var result = mapper.ProxyCall(this.LongRunningTaskAsync(cts.Token));
			var errorResponse = JsonConvert.DeserializeObject<Response<Error>>(result);
			Assert.Equal(typeof(OperationCanceledException).Name, errorResponse.Content.ExceptionType);
		}

		[Fact]
		public async Task LongRunningFireAndForgetTaskGetsCancelledAsync()
		{
			var mapper = new TelegramBotMapper(new TelegramBotClient(ApiKey), this.responseFactory);
			var cts = new CancellationTokenSource(3000);

			var result = mapper.FireAndForgetProxyCall(this.LongRunningTaskAsync(cts.Token));
			var successResponse = JsonConvert.DeserializeObject<Response<string>>(result);
			Assert.True(successResponse.IsSuccess);
			Assert.False(string.IsNullOrWhiteSpace(successResponse.CorrelationKey));

			var messageStoreResult = await this.WaitForMessageStoreAsync(mapper, successResponse.CorrelationKey);
			var correlatedResponse = JsonConvert.DeserializeObject<Response<Error>>(messageStoreResult);

			Assert.Equal(typeof(OperationCanceledException).Name, correlatedResponse.Content.ExceptionType);
		}

		[Fact]
		public async Task LongRunningFireAndForgetTaskWorkingWithDisposableObjectsGetsCancelledAsync()
		{
			var mapper = new TelegramBotMapper(new TelegramBotClient(ApiKey), this.responseFactory);

			string result;
			var cancellationTokenSource = new CancellationTokenSource(2000);

			result = mapper.FireAndForgetProxyCall(this.LongRunningTaskAsync(cancellationTokenSource.Token)
				.DisposeAfterThreadCompletionAsync(new IDisposable[]
				{
					cancellationTokenSource,
				}));

			var successResponse = JsonConvert.DeserializeObject<Response<string>>(result);
			Assert.True(successResponse.IsSuccess);
			Assert.False(string.IsNullOrWhiteSpace(successResponse.CorrelationKey));

			var messageStoreResult = await this.WaitForMessageStoreAsync(mapper, successResponse.CorrelationKey);
			var correlatedResponse = JsonConvert.DeserializeObject<Response<Error>>(messageStoreResult);

			Assert.Equal(typeof(OperationCanceledException).Name, correlatedResponse.Content.ExceptionType);
		}

		[Fact]
		public void TaskExceptionReturnsError()
		{
			var mapper = new TelegramBotMapper(new TelegramBotClient(ApiKey), this.responseFactory);
			var result = mapper.ProxyCall(this.ThrowingTaskAsync());
			var errorResponse = JsonConvert.DeserializeObject<Response<Error>>(result);
			Assert.Equal(typeof(AccessViolationException).Name, errorResponse.Content.ExceptionType);
		}

		[Fact]
		public void FireAndForgetReturnsSuccessEvenIfTaskFails()
		{
			var mapper = new TelegramBotMapper(new TelegramBotClient(ApiKey), this.responseFactory);

			var result = mapper.FireAndForgetProxyCall(this.ThrowingTaskAsync());
			var successResponse = JsonConvert.DeserializeObject<Response<string>>(result);

			Assert.True(successResponse.IsSuccess);
		}

		[Fact]
		public void MessageStoreReturnsErrorIfCorrelationKeyIsNotFound()
		{
			var mapper = new TelegramBotMapper(new TelegramBotClient(ApiKey), this.responseFactory);

			var result = mapper.GetMessageByCorrelationId("testCorrelationId");
			var errorResponse = JsonConvert.DeserializeObject<Response<Error>>(result);
			Assert.Equal(typeof(KeyNotFoundException).Name, errorResponse.Content.ExceptionType);
			Assert.Equal("testCorrelationId", errorResponse.CorrelationKey);
		}

		[Fact]
		public void FireAndForgetSuccessAddsMessageToStore()
		{
			var mapper = new TelegramBotMapper(new TelegramBotClient(ApiKey), this.responseFactory);

			mapper.HandleFireAndForgetSuccess("successTest", "testCorrelationId");
			var result = mapper.GetMessageByCorrelationId("testCorrelationId");
			var successResponse = JsonConvert.DeserializeObject<Response<string>>(result);
			Assert.Equal("successTest", successResponse.Content);
			Assert.Equal("testCorrelationId", successResponse.CorrelationKey);
		}

		[Fact]
		public void FireAndForgetErrorAddsMessageToStore()
		{
			var mapper = new TelegramBotMapper(new TelegramBotClient(ApiKey), this.responseFactory);

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
			await Task.Delay(0);
			throw new AccessViolationException("You are not allowed to be here.");
		}

		#region Helper Methods

		private async Task<string> WaitForMessageStoreAsync(ITelegramBotMapper botMapper, string correlationKey)
		{
			var messageStoreResult = string.Empty;
			for (int i = 0; i <= 10; i++)
			{
				messageStoreResult = botMapper.GetMessageByCorrelationId(correlationKey);
				if (messageStoreResult.Contains(nameof(KeyNotFoundException)))
				{
					await Task.Delay(1000);
				}
				else
				{
					break;
				}
			}

			return messageStoreResult;
		}

		#endregion
	}
}
