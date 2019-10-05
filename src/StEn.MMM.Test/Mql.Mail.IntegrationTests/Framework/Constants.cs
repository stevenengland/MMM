namespace Mql.Mail.IntegrationTests.Framework
{
	public static class Constants
	{
		public const string CategoryTraitName = "Category";

		public const string InteractiveCategoryValue = "Interactive";

		public const string MethodTraitName = "Method";

		public const string AssemblyName = "Mql.Mail.IntegrationTests";

		public const string AssemblyUnderTestName = "StEn.MMM.Mql.Mail.dll";

		public static class Methods
		{
			public const string Internal = "internal";
			public const string SendMail = "sendMail";
		}

		public static class Collections
		{
			public const string IntegrationTests = AssemblyUnderTestName + " integration tests";
		}

		public static class MailServer
		{
			public const string SmtpHostName = "smtp.strato.de";
			public const int SmtpPort = 587;
		}
	}
}
