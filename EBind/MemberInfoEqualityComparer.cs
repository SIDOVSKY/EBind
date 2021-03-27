using System.Collections.Generic;
using System.Reflection;

namespace EBind
{
    internal class MemberInfoEqualityComparer : IEqualityComparer<MemberInfo>
    {
        public static MemberInfoEqualityComparer Instance { get; } = new();

        public bool Equals(MemberInfo x, MemberInfo y) =>
            x.MetadataToken == y.MetadataToken && x.Module == y.Module;

        public int GetHashCode(MemberInfo obj) => obj.MetadataToken;
    }
}
