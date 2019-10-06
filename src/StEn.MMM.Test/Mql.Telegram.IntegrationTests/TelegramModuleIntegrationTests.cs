using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Mql.Telegram.IntegrationTests.Framework;
using Mql.Telegram.IntegrationTests.Helpers;
using Newtonsoft.Json;
using StEn.MMM.Mql.Common.Services.InApi.Entities;
using StEn.MMM.Mql.Telegram;
using Telegram.Bot.Types;
using Xunit;

namespace Mql.Telegram.IntegrationTests
{
	[Collection(Constants.Collections.IntegrationTests)]
	public class TelegramModuleIntegrationTests
	{
		public TelegramModuleIntegrationTests()
		{
			// https://colinmackay.scot/2007/06/16/unit-testing-a-static-class/
			Type staticType = typeof(TelegramModule);
			ConstructorInfo ci = staticType.TypeInitializer;
			object[] parameters = new object[0];
			ci.Invoke(null, parameters);

			TelegramModule.SetDebugOutput(true);
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.Internal)]
		public void RequestForCorrelationKeyReturnsErrorIfEntryIsNotFound()
		{
			TelegramModule.Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			TelegramModule.SetDebugOutput(true);
			TelegramModule.SetRequestTimeout(10);
			var result = TelegramModule.GetMessageByCorrelationId("test");

			var errorResponse = JsonConvert.DeserializeObject<Response<Error>>(result);
			Assert.False(errorResponse.IsSuccess);
			Assert.Equal(typeof(KeyNotFoundException).Name, errorResponse.Content.ExceptionType);
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.GetMe)]
		public void GetMeReturnsBotUser()
		{
			TelegramModule.Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			TelegramModule.SetDebugOutput(true);
			var result = TelegramModule.GetMe();
			var successResponse = JsonConvert.DeserializeObject<Response<User>>(result);
			Assert.True(successResponse.Content.IsBot);
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.GetMe)]
		public async Task StartGetMeReturnsCorrelationIdAsync()
		{
			TelegramModule.Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			TelegramModule.SetDebugOutput(true);
			var result = TelegramModule.StartGetMe();
			var successResponse = JsonConvert.DeserializeObject<Response<string>>(result);
			Assert.True(!string.IsNullOrWhiteSpace(successResponse.CorrelationKey));
			var messageStoreResult = await this.WaitForMessageStoreAsync(successResponse.CorrelationKey);

			Assert.IsType<Response<User>>(JsonConvert.DeserializeObject<Response<User>>(messageStoreResult));
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.GetUpdates)]
		public void GetUpdatesReturnsNullOrMoreMessages()
		{
			TelegramModule.Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			TelegramModule.SetDebugOutput(true);
			var result = TelegramModule.GetUpdates();
			var successResponse = JsonConvert.DeserializeObject<Response<Update[]>>(result);
			Assert.True(successResponse.IsSuccess);
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.GetUpdates)]
		public async Task StartGetUpdatesReturnsCorrelationIdAsync()
		{
			TelegramModule.Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			TelegramModule.SetDebugOutput(true);
			var result = TelegramModule.StartGetUpdates();
			var successResponse = JsonConvert.DeserializeObject<Response<string>>(result);
			Assert.True(!string.IsNullOrWhiteSpace(successResponse.CorrelationKey));

			var messageStoreResult = await this.WaitForMessageStoreAsync(successResponse.CorrelationKey);

			Assert.IsType<Response<Update[]>>(JsonConvert.DeserializeObject<Response<Update[]>>(messageStoreResult));
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.SendDocument)]
		public void SendDocumentSendsDocumentMessageToUser()
		{
			TelegramModule.Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			TelegramModule.SetDebugOutput(true);
			var result = TelegramModule.SendDocument(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_USER_ID.ToString()),
				$"assets/fake_log.txt");
			var successResponse = JsonConvert.DeserializeObject<Response<Message>>(result);
			Assert.NotEmpty(successResponse.Content.Document.FileId);
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.SendDocument)]
		public async Task StartSendDocumentReturnsCorrelationIdAsync()
		{
			TelegramModule.Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			TelegramModule.SetDebugOutput(true);
			var result = TelegramModule.StartSendDocument(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_USER_ID.ToString()),
				$"assets/fake_log.txt");
			var successResponse = JsonConvert.DeserializeObject<Response<string>>(result);
			Assert.True(!string.IsNullOrWhiteSpace(successResponse.CorrelationKey));

			var messageStoreResult = await this.WaitForMessageStoreAsync(successResponse.CorrelationKey);

			var correlatedResponse = JsonConvert.DeserializeObject<Response<Message>>(messageStoreResult);
			Assert.IsType<Response<Message>>(correlatedResponse);
			Assert.NotEmpty(correlatedResponse.Content.Document.FileId);
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.SendPhoto)]
		public void SendPhotoSendsPhotoMessageToUser()
		{
			TelegramModule.Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			TelegramModule.SetDebugOutput(true);
			var result = TelegramModule.SendPhoto(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_USER_ID.ToString()),
				$"assets/favicon-32x32.png");

			var successResponse = JsonConvert.DeserializeObject<Response<Message>>(result);
			Assert.NotEmpty(successResponse.Content.Photo);
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.SendPhoto)]
		public async Task StartSendPhotoReturnsCorrelationIdAsync()
		{
			TelegramModule.Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			TelegramModule.SetDebugOutput(true);
			var result = TelegramModule.StartSendPhoto(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_USER_ID.ToString()),
				$"assets/favicon-32x32.png");
			var successResponse = JsonConvert.DeserializeObject<Response<string>>(result);
			Assert.True(!string.IsNullOrWhiteSpace(successResponse.CorrelationKey));

			var messageStoreResult = await this.WaitForMessageStoreAsync(successResponse.CorrelationKey);

			var correlatedResponse = JsonConvert.DeserializeObject<Response<Message>>(messageStoreResult);
			Assert.IsType<Response<Message>>(correlatedResponse);
			Assert.NotEmpty(correlatedResponse.Content.Photo);
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.SendMessage)]
		public void SendTextSendsTextMessageToGroup()
		{
			TelegramModule.Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			TelegramModule.SetDebugOutput(true);
			var result = TelegramModule.SendText(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_GROUP_ID.ToString()),
				$"{nameof(this.SendTextSendsTextMessageToGroup)}");

			var successResponse = JsonConvert.DeserializeObject<Response<Message>>(result);
			Assert.Equal($"{nameof(this.SendTextSendsTextMessageToGroup)}", successResponse.Content.Text);
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.SendMessage)]
		public void SendTextSendsTextMessageToChannel()
		{
			// Sending by @channelname only works if it is a public channel
			// SendText(Secrets.CHANNEL_NAME.ToString(), $"{nameof(this.SendTextSendsTextMessageToChannel)}")
			TelegramModule.Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			TelegramModule.SetDebugOutput(true);
			var result = TelegramModule.SendText(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_CHANNEL_ID.ToString()),
				$"{nameof(this.SendTextSendsTextMessageToChannel)}");

			var successResponse = JsonConvert.DeserializeObject<Response<Message>>(result);
			Assert.Equal($"{nameof(this.SendTextSendsTextMessageToChannel)}", successResponse.Content.Text);
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.SendMessage)]
		public void SendTextSendsTextMessageToUser()
		{
			TelegramModule.Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			TelegramModule.SetDebugOutput(true);
			var result = TelegramModule.SendText(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_USER_ID.ToString()),
				$"{nameof(this.SendTextSendsTextMessageToUser)}");

			var successResponse = JsonConvert.DeserializeObject<Response<Message>>(result);
			Assert.Equal($"{nameof(this.SendTextSendsTextMessageToUser)}", successResponse.Content.Text);
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.SendMessage)]
		public void SendTextSendsTextMessageWithEmojiToUser()
		{
			TelegramModule.Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			TelegramModule.SetDebugOutput(true);
			var result = TelegramModule.SendText(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_USER_ID.ToString()),
				$"{nameof(this.SendTextSendsTextMessageToUser)} with \\U+1F601");

			var successResponse = JsonConvert.DeserializeObject<Response<Message>>(result);
			Assert.Equal($"{nameof(this.SendTextSendsTextMessageToUser)} with 😁", successResponse.Content.Text);
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.SendMessage)]
		public async Task StartSendTextReturnsCorrelationIdAsync()
		{
			TelegramModule.Initialize(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_BOT_API_KEY.ToString()),
				10);
			TelegramModule.SetDebugOutput(true);
			var result = TelegramModule.StartSendText(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.TELEGRAM_USER_ID.ToString()),
				$"{nameof(this.SendTextSendsTextMessageToUser)}");
			var successResponse = JsonConvert.DeserializeObject<Response<string>>(result);
			Assert.True(!string.IsNullOrWhiteSpace(successResponse.CorrelationKey));

			var messageStoreResult = await this.WaitForMessageStoreAsync(successResponse.CorrelationKey);

			var correlatedResponse = JsonConvert.DeserializeObject<Response<Message>>(messageStoreResult);

			Assert.IsType<Response<Message>>(correlatedResponse);
			Assert.Equal($"{nameof(this.SendTextSendsTextMessageToUser)}", correlatedResponse.Content.Text);
		}

		#region Helper Methods

		private async Task<string> WaitForMessageStoreAsync(string correlationKey)
		{
			var messageStoreResult = string.Empty;
			for (int i = 0; i <= 10; i++)
			{
				messageStoreResult = TelegramModule.GetMessageByCorrelationId(correlationKey);
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
