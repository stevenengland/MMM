using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StEn.MMM.Mql.Common.Base.Extensions;
using StEn.MMM.Mql.Common.Base.Interfaces;
using StEn.MMM.Mql.Common.Base.Utilities;
using StEn.MMM.Mql.Common.Services.InApi.Entities;
using StEn.MMM.Mql.Common.Services.InApi.Factories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StEn.MMM.Mql.Telegram.Services.Telegram
{
	public class TelegramBotMapper : ITelegramBotMapper, IErrorHandler, ISuccessHandler, IProxyCall
	{
		private readonly ITelegramBotClient botClient;

		private readonly MessageStore<string, string> messageStore = new MessageStore<string, string>(1000);

		private readonly ResponseFactory responseFactory;

		public TelegramBotMapper(ITelegramBotClient botClient, ResponseFactory responseFactory = null)
		{
			this.botClient = botClient;
			this.responseFactory = responseFactory ?? new ResponseFactory();
		}

		public int RequestTimeout { get; set; }

		public string GetMessageByCorrelationId(string correlationKey)
		{
			return this.messageStore.TryGetValue(correlationKey, out string resultValue)
				? resultValue
				: this.responseFactory.Error(new KeyNotFoundException($"There is no entry for correlation key {correlationKey} in the queue."), $"There is no entry for correlation key {correlationKey} in the queue.", correlationKey).ToString();
		}

		#region TelegramMethods

		public string GetMe()
		{
			using (var cancellationTokenSource = this.CtsFactory())
			{
				return this.ProxyCall(this.botClient.GetMeAsync(cancellationTokenSource.Token));
			}
		}

		public string StartGetMe()
		{
			using (var cancellationTokenSource = this.CtsFactory())
			{
				return this.FireAndForgetProxyCall(this.botClient.GetMeAsync(cancellationTokenSource.Token));
			}
		}

		public string SendText(string chatId, string text)
		{
			using (var cancellationTokenSource = this.CtsFactory())
			{
				return this.ProxyCall(this.botClient.SendTextMessageAsync(
					chatId: this.CreateChatId(chatId),
					text: text,
					cancellationToken: cancellationTokenSource.Token));
			}
		}

		public string StartSendText(string chatId, string text)
		{
			using (var cancellationTokenSource = this.CtsFactory())
			{
				return this.ProxyCall(this.botClient.SendTextMessageAsync(
					chatId: this.CreateChatId(chatId),
					text: text,
					cancellationToken: cancellationTokenSource.Token));
			}
		}

		#endregion

		#region ProxyCalls

		public void HandleFireAndForgetError(Exception ex, string correlationKey)
		{
			this.messageStore.Add(correlationKey, this.responseFactory.Error(ex).ToString());
		}

		public void HandleFireAndForgetSuccess<T>(T data, string correlationKey)
		{
			this.messageStore.Add(correlationKey, this.responseFactory.Success(message: data).ToString());
		}

		public string FireAndForgetProxyCall<T>(Task<T> telegramMethod)
		{
			try
			{
				string correlationKey = IDGenerator.Instance.Next;
				telegramMethod.FireAndForgetSafe(correlationKey, this, this);
				return this.responseFactory.Success(correlationKey).ToString();
			}
			catch (Exception ex)
			{
				return this.responseFactory.Error(ex).ToString();
			}
		}

		public string ProxyCall<T>(Task<T> telegramMethod)
		{
			try
			{
				var result = telegramMethod.FireSafe();
				return this.responseFactory.Success(message: result).ToString();
			}
			catch (Exception ex)
			{
				return this.responseFactory.Error(ex).ToString();
			}
		}

		private CancellationTokenSource CtsFactory(int timeout = 0)
		{
			var ctTimeout = timeout == 0 ? this.RequestTimeout : timeout;
			var cts = new CancellationTokenSource(ctTimeout);
			return cts;
		}

		#endregion

		private ChatId CreateChatId(string username)
		{
			if (username.Length > 1 && username.Substring(0, 1) == "@")
			{
				return new ChatId(username);
			}
			else if (int.TryParse(username, out int chatId))
			{
				return new ChatId(chatId);
			}
			else if (long.TryParse(username, out long identifier))
			{
				return new ChatId(identifier);
			}

			throw new NotSupportedException("The format of the specified chat identifier is not supported");
		}
	}
}
