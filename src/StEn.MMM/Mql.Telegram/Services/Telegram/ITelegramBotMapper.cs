namespace StEn.MMM.Mql.Telegram.Services.Telegram
{
	public interface ITelegramBotMapper
	{
		int RequestTimeout { get; set; }

		string GetMe();

		string GetMeStart();
	}
}
