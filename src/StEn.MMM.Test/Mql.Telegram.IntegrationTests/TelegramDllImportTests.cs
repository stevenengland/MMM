using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Mql.Telegram.IntegrationTests.Framework;
using Mql.Telegram.IntegrationTests.Helpers;
using Newtonsoft.Json;
using NUnit.Framework;
using StEn.MMM.Mql.Common.Services.InApi.Entities;
using Telegram.Bot.Types;

namespace Mql.Telegram.IntegrationTests
{
	[TestFixture]
	public class TelegramDllImportTests
	{
#if !DEBUG

		[SetUp]
		public void PerTestSetup()
		{
			// Method intentionally left empty.
		}

		[Test]
		[Category(Constants.TelegramBotApiMethods.GetMe)]
		public void RequestForCorrelationKeyReturnsErrorIfEntryIsNotFound()
		{
			Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			SetDebugOutput(true);
			var result = GetMessageByCorrelationId("test");
			var errorResponse = JsonConvert.DeserializeObject<Response<Error>>(result);
			Assert.False(errorResponse.IsSuccess);
			Assert.AreEqual(typeof(KeyNotFoundException).Name, errorResponse.Content.ExceptionType);
		}

		[Test]
		[Category(Constants.TelegramBotApiMethods.GetMe)]
		public void GetMeReturnsBotUser()
		{
			Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			SetDebugOutput(true);
			var result = GetMe();
			var successResponse = JsonConvert.DeserializeObject<Response<User>>(result);
			Assert.True(successResponse.Content.IsBot);
		}

		[Test]
		[Category(Constants.TelegramBotApiMethods.GetMe)]
		public async Task StartGetMeReturnsCorrelationId()
		{
			Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			SetDebugOutput(true);
			var result = StartGetMe();
			var successResponse = JsonConvert.DeserializeObject<Response<string>>(result);
			Assert.True(!string.IsNullOrWhiteSpace(successResponse.CorrelationKey));
			var messageStoreResult = await this.WaitForMessageStoreAsync(successResponse.CorrelationKey);

			Assert.IsInstanceOf<Response<User>>(JsonConvert.DeserializeObject<Response<User>>(messageStoreResult));
		}

		[Test]
		[Category(Constants.TelegramBotApiMethods.SendMessage)]
		public void SendTextSendsTextMessageToGroup()
		{
			Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			SetDebugOutput(true);
			var result = SendText(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_GROUP_ID.ToString()),
				$"{nameof(this.SendTextSendsTextMessageToGroup)}");
			var successResponse = JsonConvert.DeserializeObject<Response<Message>>(result);
			Assert.AreEqual($"{nameof(this.SendTextSendsTextMessageToGroup)}", successResponse.Content.Text);
		}

		[Test]
		[Category(Constants.TelegramBotApiMethods.SendMessage)]
		public void SendTextSendsTextMessageToChannel()
		{
			// Sending by @channelname only works if it is a public channel
			// SendText(Secrets.CHANNEL_NAME.ToString(), $"{nameof(this.SendTextSendsTextMessageToChannel)}")
			Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			SetDebugOutput(true);
			var result = SendText(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_CHANNEL_ID.ToString()),
				$"{nameof(this.SendTextSendsTextMessageToChannel)}");
			var successResponse = JsonConvert.DeserializeObject<Response<Message>>(result);
			Assert.AreEqual($"{nameof(this.SendTextSendsTextMessageToChannel)}", successResponse.Content.Text);
		}

		[Test]
		[Category(Constants.TelegramBotApiMethods.SendMessage)]
		public void SendTextSendsTextMessageToUser()
		{
			Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			SetDebugOutput(true);
			var result = SendText(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_USER_ID.ToString()),
				$"{nameof(this.SendTextSendsTextMessageToUser)}");
			var successResponse = JsonConvert.DeserializeObject<Response<Message>>(result);
			Assert.AreEqual($"{nameof(this.SendTextSendsTextMessageToUser)}", successResponse.Content.Text);
		}

		[Test]
		[Category(Constants.TelegramBotApiMethods.SendMessage)]
		public async Task StartSendTextReturnsCorrelationId()
		{
			Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			SetDebugOutput(true);
			var result = StartSendText(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_USER_ID.ToString()),
				$"{nameof(this.SendTextSendsTextMessageToUser)}");
			var successResponse = JsonConvert.DeserializeObject<Response<string>>(result);
			Assert.True(!string.IsNullOrWhiteSpace(successResponse.CorrelationKey));

			var messageStoreResult = await this.WaitForMessageStoreAsync(successResponse.CorrelationKey);

			Assert.IsInstanceOf<Response<Message>>(JsonConvert.DeserializeObject<Response<Message>>(messageStoreResult));
		}

		private async Task<string> WaitForMessageStoreAsync(string correlationKey)
		{
			var messageStoreResult = string.Empty;
			for (int i = 0; i <= 10; i++)
			{
				messageStoreResult = GetMessageByCorrelationId(correlationKey);
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

		#region DllImport

		[DllImport(Constants.AssemblyUnderTestName)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		private static extern string SetDebugOutput([MarshalAs(UnmanagedType.Bool)] bool enabled);

		[DllImport(Constants.AssemblyUnderTestName)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		private static extern string GetMessageByCorrelationId(
			[MarshalAs(UnmanagedType.LPWStr)] string correlationKey);

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

		[DllImport(Constants.AssemblyUnderTestName)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		private static extern string SendText(
			[MarshalAs(UnmanagedType.LPWStr)] string chatId,
			[MarshalAs(UnmanagedType.LPWStr)] string chatText);

		[DllImport(Constants.AssemblyUnderTestName)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		private static extern string StartSendText(
			[MarshalAs(UnmanagedType.LPWStr)] string chatId,
			[MarshalAs(UnmanagedType.LPWStr)] string chatText);

		#endregion

#endif
	}
}
