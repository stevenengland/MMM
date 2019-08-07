namespace StEn.MMM.Mql.Telegram.Services.Telegram
{
	internal interface ITelegramBotMapper
	{
		int RequestTimeout { get; set; }

		string GetMe();

		string GetMeAsync();
	}
}
