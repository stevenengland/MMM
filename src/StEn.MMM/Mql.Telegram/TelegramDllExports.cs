using System;
using System.Runtime.InteropServices;
#if !DEBUG
using RGiesecke.DllExport;
#endif
using StEn.MMM.Mql.Common.Base.Utilities;
using StEn.MMM.Mql.Common.Services.InApi.Factories;
using StEn.MMM.Mql.Telegram.Services.Telegram;
using Telegram.Bot;

namespace StEn.MMM.Mql.Telegram
{
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

		/// <see href="https://core.telegram.org/bots/api#getme"/>
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

		/// <see href="https://core.telegram.org/bots/api#getme"/>
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

		/// <see href="https://core.telegram.org/bots/api#sendmessage"/>
#if !DEBUG
		[DllExport("SendText", CallingConvention = CallingConvention.StdCall)]
#endif
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string SendText(
			[MarshalAs(UnmanagedType.LPWStr)] string chatId,
			[MarshalAs(UnmanagedType.LPWStr)] string chatText)
		{
			try
			{
				return Bot.SendText(chatId, chatText);
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		/// <see href="https://core.telegram.org/bots/api#sendmessage"/>
#if !DEBUG
		[DllExport("StartSendText", CallingConvention = CallingConvention.StdCall)]
#endif
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string StartSendText(
			[MarshalAs(UnmanagedType.LPWStr)] string chatId,
			[MarshalAs(UnmanagedType.LPWStr)] string chatText)
		{
			try
			{
				return Bot.StartSendText(chatId, chatText);
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

		#region Configuration API

#if !DEBUG
		[DllExport("Initialize", CallingConvention = CallingConvention.StdCall)]
#endif
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string Initialize(
			[MarshalAs(UnmanagedType.LPWStr)] string apiKey,
			int timeout)
		{
			try
			{
				Ensure.NotNullOrEmptyOrWhiteSpace(apiKey, $"{nameof(apiKey)} must not be empty or just whitespace.");
				Ensure.That<ArgumentException>(timeout > 0, $"{nameof(timeout)} must be greater than 0.");

				InitializeClass(new TelegramBotMapper(new TelegramBotClient(apiKey), responseFactory));
				SetBotTimeout(timeout);
				return responseFactory.Success().ToString();
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

#if !DEBUG
		[DllExport("SetRequestTimeout", CallingConvention = CallingConvention.StdCall)]
#endif
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string SetRequestTimeout(int timeout)
		{
			try
			{
				Ensure.That<ArgumentException>(timeout > 0, $"{nameof(timeout)} must be greater than 0.");
				SetBotTimeout(timeout);
				return responseFactory.Success().ToString();
			}
			catch (Exception e)
			{
				return responseFactory.Error(e).ToString();
			}
		}

#if !DEBUG
		[DllExport("SetDebugOutput", CallingConvention = CallingConvention.StdCall)]
#endif
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string SetDebugOutput([MarshalAs(UnmanagedType.Bool)] bool enableDebug)
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
