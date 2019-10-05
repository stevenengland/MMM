using System;
using System.Collections.Generic;
using System.Text;
using StEn.MMM.Mql.Common.Services.InApi.Entities;

namespace StEn.MMM.Mql.Mail.Services.Mail
{
	/// <summary>
	/// Covers functionality to handle mail exchange.
	/// </summary>
	public interface IMailMapper
	{
		/// <summary>
		/// Gets or sets the amount of seconds that a request can last before it gets cancelled.
		/// </summary>
		int RequestTimeout { get; set; }

		/// <summary>
		/// Gets a value indicating whether the server certificate will be validated.
		/// </summary>
		bool CheckServerCertificate { get; }

		/// <summary>
		/// Determines the <see cref="Response{T}"/> of a background thread that was triggered via a Start method.
		/// </summary>
		/// <param name="correlationKey">The identifier that was created be a Start method.</param>
		/// <returns>If the parameterKey exists a <see cref="Response{T}"/> is returned that holds the content of the corresponding Start method.</returns>
		string GetMessageByCorrelationId(string correlationKey);

		/// <summary>
		/// Sets a default value for operations that use the <paramref name="parameterKey"/>.
		/// </summary>
		/// <param name="parameterKey">The parameter for which the default value should be changed.</param>
		/// <param name="defaultValue">The default value for the <paramref name="parameterKey"/>.</param>
		/// <returns>Returns if the default value was set successfully.</returns>
		string SetDefaultValue(string parameterKey, string defaultValue);

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
		string SendMail(
			string sender,
			IEnumerable<string> recipients,
			string subject,
			string textBody,
			string htmlBody,
			IEnumerable<string> attachments = null);

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
		string StartSendMail(
			string sender,
			IEnumerable<string> recipients,
			string subject,
			string textBody,
			string htmlBody,
			IEnumerable<string> attachments = null);
	}
}
