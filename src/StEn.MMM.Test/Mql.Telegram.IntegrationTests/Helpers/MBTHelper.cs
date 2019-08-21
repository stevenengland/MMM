using System;
using System.Collections.Generic;
using System.Text;

namespace Mql.Telegram.IntegrationTests.Helpers
{
	/// <summary>
	/// Helper for Mobile-Build-Tools.
	/// <see cref="https://github.com/dansiegel/Mobile.BuildTools"/>
	/// </summary>
	internal class MBTHelper
	{
		/// <summary>
		/// Returns a converted value from the secrets. Secrets are parsed with specific logic if secrets.json does not exist, that is not always suitable.
		/// <see cref="https://github.com/dansiegel/Mobile.BuildTools/blob/333a1dad65fb4f21cbd94adaf28b49de8434623b/Mobile.BuildTools/Generators/BuildHostSecretsGenerator.cs"/>
		/// </summary>
		/// <param name="input">The value to be converted</param>
		/// <returns>A converted secret</returns>
		internal static string ConvertMaskedSecretToRealValue(string input)
		{
			return input.Substring(2);
		}
	}
}
