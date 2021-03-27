using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace EBind
{
    internal class SetterContainer : ISetterProvider
    {
        private readonly Dictionary<MemberInfo, Setter> _memberSetters = new(MemberInfoEqualityComparer.Instance);

        [DisallowNull]
        public Setter? this[MemberInfo member]
        {
            get => _memberSetters.TryGetValue(member, out var setter)
                ? setter
                : null;
            set => _memberSetters[member] = value;
        }
    }
}
