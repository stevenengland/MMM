using System;
using System.Threading;
using System.Threading.Tasks;
using StEn.MMM.Mql.Common.Base.Extensions.TaskExtensions;
using StEn.MMM.Mql.Common.Base.Interfaces;
using StEn.MMM.Mql.Common.Base.Utilities;
using StEn.MMM.Mql.Common.Services.InApi.Entities;
using StEn.MMM.Mql.Common.Services.InApi.Factories;
using Telegram.Bot;

namespace StEn.MMM.Mql.Telegram.Services.TelegramBotClient
{
	internal class TelegramBotMapper : ITelegramBotApi, IErrorHandler, ISuccessHandler, IProxyCall
	{
		private readonly ITelegramBotClient botClient;

		private readonly MessageStore<string, object> messageStore = new MessageStore<string, object>(1000);

		public TelegramBotMapper(ITelegramBotClient botClient)
		{
			this.botClient = botClient;
		}

		public int RequestTimeout { get; set; }

		public void HandleError(Exception ex, string correlationKey)
		{
			this.messageStore.Add(correlationKey, MessageFactory.Error(ex).ToString());
		}

		public void HandleSuccess<T>(T data, string correlationKey)
		{
			this.messageStore.Add(correlationKey, MessageFactory.Success(message: new Message<T>() { Payload = data }).ToString());
		}

		public string GetMe()
		{
			return this.ProxyCall(this.botClient.GetMeAsync(this.CtFactory()));
		}

		public string GetMeAsync()
		{
			return this.FireAndForgetProxyCall(this.botClient.GetMeAsync(this.CtFactory()));
		}

		public string FireAndForgetProxyCall<T>(Task<T> telegramMethod)
		{
			try
			{
				string correlationKey = IDGenerator.Instance.Next;
				telegramMethod.FireAndForgetSafe(correlationKey, this, this);
				return MessageFactory.Success(correlationKey).ToString();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				throw;
			}
		}

		public string ProxyCall<T>(Task<T> telegramMethod)
		{
			try
			{
				var result = telegramMethod.FireSafe();
				return MessageFactory.Success(message: new Message<T>() { Payload = result }).ToString();
			}
			catch (Exception ex)
			{
				return MessageFactory.Error(ex).ToString();
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
