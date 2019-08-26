using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StEn.MMM.Mql.Generator.Mql
{
	public static class MqlTemplateGenerator
	{
		internal static void WriteTemplateOutput(string templateFile, string templateText)
		{
			File.WriteAllText(templateFile, templateText);
			File.WriteAllText(templateFile, templateText);
		}

		internal static string GenerateTemplateText(
			string inputTemplateFile,
			string assemblyName,
			string assemblyVersion,
			IEnumerable<Mql5FunctionDefinition> functionDefinitions)
		{
			var template = new MqlTemplate();
			template.VersionSection = assemblyVersion;

			var sortedFunctionDefinitions = functionDefinitions.OrderBy(x => x.DocumentationOrder).ToList();

			var builder = new StringBuilder();
			builder.Append($"#import \"{assemblyName}.dll\"\n");

			foreach (var definition in sortedFunctionDefinitions)
			{
				builder.Append("\t" + CreateMethodImportDirective(definition) + "\n");
			}

			builder.Append($"#import");
			template.ImportSection = builder.ToString();

			builder.Clear();

			foreach (var definition in sortedFunctionDefinitions)
			{
				builder.Append("\t" + CreateMethodExample(definition) + "\n");
				builder.Append("\t" + "Sleep(1000);" + "\n");
			}

			template.ExampleSection = builder.ToString();

			// Replace text passages
			var templateText = File.ReadAllText(inputTemplateFile);
			templateText = templateText.Replace($"<|{nameof(template.ExampleSection)}|>", template.ExampleSection);
			templateText = templateText.Replace($"<|{nameof(template.ImportSection)}|>", template.ImportSection);
			templateText = templateText.Replace($"<|{nameof(template.VersionSection)}|>", template.VersionSection);

			return templateText;
		}

		private static string CreateMethodImportDirective(Mql5FunctionDefinition definition)
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.Append(definition.MethodReturnType + " " + definition.MethodName + "(");
			for (int i = 0; i < definition.Parameters.Count; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(", ");
				}

				stringBuilder.Append(definition.Parameters[i].ParameterType + " " + definition.Parameters[i].ParameterName);
			}

			stringBuilder.Append(");");

			return stringBuilder.ToString();
		}

		private static string CreateMethodExample(Mql5FunctionDefinition definition)
		{
			var builder = new StringBuilder();
			builder.Append("resultOf" + definition.MethodName + " = " + definition.MethodName + "(");
			for (int i = 0; i < definition.Parameters.Count; i++)
			{
				if (i != 0)
				{
					builder.Append(", ");
				}

				if (definition.Parameters[i].ParameterType == "string")
				{
					builder.Append("\"" + definition.Parameters[i].ParameterExample + "\"");
				}
				else
				{
					builder.Append(definition.Parameters[i].ParameterExample);
				}
			}

			builder.Append(");");
			return builder.ToString();
		}
	}
}
