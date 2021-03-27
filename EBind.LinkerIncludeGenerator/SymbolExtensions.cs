using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace EBind.LinkerIncludeGenerator
{
    internal static class SymbolExtensions
    {
        public static IEnumerable<IEventSymbol> Events(this ITypeSymbol? symbol)
        {
            do
            {
                foreach (var member in symbol?.GetMembers().OfType<IEventSymbol>() ?? Enumerable.Empty<IEventSymbol>())
                {
                    yield return member;
                }

                symbol = symbol?.BaseType;
            }
            while (symbol is not null && symbol.Name != "Object");
        }
    }
}
