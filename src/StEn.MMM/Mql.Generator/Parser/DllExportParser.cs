using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StEn.MMM.Mql.Common.Base.Attributes;
using StEn.MMM.Mql.Generator.Mql;

namespace StEn.MMM.Mql.Generator.Parser
{
	internal static class DllExportParser
	{
		internal static List<Mql5FunctionDefinition> GetFunctionDefinitionBySourceFile(string filePath)
		{
			List<Mql5FunctionDefinition> definitions = new List<Mql5FunctionDefinition>();

			var programText = File.ReadAllText(filePath);
			var programTree = CSharpSyntaxTree.ParseText(programText)
				.WithFilePath(filePath);

			var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
			var compilation = CSharpCompilation.Create(
				"MyCompilation",
				syntaxTrees: new[] { programTree },
				new[]
				{
					mscorlib,
				});
			var model = compilation.GetSemanticModel(programTree);

			var root = programTree.GetRoot();

			var publicMethods = root.DescendantNodes()
				.OfType<MethodDeclarationSyntax>()
				.Where(method => method.Modifiers
						.Any(modifier => modifier.Kind() == SyntaxKind.PublicKeyword));

			foreach (var methodDeclarationSyntax in publicMethods)
			{
				var definition = new Mql5FunctionDefinition();
				foreach (var attributeListSyntax in methodDeclarationSyntax.AttributeLists)
				{
					foreach (var attributeSyntax in attributeListSyntax.Attributes)
					{
						var name = (IdentifierNameSyntax)attributeSyntax.Name;
						if (name.Identifier.Text == nameof(MqlFuncDocAttribute).Replace("Attribute", string.Empty))
						{
							var methodSymbol = model.GetDeclaredSymbol(methodDeclarationSyntax);
							definition.ClassName = methodSymbol.ContainingType.Name;
							definition.MethodName = methodSymbol.Name;
							definition.MethodReturnType = methodSymbol.ReturnsVoid
									? MapNetTypeToMqlType("void")
									: MapNetTypeToMqlType(methodSymbol.ReturnType.Name);

							foreach (var parameterSyntax in methodDeclarationSyntax.ParameterList.Parameters)
							{
								var parameterSymbol = model.GetDeclaredSymbol(parameterSyntax);
								var exampleValue = GetExampleValue(parameterSyntax);
								if (string.IsNullOrWhiteSpace(exampleValue))
								{
									throw new ArgumentException($"{parameterSymbol.Name} in {methodSymbol.Name} has no documentation attribute assigned");
								}

								string mappableType = string.Empty;
								if (parameterSymbol.Type is IArrayTypeSymbol)
								{
									var x = parameterSymbol.Type as IArrayTypeSymbol;
									mappableType = x.ElementType.Name + "[]";
								}
								else
								{
									mappableType = parameterSymbol.Type.Name;
								}

								definition.Parameters.Add(new FunctionParameter()
								{
									ParameterName = parameterSymbol.Name,
									ParameterType = MapNetTypeToMqlType(mappableType),
									ParameterExample = exampleValue,
								});
							}

							var xmlTrivia = methodDeclarationSyntax.GetLeadingTrivia()
								.Select(i => i.GetStructure())
								.OfType<DocumentationCommentTriviaSyntax>()
								.FirstOrDefault();

							definition.MethodXmlComments = xmlTrivia;
						}

						if (name.Identifier.Text == nameof(MqlFuncDocAttribute).Replace("Attribute", string.Empty))
						{
							foreach (var argument in attributeSyntax.ArgumentList.Arguments)
							{
								if (argument.NameEquals.Name.Identifier.Text == nameof(MqlFuncDocAttribute.Order))
								{
									var expression = argument.Expression as LiteralExpressionSyntax;
									definition.DocumentationOrder = int.TryParse(expression?.Token.ValueText, out var order) ? order : int.MaxValue;
								}

								if (argument.NameEquals.Name.Identifier.Text == nameof(MqlFuncDocAttribute.AdditionalCodeLines))
								{
									var expression = argument.Expression as LiteralExpressionSyntax;
									definition.AdditionalCodeLines = expression?.Token.ValueText;
								}
							}
						}
					}
				}

				definitions.Add(definition);
			}

			return definitions;
		}

		private static string GetExampleValue(ParameterSyntax parameterSyntax)
		{
			// parameterSyntax.AttributeLists.Where(attribute => attribute.Attributes.Where(x => (IdentifierNameSyntax) x.Name.))
			foreach (var attributeList in parameterSyntax.AttributeLists)
			{
				foreach (var attribute in attributeList.Attributes)
				{
					if (attribute.Name is IdentifierNameSyntax)
					{
						var identifierNameSyntax = attribute.Name as IdentifierNameSyntax;
						if (identifierNameSyntax.Identifier.Text == nameof(MqlParamDocAttribute).Replace("Attribute", string.Empty))
						{
							foreach (var argument in attribute.ArgumentList.Arguments)
							{
								if (argument.NameEquals.Name.Identifier.Text == nameof(MqlParamDocAttribute.ExampleValue))
								{
									var expression = argument.Expression as LiteralExpressionSyntax;
									return expression?.Token.ValueText;
								}
							}

							return string.Empty;
						}
					}
				}
			}

			return string.Empty;
		}

		private static string MapNetTypeToMqlType(string netType)
		{
			switch (netType.ToLower())
			{
				case "void":
					return "void";
				case "string":
					return "string";
				case "string[]":
					return "string &[]";
				case "int":
				case "int32":
					return "int";
				case "bool":
				case "boolean":
					return "bool";
				default:
					throw new NotImplementedException(netType);
			}
		}
	}
}
