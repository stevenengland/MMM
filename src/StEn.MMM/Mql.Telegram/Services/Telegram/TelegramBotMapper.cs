using System;
using System.Threading;
using System.Threading.Tasks;
using StEn.MMM.Mql.Common.Base.Extensions.TaskExtensions;
using StEn.MMM.Mql.Common.Base.Interfaces;
using StEn.MMM.Mql.Common.Base.Utilities;
using StEn.MMM.Mql.Common.Services.InApi.Entities;
using StEn.MMM.Mql.Common.Services.InApi.Factories;
using Telegram.Bot;

namespace StEn.MMM.Mql.Telegram.Services.Telegram
{
	public class TelegramBotMapper : ITelegramBotMapper, IErrorHandler, ISuccessHandler, IProxyCall
	{
		private readonly ITelegramBotClient botClient;

		private readonly MessageStore<string, object> messageStore = new MessageStore<string, object>(1000);

		public TelegramBotMapper(ITelegramBotClient botClient)
		{
			this.botClient = botClient;
		}

		public int RequestTimeout { get; set; }

		public void HandleFireAndForgetError(Exception ex, string correlationKey)
		{
			this.messageStore.Add(correlationKey, ResponseFactory.Error(ex).ToString());
		}

		public void HandleFireAndForgetSuccess<T>(T data, string correlationKey)
		{
			this.messageStore.Add(correlationKey, ResponseFactory.Success(message: new Message<T>() { Payload = data }).ToString());
		}

		public string GetMe()
		{
			return this.ProxyCall(this.botClient.GetMeAsync(this.CtFactory()));
		}

		public string GetMeStart()
		{
			return this.FireAndForgetProxyCall(this.botClient.GetMeAsync(this.CtFactory()));
		}

		public string FireAndForgetProxyCall<T>(Task<T> telegramMethod)
		{
			try
			{
				string correlationKey = IDGenerator.Instance.Next;
				telegramMethod.FireAndForgetSafe(correlationKey, this, this);
				return ResponseFactory.Success(correlationKey).ToString();
			}
			catch (Exception ex)
			{
				return ResponseFactory.Error(ex).ToString();
			}
		}

		public string ProxyCall<T>(Task<T> telegramMethod)
		{
			try
			{
				var result = telegramMethod.FireSafe();
				return ResponseFactory.Success(message: new Message<T>() { Payload = result }).ToString();
			}
			catch (Exception ex)
			{
				return ResponseFactory.Error(ex).ToString();
			}
		}

		private CancellationToken CtFactory(int? timeout = null)
		{
			var ctTimeout = timeout ?? this.RequestTimeout;
			var cts = new CancellationTokenSource(ctTimeout);
			return cts.Token;
		}
	}
}
