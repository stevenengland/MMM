using StEn.MMM.Mql.Telegram;
using Xunit;

namespace Mql.Telegram.Tests
{
	public class DllExportsTests
	{
		[Fact]
		public void FirstTest()
		{
			DllExports.Initialize("test", 10);
		}
	}
}
