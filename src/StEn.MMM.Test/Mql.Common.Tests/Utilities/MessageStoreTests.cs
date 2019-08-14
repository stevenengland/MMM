using StEn.MMM.Mql.Common.Base.Utilities;
using Xunit;

namespace Mql.Common.Tests.Utilities
{
	public class MessageStoreTests
	{
		[Fact]
		public void MaxCapacityIsGuarantied()
		{
			var store = new MessageStore<int, int>(10);
			for (int i = 1; i <= 11; i++)
			{
				store.Add(i, i);
			}

			Assert.False(store.TryGetValue(1, out _));
			Assert.True(store.TryGetValue(11, out _));
		}
	}
}
