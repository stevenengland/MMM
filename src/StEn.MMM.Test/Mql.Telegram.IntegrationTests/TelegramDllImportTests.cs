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
		[Category(Constants.TelegramBotApiMethods.Internal)]
		public void ConfigurationFunctionsCanBeCalled()
		{
			Assert.IsInstanceOf<Response<string>>(JsonConvert.DeserializeObject<Response<string>>(
				Initialize(
					MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
					10)));

			Assert.IsInstanceOf<Response<string>>(JsonConvert.DeserializeObject<Response<string>>(
				SetDebugOutput(true)));

			Assert.IsInstanceOf<Response<string>>(JsonConvert.DeserializeObject<Response<string>>(
				SetRequestTimeout(10)));

			Assert.IsInstanceOf<Response<string>>(JsonConvert.DeserializeObject<Response<string>>(
				SetDefaultValue("ParseMode", "html")));
		}

		[Test]
		[Category(Constants.TelegramBotApiMethods.GetMe)]
		public void RequestForCorrelationKeyReturnsErrorIfEntryIsNotFound()
		{
			Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			SetDebugOutput(true);
			SetRequestTimeout(10);
			var result = GetMessageByCorrelationId("test");
			Console.WriteLine($"JSON for {nameof(result)} is: {result}");

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
			Console.WriteLine($"JSON for {nameof(result)} is: {result}");
			var successResponse = JsonConvert.DeserializeObject<Response<User>>(result);
			Assert.True(successResponse.Content.IsBot);
		}

		[Test]
		[Category(Constants.TelegramBotApiMethods.GetMe)]
		public async Task StartGetMeReturnsCorrelationIdAsync()
		{
			Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			SetDebugOutput(true);
			var result = StartGetMe();
			var successResponse = JsonConvert.DeserializeObject<Response<string>>(result);
			Assert.True(!string.IsNullOrWhiteSpace(successResponse.CorrelationKey));
			var messageStoreResult = await this.WaitForMessageStoreAsync(successResponse.CorrelationKey);
			Console.WriteLine($"JSON for {nameof(messageStoreResult)} is: {messageStoreResult}");

			Assert.IsInstanceOf<Response<User>>(JsonConvert.DeserializeObject<Response<User>>(messageStoreResult));
		}

		[Test]
		[Category(Constants.TelegramBotApiMethods.GetUpdates)]
		public void GetUpdatesReturnsNullOrMoreMessages()
		{
			Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			SetDebugOutput(true);
			var result = GetUpdates();
			Console.WriteLine($"JSON for {nameof(result)} is: {result}");
			var successResponse = JsonConvert.DeserializeObject<Response<Update[]>>(result);
			Assert.True(successResponse.IsSuccess);
		}

		[Test]
		[Category(Constants.TelegramBotApiMethods.GetUpdates)]
		public async Task StartGetUpdatesReturnsCorrelationIdAsync()
		{
			Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			SetDebugOutput(true);
			var result = StartGetUpdates();
			var successResponse = JsonConvert.DeserializeObject<Response<string>>(result);
			Assert.True(!string.IsNullOrWhiteSpace(successResponse.CorrelationKey));

			var messageStoreResult = await this.WaitForMessageStoreAsync(successResponse.CorrelationKey);
			Console.WriteLine($"JSON for {nameof(messageStoreResult)} is: {messageStoreResult}");

			Assert.IsInstanceOf<Response<Update[]>>(JsonConvert.DeserializeObject<Response<Update[]>>(messageStoreResult));
		}

		[Test]
		[Category(Constants.TelegramBotApiMethods.SendDocument)]
		public void SendDocumentSendsDocumentMessageToUser()
		{
			Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			SetDebugOutput(true);
			var result = SendDocument(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_USER_ID.ToString()),
				$"assets/fake_log.txt");
			Console.WriteLine($"JSON for {nameof(result)} is: {result}");
			var successResponse = JsonConvert.DeserializeObject<Response<Message>>(result);
			Assert.IsNotEmpty(successResponse.Content.Document.FileId);
		}

		[Test]
		[Category(Constants.TelegramBotApiMethods.SendDocument)]
		public async Task StartSendDocumentReturnsCorrelationIdAsync()
		{
			Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			SetDebugOutput(true);
			var result = StartSendDocument(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_USER_ID.ToString()),
				$"assets/fake_log.txt");
			var successResponse = JsonConvert.DeserializeObject<Response<string>>(result);
			Assert.True(!string.IsNullOrWhiteSpace(successResponse.CorrelationKey));

			var messageStoreResult = await this.WaitForMessageStoreAsync(successResponse.CorrelationKey);
			Console.WriteLine($"JSON for {nameof(messageStoreResult)} is: {messageStoreResult}");

			var correlatedResponse = JsonConvert.DeserializeObject<Response<Message>>(messageStoreResult);
			Assert.IsInstanceOf<Response<Message>>(correlatedResponse);
			Assert.IsNotEmpty(correlatedResponse.Content.Document.FileId);
		}

		[Test]
		[Category(Constants.TelegramBotApiMethods.SendPhoto)]
		public void SendPhotoSendsPhotoMessageToUser()
		{
			Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			SetDebugOutput(true);
			var result = SendPhoto(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_USER_ID.ToString()),
				$"assets/favicon-32x32.png");
			Console.WriteLine($"JSON for {nameof(result)} is: {result}");
			var successResponse = JsonConvert.DeserializeObject<Response<Message>>(result);
			Assert.IsNotEmpty(successResponse.Content.Photo);
		}

		[Test]
		[Category(Constants.TelegramBotApiMethods.SendPhoto)]
		public async Task StartSendPhotoReturnsCorrelationIdAsync()
		{
			Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			SetDebugOutput(true);
			var result = StartSendPhoto(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_USER_ID.ToString()),
				$"assets/favicon-32x32.png");
			var successResponse = JsonConvert.DeserializeObject<Response<string>>(result);
			Assert.True(!string.IsNullOrWhiteSpace(successResponse.CorrelationKey));

			var messageStoreResult = await this.WaitForMessageStoreAsync(successResponse.CorrelationKey);
			Console.WriteLine($"JSON for {nameof(messageStoreResult)} is: {messageStoreResult}");

			var correlatedResponse = JsonConvert.DeserializeObject<Response<Message>>(messageStoreResult);
			Assert.IsInstanceOf<Response<Message>>(correlatedResponse);
			Assert.IsNotEmpty(correlatedResponse.Content.Photo);
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
			Console.WriteLine($"JSON for {nameof(result)} is: {result}");
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
			Console.WriteLine($"JSON for {nameof(result)} is: {result}");
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
			Console.WriteLine($"JSON for {nameof(result)} is: {result}");
			var successResponse = JsonConvert.DeserializeObject<Response<Message>>(result);
			Assert.AreEqual($"{nameof(this.SendTextSendsTextMessageToUser)}", successResponse.Content.Text);
		}

		[Test]
		[Category(Constants.TelegramBotApiMethods.SendMessage)]
		public void SendTextSendsTextMessageWithEmojiToUser()
		{
			Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			SetDebugOutput(true);
			var result = SendText(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_USER_ID.ToString()),
				$"{nameof(this.SendTextSendsTextMessageToUser)} with \\U+1F601");
			Console.WriteLine($"JSON for {nameof(result)} is: {result}");
			var successResponse = JsonConvert.DeserializeObject<Response<Message>>(result);
			Assert.AreEqual($"{nameof(this.SendTextSendsTextMessageToUser)} with 😁", successResponse.Content.Text);
		}

		[Test]
		[Category(Constants.TelegramBotApiMethods.SendMessage)]
		public async Task StartSendTextReturnsCorrelationIdAsync()
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
			Console.WriteLine($"JSON for {nameof(messageStoreResult)} is: {messageStoreResult}");

			var correlatedResponse = JsonConvert.DeserializeObject<Response<Message>>(messageStoreResult);

			Assert.IsInstanceOf<Response<Message>>(correlatedResponse);
			Assert.AreEqual($"{nameof(this.SendTextSendsTextMessageToUser)}", correlatedResponse.Content.Text);
		}

		#region DllImport

		[DllImport(Constants.AssemblyUnderTestName)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		private static extern string SetRequestTimeout(int timeout);

		[DllImport(Constants.AssemblyUnderTestName)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		private static extern string SetDebugOutput([MarshalAs(UnmanagedType.Bool)] bool enabled);

		[DllImport(Constants.AssemblyUnderTestName)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		private static extern string SetDefaultValue(
			[MarshalAs(UnmanagedType.LPWStr)] string parameterKey,
			[MarshalAs(UnmanagedType.LPWStr)] string defaultValue);

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
		private static extern string GetUpdates();

		[DllImport(Constants.AssemblyUnderTestName)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		private static extern string StartGetUpdates();

		[DllImport(Constants.AssemblyUnderTestName)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		private static extern string SendDocument(
			[MarshalAs(UnmanagedType.LPWStr)] string chatId,
			[MarshalAs(UnmanagedType.LPWStr)] string file);

		[DllImport(Constants.AssemblyUnderTestName)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		private static extern string StartSendDocument(
			[MarshalAs(UnmanagedType.LPWStr)] string chatId,
			[MarshalAs(UnmanagedType.LPWStr)] string file);

		[DllImport(Constants.AssemblyUnderTestName)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		private static extern string SendPhoto(
			[MarshalAs(UnmanagedType.LPWStr)] string chatId,
			[MarshalAs(UnmanagedType.LPWStr)] string photoFile);

		[DllImport(Constants.AssemblyUnderTestName)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		private static extern string StartSendPhoto(
			[MarshalAs(UnmanagedType.LPWStr)] string chatId,
			[MarshalAs(UnmanagedType.LPWStr)] string photoFile);

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

		#region Helper Methods

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

		#endregion
#endif
	}
}
