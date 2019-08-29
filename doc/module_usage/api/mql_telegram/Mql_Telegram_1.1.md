# Mql_Telegram Documentation
## <a name="GetMe" /> GetMe
```c
string GetMe ()
```
A simple method for testing your bots auth token.

See <a href="https://core.telegram.org/bots/api#getme">Telegram API</a> for more details.

<dl>
<dt>Returns</dt>
<dd>
A JSON string representing a <a href="https://mmm.steven-england.info/Generic-Response">Generic-Response</a>.
On success the payload is a <a href="https://core.telegram.org/bots/api#user">Telegram User</a> containing basic information about the bot.
</dd>

</dl>

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
string Initialize (string apiKey,
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
<td>string</td><td>apiKey</td><td>The API token for the Telegram bot.</td></tr>
<tr>
<td>int</td><td>timeout</td><td>Seconds that a request to the Telegram API can last before the call gets cancelled.</td></tr>
</table>

</dd>

</dl>

## <a name="SendText" /> SendText
```c
string SendText (string chatId,
                 string chatText)
```
Use this method to send text messages.

See <a href="https://core.telegram.org/bots/api#sendmessage">Telegram API</a> for more details.

<dl>
<dt>Returns</dt>
<dd>
A JSON string representing a <a href="https://mmm.steven-england.info/Generic-Response">Generic-Response</a>.
On success the payload is a <a href="https://core.telegram.org/bots/api#message">Telegram Message</a>.
</dd>
<dt>Parameter</dt>
<dd>
<table>
<tr>
<th>Type</th><th>Name</th><th>Description</th></tr>
<tr>
<td>string</td><td>chatId</td><td>Identifier for the target chat. You have different options for identifiers:
<ul><li>The Username of the channel (in the format @channel_username)</li><li>The ID of a user, group or channel (in the format "1546456487" or "-165489645654654" etc.)</li></ul>
The user name of a channel can only be used if the channel is public.</td></tr>
<tr>
<td>string</td><td>chatText</td><td>Text of the message to be sent.</td></tr>
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

## <a name="SetRequestTimeout" /> SetRequestTimeout
```c
string SetRequestTimeout (int timeout)
```
Adjusts the timespan that a call to the Telegram API can last before it gets cancelled.
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
<td>int</td><td>timeout</td><td>Seconds that a request to the Telegram API can last.</td></tr>
</table>

</dd>

</dl>

## <a name="StartGetMe" /> StartGetMe
```c
string StartGetMe ()
```
A simple method for testing your bots auth token.

See <a href="https://core.telegram.org/bots/api#getme">Telegram API</a> for more details.

<dl>
<dt>Returns</dt>
<dd>
A JSON string representing a <a href="https://mmm.steven-england.info/Generic-Response">Generic-Response</a>.
On success the payload is a <a href="https://core.telegram.org/bots/api#user">Telegram User</a> containing basic information about the bot.
</dd>
<dt>Remarks</dt>
<dd>
This method starts an independent background thread and immediately returns a response with a correlation key but empty payload.

You can use the correlation key to check the result of the thread later via <a href="#GetMessageByCorrelationId">GetMessageByCorrelationId</a>.

</dd>

</dl>

## <a name="StartSendText" /> StartSendText
```c
string StartSendText (string chatId,
                      string chatText)
```
Use this method to send text messages.

See <a href="https://core.telegram.org/bots/api#sendmessage">Telegram API</a> for more details.

<dl>
<dt>Returns</dt>
<dd>
A JSON string representing a <a href="https://mmm.steven-england.info/Generic-Response">Generic-Response</a>.
On success the payload is a <a href="https://core.telegram.org/bots/api#message">Telegram Message</a>.
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
<td>string</td><td>chatId</td><td>Identifier for the target chat. You have different options for identifiers:
<ul><li>The Username of the channel (in the format @channel_username)</li><li>The ID of a user, group or channel (in the format "1546456487" or "-165489645654654" etc.)</li></ul>
The user name of a channel can only be used if the channel is public.</td></tr>
<tr>
<td>string</td><td>chatText</td><td>Text of the message to be sent.</td></tr>
</table>

</dd>

</dl>

