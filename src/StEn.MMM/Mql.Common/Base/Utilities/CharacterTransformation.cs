using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NeoSmart.Unicode;

namespace StEn.MMM.Mql.Common.Base.Utilities
{
	public static class CharacterTransformation
	{
		public static string TransformSpecialCharacters(string text)
		{
			text = DecodeEncodedNonAsciiCharacters(text);
			return text;
		}

		public static string DecodeEncodedNonAsciiCharacters(string text)
		{
			return System.Text.RegularExpressions.Regex.Replace(
				text,
				@"\\[uU]\+(?<Value>[a-zA-Z0-9]{4,6})",
				m =>
				{
					try
					{
						return new Codepoint($"U+{m.Groups["Value"].Value}").AsString();
					}
					catch
					{
						return m.Value;
					}
				});
		}
	}
}
