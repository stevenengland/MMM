using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using RGiesecke.DllExport;
using StEn.MMM.Mql.Common;
using StEn.MMM.Mql.Common.Services.InApi.Factories;
using StEn.MMM.Mql.Telegram.Services.TelegramBotClient;
using Telegram.Bot;

namespace StEn.MMM.Mql.Telegram
{
	public static class DllExports
	{
		static DllExports()
		{
		}

		internal static ITelegramBotApi Bot { get; set; }

		[DllExport("GetMe", CallingConvention = CallingConvention.StdCall)]
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

		[DllExport("GetMeAsync", CallingConvention = CallingConvention.StdCall)]
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

		[DllExport("Initialize", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static string Initialize(
			[MarshalAs(UnmanagedType.LPWStr)] string apiKey,
			int timeout)
		{
			try
			{
				Bot = new TelegramBotMapper(new TelegramBotClient(apiKey))
				{
					RequestTimeout = timeout,
				};
				return MessageFactory.Success().ToString();
			}
			catch (Exception e)
			{
				return MessageFactory.Error(e).ToString();
			}
		}
	}
}
