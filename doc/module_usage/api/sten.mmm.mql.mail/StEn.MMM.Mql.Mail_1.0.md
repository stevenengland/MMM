# StEn.MMM.Mql.Mail 1.0 Documentation
## <a name="GetMessageByCorrelationId" /> GetMessageByCorrelationId
```c
string GetMessageByCorrelationId (string correlationKey)
```
Use this method to check if and which result for a given correlation key was obtained.
<dl>
<dt>Returns</dt>
<dd>
A JSON string representing a <a href="https://mmm.steven-england.info/Generic-Response">Generic-Response</a>.
On success the payload is exactly of that type that was obtained from the corresponding "Start" method that generated the correlation key.
If the correlation key was not found the corresponding method has not finished yet.
</dd>
<dt>Parameter</dt>
<dd>
<table>
<tr>
<th>Type</th><th>Name</th><th>Description</th></tr>
<tr>
<td>string</td><td>correlationKey</td><td>The correlation key that was provided by a "Start" method.</td></tr>
</table>

</dd>

</dl>

## <a name="Initialize" /> Initialize
```c
string Initialize (string smtpServer,
                   int smtpPort,
                   string smtpUserName,
                   string smtpPassword,
                   int timeout)
```
Must be called first before any other method is used. It initializes the framework.
<dl>
<dt>Returns</dt>
<dd>
A JSON string representing a <a href="https://mmm.steven-england.info/Generic-Response">Generic-Response</a>.
On success there is no payload but only the success flag in the header data.
</dd>
<dt>Parameter</dt>
<dd>
<table>
<tr>
<th>Type</th><th>Name</th><th>Description</th></tr>
<tr>
<td>string</td><td>smtpServer</td><td>The host name of the SMTP server.</td></tr>
<tr>
<td>int</td><td>smtpPort</td><td>The port of the SMTP server.</td></tr>
<tr>
<td>string</td><td>smtpUserName</td><td>The username for the SMTP account.</td></tr>
<tr>
<td>string</td><td>smtpPassword</td><td>The password for the SMTP account.</td></tr>
<tr>
<td>int</td><td>timeout</td><td>Seconds that a request to the server can last before the call gets cancelled.</td></tr>
</table>

</dd>

</dl>

## <a name="SendMail" /> SendMail
```c
string SendMail (string sender,
                 string &[] recipients,
                 string subject,
                 string textBody,
                 string htmlBody)
```
Sends an email.
<dl>
<dt>Returns</dt>
<dd>
A JSON string representing a <a href="https://mmm.steven-england.info/Generic-Response">Generic-Response</a>.
On success there is no payload but only the success flag in the header data.
</dd>
<dt>Parameter</dt>
<dd>
<table>
<tr>
<th>Type</th><th>Name</th><th>Description</th></tr>
<tr>
<td>string</td><td>sender</td><td>Identifier for sender (FROM header). You have different options for identifiers:
<ul><li>A plain email address like name@domain.com</li><li>A nick name format like NickName &lt;name@domain.com&gt;</li></ul>
Please be aware of the correct format in the latter case.</td></tr>
<tr>
<td>string &[]</td><td>recipients</td><td>A list of email addresses you like to send the mails to. Add an identifier in front of each recipient that indicates the kind of recipient:
<ul><li>1-n addresses like to:name@domain.com -&gt; will be send as "TO"</li><li>0-n addresses like cc:name@domain.com -&gt; will be send as "CC"</li><li>0-n addresses like bcc:name@domain.com -&gt; will be send as "BCC"</li></ul>
Like in the sender field, you can also use nick name styles like NickName &lt;name@domain.com&gt;.</td></tr>
<tr>
<td>string</td><td>subject</td><td>The subject of your email.</td></tr>
<tr>
<td>string</td><td>textBody</td><td>The plain text body of your email. You can fill just this one or both, the plain text body and the HTML body.</td></tr>
<tr>
<td>string</td><td>htmlBody</td><td>The HTML style body of your email. You can fill just this one or both, the plain text body and the HTML body.</td></tr>
</table>

</dd>

</dl>

## <a name="SendMail" /> SendMail
```c
string SendMail (string sender,
                 string &[] recipients,
                 string subject,
                 string textBody,
                 string htmlBody,
                 string &[] attachments)
```
Sends an email.
<dl>
<dt>Returns</dt>
<dd>
A JSON string representing a <a href="https://mmm.steven-england.info/Generic-Response">Generic-Response</a>.
On success there is no payload but only the success flag in the header data.
</dd>
<dt>Parameter</dt>
<dd>
<table>
<tr>
<th>Type</th><th>Name</th><th>Description</th></tr>
<tr>
<td>string</td><td>sender</td><td>Identifier for sender (FROM header). You have different options for identifiers:
<ul><li>A plain email address like name@domain.com</li><li>A nick name format like NickName &lt;name@domain.com&gt;</li></ul>
Please be aware of the correct format in the latter case.</td></tr>
<tr>
<td>string &[]</td><td>recipients</td><td>A list of email addresses you like to send the mails to. Add an identifier in front of each recipient that indicates the kind of recipient:
<ul><li>1-n addresses like to:name@domain.com -&gt; will be send as "TO"</li><li>0-n addresses like cc:name@domain.com -&gt; will be send as "CC"</li><li>0-n addresses like bcc:name@domain.com -&gt; will be send as "BCC"</li></ul>
Like in the sender field, you can also use nick name styles like NickName &lt;name@domain.com&gt;.</td></tr>
<tr>
<td>string</td><td>subject</td><td>The subject of your email.</td></tr>
<tr>
<td>string</td><td>textBody</td><td>The plain text body of your email. You can fill just this one or both, the plain text body and the HTML body.</td></tr>
<tr>
<td>string</td><td>htmlBody</td><td>The HTML style body of your email. You can fill just this one or both, the plain text body and the HTML body.</td></tr>
<tr>
<td>string &[]</td><td>attachments</td><td>A list of paths to files you like to send as attachments.</td></tr>
</table>

</dd>

</dl>

## <a name="SetDebugOutput" /> SetDebugOutput
```c
string SetDebugOutput (bool enableDebug)
```
Enables or disables a more verbose output in case of exceptions.
<dl>
<dt>Returns</dt>
<dd>
A JSON string representing a <a href="https://mmm.steven-england.info/Generic-Response">Generic-Response</a>.
On success there is no payload but only the success flag in the header data.
</dd>
<dt>Parameter</dt>
<dd>
<table>
<tr>
<th>Type</th><th>Name</th><th>Description</th></tr>
<tr>
<td>bool</td><td>enableDebug</td><td>Indicates, if the debug output should be generated.</td></tr>
</table>

</dd>

</dl>

## <a name="SetDefaultValue" /> SetDefaultValue
```c
string SetDefaultValue (string parameterKey,
                        string defaultValue)
```
Enables or disables a more verbose output in case of exceptions.
<dl>
<dt>Returns</dt>
<dd>
A JSON string representing a <a href="https://mmm.steven-england.info/Generic-Response">Generic-Response</a>.
On success there is no payload but only the success flag in the header data.
</dd>
<dt>Parameter</dt>
<dd>
<table>
<tr>
<th>Type</th><th>Name</th><th>Description</th></tr>
<tr>
<td>string</td><td>parameterKey</td><td>The parameter for which the default value should be changed. Valid inputs are:
<ul><li>CheckServerCertificate - true/false</li></ul>
The parameter takes effect for every upcoming call to a function using this parameter.</td></tr>
<tr>
<td>string</td><td>defaultValue</td><td>The default value for the parameter.</td></tr>
</table>

</dd>

</dl>

## <a name="SetRequestTimeout" /> SetRequestTimeout
```c
string SetRequestTimeout (int timeout)
```
Adjusts the timespan that a call to the server can last before it gets cancelled.
<dl>
<dt>Returns</dt>
<dd>
A JSON string representing a <a href="https://mmm.steven-england.info/Generic-Response">Generic-Response</a>.
On success there is no payload but only the success flag in the header data.
</dd>
<dt>Parameter</dt>
<dd>
<table>
<tr>
<th>Type</th><th>Name</th><th>Description</th></tr>
<tr>
<td>int</td><td>timeout</td><td>Seconds that a request to the server can last.</td></tr>
</table>

</dd>

</dl>

## <a name="StartSendMail" /> StartSendMail
```c
string StartSendMail (string sender,
                      string &[] recipients,
                      string subject,
                      string textBody,
                      string htmlBody)
```
Sends an email.
<dl>
<dt>Returns</dt>
<dd>
A JSON string representing a <a href="https://mmm.steven-england.info/Generic-Response">Generic-Response</a>.
On success there is no payload but only the success flag in the header data.
</dd>
<dt>Remarks</dt>
<dd>
This method starts an independent background thread and immediately returns a response with a correlation key but empty payload.

You can use the correlation key to check the result of the thread later via <a href="#GetMessageByCorrelationId">GetMessageByCorrelationId</a>.

</dd>
<dt>Parameter</dt>
<dd>
<table>
<tr>
<th>Type</th><th>Name</th><th>Description</th></tr>
<tr>
<td>string</td><td>sender</td><td>Identifier for sender (FROM header). You have different options for identifiers:
<ul><li>A plain email address like name@domain.com</li><li>A nick name format like NickName &lt;name@domain.com&gt;</li></ul>
Please be aware of the correct format in the latter case.</td></tr>
<tr>
<td>string &[]</td><td>recipients</td><td>A list of email addresses you like to send the mails to. Add an identifier in front of each recipient that indicates the kind of recipient:
<ul><li>1-n addresses like to:name@domain.com -&gt; will be send as "TO"</li><li>0-n addresses like cc:name@domain.com -&gt; will be send as "CC"</li><li>0-n addresses like bcc:name@domain.com -&gt; will be send as "BCC"</li></ul>
Like in the sender field, you can also use nick name styles like NickName &lt;name@domain.com&gt;.</td></tr>
<tr>
<td>string</td><td>subject</td><td>The subject of your email.</td></tr>
<tr>
<td>string</td><td>textBody</td><td>The plain text body of your email. You can fill just this one or both, the plain text body and the HTML body.</td></tr>
<tr>
<td>string</td><td>htmlBody</td><td>The HTML style body of your email. You can fill just this one or both, the plain text body and the HTML body.</td></tr>
</table>

</dd>

</dl>

## <a name="StartSendMail" /> StartSendMail
```c
string StartSendMail (string sender,
                      string &[] recipients,
                      string subject,
                      string textBody,
                      string htmlBody,
                      string &[] attachments)
```
Sends an email.
<dl>
<dt>Returns</dt>
<dd>
A JSON string representing a <a href="https://mmm.steven-england.info/Generic-Response">Generic-Response</a>.
On success there is no payload but only the success flag in the header data.
</dd>
<dt>Remarks</dt>
<dd>
This method starts an independent background thread and immediately returns a response with a correlation key but empty payload.

You can use the correlation key to check the result of the thread later via <a href="#GetMessageByCorrelationId">GetMessageByCorrelationId</a>.

</dd>
<dt>Parameter</dt>
<dd>
<table>
<tr>
<th>Type</th><th>Name</th><th>Description</th></tr>
<tr>
<td>string</td><td>sender</td><td>Identifier for sender (FROM header). You have different options for identifiers:
<ul><li>A plain email address like name@domain.com</li><li>A nick name format like NickName &lt;name@domain.com&gt;</li></ul>
Please be aware of the correct format in the latter case.</td></tr>
<tr>
<td>string &[]</td><td>recipients</td><td>A list of email addresses you like to send the mails to. Add an identifier in front of each recipient that indicates the kind of recipient:
<ul><li>1-n addresses like to:name@domain.com -&gt; will be send as "TO"</li><li>0-n addresses like cc:name@domain.com -&gt; will be send as "CC"</li><li>0-n addresses like bcc:name@domain.com -&gt; will be send as "BCC"</li></ul>
Like in the sender field, you can also use nick name styles like NickName &lt;name@domain.com&gt;.</td></tr>
<tr>
<td>string</td><td>subject</td><td>The subject of your email.</td></tr>
<tr>
<td>string</td><td>textBody</td><td>The plain text body of your email. You can fill just this one or both, the plain text body and the HTML body.</td></tr>
<tr>
<td>string</td><td>htmlBody</td><td>The HTML style body of your email. You can fill just this one or both, the plain text body and the HTML body.</td></tr>
<tr>
<td>string &[]</td><td>attachments</td><td>A list of paths to files you like to send as attachments.</td></tr>
</table>

</dd>

</dl>

