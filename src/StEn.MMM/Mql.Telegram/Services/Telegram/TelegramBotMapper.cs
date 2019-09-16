using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using StEn.MMM.Mql.Common.Base.Extensions;
using StEn.MMM.Mql.Common.Base.Interfaces;
using StEn.MMM.Mql.Common.Base.Utilities;
using StEn.MMM.Mql.Common.Services.InApi.Factories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace StEn.MMM.Mql.Telegram.Services.Telegram
{
	/// <inheritdoc cref="ITelegramBotMapper" />
	public class TelegramBotMapper : ITelegramBotMapper, IErrorHandler, ISuccessHandler, IProxyCall
	{
		private readonly ITelegramBotClient botClient;

		private readonly MessageStore<string, string> messageStore = new MessageStore<string, string>(1000);

		private readonly ResponseFactory responseFactory;

		/// <summary>
		/// Initializes a new instance of the <see cref="TelegramBotMapper"/> class.
		/// </summary>
		/// <param name="botClient">Reference to <see cref="ITelegramBotMapper"/>. For testing purposes only.</param>
		/// <param name="responseFactory">Reference to a <see cref="ResponseFactory"/>. For testing purposes only.</param>
		public TelegramBotMapper(ITelegramBotClient botClient, ResponseFactory responseFactory = null)
		{
			this.botClient = botClient;
			this.responseFactory = responseFactory ?? new ResponseFactory();
		}

		/// <inheritdoc />
		public int RequestTimeout { get; set; }

		/// <inheritdoc />
		public string GetMessageByCorrelationId(string correlationKey)
		{
			return this.messageStore.TryGetValue(correlationKey, out string resultValue)
				? resultValue
				: this.responseFactory.Error(new KeyNotFoundException($"There is no entry for correlation key {correlationKey} in the queue."), $"There is no entry for correlation key {correlationKey} in the queue.", correlationKey).ToString();
		}

		#region TelegramMethods

		/// <inheritdoc />
		public string GetMe()
		{
			using (var cancellationTokenSource = this.CtsFactory())
			{
				return this.ProxyCall(this.botClient.GetMeAsync(cancellationTokenSource.Token));
			}
		}

		/// <inheritdoc />
		public string StartGetMe()
		{
			var cancellationTokenSource = this.CtsFactory();
			return this.FireAndForgetProxyCall(this.botClient.GetMeAsync(cancellationTokenSource.Token)
				.DisposeAfterThreadCompletionAsync(new IDisposable[]
				{
					cancellationTokenSource,
				}));
		}

		/// <inheritdoc />
		public string GetUpdates()
		{
			using (var cancellationTokenSource = this.CtsFactory())
			{
				return this.ProxyCall(this.botClient.GetUpdatesAsync(
					cancellationToken: cancellationTokenSource.Token));
			}
		}

		/// <inheritdoc />
		public string StartGetUpdates()
		{
			var cancellationTokenSource = this.CtsFactory();
			return this.FireAndForgetProxyCall(this.botClient.GetUpdatesAsync(
					cancellationToken: cancellationTokenSource.Token)
				.DisposeAfterThreadCompletionAsync(new IDisposable[]
				{
					cancellationTokenSource,
				}));
		}

		/// <inheritdoc />
		public string SendPhoto(string chatId, string photoFile)
		{
			using (var fs = System.IO.File.OpenRead(photoFile))
			{
				var inputOnlineFile = new InputOnlineFile(fs);
				using (var cancellationTokenSource = this.CtsFactory())
				{
					return this.ProxyCall(this.botClient.SendPhotoAsync(
						chatId: this.CreateChatId(chatId),
						photo: inputOnlineFile,
						cancellationToken: cancellationTokenSource.Token));
				}
			}
		}

		/// <inheritdoc />
		public string StartSendPhoto(string chatId, string photoFile)
		{
			var fs = System.IO.File.OpenRead(photoFile);

			var inputOnlineFile = new InputOnlineFile(fs);
			var cancellationTokenSource = this.CtsFactory();

			return this.FireAndForgetProxyCall(this.botClient.SendPhotoAsync(
				chatId: this.CreateChatId(chatId),
				photo: inputOnlineFile,
				cancellationToken: cancellationTokenSource.Token)
					.DisposeAfterThreadCompletionAsync(new IDisposable[]
					{
						cancellationTokenSource,
						fs,
					}));
		}

		/// <inheritdoc/>
		public string SendText(string chatId, string text)
		{
			using (var cancellationTokenSource = this.CtsFactory())
			{
				return this.ProxyCall(this.botClient.SendTextMessageAsync(
					chatId: this.CreateChatId(chatId),
					text: CharacterTransformation.TransformSpecialCharacters(text),
					cancellationToken: cancellationTokenSource.Token));
			}
		}

		/// <inheritdoc/>
		public string StartSendText(string chatId, string text)
		{
			var cancellationTokenSource = this.CtsFactory();
			return this.FireAndForgetProxyCall(this.botClient.SendTextMessageAsync(
					chatId: this.CreateChatId(chatId),
					text: CharacterTransformation.TransformSpecialCharacters(text),
					cancellationToken: cancellationTokenSource.Token)
				.DisposeAfterThreadCompletionAsync(new IDisposable[]
				{
					cancellationTokenSource,
				}));
		}

		#endregion

		#region ProxyCalls

		/// <inheritdoc/>
		public void HandleFireAndForgetError(Exception ex, string correlationKey)
		{
			this.messageStore.Add(correlationKey, this.responseFactory.Error(ex).ToString());
		}

		/// <inheritdoc />
		public void HandleFireAndForgetSuccess<T>(T data, string correlationKey)
		{
			this.messageStore.Add(correlationKey, this.responseFactory.Success<T>(message: data, correlationKey).ToString());
		}

		/// <inheritdoc />
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

		/// <inheritdoc />
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
