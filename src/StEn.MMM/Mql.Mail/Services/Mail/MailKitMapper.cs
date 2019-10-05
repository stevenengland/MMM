using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Org.BouncyCastle.Bcpg;
using StEn.MMM.Mql.Common.Base.Extensions;
using StEn.MMM.Mql.Common.Base.Interfaces;
using StEn.MMM.Mql.Common.Base.Utilities;
using StEn.MMM.Mql.Common.Services.InApi.Factories;

namespace StEn.MMM.Mql.Mail.Services.Mail
{
	/// <inheritdoc cref="IMailMapper" />
	public class MailKitMapper : IMailMapper, IErrorHandler, ISuccessHandler, IProxyCall
	{
		private readonly string username;

		private readonly string password;

		private readonly string smtpServer;

		private readonly int smtpPort;

		private readonly MessageStore<string, string> messageStore = new MessageStore<string, string>(1000);

		private readonly ResponseFactory responseFactory;

		/// <summary>
		/// Initializes a new instance of the <see cref="MailKitMapper"/> class.
		/// </summary>
		/// <param name="username">The username for the email account.</param>
		/// <param name="password">The password for the email account.</param>
		/// <param name="smtpServer">The host name of the SMTP server.</param>
		/// <param name="smtpPort">The port of the SMTP server.</param>
		/// <param name="responseFactory">Reference to a <see cref="ResponseFactory"/>. For testing purposes only.</param>
		public MailKitMapper(
			string username,
			string password,
			string smtpServer,
			int smtpPort,
			ResponseFactory responseFactory = null)
		{
			this.username = username;
			this.password = password;
			this.smtpServer = smtpServer;
			this.smtpPort = smtpPort;
			this.responseFactory = responseFactory ?? new ResponseFactory();
		}

		/// <inheritdoc />
		public int RequestTimeout { get; set; }

		/// <inheritdoc />
		public bool CheckServerCertificate { get; private set; } = true;

		/// <summary>
		/// Transform a string containing email information into a <see cref="MailboxAddress"/>.
		/// </summary>
		/// <param name="addressText">A normal email address like a@b.com or a nick name styled one like a &lt;a@b.com&gt;.</param>
		/// <returns>Returns a <see cref="MailboxAddress"/>.</returns>
		/// <exception cref="ArgumentException">Thrown if the text does not match a supported email style.</exception>
		public static MailboxAddress StringToMailboxAddress(string addressText)
		{
			if (Regex.IsMatch(addressText, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
			{
				return new MailboxAddress(addressText);
			}

			var match = Regex.Match(addressText, @"(.*?)\s?<(\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)>");
			if (match.Success)
			{
				return new MailboxAddress(name: match.Groups[1].Value, address: match.Groups[2].Value);
			}

			throw new ArgumentException($"'{addressText}' is not a valid email address.");
		}

		/// <inheritdoc />
		public string GetMessageByCorrelationId(string correlationKey)
		{
			return this.messageStore.TryGetValue(correlationKey, out string resultValue)
				? resultValue
				: this.responseFactory.Error(new KeyNotFoundException($"There is no entry for correlation key {correlationKey} in the queue."), $"There is no entry for correlation key {correlationKey} in the queue.", correlationKey).ToString();
		}

		/// <inheritdoc/>
		public string SetDefaultValue(string parameterKey, string defaultValue)
		{
			var responseText = string.Empty;
			bool disabled;
			switch (parameterKey)
			{
				case nameof(this.CheckServerCertificate):
					if (bool.TryParse(defaultValue.ToLower(), out disabled))
					{
						this.CheckServerCertificate = disabled;
					}
					else
					{
						return this.responseFactory.Error(new ArgumentException($"Invalid value for {nameof(defaultValue)}: {defaultValue}"), $"The value must be 'true' or 'false'.").ToString();
					}

					responseText = this.responseFactory.Success().ToString();
					break;
				default:
					return this.responseFactory.Error(new KeyNotFoundException($"Unknown {nameof(parameterKey)}: {parameterKey}"), $"Cannot set default value for unknown {nameof(parameterKey)}").ToString();
			}

			return responseText;
		}

		/// <inheritdoc />
		public string SendMail(
			string sender,
			IEnumerable<string> recipients,
			string subject,
			string textBody,
			string htmlBody,
			IEnumerable<string> attachments = null)
		{
			using (var cancellationTokenSource = this.CtsFactory())
			{
				return this.ProxyCall(this.SendMailAsync(
					sender: sender,
					recipients: recipients,
					subject: subject,
					textBody: textBody,
					htmlBody: htmlBody,
					attachments: attachments,
					cancellationToken: cancellationTokenSource.Token));
			}
		}

		/// <inheritdoc />
		public string StartSendMail(
			string sender,
			IEnumerable<string> recipients,
			string subject,
			string textBody,
			string htmlBody,
			IEnumerable<string> attachments = null)
		{
			var cancellationTokenSource = this.CtsFactory();
			return this.FireAndForgetProxyCall(this.SendMailAsync(
					sender: sender,
					recipients: recipients,
					subject: subject,
					textBody: textBody,
					htmlBody: htmlBody,
					attachments: attachments,
					cancellationToken: cancellationTokenSource.Token)
				.DisposeAfterThreadCompletionAsync(new IDisposable[]
				{
					cancellationTokenSource,
				}));
		}

		/// <summary>
		/// Proxy function to send an email.
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
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success there is no payload but only the success flag in the header data.
		/// </returns>
		public async Task<string> SendMailAsync(
			string sender,
			IEnumerable<string> recipients,
			string subject,
			string textBody,
			string htmlBody,
			IEnumerable<string> attachments = null,
			CancellationToken cancellationToken = default)
		{
			using (var client = new SmtpClient())
			{
				client.ServerCertificateValidationCallback = (s, c, h, e) => this.CheckServerCertificate;
				await client.ConnectAsync(this.smtpServer, this.smtpPort, SecureSocketOptions.Auto, cancellationToken).ConfigureAwait(false);
				await client.AuthenticateAsync(this.username, this.password, cancellationToken).ConfigureAwait(false);
				await client.SendAsync(
					message: this.CreateMessage(sender, recipients, subject, textBody, htmlBody, attachments),
					cancellationToken: cancellationToken).ConfigureAwait(false);
				await client.DisconnectAsync(true, cancellationToken).ConfigureAwait(false);
			}

			return "The mail was sent successfully.";
		}

		#region ProxyCalls

		/// <inheritdoc/>
		public void HandleFireAndForgetError(Exception ex, string correlationKey)
		{
			this.messageStore.Add(correlationKey, this.responseFactory.Error(ex).ToString());
		}

		/// <inheritdoc />
		public void HandleFireAndForgetSuccess<T>(T data, string correlationKey)
		{
			this.messageStore.Add(correlationKey, this.responseFactory.Success<T>(message: data, correlationKey).ToString());
		}

		/// <inheritdoc />
		public string FireAndForgetProxyCall<T>(Task<T> method)
		{
			try
			{
				var correlationKey = IDGenerator.Instance.Next;
				method.FireAndForgetSafe(correlationKey, this, this);
				return this.responseFactory.Success(correlationKey).ToString();
			}
			catch (Exception ex)
			{
				return this.responseFactory.Error(ex).ToString();
			}
		}

		/// <inheritdoc />
		public string ProxyCall<T>(Task<T> method)
		{
			try
			{
				var result = method.FireSafe();
				return this.responseFactory.Success(message: result).ToString();
			}
			catch (Exception ex)
			{
				return this.responseFactory.Error(ex).ToString();
			}
		}

		private CancellationTokenSource CtsFactory(int timeout = 0)
		{
			var ctTimeout = timeout == 0 ? this.RequestTimeout : timeout;
			var cts = new CancellationTokenSource(ctTimeout);
			return cts;
		}

		#endregion

		private MimeMessage CreateMessage(
			string sender,
			IEnumerable<string> recipients,
			string subject,
			string textBody,
			string htmlBody,
			IEnumerable<string> attachments = null)
		{
			var message = new MimeMessage();

			message.From.Add(StringToMailboxAddress(sender));

			foreach (var recipient in recipients)
			{
				if (recipient.ToLower().StartsWith("to:"))
				{
					message.To.Add(StringToMailboxAddress(recipient.Substring(3)));
				}
				else if (recipient.ToLower().StartsWith("cc:"))
				{
					message.Cc.Add(StringToMailboxAddress(recipient.Substring(3)));
				}
				else if (recipient.ToLower().StartsWith("bcc:"))
				{
					message.Bcc.Add(StringToMailboxAddress(recipient.Substring(4)));
				}
				else
				{
					throw new ArgumentException($"'{recipient}' has no valid prefix (to:, cc:, ...).");
				}
			}

			if (!message.To.Any())
			{
				throw new ArgumentException($"You must provide at least one 'to' recipient.");
			}

			message.Subject = subject;

			var builder = new BodyBuilder();

			if (!string.IsNullOrWhiteSpace(textBody))
			{
				builder.TextBody = CharacterTransformation.TransformSpecialCharacters(textBody);
			}

			if (!string.IsNullOrWhiteSpace(htmlBody))
			{
				builder.HtmlBody = CharacterTransformation.TransformSpecialCharacters(htmlBody);
			}

			if (attachments != null)
			{
				foreach (var attachment in attachments)
				{
					builder.Attachments.Add(attachment);
				}
			}

			// Now we just need to set the message body and we're done
			message.Body = builder.ToMessageBody();

			return message;
		}
	}
}
