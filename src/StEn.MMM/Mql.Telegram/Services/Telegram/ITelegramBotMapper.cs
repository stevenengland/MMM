using StEn.MMM.Mql.Common.Services.InApi.Entities;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace StEn.MMM.Mql.Telegram.Services.Telegram
{
	/// <summary>
	/// The mapper interface for the Telegram Bot API.
	/// </summary>
	public interface ITelegramBotMapper
	{
		/// <summary>
		/// Gets or sets the amount of seconds that a request can last before it gets cancelled.
		/// </summary>
		int RequestTimeout { get; set; }

		/// <summary>
		/// Gets the current default parse mode.
		/// </summary>
		ParseMode ParseMode { get; }

		/// <summary>
		/// Gets a value indicating whether the default value for notification disabling is true.
		/// </summary>
		bool DisableNotifications { get; }

		/// <summary>
		/// Gets a value indicating whether the default value for web page preview disabling is true.
		/// </summary>
		bool DisableWebPagePreview { get; }

		/// <summary>
		/// Sets a default value for operations that use the <paramref name="parameterKey"/>.
		/// </summary>
		/// <param name="parameterKey">The parameter for which the default value should be changed.</param>
		/// <param name="defaultValue">The default value for the <paramref name="parameterKey"/>.</param>
		/// <returns>Returns if the default value was set successfully.</returns>
		string SetDefaultValue(string parameterKey, string defaultValue);

		/// <summary>
		/// Determines the <see cref="Response{T}"/> of a background thread that was triggered via a Start method.
		/// </summary>
		/// <param name="correlationKey">The identifier that was created be a Start method.</param>
		/// <returns>If the parameterKey exists a <see cref="Response{T}"/> is returned that holds the content of the corresponding Start method.</returns>
		string GetMessageByCorrelationId(string correlationKey);

		/// <summary>
		/// A simple method for testing your bots auth token.
		/// </summary>
		/// <returns>Returns basic information about the bot in form of <see cref="User"/> object within a <see cref="Response{T}"/>.</returns>
		/// <see href="https://core.telegram.org/bots/api#getme"/>
		string GetMe();

		/// <summary>
		/// A simple method for testing your bots auth token.
		/// This method runs in a background thread and returns correlation information to obtain the result of the background thread.
		/// </summary>
		/// <returns>Returns correlation information to obtain the result of the background thread.</returns>
		/// <see href="https://core.telegram.org/bots/api#getme"/>
		string StartGetMe();

		/// <summary>
		/// Use this method to receive incoming updates using long polling.
		/// </summary>
		/// <returns>An Array of <see cref="Update"/> is returned.</returns>
		/// <see href="https://core.telegram.org/bots/api#getupdates"/>
		string GetUpdates();

		/// <summary>
		/// Use this method to receive incoming updates using long polling.
		/// This method runs in a background thread and returns correlation information to obtain the result of the background thread.
		/// </summary>
		/// <returns>Returns correlation information to obtain the result of the background thread.</returns>
		/// <see href="https://core.telegram.org/bots/api#getupdates"/>
		string StartGetUpdates();

		/// <summary>
		/// Use this method to send general files. On success, the sent Description is returned.
		/// Bots can send files of any type of up to 50 MB in size.
		/// </summary>
		/// <param name="chatId"><see cref="ChatId"/> for the target chat.</param>
		/// <param name="file">File to send.</param>
		/// <returns>On success, the sent Description is returned.</returns>
		/// <see href="https://core.telegram.org/bots/api#senddocument"/>
		string SendDocument(string chatId, string file);

		/// <summary>
		/// Use this method to send general files. On success, the sent Description is returned.
		/// Bots can send files of any type of up to 50 MB in size.
		/// </summary>
		/// <param name="chatId"><see cref="ChatId"/> for the target chat.</param>
		/// <param name="file">File to send.</param>
		/// <returns>Returns correlation information to obtain the result of the background thread.</returns>
		/// <see href="https://core.telegram.org/bots/api#senddocument"/>
		string StartSendDocument(string chatId, string file);

		/// <summary>
		/// Use this method to send photos. On success, the sent Description is returned.
		/// </summary>
		/// <param name="chatId"><see cref="ChatId"/> for the target chat.</param>
		/// <param name="photoFile">Photo to send.</param>
		/// <returns>On success, the sent Description is returned.</returns>
		/// <see href="https://core.telegram.org/bots/api#sendphoto"/>
		string SendPhoto(string chatId, string photoFile);

		/// <summary>
		/// Use this method to send photos. On success, the sent Description is returned.
		/// This method runs in a background thread and returns correlation information to obtain the result of the background thread.
		/// </summary>
		/// <param name="chatId"><see cref="ChatId"/> for the target chat.</param>
		/// <param name="photoFile">Photo to send.</param>
		/// <returns>Returns correlation information to obtain the result of the background thread.</returns>
		/// <see href="https://core.telegram.org/bots/api#sendphoto"/>
		string StartSendPhoto(string chatId, string photoFile);

		/// <summary>
		/// Use this method to send text messages. On success, the sent Description is returned.
		/// </summary>
		/// <param name="chatId"><see cref="ChatId"/> for the target chat.</param>
		/// <param name="text">Text of the message to be sent.</param>
		/// <returns>On success, the sent Description is returned within a <see cref="Response{T}"/>.</returns>
		/// <see href="https://core.telegram.org/bots/api#sendmessage"/>
		string SendText(string chatId, string text);

		/// <summary>
		/// Use this method to send text messages. On success, the sent Description is returned.
		/// This method runs in a background thread and returns correlation information to obtain the result of the background thread.
		/// </summary>
		/// <param name="chatId"><see cref="ChatId"/> for the target chat.</param>
		/// <param name="text">Text of the message to be sent.</param>
		/// <returns>Returns correlation information to obtain the result of the background thread.</returns>
		/// <see href="https://core.telegram.org/bots/api#sendmessage"/>
		string StartSendText(string chatId, string text);
	}
}
