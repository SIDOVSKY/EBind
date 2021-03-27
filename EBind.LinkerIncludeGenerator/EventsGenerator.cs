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
    public class EventsGenerator : ISourceGenerator
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

                var tuples = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<ObjectCreationExpressionSyntax>()
                    .Where(o => model.GetTypeInfo(o).ConvertedType?.Name == "EBinding")
                    .SelectMany(n => n.DescendantNodes())
                    .OfType<TupleExpressionSyntax>()
                    .Where(t => t.Arguments.Count == 3);

                foreach (var tuple in tuples)
                {
                    if (model.GetTypeInfo(tuple).ConvertedType is not INamedTypeSymbol tupleSymbol)
                        continue;

                    var targetTypeSymbol = tupleSymbol.TupleElements[0].Type;
                    var eventNameSymbol = tupleSymbol.TupleElements[1];
                    var eventNameSyntax = tuple.Arguments[1].Expression;

                    if (eventNameSymbol.Name != "eventName")
                        continue;

                    var eventName = model.GetConstantValue(eventNameSyntax).Value as string;

                    var eventSymbol = targetTypeSymbol.Events().FirstOrDefault(e => e.Name == eventName);

                    if (eventSymbol is null
                        || targetTypeSymbol.DeclaredAccessibility < Accessibility.Internal
                        || eventSymbol.DeclaredAccessibility < Accessibility.Internal)
                    {
                        continue;
                    }

                    var targetFullType = targetTypeSymbol.ToDisplayString(SymbolFormat.FullType);

                    includeLines.Add(
                        $"Include<{targetFullType}>(o => o.{eventName} += delegate {{ }});");
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
    internal class Events
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
            context.AddSource("LinkerInclude.Events", SourceText.From(includeCode, Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
