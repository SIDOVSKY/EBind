using System;
using System.Reflection;

namespace EBind.MethodDelegates
{
    [Platform.Linker.Preserve(AllMembers = true)]
    internal class FunctionDelegate<T0, TReturn> : DelegateBase<Func<T0?, TReturn>>
    {
        public FunctionDelegate(MethodInfo method) : base(method)
        {
        }

        protected override object? Invoke(Func<T0?, TReturn> del, object?[] parameters) =>
            del.Invoke((T0)parameters[0]);
    }

    [Platform.Linker.Preserve(AllMembers = true)]
    internal class FunctionDelegate<T0, T1, TReturn> : DelegateBase<Func<T0?, T1?, TReturn>>
    {
        public FunctionDelegate(MethodInfo method) : base(method)
        {
        }

        protected override object? Invoke(Func<T0?, T1?, TReturn> del, object?[] parameters) =>
            del.Invoke(
                (T0)parameters[0],
                (T1)parameters[1]
            );
    }

    [Platform.Linker.Preserve(AllMembers = true)]
    internal class FunctionDelegate<T0, T1, T2, TReturn> : DelegateBase<Func<T0?, T1?, T2?, TReturn>>
    {
        public FunctionDelegate(MethodInfo method) : base(method)
        {
        }

        protected override object? Invoke(Func<T0?, T1?, T2?, TReturn> del, object?[] parameters) =>
            del.Invoke(
                (T0)parameters[0],
                (T1)parameters[1],
                (T2)parameters[2]
            );
    }

    [Platform.Linker.Preserve(AllMembers = true)]
    internal class FunctionDelegate<T0, T1, T2, T3, TReturn> : DelegateBase<Func<T0?, T1?, T2?, T3?, TReturn>>
    {
        public FunctionDelegate(MethodInfo method) : base(method)
        {
        }

        protected override object? Invoke(Func<T0?, T1?, T2?, T3?, TReturn> del, object?[] parameters) =>
            del.Invoke(
                (T0)parameters[0],
                (T1)parameters[1],
                (T2)parameters[2],
                (T3)parameters[3]
            );
    }

    [Platform.Linker.Preserve(AllMembers = true)]
    internal class FunctionDelegate<T0, T1, T2, T3, T4, TReturn> : DelegateBase<Func<T0?, T1?, T2?, T3?, T4?, TReturn>>
    {
        public FunctionDelegate(MethodInfo method) : base(method)
        {
        }

        protected override object? Invoke(Func<T0?, T1?, T2?, T3?, T4?, TReturn> del, object?[] parameters) =>
            del.Invoke(
                (T0)parameters[0],
                (T1)parameters[1],
                (T2)parameters[2],
                (T3)parameters[3],
                (T4)parameters[4]
            );
    }
}
