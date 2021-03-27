using System;

namespace EBind.Platform.Linker
{
    /// <summary>
    /// Prevents the Mono Linker from linking the target.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class PreserveAttribute : Attribute
    {
        public bool AllMembers;
        public bool Conditional;
    }
}
