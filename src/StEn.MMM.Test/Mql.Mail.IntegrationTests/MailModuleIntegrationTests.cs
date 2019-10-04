using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Moq;
using Mql.Mail.IntegrationTests.Framework;
using Mql.Mail.IntegrationTests.Helpers;
using Newtonsoft.Json;
using NUnit.Framework;
using StEn.MMM.Mql.Common.Services.InApi.Entities;
using StEn.MMM.Mql.Mail;
using StEn.MMM.Mql.Mail.Services.Mail;
using Xunit;
using Assert = Xunit.Assert;

namespace Mql.Mail.IntegrationTests
{
	[Collection(Constants.Collections.IntegrationTests)]
	public class MailModuleIntegrationTests
	{
		public MailModuleIntegrationTests()
		{
			// https://colinmackay.scot/2007/06/16/unit-testing-a-static-class/
			Type staticType = typeof(MailModule);
			ConstructorInfo ci = staticType.TypeInitializer;
			object[] parameters = new object[0];
			ci.Invoke(null, parameters);

			MailModule.SetDebugOutput(true);
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.Methods.SendMail)]
		public void SendMailSucceeds()
		{
			this.CallBasicModuleInit();

			var response = MailModule.SendMail(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.MAIL_SENDER_MAIL),
				new[]
				{
					MBTHelper.ConvertMaskedSecretToRealValue(Secrets.MAIL_RECIPIENT_MAIL_1),
					MBTHelper.ConvertMaskedSecretToRealValue(Secrets.MAIL_RECIPIENT_MAIL_2),
					MBTHelper.ConvertMaskedSecretToRealValue(Secrets.MAIL_RECIPIENT_MAIL_3),
				},
				"test",
				"test \\U+1F601",
				"<b>test</b> \\U+1F601");
			Assert.True(JsonConvert.DeserializeObject<Response<string>>(response).IsSuccess);
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.Methods.SendMail)]
		public async Task SendMailSucceedsAsync()
		{
			this.CallBasicModuleInit();

			var response = MailModule.StartSendMail(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.MAIL_SENDER_MAIL),
				new[]
				{
					MBTHelper.ConvertMaskedSecretToRealValue(Secrets.MAIL_RECIPIENT_MAIL_1),
					MBTHelper.ConvertMaskedSecretToRealValue(Secrets.MAIL_RECIPIENT_MAIL_2),
					MBTHelper.ConvertMaskedSecretToRealValue(Secrets.MAIL_RECIPIENT_MAIL_3),
				},
				"test",
				"test",
				"<b>test</b>");
			var successResponse = JsonConvert.DeserializeObject<Response<string>>(response);
			Assert.True(!string.IsNullOrWhiteSpace(successResponse.CorrelationKey));

			var messageStoreResult = await this.WaitForMessageStoreAsync(successResponse.CorrelationKey);

			Assert.IsType<Response<string>>(JsonConvert.DeserializeObject<Response<string>>(messageStoreResult));
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.Methods.SendMail)]
		public void SendMailWithAttachmentSucceeds()
		{
			this.CallBasicModuleInit();

			var response = MailModule.SendMail(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.MAIL_SENDER_MAIL),
				new[]
				{
					MBTHelper.ConvertMaskedSecretToRealValue(Secrets.MAIL_RECIPIENT_MAIL_1),
				},
				"test",
				"test",
				"<b>test</b>",
				new[]
				{
					"assets/favicon-32x32.png",
					"assets/fake_log.txt",
				});
			Assert.True(JsonConvert.DeserializeObject<Response<string>>(response).IsSuccess);
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.Methods.SendMail)]
		public async Task SendMailWithAttachmentSucceedsAsync()
		{
			this.CallBasicModuleInit();

			var response = MailModule.StartSendMail(
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.MAIL_SENDER_MAIL),
				new[]
				{
					MBTHelper.ConvertMaskedSecretToRealValue(Secrets.MAIL_RECIPIENT_MAIL_1),
				},
				"test",
				"test",
				"<b>test</b>",
				new[]
				{
					"assets/favicon-32x32.png",
					"assets/fake_log.txt",
				});
			var successResponse = JsonConvert.DeserializeObject<Response<string>>(response);
			Assert.True(!string.IsNullOrWhiteSpace(successResponse.CorrelationKey));

			var messageStoreResult = await this.WaitForMessageStoreAsync(successResponse.CorrelationKey);

			Assert.IsType<Response<string>>(JsonConvert.DeserializeObject<Response<string>>(messageStoreResult));
		}

		#region Helper Methods

		private void CallBasicModuleInit()
		{
			MailModule.Initialize(
				Constants.MailServer.SmtpHostName,
				Constants.MailServer.SmtpPort,
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.MAIL_PROVIDER_USER_NAME.ToString()),
				MBTHelper.ConvertMaskedSecretToRealValue(Secrets.MAIL_PROVIDER_USER_PASS.ToString()),
				30);
		}

		private async Task<string> WaitForMessageStoreAsync(string correlationKey)
		{
			var messageStoreResult = string.Empty;
			for (int i = 0; i <= 10; i++)
			{
				messageStoreResult = MailModule.GetMessageByCorrelationId(correlationKey);
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
