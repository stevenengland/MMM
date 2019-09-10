using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace StEn.MMM.Mql.Generator.Parser
{
	internal static class AssemblyPropertyParser
	{
		internal static string GetAssemblyNameByProjectFile(string projectFile)
		{
			var csprojText = File.ReadAllText(projectFile);
			Match match = Regex.Match(csprojText, "<AssemblyName>(.*)</AssemblyName>");
			if (match.Success)
			{
				return match.Groups[1].Value;
			}

			throw new KeyNotFoundException();
		}

		internal static string GetAssemblyVersionByProjectFile(string projectFile)
		{
			var csprojText = File.ReadAllText(projectFile);
			Match match = Regex.Match(csprojText, @"<AssemblyVersion>(\d+\.\d+)\.\d+\.\d+</AssemblyVersion>");
			if (match.Success)
			{
				return match.Groups[1].Value;
			}

			throw new KeyNotFoundException();
		}
	}
}
