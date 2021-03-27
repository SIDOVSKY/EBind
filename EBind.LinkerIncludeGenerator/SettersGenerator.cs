using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using GeneratorExecutionContext = Uno.SourceGeneration.GeneratorExecutionContext;
using GeneratorInitializationContext = Uno.SourceGeneration.GeneratorInitializationContext;
using ISourceGenerator = Uno.SourceGeneration.ISourceGenerator;

namespace EBind.LinkerIncludeGenerator
{
    public class SettersGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Launch();
            }
#endif

            var includeLines = new List<string>();

            foreach (var tree in context.Compilation.SyntaxTrees)
            {
                var model = context.Compilation.GetSemanticModel(tree);

                var memberExpressions = SyntaxHelper.ExtractExpressionLambdas(tree, model)
                    .SelectMany(n => n.DescendantNodes())
                    .OfType<MemberAccessExpressionSyntax>();

                foreach (var memberAccess in memberExpressions)
                {
                    var symbol = model.GetSymbolInfo(memberAccess).Symbol;

                    if (symbol is not IPropertySymbol { SetMethod: { DeclaredAccessibility: >= Accessibility.Internal } }
                        and not IFieldSymbol { IsConst: false, IsReadOnly: false }
                        || symbol.ContainingType.DeclaredAccessibility < Accessibility.Internal
                        || symbol.DeclaredAccessibility < Accessibility.Internal)
                    {
                        continue;
                    }

                    var targetFullType = symbol.ContainingType.ToDisplayString(SymbolFormat.FullType);

                    includeLines.Add(
                        $"Include<{targetFullType}>(o => o.{symbol.Name} = o.{symbol.Name});");
                }
            }

            if (includeLines.Count == 0)
                return;

            includeLines.Sort();

            var sourceBuilder = new StringBuilder(
@"using System;

namespace EBind.LinkerInclude
{
    [Preserve(AllMembers = true)]
    internal class Setters
    {
        private sealed class PreserveAttribute : Attribute
        {
            public bool AllMembers;
            public bool Conditional;
        }

        void Include<T>(Action<T> action)
        {");

            foreach (var line in includeLines.Distinct())
            {
                sourceBuilder.AppendLine();
                sourceBuilder.Append(' ', 12).Append(line);
            }

            sourceBuilder.Append(@"
        }
    }
}");

            var includeCode = sourceBuilder.ToString();
            context.AddSource("LinkerInclude.Setters", SourceText.From(includeCode, Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
