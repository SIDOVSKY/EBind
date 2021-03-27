using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EBind.LinkerIncludeGenerator
{
    public static class SyntaxHelper
    {
        public static IEnumerable<LambdaExpressionSyntax> ExtractExpressionLambdas(SyntaxTree tree, SemanticModel model)
        {
            return tree.GetRoot()
                .DescendantNodes()
                .OfType<LambdaExpressionSyntax>()
                .Where(l => model.GetTypeInfo(l).ConvertedType?.Name == "Expression");
        }
    }
}
