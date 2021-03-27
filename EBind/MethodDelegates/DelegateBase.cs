using System;
using System.Reflection;

namespace EBind.MethodDelegates
{
    internal abstract class DelegateBase<TDelegate> : IMethodDelegate where TDelegate : Delegate
    {
        protected Lazy<TDelegate> LazyInvoker { get; }
        protected MethodInfo Method { get; }

        protected DelegateBase(MethodInfo method)
        {
            Method = method;
            LazyInvoker = new Lazy<TDelegate>(() => (TDelegate)Method.CreateDelegate(typeof(TDelegate)));
        }

        protected abstract object? Invoke(TDelegate del, object?[] args);

        public object? Invoke(params object?[] args) => Invoke(LazyInvoker.Value, args);
    }
}
