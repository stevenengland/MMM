using StEn.MMM.Mql.Common.Base.Utilities;
using Xunit;

namespace Mql.Common.Tests.Utilities
{
	public class IdGeneratorTests
	{
		[Fact]
		public void SeriesOfCorrectFormattedIdsIsGenerated()
		{
			var id = IDGenerator.Instance.Next;
			Assert.NotNull(id);
		}
	}
}
