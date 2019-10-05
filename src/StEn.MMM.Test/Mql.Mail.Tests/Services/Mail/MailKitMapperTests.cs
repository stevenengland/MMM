using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StEn.MMM.Mql.Common.Base.Extensions;
using StEn.MMM.Mql.Common.Services.InApi.Entities;
using StEn.MMM.Mql.Common.Services.InApi.Factories;
using StEn.MMM.Mql.Mail.Services.Mail;
using Xunit;

namespace Mql.Mail.Tests.Services.Mail
{
	public class MailKitMapperTests
	{
		// Fake key but correct format
		private const string ApiKey = "1234567:4TT8bAc8GHUspu3ERYn-KGcvsvGB9u_n4ddy";
		private readonly ResponseFactory responseFactory;

		public MailKitMapperTests()
		{
			this.responseFactory = new ResponseFactory
			{
				IsDebugEnabled = true,
			};
		}

		[Fact]
		public void LongRunningTaskGetsCancelled()
		{
			var mapper = this.CreateMailKitMapper();
			var cts = new CancellationTokenSource(1000);

			var result = mapper.ProxyCall(this.LongRunningTaskAsync(cts.Token));
			var errorResponse = JsonConvert.DeserializeObject<Response<Error>>(result);
			Assert.Equal(typeof(OperationCanceledException).Name, errorResponse.Content.ExceptionType);
		}

		[Theory]
		[InlineData("true", true)]
		[InlineData("false", false)]
		public void SetDefaultForDisableNotificationIsHandled(string actual, bool expected)
		{
			var mapper = this.CreateMailKitMapper();
			JsonConvert.DeserializeObject<Response<string>>(mapper.SetDefaultValue($"{nameof(mapper.CheckServerCertificate)}", actual));
			Assert.Equal(expected, mapper.CheckServerCertificate);
			var result = JsonConvert.DeserializeObject<Response<Error>>(mapper.SetDefaultValue($"{nameof(mapper.CheckServerCertificate)}", "unknown"));
			Assert.Equal(typeof(ArgumentException).Name, result.Content.ExceptionType);
		}

		[Fact]
		public void SetDefaultForUnknownKeyThrowsException()
		{
			var mapper = this.CreateMailKitMapper();
			var result = JsonConvert.DeserializeObject<Response<Error>>(mapper.SetDefaultValue($"UnknownStuff", "test"));
			Assert.Equal(typeof(KeyNotFoundException).Name, result.Content.ExceptionType);
		}

		[Fact]
		public async Task LongRunningFireAndForgetTaskGetsCancelledAsync()
		{
			var mapper = this.CreateMailKitMapper();
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
			var mapper = this.CreateMailKitMapper();

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
			var mapper = this.CreateMailKitMapper();
			var result = mapper.ProxyCall(this.ThrowingTaskAsync());
			var errorResponse = JsonConvert.DeserializeObject<Response<Error>>(result);
			Assert.Equal(typeof(AccessViolationException).Name, errorResponse.Content.ExceptionType);
		}

		[Fact]
		public void FireAndForgetReturnsSuccessEvenIfTaskFails()
		{
			var mapper = this.CreateMailKitMapper();

			var result = mapper.FireAndForgetProxyCall(this.ThrowingTaskAsync());
			var successResponse = JsonConvert.DeserializeObject<Response<string>>(result);

			Assert.True(successResponse.IsSuccess);
		}

		[Fact]
		public void MessageStoreReturnsErrorIfCorrelationKeyIsNotFound()
		{
			var mapper = this.CreateMailKitMapper();

			var result = mapper.GetMessageByCorrelationId("testCorrelationId");
			var errorResponse = JsonConvert.DeserializeObject<Response<Error>>(result);
			Assert.Equal(typeof(KeyNotFoundException).Name, errorResponse.Content.ExceptionType);
			Assert.Equal("testCorrelationId", errorResponse.CorrelationKey);
		}

		[Fact]
		public void FireAndForgetSuccessAddsMessageToStore()
		{
			var mapper = this.CreateMailKitMapper();

			mapper.HandleFireAndForgetSuccess("successTest", "testCorrelationId");
			var result = mapper.GetMessageByCorrelationId("testCorrelationId");
			var successResponse = JsonConvert.DeserializeObject<Response<string>>(result);
			Assert.Equal("successTest", successResponse.Content);
			Assert.Equal("testCorrelationId", successResponse.CorrelationKey);
		}

		[Fact]
		public void FireAndForgetErrorAddsMessageToStore()
		{
			var mapper = this.CreateMailKitMapper();

			mapper.HandleFireAndForgetError(new ArgumentException("something went wrong for testing purpose"), "testCorrelationId");
			var result = mapper.GetMessageByCorrelationId("testCorrelationId");
			var errorResponse = JsonConvert.DeserializeObject<Response<Error>>(result);
			Assert.Equal(typeof(ArgumentException).Name, errorResponse.Content.ExceptionType);
		}

		[Theory]
		[InlineData("sten<test@test.de>", "sten", "test@test.de")]
		[InlineData("sten <test@test.de>", "sten", "test@test.de")]
		[InlineData("test@test.de", null, "test@test.de")]
		public void StringsAreConvertedToMailboxes(string actual, string expectedName, string expectedAddress)
		{
			var mailBoxAddress = MailKitMapper.StringToMailboxAddress(actual);
			Assert.True(mailBoxAddress.Name == expectedName);
			Assert.True(mailBoxAddress.Address == expectedAddress);
		}

		#region Helper Methods

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

		private MailKitMapper CreateMailKitMapper()
		{
			return new MailKitMapper(
				"test",
				"test",
				"test",
				465,
				this.responseFactory);
		}

		private async Task<string> WaitForMessageStoreAsync(IMailMapper mailMapper, string correlationKey)
		{
			var messageStoreResult = string.Empty;
			for (int i = 0; i <= 10; i++)
			{
				messageStoreResult = mailMapper.GetMessageByCorrelationId(correlationKey);
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
