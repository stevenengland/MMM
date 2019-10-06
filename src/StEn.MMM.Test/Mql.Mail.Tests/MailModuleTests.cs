using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Moq;
using Mql.Mail.Tests.Framework;
using Newtonsoft.Json;
using StEn.MMM.Mql.Common.Services.InApi.Entities;
using StEn.MMM.Mql.Mail;
using StEn.MMM.Mql.Mail.Services.Mail;
using Xunit;

namespace Mql.Mail.Tests
{
	[Collection(Constants.Collections.Tests)]
	public class MailModuleTests
	{
		public MailModuleTests()
		{
			// https://colinmackay.scot/2007/06/16/unit-testing-a-static-class/
			Type staticType = typeof(MailModule);
			ConstructorInfo ci = staticType.TypeInitializer;
			object[] parameters = new object[0];
			ci.Invoke(null, parameters);

			MailModule.SetDebugOutput(true);
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.Methods.Internal)]
		public void InitializationSucceeds()
		{
			var exportResponse = this.CallBasicModuleInit();
			var response = JsonConvert.DeserializeObject<Response<string>>(exportResponse);
			Assert.IsType<Response<string>>(response);
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.Methods.Internal)]
		public void InitializationValuesAreValidated()
		{
			var mock = new Mock<IMailMapper>();

			this.CallBasicModuleInit();
			MailModule.MailMapper = mock.Object;

			Assert.Contains("smtpServer", MailModule.Initialize(
				string.Empty,
				0,
				string.Empty,
				string.Empty,
				0));
			Assert.Contains("smtpPort", MailModule.Initialize(
				"test",
				0,
				string.Empty,
				string.Empty,
				0));
			Assert.Contains("smtpUserName", MailModule.Initialize(
				"test",
				1,
				string.Empty,
				string.Empty,
				0));
			Assert.Contains("smtpPassword", MailModule.Initialize(
				"test",
				1,
				"test",
				string.Empty,
				0));
			Assert.Contains("timeout", MailModule.Initialize(
				"test",
				1,
				"test",
				"test",
				0));
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.Methods.Internal)]
		public void MissingInitializationLeedsToErrors()
		{
			var exportResponse = MailModule.GetMessageByCorrelationId("ss");
			var responseError = JsonConvert.DeserializeObject<Response<Error>>(exportResponse);
			Assert.Equal(typeof(ApplicationException).Name, responseError.Content.ExceptionType);
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.Methods.Internal)]
		public void TimeoutIsSet()
		{
			this.CallBasicModuleInit();
			Assert.True(MailModule.MailMapper.RequestTimeout == 30000);
			MailModule.SetRequestTimeout(20);
			Assert.True(MailModule.MailMapper.RequestTimeout == 20000);
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.Methods.Internal)]
		[Category(Constants.Methods.Internal)]
		public void RequestForCorrelationKeyReturnsErrorIfEntryIsNotFound()
		{
			this.CallBasicModuleInit();
			MailModule.SetDebugOutput(true);
			MailModule.SetRequestTimeout(10);
			var result = MailModule.GetMessageByCorrelationId("test");

			var errorResponse = JsonConvert.DeserializeObject<Response<Error>>(result);
			Assert.False(errorResponse.IsSuccess);
			Assert.Equal(typeof(KeyNotFoundException).Name, errorResponse.Content.ExceptionType);
		}

		[Fact]
		public void DefaultValuesAreSet()
		{
			this.CallBasicModuleInit();

			MailModule.SetDefaultValue(nameof(MailModule.MailMapper.CheckServerCertificate), "false");
			Assert.False(MailModule.MailMapper.CheckServerCertificate);
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.Methods.Internal)]
		public void SetDefaultValueInputsAreValidated()
		{
			var mock = new Mock<IMailMapper>();

			this.CallBasicModuleInit();
			MailModule.MailMapper = mock.Object;

			Assert.Contains(nameof(ArgumentException), MailModule.SetDefaultValue(string.Empty, "some text"));
			Assert.Contains(nameof(ArgumentException), MailModule.SetDefaultValue("some text", string.Empty));
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.Methods.Internal)]
		[Trait(Constants.MethodTraitName, Constants.Methods.SendMail)]
		public void SendMailInputsAreValidated()
		{
			var mock = new Mock<IMailMapper>();

			this.CallBasicModuleInit();
			MailModule.MailMapper = mock.Object;

			Assert.Contains("sender", MailModule.SendMail(
				string.Empty,
				new[] { string.Empty },
				string.Empty,
				string.Empty,
				string.Empty));
			Assert.Contains("recipients", MailModule.SendMail(
				"test",
				new[] { string.Empty },
				string.Empty,
				string.Empty,
				string.Empty));
			Assert.Contains("subject", MailModule.SendMail(
				"test",
				new[] { "test" },
				string.Empty,
				string.Empty,
				string.Empty));
			Assert.Contains("textBody", MailModule.SendMail(
				"test",
				new[] { "test" },
				"test",
				string.Empty,
				string.Empty));
			Assert.Contains("htmlBody", MailModule.SendMail(
				"test",
				new[] { "test" },
				"test",
				string.Empty,
				string.Empty));
		}

		[Fact]
		[Trait(Constants.MethodTraitName, Constants.Methods.SendMail)]
		public void SendMailSucceeds()
		{
			var mock = new Mock<IMailMapper>();
			mock.Setup(
				x => x.SendMail(
					It.IsAny<string>(),
					It.IsAny<IEnumerable<string>>(),
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<string>(),
					null)).Returns("ok");

			mock.Setup(
				x => x.SendMail(
				It.IsAny<string>(),
				It.IsAny<IEnumerable<string>>(),
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<IEnumerable<string>>())).Returns("ok");

			this.CallBasicModuleInit();
			MailModule.MailMapper = mock.Object;

			Assert.True(MailModule.SendMail(
							"test",
							new[] { "test" },
							"test",
							"test",
							"test") == "ok");

			Assert.True(MailModule.SendMail(
				            "test",
				            new[] { "test" },
				            "test",
				            "test",
				            "test",
				            new[] { "test" }) == "ok");
		}

		#region Helper Methods

		private string CallBasicModuleInit()
		{
			return MailModule.Initialize(
				"test.test.com",
				465,
				"test",
				"test",
				30);
		}

		#endregion
	}
}
