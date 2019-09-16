using System;
using System.IO;
using System.Linq;
using StEn.MMM.Mql.Generator.Documentation;
using StEn.MMM.Mql.Generator.Mql;
using StEn.MMM.Mql.Generator.Parser;

namespace StEn.MMM.Mql.Generator
{
	public static class Program
	{
		public static void Main(string[] args)
		{
#if DEBUG
			args = new[]
			{
				"D:/coding/MMM/src/StEn.MMM/Mql.Telegram|D:/coding/MMM/src/StEn.MMM/Mql.Mail",
				"D:/coding/MMM/doc",
			};
#endif
			if (args.Length < 2)
			{
				Console.WriteLine("Not enough arguments given.");
				Environment.Exit(1);
			}

			var pathToSourcesList = args[0].Split("|");
			var pathToOutput = args[1];

			foreach (var pathToSources in pathToSourcesList)
			{
				if (!Directory.Exists(pathToSources))
				{
					Console.WriteLine("Directory to sources does not exist.");
					Environment.Exit(1);
				}

				if (!Directory.Exists(args[1]))
				{
					Console.WriteLine("Directory for output does not exist.");
					Environment.Exit(1);
				}

				// Detect export file
				var dllExportFile = Directory.EnumerateFiles(pathToSources, "*.*", SearchOption.AllDirectories)
					.FirstOrDefault(file => file.ToLower().EndsWith("DllExports.cs".ToLower()));

				// Detect project file
				var projectFile = Directory.EnumerateFiles(pathToSources, "*.*", SearchOption.AllDirectories)
					.FirstOrDefault(file => file.ToLower().EndsWith(".csproj".ToLower()));

				var xmlCommentFile = Directory.EnumerateFiles(pathToSources, "*.*", SearchOption.AllDirectories)
					.FirstOrDefault(file => file.ToLower().EndsWith($"{AssemblyPropertyParser.GetAssemblyNameByProjectFile(projectFile)}.xml".ToLower()) &&
											file.ToLower().Contains("release") &&
											file.ToLower().Contains("x64"));

				var binaryFile = Directory.EnumerateFiles(pathToSources, "*.*", SearchOption.AllDirectories)
					.FirstOrDefault(file => file.ToLower().EndsWith($"{AssemblyPropertyParser.GetAssemblyNameByProjectFile(projectFile)}.dll".ToLower()) &&
											file.ToLower().Contains("release") &&
											file.ToLower().Contains("x64"));

				if (dllExportFile == null)
				{
					throw new FileNotFoundException($"{nameof(dllExportFile)} in {pathToSources} was not found");
				}

				if (projectFile == null)
				{
					throw new FileNotFoundException($"{nameof(projectFile)} in {pathToSources} was not found");
				}

				if (xmlCommentFile == null)
				{
					throw new FileNotFoundException($"{nameof(xmlCommentFile)} in {pathToSources} was not found");
				}

				if (binaryFile == null)
				{
					throw new FileNotFoundException($"{nameof(binaryFile)} in {pathToSources} was not found");
				}

				// Read exported functions and their properties
				var functionDefinitions = DllExportParser.GetFunctionDefinitionBySourceFile(dllExportFile);

				// Generate the template text
				var templateText = MqlTemplateGenerator.GenerateTemplateText(
					"Templates/Mql5Basic.template",
					AssemblyPropertyParser.GetAssemblyNameByProjectFile(projectFile),
					AssemblyPropertyParser.GetAssemblyVersionByProjectFile(projectFile),
					functionDefinitions);

				// Write the MQL template text to output files
				MqlTemplateGenerator.WriteTemplateOutput(
					$"{pathToOutput}/module_usage/implementation/metatrader5/MQL5/Experts/MMM_Modules/{AssemblyPropertyParser.GetAssemblyNameByProjectFile(projectFile).ToLower()}/{AssemblyPropertyParser.GetAssemblyNameByProjectFile(projectFile)}_{AssemblyPropertyParser.GetAssemblyVersionByProjectFile(projectFile)}.mq5",
					templateText);
				MqlTemplateGenerator.WriteTemplateOutput(
					$"{pathToOutput}/module_usage/implementation/metatrader5/MQL5/Experts/MMM_Modules/{AssemblyPropertyParser.GetAssemblyNameByProjectFile(projectFile).ToLower()}/{AssemblyPropertyParser.GetAssemblyNameByProjectFile(projectFile)}_latest.mq5",
					templateText);

				// Write the online documentation to output files
				var documentationText = DocumentationGenerator.GenerateDocumentationText(
					xmlCommentFile,
					binaryFile,
					AssemblyPropertyParser.GetAssemblyNameByProjectFile(projectFile),
					AssemblyPropertyParser.GetAssemblyVersionByProjectFile(projectFile),
					functionDefinitions);

				DocumentationGenerator.WriteTemplateOutput(
					$"{pathToOutput}/module_usage/api/{AssemblyPropertyParser.GetAssemblyNameByProjectFile(projectFile).ToLower()}/{AssemblyPropertyParser.GetAssemblyNameByProjectFile(projectFile)}_{AssemblyPropertyParser.GetAssemblyVersionByProjectFile(projectFile)}.md",
					documentationText);
				DocumentationGenerator.WriteTemplateOutput(
					$"{pathToOutput}/module_usage/api/{AssemblyPropertyParser.GetAssemblyNameByProjectFile(projectFile).ToLower()}/{AssemblyPropertyParser.GetAssemblyNameByProjectFile(projectFile)}_latest.md",
					documentationText);
			}
		}
	}
}
