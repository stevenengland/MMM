using System;
using System.IO;
using System.Runtime.InteropServices;
using StEn.MMM.Mql.Common.Base.Attributes;
#if !DEBUG
using RGiesecke.DllExport;
#endif
using StEn.MMM.Mql.Common.Base.Utilities;
using StEn.MMM.Mql.Common.Services.InApi.Entities;
using StEn.MMM.Mql.Common.Services.InApi.Factories;
using StEn.MMM.Mql.Telegram.Services.Telegram;
using Telegram.Bot;

namespace StEn.MMM.Mql.Telegram
{
	/// <summary>
	/// Contains dll exports for the Telegram Bot API.
	/// </summary>
	public class TelegramDllExports
	{
		private static ITelegramBotMapper bot;

		private static bool isInitialized;

		private static ResponseFactory responseFactory = new ResponseFactory();

		static TelegramDllExports()
		{
			// https://colinmackay.scot/2007/06/16/unit-testing-a-static-class/
			ResetClass();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TelegramDllExports"/> class.
		/// </summary>
		protected TelegramDllExports()
		{
		}

		/// <summary>
		/// Gets or sets the Bot to be used. In order to use it you must call <see cref="Initialize"/> first.
		/// The public non static constructor is meant for testing only.
		/// </summary>
		public static ITelegramBotMapper Bot
		{
			get
			{
				Ensure.That<ApplicationException>(isInitialized, $"The framework is not initialized yet. Please run the {nameof(Initialize)} method first.");
				return bot;
			}
			set => bot = value;
		}

		/// <summary>
		/// <para>A simple method for testing your bots auth token.</para>
		/// <para>See <see href="https://core.telegram.org/bots/api#getme">Telegram API</see> for more details.</para>
		/// </summary>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success the payload is a <see href="https://core.telegram.org/bots/api#user">Telegram User</see> containing basic information about the bot.
		/// </returns>
#if !DEBUG
		[DllExport("GetMe", CallingConvention = CallingConvention.StdCall)]
#endif
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string GetMe()
		{
			try
			{
				return Bot.GetMe();
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		/// <summary>
		/// <para>A simple method for testing your bots auth token.</para>
		/// <para>See <see href="https://core.telegram.org/bots/api#getme">Telegram API</see> for more details.</para>
		/// </summary>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success the payload is a <see href="https://core.telegram.org/bots/api#user">Telegram User</see> containing basic information about the bot.
		/// </returns>
		/// <remarks>
		/// <para>This method starts an independent background thread and immediately returns a response with a correlation key but empty payload.</para>
		/// <para>You can use the correlation key to check the result of the thread later via <see cref="GetMessageByCorrelationId"/>.</para>
		/// </remarks>
#if !DEBUG
		[DllExport("StartGetMe", CallingConvention = CallingConvention.StdCall)]
#endif
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string StartGetMe()
		{
			try
			{
				return Bot.StartGetMe();
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		/// <summary>
		/// <para>Use this method to receive incoming updates.</para>
		/// <para>See <see href="https://core.telegram.org/bots/api#getupdates">Telegram API</see> for more details.</para>
		/// </summary>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success the payload is an array of <see href="https://core.telegram.org/bots/api#update">Telegram Updates</see>.
		/// </returns>
#if !DEBUG
		[DllExport("GetUpdates", CallingConvention = CallingConvention.StdCall)]
#endif
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string GetUpdates()
		{
			try
			{
				return Bot.GetUpdates();
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		/// <summary>
		/// <para>Use this method to receive incoming updates.</para>
		/// <para>See <see href="https://core.telegram.org/bots/api#getupdates">Telegram API</see> for more details.</para>
		/// </summary>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success the payload is an array of <see href="https://core.telegram.org/bots/api#update">Telegram Updates</see>.
		/// </returns>
		/// <remarks>
		/// <para>This method starts an independent background thread and immediately returns a response with a correlation key but empty payload.</para>
		/// <para>You can use the correlation key to check the result of the thread later via <see cref="GetMessageByCorrelationId"/>.</para>
		/// </remarks>
#if !DEBUG
		[DllExport("StartGetUpdates", CallingConvention = CallingConvention.StdCall)]
#endif
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string StartGetUpdates()
		{
			try
			{
				return Bot.StartGetUpdates();
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		/// <summary>
		/// <para>Use this method to send general files. Bots can send files of any type of up to 50 MB in size.</para>
		/// <para>See <see href="https://core.telegram.org/bots/api#senddocument">Telegram API</see> for more details.</para>
		/// </summary>
		/// <param name="chatId">
		/// Identifier for the target chat. You have different options for identifiers:
		/// <list type="bullet">
		///   <item>The Username of the channel (in the format @channel_username)</item>
		///   <item>The ID of a user, group or channel (in the format "1546456487" or "-165489645654654" etc.)</item>
		/// </list>
		/// The user name of a channel can only be used if the channel is public.
		/// </param>
		/// <param name="file">Path to the document file.</param>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success the payload is a <see href="https://core.telegram.org/bots/api#message">Telegram Message</see>.
		/// </returns>
#if !DEBUG
		[DllExport("SendDocument", CallingConvention = CallingConvention.StdCall)]
#endif
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string SendDocument(
			[MqlParamDoc(ExampleValue = "-1001167825793")]
			[MarshalAs(UnmanagedType.LPWStr)] string chatId,
			[MqlParamDoc(ExampleValue = "D:/pathToFile/log.txt")]
			[MarshalAs(UnmanagedType.LPWStr)] string file)
		{
			try
			{
				Ensure.NotNullOrEmptyOrWhiteSpace(chatId, $"The argument {nameof(chatId)} must not be empty or just whitespace.");
				Ensure.That<ArgumentException>(File.Exists(file), $"The argument {nameof(file)} does not contain a valid file path.");
				return Bot.SendDocument(chatId, file);
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		/// <summary>
		/// <para>Use this method to send general files. Bots can send files of any type of up to 50 MB in size.</para>
		/// <para>See <see href="https://core.telegram.org/bots/api#senddocument">Telegram API</see> for more details.</para>
		/// </summary>
		/// <param name="chatId">
		/// Identifier for the target chat. You have different options for identifiers:
		/// <list type="bullet">
		///   <item>The Username of the channel (in the format @channel_username)</item>
		///   <item>The ID of a user, group or channel (in the format "1546456487" or "-165489645654654" etc.)</item>
		/// </list>
		/// The user name of a channel can only be used if the channel is public.
		/// </param>
		/// <param name="file">Path to the document file.</param>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success the payload is a <see href="https://core.telegram.org/bots/api#message">Telegram Message</see>.
		/// </returns>
		/// <remarks>
		/// <para>This method starts an independent background thread and immediately returns a response with a correlation key but empty payload.</para>
		/// <para>You can use the correlation key to check the result of the thread later via <see cref="GetMessageByCorrelationId"/>.</para>
		/// </remarks>
#if !DEBUG
		[DllExport("StartSendDocument", CallingConvention = CallingConvention.StdCall)]
#endif
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string StartSendDocument(
			[MqlParamDoc(ExampleValue = "-1001167825793")]
			[MarshalAs(UnmanagedType.LPWStr)] string chatId,
			[MqlParamDoc(ExampleValue = "D:/pathToFile/log.txt")]
			[MarshalAs(UnmanagedType.LPWStr)] string file)
		{
			try
			{
				Ensure.NotNullOrEmptyOrWhiteSpace(chatId, $"The argument {nameof(chatId)} must not be empty or just whitespace.");
				Ensure.That<ArgumentException>(File.Exists(file), $"The argument {nameof(file)} does not contain a valid file path.");
				return Bot.StartSendDocument(chatId, file);
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		/// <summary>
		/// <para>Use this method to send a photo.</para>
		/// <para>See <see href="https://core.telegram.org/bots/api#sendphoto">Telegram API</see> for more details.</para>
		/// </summary>
		/// <param name="chatId">
		/// Identifier for the target chat. You have different options for identifiers:
		/// <list type="bullet">
		///   <item>The Username of the channel (in the format @channel_username)</item>
		///   <item>The ID of a user, group or channel (in the format "1546456487" or "-165489645654654" etc.)</item>
		/// </list>
		/// The user name of a channel can only be used if the channel is public.
		/// </param>
		/// <param name="photoFile">Path to the photo file.</param>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success the payload is a <see href="https://core.telegram.org/bots/api#message">Telegram Message</see>.
		/// </returns>
#if !DEBUG
		[DllExport("SendPhoto", CallingConvention = CallingConvention.StdCall)]
#endif
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string SendPhoto(
			[MqlParamDoc(ExampleValue = "-1001167825793")]
			[MarshalAs(UnmanagedType.LPWStr)] string chatId,
			[MqlParamDoc(ExampleValue = "D:/pathToPhoto/photo.png")]
			[MarshalAs(UnmanagedType.LPWStr)] string photoFile)
		{
			try
			{
				Ensure.NotNullOrEmptyOrWhiteSpace(chatId, $"The argument {nameof(chatId)} must not be empty or just whitespace.");
				Ensure.That<ArgumentException>(File.Exists(photoFile), $"The argument {nameof(photoFile)} does not contain a valid file path.");
				return Bot.SendPhoto(chatId, photoFile);
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		/// <summary>
		/// <para>Use this method to send a photo.</para>
		/// <para>See <see href="https://core.telegram.org/bots/api#sendphoto">Telegram API</see> for more details.</para>
		/// </summary>
		/// <param name="chatId">
		/// Identifier for the target chat. You have different options for identifiers:
		/// <list type="bullet">
		///   <item>The Username of the channel (in the format @channel_username)</item>
		///   <item>The ID of a user, group or channel (in the format "1546456487" or "-165489645654654" etc.)</item>
		/// </list>
		/// The user name of a channel can only be used if the channel is public.
		/// </param>
		/// <param name="photoFile">Path to the photo file.</param>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success the payload is a <see href="https://core.telegram.org/bots/api#message">Telegram Message</see>.
		/// </returns>
		/// <remarks>
		/// <para>This method starts an independent background thread and immediately returns a response with a correlation key but empty payload.</para>
		/// <para>You can use the correlation key to check the result of the thread later via <see cref="GetMessageByCorrelationId"/>.</para>
		/// </remarks>
#if !DEBUG
		[DllExport("StartSendPhoto", CallingConvention = CallingConvention.StdCall)]
#endif
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string StartSendPhoto(
			[MqlParamDoc(ExampleValue = "-1001167825793")]
			[MarshalAs(UnmanagedType.LPWStr)] string chatId,
			[MqlParamDoc(ExampleValue = "D:/pathToPhoto/photo.png")]
			[MarshalAs(UnmanagedType.LPWStr)] string photoFile)
		{
			try
			{
				Ensure.NotNullOrEmptyOrWhiteSpace(chatId, $"The argument {nameof(chatId)} must not be empty or just whitespace.");
				Ensure.That<ArgumentException>(File.Exists(photoFile), $"The argument {nameof(photoFile)} does not contain a valid file path.");
				return Bot.StartSendPhoto(chatId, photoFile);
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		/// <summary>
		/// <para>Use this method to send text messages.</para>
		/// <para>See <see href="https://core.telegram.org/bots/api#sendmessage">Telegram API</see> for more details.</para>
		/// </summary>
		/// <param name="chatId">
		/// Identifier for the target chat. You have different options for identifiers:
		/// <list type="bullet">
		///   <item>The Username of the channel (in the format @channel_username)</item>
		///   <item>The ID of a user, group or channel (in the format "1546456487" or "-165489645654654" etc.)</item>
		/// </list>
		/// The user name of a channel can only be used if the channel is public.
		/// </param>
		/// <param name="chatText">Text of the message to be sent.</param>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success the payload is a <see href="https://core.telegram.org/bots/api#message">Telegram Message</see>.
		/// </returns>
#if !DEBUG
		[DllExport("SendText", CallingConvention = CallingConvention.StdCall)]
#endif
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string SendText(
			[MqlParamDoc(ExampleValue = "-1001167825793")]
			[MarshalAs(UnmanagedType.LPWStr)] string chatId,
			[MqlParamDoc(ExampleValue = "Some text")]
			[MarshalAs(UnmanagedType.LPWStr)] string chatText)
		{
			try
			{
				Ensure.NotNullOrEmptyOrWhiteSpace(chatId, $"The argument {nameof(chatId)} must not be empty or just whitespace.");
				Ensure.NotNullOrEmptyOrWhiteSpace(chatText, $"The argument {nameof(chatText)} must not be empty or just whitespace.");
				return Bot.SendText(chatId, chatText);
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		/// <summary>
		/// <para>Use this method to send text messages.</para>
		/// <para>See <see href="https://core.telegram.org/bots/api#sendmessage">Telegram API</see> for more details.</para>
		/// </summary>
		/// <param name="chatId">
		/// Identifier for the target chat. You have different options for identifiers:
		/// <list type="bullet">
		///   <item>The Username of the channel (in the format @channel_username)</item>
		///   <item>The ID of a user, group or channel (in the format "1546456487" or "-165489645654654" etc.)</item>
		/// </list>
		/// The user name of a channel can only be used if the channel is public.
		/// </param>
		/// <param name="chatText">Text of the message to be sent.</param>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success the payload is a <see href="https://core.telegram.org/bots/api#message">Telegram Message</see>.
		/// </returns>
		/// <remarks>
		/// <para>This method starts an independent background thread and immediately returns a response with a correlation key but empty payload.</para>
		/// <para>You can use the correlation key to check the result of the thread later via <see cref="GetMessageByCorrelationId"/>.</para>
		/// </remarks>
#if !DEBUG
		[DllExport("StartSendText", CallingConvention = CallingConvention.StdCall)]
#endif
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string StartSendText(
			[MqlParamDoc(ExampleValue = "-1001167825793")]
			[MarshalAs(UnmanagedType.LPWStr)] string chatId,
			[MqlParamDoc(ExampleValue = "Some text in <b>bold</b> with smile emoji: \\U+1F601")]
			[MarshalAs(UnmanagedType.LPWStr)] string chatText)
		{
			try
			{
				Ensure.NotNullOrEmptyOrWhiteSpace(chatId, $"The argument {nameof(chatId)} must not be empty or just whitespace.");
				Ensure.NotNullOrEmptyOrWhiteSpace(chatText, $"The argument {nameof(chatText)} must not be empty or just whitespace.");
				return Bot.StartSendText(chatId, chatText);
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
#if !DEBUG
		[DllExport("GetMessageByCorrelationId", CallingConvention = CallingConvention.StdCall)]
#endif
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string GetMessageByCorrelationId(
			[MqlParamDoc(ExampleValue = "w8er4345grt76567")]
			[MarshalAs(UnmanagedType.LPWStr)] string correlationKey)
		{
			try
			{
				Ensure.NotNullOrEmptyOrWhiteSpace(correlationKey, $"The argument {nameof(correlationKey)} must not be empty or just whitespace.");
				return Bot.GetMessageByCorrelationId(correlationKey);
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
		/// <param name="apiKey">The API token for the Telegram bot.</param>
		/// <param name="timeout">Seconds that a request to the Telegram API can last before the call gets cancelled.</param>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success there is no payload but only the success flag in the header data.
		/// </returns>
#if !DEBUG
		[DllExport("Initialize", CallingConvention = CallingConvention.StdCall)]
#endif
		[MqlFuncDoc(Order = 1)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string Initialize(
			[MqlParamDoc(ExampleValue = "876708006:BAFEUxGUwPeLFKwPFu4GWjq0saUmVEsKxb4")]
			[MarshalAs(UnmanagedType.LPWStr)] string apiKey,
			[MqlParamDoc(ExampleValue = "3")]
			int timeout)
		{
			try
			{
				Ensure.NotNullOrEmptyOrWhiteSpace(apiKey, $"The argument {nameof(apiKey)} must not be empty or just whitespace.");
				Ensure.That<ArgumentException>(timeout > 0, $"The argument {nameof(timeout)} must be greater than 0.");

				InitializeClass(new TelegramBotMapper(new TelegramBotClient(apiKey), responseFactory));
				SetBotTimeout(timeout);
				return responseFactory.Success().ToString();
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		/// <summary>
		/// Adjusts the timespan that a call to the Telegram API can last before it gets cancelled.
		/// </summary>
		/// <param name="timeout">Seconds that a request to the Telegram API can last.</param>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success there is no payload but only the success flag in the header data.
		/// </returns>
#if !DEBUG
		[DllExport("SetRequestTimeout", CallingConvention = CallingConvention.StdCall)]
#endif
		[MqlFuncDoc(Order = 3)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string SetRequestTimeout(
			[MqlParamDoc(ExampleValue = "3")]
			int timeout)
		{
			try
			{
				Ensure.That<ArgumentException>(timeout > 0, $"The argument {nameof(timeout)} must be greater than 0.");
				SetBotTimeout(timeout);
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
#if !DEBUG
		[DllExport("SetDebugOutput", CallingConvention = CallingConvention.StdCall)]
#endif
		[MqlFuncDoc(Order = 2)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string SetDebugOutput(
			[MqlParamDoc(ExampleValue = "true")]
			[MarshalAs(UnmanagedType.Bool)] bool enableDebug)
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
		/// <item>DisableWebPagePreview - true/false</item>
		/// <item>DisableNotifications - true/false</item>
		/// <item>ParseMode - HTML/Markdown/Default</item>
		/// </list>
		/// The parameter takes effect for every upcoming call to a function using this parameter.
		/// </param>
		/// <param name="defaultValue">The default value for the parameter.</param>
		/// <returns>
		/// A JSON string representing a <see wikiref="https://mmm.steven-england.info/Generic-Response"/>.
		/// On success there is no payload but only the success flag in the header data.
		/// </returns>
#if !DEBUG
		[DllExport("SetDefaultValue", CallingConvention = CallingConvention.StdCall)]
#endif
		[MqlFuncDoc(Order = 3)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string SetDefaultValue(
			[MqlParamDoc(ExampleValue = "ParseMode")]
			[MarshalAs(UnmanagedType.LPWStr)] string parameterKey,
			[MqlParamDoc(ExampleValue = "html")]
			[MarshalAs(UnmanagedType.LPWStr)] string defaultValue)
		{
			try
			{
				Ensure.NotNullOrEmptyOrWhiteSpace(parameterKey, $"The argument {nameof(parameterKey)} must not be empty or just whitespace.");
				Ensure.NotNullOrEmptyOrWhiteSpace(defaultValue, $"The argument {nameof(defaultValue)} must not be empty or just whitespace.");
				bot.SetDefaultValue(parameterKey, defaultValue);
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
			Bot = null;
			isInitialized = false;
		}

		private static void InitializeClass(ITelegramBotMapper telegramBotMapper)
		{
			Bot = telegramBotMapper;
			isInitialized = true;
		}

		private static void SetBotTimeout(int timeout)
		{
			Bot.RequestTimeout = timeout * 1000;
		}
	}
}
