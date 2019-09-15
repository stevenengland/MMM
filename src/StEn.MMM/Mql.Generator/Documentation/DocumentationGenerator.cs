using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using LoxSmoke.DocXml;
using StEn.MMM.Mql.Generator.Mql;

namespace StEn.MMM.Mql.Generator.Documentation
{
	internal static class DocumentationGenerator
	{
		private static DocXmlReader reader;
		private static Type dllExportsType;

		internal static void WriteTemplateOutput(string templateFile, string templateText)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(templateFile));
			File.WriteAllText(templateFile, templateText);
			File.WriteAllText(templateFile, templateText);
		}

		internal static string GenerateDocumentationText(
			string xmlCommentFile,
			string binaryFile,
			string assemblyName,
			string assemblyVersion,
			IEnumerable<Mql5FunctionDefinition> definitions)
		{
			var sortedFunctionDefinitions = definitions.OrderBy(x => x.MethodName).ToList();
			var asm = Assembly.UnsafeLoadFrom(binaryFile);
			dllExportsType = asm.GetTypes().FirstOrDefault(t => t.IsClass && t.Name.EndsWith("DllExports"));

			reader = new DocXmlReader($"{xmlCommentFile}");

			var builder = new StringBuilder();
			builder.Append($"# {assemblyName} {assemblyVersion} Documentation\n");
			foreach (var definition in sortedFunctionDefinitions)
			{
				builder.Append(GenerateFunctionDocumentationText(definition) + "\n");
			}

			return builder.ToString();
		}

		private static string GenerateFunctionDocumentationText(Mql5FunctionDefinition definition)
		{
			var comments = reader.GetMethodComments(dllExportsType.GetMethod(definition.MethodName));
			var builder = new StringBuilder();

			if (comments.Summary == null)
			{
				throw new KeyNotFoundException();
			}

			builder.Append(MethodHeader(definition) + "\n");
			builder.Append(MethodDefinitionSnippet(definition) + "\n");
			builder.Append(MethodSummery(comments.Summary) + "\n");
			builder.Append("<dl>\n");

			if (!string.IsNullOrWhiteSpace(comments.Returns))
			{
				builder.Append("<dt>Returns</dt>\n");
				builder.Append("<dd>\n");
				builder.Append(MethodReturns(comments.Returns));
				builder.Append("\n</dd>\n");
			}

			if (!string.IsNullOrWhiteSpace(comments.Remarks))
			{
				builder.Append("<dt>Remarks</dt>\n");
				builder.Append("<dd>\n");
				builder.Append(MethodRemarks(comments.Remarks));
				builder.Append("\n</dd>\n");
			}

			if (comments.Parameters.Count > 0)
			{
				builder.Append("<dt>Parameter</dt>\n");
				builder.Append("<dd>\n");
				builder.Append(MethodParameter(comments.Parameters, definition.Parameters));
				builder.Append("\n</dd>\n");
			}

			builder.Append("\n</dl>\n");

			return builder.ToString();
		}

		private static string MethodHeader(Mql5FunctionDefinition definition)
		{
			return $"## <a name=\"{definition.MethodName}\" /> {definition.MethodName}";
		}

		private static string MethodDefinitionSnippet(Mql5FunctionDefinition definition)
		{
			var builder = new StringBuilder();
			builder.Append("```c\n");

			var methodIntro = definition.MethodReturnType + " " + definition.MethodName + " (";
			builder.Append(methodIntro);
			for (int i = 0; i < definition.Parameters.Count; i++)
			{
				if (i != 0)
				{
					builder.Append("," + "\n");
					for (int j = 0; j < methodIntro.Length; j++)
					{
						builder.Append(" ");
					}
				}

				builder.Append(definition.Parameters[i].ParameterType + " " + definition.Parameters[i].ParameterName);
			}

			builder.Append(")\n");

			builder.Append("```");

			return builder.ToString();
		}

		private static string MethodSummery(string commentsSummary)
		{
			return ReplaceTags(commentsSummary);
		}

		private static string MethodReturns(string commentsReturns)
		{
			return ReplaceTags(commentsReturns);
		}

		private static string MethodRemarks(string commentsRemarks)
		{
			return ReplaceTags(commentsRemarks);
		}

		private static string MethodParameter(IReadOnlyCollection<(string Name, string Text)> commentsParameters, IEnumerable<FunctionParameter> definitionParameters)
		{
			var builder = new StringBuilder();

			builder.Append("<table>\n");
			builder.Append("<tr>\n");
			builder.Append("<th>Type</th>");
			builder.Append("<th>Name</th>");
			builder.Append("<th>Description</th>");
			builder.Append("</tr>\n");

			foreach (var definitionParameter in definitionParameters)
			{
				var connectedCommentParameter =
					commentsParameters.First(x => x.Name == definitionParameter.ParameterName);

				builder.Append("<tr>\n");
				builder.Append($"<td>{definitionParameter.ParameterType}</td>");
				builder.Append($"<td>{definitionParameter.ParameterName}</td>");
				builder.Append($"<td>{ReplaceTags(connectedCommentParameter.Text)}</td>");
				builder.Append("</tr>\n");
			}

			builder.Append("</table>\n");

			return builder.ToString();
		}

		private static string ReplaceTags(string text)
		{
			// <see>
			text = Regex.Replace(text, "<see\\s+?cref=\"(.*)\".*\\/>", match =>
			{
				var parts1 = match.ToString().Split("(").First();
				var parts2 = parts1.Split(".").Last();
				var result = Regex.Match(parts2, "^([a-zA-Z0-9_]+)").Value;
				return $"<a href=\"#{result}\">{result}</a>";
			});
			text = Regex.Replace(text, "<see\\s+?wikiref=\"(.*\\/(.*?))\".*\\/>", "<a href=\"$1\">$2</a>");
			text = Regex.Replace(text, "<see.*href=\"(.*)\".*\\/>", "<a href=\"$1\">$1</a>");
			text = Regex.Replace(text, "<see.*href=\"(.*)\".*?>(.+?)<\\/see>", "<a href=\"$1\">$2</a>");

			// <para>
			text = Regex.Replace(text, "<para>(.*?)<\\/para>", "$1\n");

			// <list>
			text = Regex.Replace(
				text,
				"<list\\s+type=\"bullet\">(.*?)<\\/list>",
				match =>
				{
					string itemText = match.Groups[1].ToString();
					itemText = "<ul>" + Regex.Replace(itemText, "<item>(.*?)<\\/item>", "<li>$1</li>") + "</ul>";
					return itemText;
				},
				RegexOptions.Singleline);

			return text;
		}
	}
}
