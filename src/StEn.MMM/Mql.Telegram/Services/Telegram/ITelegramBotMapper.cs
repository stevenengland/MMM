namespace StEn.MMM.Mql.Telegram.Services.Telegram
{
	public interface ITelegramBotMapper
	{
		int RequestTimeout { get; set; }

		string GetMe();

		string StartGetMe();

		string SendText(string chatId, string text);

		string StartSendText(string chatId, string text);
	}
}
