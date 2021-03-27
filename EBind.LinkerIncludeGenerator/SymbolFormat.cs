using Microsoft.CodeAnalysis;

namespace EBind.LinkerIncludeGenerator
{
    public static class SymbolFormat
    {
        public static SymbolDisplayFormat FullType { get; } = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters);
    }
}
