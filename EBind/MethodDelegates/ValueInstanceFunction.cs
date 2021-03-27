using System.Reflection;
using System.Runtime.CompilerServices;

namespace EBind.MethodDelegates
{
    [Platform.Linker.Preserve(AllMembers = true)]
    internal class ValueInstanceFunction<TTarget, TReturn> : DelegateBase<RefFunc<TTarget, TReturn>> where TTarget : struct
    {
        public ValueInstanceFunction(MethodInfo method) : base(method)
        {
        }

        protected override object? Invoke(RefFunc<TTarget, TReturn> del, object?[] parameters) =>
            del.Invoke(ref Unsafe.Unbox<TTarget>(parameters[0]!));
    }

    [Platform.Linker.Preserve(AllMembers = true)]
    internal class ValueInstanceFunction<TTarget, T1, TReturn> : DelegateBase<RefFunc<TTarget, T1, TReturn>> where TTarget : struct
    {
        public ValueInstanceFunction(MethodInfo method) : base(method)
        {
        }

        protected override object? Invoke(RefFunc<TTarget, T1, TReturn> del, object?[] parameters) =>
            del.Invoke(
                ref Unsafe.Unbox<TTarget>(parameters[0]!),
                (T1)parameters[1]
            );
    }

    [Platform.Linker.Preserve(AllMembers = true)]
    internal class ValueInstanceFunction<TTarget, T1, T2, TReturn> : DelegateBase<RefFunc<TTarget, T1, T2, TReturn>> where TTarget : struct
    {
        public ValueInstanceFunction(MethodInfo method) : base(method)
        {
        }

        protected override object? Invoke(RefFunc<TTarget, T1, T2, TReturn> del, object?[] parameters) =>
            del.Invoke(
                ref Unsafe.Unbox<TTarget>(parameters[0]!),
                (T1)parameters[1],
                (T2)parameters[2]
            );
    }

    [Platform.Linker.Preserve(AllMembers = true)]
    internal class ValueInstanceFunction<TTarget, T1, T2, T3, TReturn> : DelegateBase<RefFunc<TTarget, T1, T2, T3, TReturn>> where TTarget : struct
    {
        public ValueInstanceFunction(MethodInfo method) : base(method)
        {
        }

        protected override object? Invoke(RefFunc<TTarget, T1, T2, T3, TReturn> del, object?[] parameters) =>
            del.Invoke(
                ref Unsafe.Unbox<TTarget>(parameters[0]!),
                (T1)parameters[1],
                (T2)parameters[2],
                (T3)parameters[3]
            );
    }

    [Platform.Linker.Preserve(AllMembers = true)]
    internal class ValueInstanceFunction<TTarget, T1, T2, T3, T4, TReturn> : DelegateBase<RefFunc<TTarget, T1, T2, T3, T4, TReturn>> where TTarget : struct
    {
        public ValueInstanceFunction(MethodInfo method) : base(method)
        {
        }

        protected override object? Invoke(RefFunc<TTarget, T1, T2, T3, T4, TReturn> del, object?[] parameters) =>
            del.Invoke(
                ref Unsafe.Unbox<TTarget>(parameters[0]!),
                (T1)parameters[1],
                (T2)parameters[2],
                (T3)parameters[3],
                (T4)parameters[4]
            );
    }
}
