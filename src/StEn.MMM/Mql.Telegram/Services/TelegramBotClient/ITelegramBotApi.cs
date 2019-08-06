namespace StEn.MMM.Mql.Telegram.Services.TelegramBotClient
{
	internal interface ITelegramBotApi
	{
		int RequestTimeout { get; set; }

		string GetMe();

		string GetMeAsync();
	}
}
