using System;
using System.Reflection;

namespace EBind.MethodDelegates
{
    [Platform.Linker.Preserve(AllMembers = true)]
    internal class ActionDelegate<T0> : DelegateBase<Action<T0?>>
    {
        public ActionDelegate(MethodInfo method) : base(method)
        {
        }

        protected override object? Invoke(Action<T0?> del, object?[] parameters)
        {
            del.Invoke((T0)parameters[0]);
            return null;
        }
    }

    [Platform.Linker.Preserve(AllMembers = true)]
    internal class ActionDelegate<T0, T1> : DelegateBase<Action<T0?, T1?>>
    {
        public ActionDelegate(MethodInfo method) : base(method)
        {
        }

        protected override object? Invoke(Action<T0?, T1?> del, object?[] parameters) {
            del.Invoke(
                (T0)parameters[0],
                (T1)parameters[1]);
            return null;
        }
    }

    [Platform.Linker.Preserve(AllMembers = true)]
    internal class ActionDelegate<T0, T1, T2> : DelegateBase<Action<T0?, T1?, T2?>>
    {
        public ActionDelegate(MethodInfo method) : base(method)
        {
        }

        protected override object? Invoke(Action<T0?, T1?, T2?> del, object?[] parameters) {
            del.Invoke(
                (T0)parameters[0],
                (T1)parameters[1],
                (T2)parameters[2]);
            return null;
        }
    }

    [Platform.Linker.Preserve(AllMembers = true)]
    internal class ActionDelegate<T0, T1, T2, T3> : DelegateBase<Action<T0?, T1?, T2?, T3?>>
    {
        public ActionDelegate(MethodInfo method) : base(method)
        {
        }

        protected override object? Invoke(Action<T0?, T1?, T2?, T3?> del, object?[] parameters) {
            del.Invoke(
                (T0)parameters[0],
                (T1)parameters[1],
                (T2)parameters[2],
                (T3)parameters[3]);
            return null;
        }
    }

    [Platform.Linker.Preserve(AllMembers = true)]
    internal class ActionDelegate<T0, T1, T2, T3, T4> : DelegateBase<Action<T0?, T1?, T2?, T3?, T4?>>
    {
        public ActionDelegate(MethodInfo method) : base(method)
        {
        }

        protected override object? Invoke(Action<T0?, T1?, T2?, T3?, T4?> del, object?[] parameters) {
            del.Invoke(
                (T0)parameters[0],
                (T1)parameters[1],
                (T2)parameters[2],
                (T3)parameters[3],
                (T4)parameters[4]);
            return null;
        }
    }
}
