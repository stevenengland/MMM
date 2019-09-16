using System;
using System.Collections.Generic;
using System.Text;
using StEn.MMM.Mql.Common.Base.Utilities;
using Xunit;

namespace Mql.Common.Tests.Utilities
{
	public class CharacterTransformationTests
	{
		[Theory]
		[InlineData("Hello friend \\U+1F601", "Hello friend 😁")]
		[InlineData("Hello \\U+1F601 friend \\U+1F601", "Hello 😁 friend 😁")]
		[InlineData("This makes no sense \\U+FFFFFF", "This makes no sense \\U+FFFFFF")]
		public void EmojisAreReplaced(string actual, string expected)
		{
			actual = CharacterTransformation.DecodeEncodedNonAsciiCharacters(actual);
			Assert.Equal(expected, actual);
		}
	}
}
