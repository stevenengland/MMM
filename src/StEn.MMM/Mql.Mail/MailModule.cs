using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using Org.BouncyCastle.Asn1.X509;
using StEn.MMM.Mql.Common.Base.Attributes;
using StEn.MMM.Mql.Common.Base.Utilities;
using StEn.MMM.Mql.Common.Services.InApi.Factories;
using StEn.MMM.Mql.Mail.Services.Mail;

namespace StEn.MMM.Mql.Mail
{
	/// <summary>
	/// Contains exports for the email module.
	/// </summary>
	public class MailModule
	{
		private static IMailMapper mailMapper;

		private static bool isInitialized;

		private static ResponseFactory responseFactory = new ResponseFactory();

		static MailModule()
		{
			// https://colinmackay.scot/2007/06/16/unit-testing-a-static-class/
			ResetClass();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MailModule"/> class.
		/// </summary>
		protected MailModule()
		{
		}

		/// <summary>
		/// Gets or sets the email client to be used. In order to use it you must call <see cref="Initialize"/> first.
		/// The public non static constructor is meant for testing only.
		/// </summary>
		public static IMailMapper MailMapper
		{
			get
			{
				Ensure.That<ApplicationException>(isInitialized, $"The framework is not initialized yet. Please run the {nameof(Initialize)} method first.");
				return mailMapper;
			}
			set => mailMapper = value;
		}

		/// <summary>
		/// Sends an email.
		/// </summary>
		/// <param name="sender">
		/// Identifier for sender (FROM header). You have different options for identifiers:
		/// <list type="bullet">
		///   <item>A plain email address like name@domain.com</item>
		///   <item>A nick name format like NickName &lt;name@domain.com&gt;</item>
		/// </list>
		/// Please be aware of the correct format in the latter case.
		/// </param>
		/// <param name="recipients">
		/// A list of email addresses you like to send the mails to. Add an identifier in front of each recipient that indicates the kind of recipient:
		/// <list type="bullet">
		///   <item>1-n addresses like to:name@domain.com -> will be send as "TO"</item>
		///   <item>0-n addresses like cc:name@domain.com -> will be send as "CC"</item>
		///   <item>0-n addresses like bcc:name@domain.com -> will be send as "BCC"</item>
		/// </list>
		/// Like in the sender field, you can also use nick name styles like NickName &lt;name@domain.com&gt;.
		/// </param>
		/// <param name="subject">The subject of your email.</param>
		/// <param name="textBody">The plain text body of your email. You can fill just this one or both, the plain text body and the HTML body.</param>
		/// <param name="htmlBody">The HTML style body of your email. You can fill just this one or both, the plain text body and the HTML body.</param>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success there is no payload but only the success flag in the header data.
		/// </returns>
		[MqlFuncDoc(AdditionalCodeLines = @"// See the different usage and writing possibilities of the recipients:
	// You need one ore more 'to' addresses and optional 'cc', 'bcc'.
	// You can write plain mail addresses or those with a nick name.
	string recipients[] = {""to:recipient1 <r1@sten.info>"", ""to:r2@sten.info"", ""cc:r3@sten.info"", ""bcc:r4@sten.info""};
")]
		public static string SendMail(
			[MqlParamDoc(ExampleValue = "StEn <test@sten.info>")]
			string sender,
			[MqlParamDoc(ExampleValue = "recipients")]
			string[] recipients,
			[MqlParamDoc(ExampleValue = "test subject")]
			string subject,
			[MqlParamDoc(ExampleValue = "plain text body")]
			string textBody,
			[MqlParamDoc(ExampleValue = "<b>html</b> body")]
			string htmlBody)
		{
			try
			{
				Ensure.NotNullOrEmptyOrWhiteSpace(sender, $"The argument {nameof(sender)} must not be empty or just whitespace.");
				Ensure.That<ArgumentException>(recipients.Any(), $"The argument {nameof(recipients)} does not contain any value.");
				Ensure.NotNullOrEmptyOrWhiteSpace(subject, $"The argument {nameof(subject)} must not be empty or just whitespace.");
				Ensure.That<ArgumentException>(!string.IsNullOrWhiteSpace(htmlBody + textBody), $"Either {nameof(textBody)} or {nameof(htmlBody)} must be provided but both are empty.");
				return MailMapper.SendMail(
					sender: sender,
					recipients: recipients,
					subject: subject,
					textBody: textBody,
					htmlBody: htmlBody);
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		/// <summary>
		/// Sends an email.
		/// </summary>
		/// <param name="sender">
		/// Identifier for sender (FROM header). You have different options for identifiers:
		/// <list type="bullet">
		///   <item>A plain email address like name@domain.com</item>
		///   <item>A nick name format like NickName &lt;name@domain.com&gt;</item>
		/// </list>
		/// Please be aware of the correct format in the latter case.
		/// </param>
		/// <param name="recipients">
		/// A list of email addresses you like to send the mails to. Add an identifier in front of each recipient that indicates the kind of recipient:
		/// <list type="bullet">
		///   <item>1-n addresses like to:name@domain.com -> will be send as "TO"</item>
		///   <item>0-n addresses like cc:name@domain.com -> will be send as "CC"</item>
		///   <item>0-n addresses like bcc:name@domain.com -> will be send as "BCC"</item>
		/// </list>
		/// Like in the sender field, you can also use nick name styles like NickName &lt;name@domain.com&gt;.
		/// </param>
		/// <param name="subject">The subject of your email.</param>
		/// <param name="textBody">The plain text body of your email. You can fill just this one or both, the plain text body and the HTML body.</param>
		/// <param name="htmlBody">The HTML style body of your email. You can fill just this one or both, the plain text body and the HTML body.</param>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success there is no payload but only the success flag in the header data.
		/// </returns>
		/// <remarks>
		/// <para>This method starts an independent background thread and immediately returns a response with a correlation key but empty payload.</para>
		/// <para>You can use the correlation key to check the result of the thread later via <see cref="GetMessageByCorrelationId"/>.</para>
		/// </remarks>
		[MqlFuncDoc]
		public static string StartSendMail(
			[MqlParamDoc(ExampleValue = "StEn <test@sten.info>")]
			string sender,
			[MqlParamDoc(ExampleValue = "recipients")]
			string[] recipients,
			[MqlParamDoc(ExampleValue = "test subject")]
			string subject,
			[MqlParamDoc(ExampleValue = "plain text body")]
			string textBody,
			[MqlParamDoc(ExampleValue = "<b>html</b> body")]
			string htmlBody)
		{
			try
			{
				Ensure.NotNullOrEmptyOrWhiteSpace(sender, $"The argument {nameof(sender)} must not be empty or just whitespace.");
				Ensure.That<ArgumentException>(recipients.Any(), $"The argument {nameof(recipients)} does not contain any value.");
				Ensure.NotNullOrEmptyOrWhiteSpace(subject, $"The argument {nameof(subject)} must not be empty or just whitespace.");
				Ensure.That<ArgumentException>(!string.IsNullOrWhiteSpace(htmlBody + textBody), $"Either {nameof(textBody)} or {nameof(htmlBody)} must be provided but both are empty.");
				return MailMapper.StartSendMail(
					sender: sender,
					recipients: recipients,
					subject: subject,
					textBody: textBody,
					htmlBody: htmlBody);
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		/// <summary>
		/// Sends an email.
		/// </summary>
		/// <param name="sender">
		/// Identifier for sender (FROM header). You have different options for identifiers:
		/// <list type="bullet">
		///   <item>A plain email address like name@domain.com</item>
		///   <item>A nick name format like NickName &lt;name@domain.com&gt;</item>
		/// </list>
		/// Please be aware of the correct format in the latter case.
		/// </param>
		/// <param name="recipients">
		/// A list of email addresses you like to send the mails to. Add an identifier in front of each recipient that indicates the kind of recipient:
		/// <list type="bullet">
		///   <item>1-n addresses like to:name@domain.com -> will be send as "TO"</item>
		///   <item>0-n addresses like cc:name@domain.com -> will be send as "CC"</item>
		///   <item>0-n addresses like bcc:name@domain.com -> will be send as "BCC"</item>
		/// </list>
		/// Like in the sender field, you can also use nick name styles like NickName &lt;name@domain.com&gt;.
		/// </param>
		/// <param name="subject">The subject of your email.</param>
		/// <param name="textBody">The plain text body of your email. You can fill just this one or both, the plain text body and the HTML body.</param>
		/// <param name="htmlBody">The HTML style body of your email. You can fill just this one or both, the plain text body and the HTML body.</param>
		/// <param name="attachments">A list of paths to files you like to send as attachments.</param>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success there is no payload but only the success flag in the header data.
		/// </returns>
		[MqlFuncDoc(AdditionalCodeLines = @"string attachments[1];
	attachments[0] = TerminalInfoString(TERMINAL_DATA_PATH) + ""\\MQL5\\Libraries\\""+""photo.png"";
")]
		public static string SendMail(
			[MqlParamDoc(ExampleValue = "StEn <test@sten.info>")]
			string sender,
			[MqlParamDoc(ExampleValue = "recipients")]
			string[] recipients,
			[MqlParamDoc(ExampleValue = "test subject")]
			string subject,
			[MqlParamDoc(ExampleValue = "plain text body")]
			string textBody,
			[MqlParamDoc(ExampleValue = "<b>html</b> body")]
			string htmlBody,
			[MqlParamDoc(ExampleValue = "attachments")]
			string[] attachments)
		{
			try
			{
				Ensure.NotNullOrEmptyOrWhiteSpace(sender, $"The argument {nameof(sender)} must not be empty or just whitespace.");
				Ensure.That<ArgumentException>(recipients.Any(), $"The argument {nameof(recipients)} does not contain any value.");
				Ensure.NotNullOrEmptyOrWhiteSpace(subject, $"The argument {nameof(subject)} must not be empty or just whitespace.");
				Ensure.That<ArgumentException>(!string.IsNullOrWhiteSpace(htmlBody + textBody), $"Either {nameof(textBody)} or {nameof(htmlBody)} must be provided but both are empty.");
				Ensure.That<ArgumentException>(attachments.Any(), $"The argument {nameof(attachments)} does not contain any value.");
				return MailMapper.SendMail(
					sender: sender,
					recipients: recipients,
					subject: subject,
					textBody: textBody,
					htmlBody: htmlBody,
					attachments: attachments);
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		/// <summary>
		/// Sends an email.
		/// </summary>
		/// <param name="sender">
		/// Identifier for sender (FROM header). You have different options for identifiers:
		/// <list type="bullet">
		///   <item>A plain email address like name@domain.com</item>
		///   <item>A nick name format like NickName &lt;name@domain.com&gt;</item>
		/// </list>
		/// Please be aware of the correct format in the latter case.
		/// </param>
		/// <param name="recipients">
		/// A list of email addresses you like to send the mails to. Add an identifier in front of each recipient that indicates the kind of recipient:
		/// <list type="bullet">
		///   <item>1-n addresses like to:name@domain.com -> will be send as "TO"</item>
		///   <item>0-n addresses like cc:name@domain.com -> will be send as "CC"</item>
		///   <item>0-n addresses like bcc:name@domain.com -> will be send as "BCC"</item>
		/// </list>
		/// Like in the sender field, you can also use nick name styles like NickName &lt;name@domain.com&gt;.
		/// </param>
		/// <param name="subject">The subject of your email.</param>
		/// <param name="textBody">The plain text body of your email. You can fill just this one or both, the plain text body and the HTML body.</param>
		/// <param name="htmlBody">The HTML style body of your email. You can fill just this one or both, the plain text body and the HTML body.</param>
		/// <param name="attachments">A list of paths to files you like to send as attachments.</param>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success there is no payload but only the success flag in the header data.
		/// </returns>
		/// <remarks>
		/// <para>This method starts an independent background thread and immediately returns a response with a correlation key but empty payload.</para>
		/// <para>You can use the correlation key to check the result of the thread later via <see cref="GetMessageByCorrelationId"/>.</para>
		/// </remarks>
		[MqlFuncDoc]
		public static string StartSendMail(
			[MqlParamDoc(ExampleValue = "StEn <test@sten.info>")]
			string sender,
			[MqlParamDoc(ExampleValue = "recipients")]
			string[] recipients,
			[MqlParamDoc(ExampleValue = "test subject")]
			string subject,
			[MqlParamDoc(ExampleValue = "plain text body")]
			string textBody,
			[MqlParamDoc(ExampleValue = "<b>html</b> body")]
			string htmlBody,
			[MqlParamDoc(ExampleValue = "attachments")]
			string[] attachments)
		{
			try
			{
				Ensure.NotNullOrEmptyOrWhiteSpace(sender, $"The argument {nameof(sender)} must not be empty or just whitespace.");
				Ensure.That<ArgumentException>(recipients.Any(), $"The argument {nameof(recipients)} does not contain any value.");
				Ensure.NotNullOrEmptyOrWhiteSpace(subject, $"The argument {nameof(subject)} must not be empty or just whitespace.");
				Ensure.That<ArgumentException>(!string.IsNullOrWhiteSpace(htmlBody + textBody), $"Either {nameof(textBody)} or {nameof(htmlBody)} must be provided but both are empty.");
				Ensure.That<ArgumentException>(attachments.Any(), $"The argument {nameof(attachments)} does not contain any value.");
				return MailMapper.StartSendMail(
					sender: sender,
					recipients: recipients,
					subject: subject,
					textBody: textBody,
					htmlBody: htmlBody,
					attachments: attachments);
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		/// <summary>
		/// Use this method to check if and which result for a given correlation key was obtained.
		/// </summary>
		/// <param name="correlationKey">The correlation key that was provided by a "Start" method.</param>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success the payload is exactly of that type that was obtained from the corresponding "Start" method that generated the correlation key.
		/// If the correlation key was not found the corresponding method has not finished yet.
		/// </returns>
		[MqlFuncDoc]
		public static string GetMessageByCorrelationId(
			[MqlParamDoc(ExampleValue = "w8er4345grt76567")]
			string correlationKey)
		{
			try
			{
				Ensure.NotNullOrEmptyOrWhiteSpace(correlationKey, $"The argument {nameof(correlationKey)} must not be empty or just whitespace.");
				return MailMapper.GetMessageByCorrelationId(correlationKey);
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		#region Configuration API

		/// <summary>
		/// Must be called first before any other method is used. It initializes the framework.
		/// </summary>
		/// <param name="smtpServer">The host name of the SMTP server.</param>
		/// <param name="smtpPort">The port of the SMTP server.</param>
		/// <param name="smtpUserName">The username for the SMTP account.</param>
		/// <param name="smtpPassword">The password for the SMTP account.</param>
		/// <param name="timeout">Seconds that a request to the server can last before the call gets cancelled.</param>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success there is no payload but only the success flag in the header data.
		/// </returns>
		[MqlFuncDoc(Order = 1)]
		public static string Initialize(
			[MqlParamDoc(ExampleValue = "smtp.strato.de")]
			string smtpServer,
			[MqlParamDoc(ExampleValue = "465")]
			int smtpPort,
			[MqlParamDoc(ExampleValue = "integration_test@mmm.steven-england.info")]
			string smtpUserName,
			[MqlParamDoc(ExampleValue = "aksddf933++")]
			string smtpPassword,
			[MqlParamDoc(ExampleValue = "10")]
			int timeout)
		{
			try
			{
				Ensure.NotNullOrEmptyOrWhiteSpace(smtpServer, $"The argument {nameof(smtpServer)} must not be empty or just whitespace.");
				Ensure.That<ArgumentException>(smtpPort > 0, $"The argument {nameof(smtpPort)} must be greater than 0.");
				Ensure.NotNullOrEmptyOrWhiteSpace(smtpUserName, $"The argument {nameof(smtpUserName)} must not be empty or just whitespace.");
				Ensure.NotNullOrEmptyOrWhiteSpace(smtpPassword, $"The argument {nameof(smtpPassword)} must not be empty or just whitespace.");
				Ensure.That<ArgumentException>(timeout > 0, $"The argument {nameof(timeout)} must be greater than 0.");

				InitializeClass(new MailKitMapper(
					smtpUserName,
					smtpPassword,
					smtpServer,
					smtpPort,
					responseFactory));
				SetTimeout(timeout);
				return responseFactory.Success().ToString();
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		/// <summary>
		/// Adjusts the timespan that a call to the server can last before it gets cancelled.
		/// </summary>
		/// <param name="timeout">Seconds that a request to the server can last.</param>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success there is no payload but only the success flag in the header data.
		/// </returns>
		[MqlFuncDoc(Order = 3)]
		public static string SetRequestTimeout(
			[MqlParamDoc(ExampleValue = "30")]
			int timeout)
		{
			try
			{
				Ensure.That<ArgumentException>(timeout > 0, $"The argument {nameof(timeout)} must be greater than 0.");
				SetTimeout(timeout);
				return responseFactory.Success().ToString();
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		/// <summary>
		/// Enables or disables a more verbose output in case of exceptions.
		/// </summary>
		/// <param name="enableDebug">Indicates, if the debug output should be generated.</param>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success there is no payload but only the success flag in the header data.
		/// </returns>
		[MqlFuncDoc(Order = 2)]
		public static string SetDebugOutput(
			[MqlParamDoc(ExampleValue = "true")]
			bool enableDebug)
		{
			try
			{
				responseFactory.IsDebugEnabled = enableDebug;
				return responseFactory.Success().ToString();
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		/// <summary>
		/// Enables or disables a more verbose output in case of exceptions.
		/// </summary>
		/// <param name="parameterKey">
		/// The parameter for which the default value should be changed. Valid inputs are:
		/// <list type="bullet">
		/// <item>CheckServerCertificate - true/false</item>
		/// </list>
		/// The parameter takes effect for every upcoming call to a function using this parameter.
		/// </param>
		/// <param name="defaultValue">The default value for the parameter.</param>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success there is no payload but only the success flag in the header data.
		/// </returns>
		[MqlFuncDoc(Order = 3)]
		public static string SetDefaultValue(
			[MqlParamDoc(ExampleValue = "CheckServerCertificate")]
			string parameterKey,
			[MqlParamDoc(ExampleValue = "true")]
			string defaultValue)
		{
			try
			{
				Ensure.NotNullOrEmptyOrWhiteSpace(parameterKey, $"The argument {nameof(parameterKey)} must not be empty or just whitespace.");
				Ensure.NotNullOrEmptyOrWhiteSpace(defaultValue, $"The argument {nameof(defaultValue)} must not be empty or just whitespace.");
				MailMapper.SetDefaultValue(parameterKey, defaultValue);
				return responseFactory.Success().ToString();
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		#endregion

		private static void ResetClass()
		{
			MailMapper = null;
			isInitialized = false;
		}

		private static void InitializeClass(IMailMapper mailMapper)
		{
			MailMapper = mailMapper;
			isInitialized = true;
		}

		private static void SetTimeout(int timeout)
		{
			MailMapper.RequestTimeout = timeout * 1000;
		}
	}
}
