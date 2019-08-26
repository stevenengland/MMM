using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StEn.MMM.Mql.Generator.Mql
{
	internal class Mql5FunctionDefinition
	{
		public string MethodName { get; set; }

		public string MethodReturnType { get; set; }

		public List<FunctionParameter> Parameters { get; set; } = new List<FunctionParameter>();

		public DocumentationCommentTriviaSyntax MethodXmlComments { get; set; }

		public int DocumentationOrder { get; set; } = int.MaxValue;
	}
}
