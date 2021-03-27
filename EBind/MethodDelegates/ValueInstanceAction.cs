using System.Reflection;
using System.Runtime.CompilerServices;

namespace EBind.MethodDelegates
{
    [Platform.Linker.Preserve(AllMembers = true)]
    internal class ValueInstanceAction<TTarget> : DelegateBase<RefAction<TTarget>> where TTarget : struct
    {
        public ValueInstanceAction(MethodInfo method) : base(method)
        {
        }

        protected override object? Invoke(RefAction<TTarget> del, object?[] parameters)
        {
            del.Invoke(ref Unsafe.Unbox<TTarget>(parameters[0]!));
            return null;
        }
    }

    [Platform.Linker.Preserve(AllMembers = true)]
    internal class ValueInstanceAction<TTarget, T1> : DelegateBase<RefAction<TTarget, T1>> where TTarget : struct
    {
        public ValueInstanceAction(MethodInfo method) : base(method)
        {
        }

        protected override object? Invoke(RefAction<TTarget, T1> del, object?[] parameters)
        {
            del.Invoke(
                ref Unsafe.Unbox<TTarget>(parameters[0]!),
                (T1)parameters[1]
            );
            return null;
        }
    }

    [Platform.Linker.Preserve(AllMembers = true)]
    internal class ValueInstanceAction<TTarget, T1, T2> : DelegateBase<RefAction<TTarget, T1, T2>> where TTarget : struct
    {
        public ValueInstanceAction(MethodInfo method) : base(method)
        {
        }

        protected override object? Invoke(RefAction<TTarget, T1, T2> del, object?[] parameters) {
            del.Invoke(
                ref Unsafe.Unbox<TTarget>(parameters[0]!),
                (T1)parameters[1],
                (T2)parameters[2]);
            return null;
        }
    }

    [Platform.Linker.Preserve(AllMembers = true)]
    internal class ValueInstanceAction<TTarget, T1, T2, T3> : DelegateBase<RefAction<TTarget, T1, T2, T3>> where TTarget : struct
    {
        public ValueInstanceAction(MethodInfo method) : base(method)
        {
        }

        protected override object? Invoke(RefAction<TTarget, T1, T2, T3> del, object?[] parameters) {
            del.Invoke(
                ref Unsafe.Unbox<TTarget>(parameters[0]!),
                (T1)parameters[1],
                (T2)parameters[2],
                (T3)parameters[3]);
            return null;
        }
    }

    [Platform.Linker.Preserve(AllMembers = true)]
    internal class ValueInstanceAction<TTarget, T1, T2, T3, T4> : DelegateBase<RefAction<TTarget, T1, T2, T3, T4>> where TTarget : struct
    {
        public ValueInstanceAction(MethodInfo method) : base(method)
        {
        }

        protected override object? Invoke(RefAction<TTarget, T1, T2, T3, T4> del, object?[] parameters) {
            del.Invoke(
                ref Unsafe.Unbox<TTarget>(parameters[0]!),
                (T1)parameters[1],
                (T2)parameters[2],
                (T3)parameters[3],
                (T4)parameters[4]);
            return null;
        }
    }
}
