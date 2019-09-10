namespace Mql.Telegram.IntegrationTests.Framework
{
	public static class Constants
	{
		public const string CategoryTraitName = "Category";

		public const string InteractiveCategoryValue = "Interactive";

		public const string MethodTraitName = "Method";

		public const string AssemblyName = "Mql.Telegram.IntegrationTests";

		public const string AssemblyUnderTestName = "Mql_Telegram.dll";

		public static class TestCollections
		{
			public const string GettingUpdates = "Getting Updates";
		}

		public static class TelegramBotApiMethods
		{
			public const string GetMe = "getMe";
			public const string GetUpdates = "getUpdates";
			public const string SendMessage = "sendMessage";
			public const string SendPhoto = "sendPhoto";
		}
	}
}
