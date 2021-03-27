using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace EBind
{
    internal delegate void Setter(object target, object? value);

    internal interface ISetterProvider
    {
        [DisallowNull]
        Setter? this[MemberInfo member] { get; set; }
    }
}
