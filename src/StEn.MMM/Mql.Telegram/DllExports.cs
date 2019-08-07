using System;
using System.Runtime.InteropServices;
#if RELEASE
using RGiesecke.DllExport;
#endif
using StEn.MMM.Mql.Common.Base.Utilities;
using StEn.MMM.Mql.Common.Services.InApi.Factories;
using StEn.MMM.Mql.Telegram.Services.Telegram;
using Telegram.Bot;

namespace StEn.MMM.Mql.Telegram
{
	public class DllExports
	{
		static DllExports()
		{
		}

		internal static ITelegramBotMapper Bot { get; set; }

#if RELEASE
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
				Console.WriteLine(e);
				throw;
			}
		}

#if RELEASE
		[DllExport("GetMeAsync", CallingConvention = CallingConvention.StdCall)]
#endif
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string GetMeAsync()
		{
			try
			{
				return Bot.GetMeAsync();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		/*
		[DllExport("TelegramSendText", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string TelegramSendText(
			[MarshalAs(UnmanagedType.LPWStr)] string apiKey,
			[MarshalAs(UnmanagedType.LPWStr)] string chatId,
			[MarshalAs(UnmanagedType.LPWStr)] string chatText)
			{
				try
				{
					var botClient = new TelegramBotClient("YOUR_ACCESS_TOKEN_HERE");
					var me = botClient.GetMeAsync().Result;
					DllExports.apiKey = me.FirstName;
					return DllExports.apiKey;
				}
				catch (Exception e)
				{
					return new Response()
					{
						IsSuccess = false,
					}.ToString();
				}
		}
		*/

#region Configuration API

#if RELEASE
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
				Bot = new TelegramBotMapper(new TelegramBotClient(apiKey))
				{
					RequestTimeout = timeout,
				};
				return ResponseFactory.Success().ToString();
			}
			catch (Exception e)
			{
				return ResponseFactory.Error(e).ToString();
			}
		}

#if RELEASE
		[DllExport("SetRequestTimeout", CallingConvention = CallingConvention.StdCall)]
#endif
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string SetRequestTimeout(int timeout)
		{
			try
			{
				Ensure.That<ArgumentException>(timeout > 0, $"{nameof(timeout)} must be greater than 0.");
				Bot.RequestTimeout = timeout;
				return ResponseFactory.Success().ToString();
			}
			catch (Exception e)
			{
				return ResponseFactory.Error(e).ToString();
			}
		}

#endregion
	}
}
