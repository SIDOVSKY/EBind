using System;
using System.Reflection;
using EBind.MethodDelegates;

namespace EBind.Platform
{
    internal static class AotCompatibleDelegateFactory
    {
        /// <summary>
        /// Requires concrete <see cref="MethodInfo"/>s because produced delegate may call the method non-virtually.
        /// </summary>
        public static unsafe IMethodDelegate Create(MethodInfo method)
        {
            var parameters = method.GetParameters();
            var parameterCount = parameters.Length;
            var returnsVoid = method.ReturnType == typeof(void);

            // Check whether faster delegates based on C# 9 function pointers can be used.
            // AOT compiler cannot share the same code for generic function pointers between value-types.
            // It compiles function pointer generics only with concrete type arguments used in the code.
            var noValueTypesInSignature = !method.ReflectedType.IsValueType
                && (returnsVoid || !method.ReturnType.IsValueType)
                && Array.TrueForAll(parameters, p => !p.ParameterType.IsValueType);

            if (noValueTypesInSignature)
                return MethodPointerDelegate(method, parameterCount, returnsVoid);

            // Cannot use function pointers with value-types in methods.
            return new ReflectionMethodDelegate(method);
        }

        private static unsafe IMethodDelegate MethodPointerDelegate(MethodInfo method, int parametersCount, bool returnsVoid)
        {
            var arity = (method.IsStatic ? 0 : 1) + parametersCount;

            var ptr = method.MethodHandle.GetFunctionPointer();

            Func<object?[], object?> del = (arity, returnsVoid) switch
            {
                (1, true) => a =>
                {
                    ((delegate*<object?, void>)ptr)(a[0]);
                    return null;
                },
                (2, true) => a =>
                {
                    ((delegate*<object?, object?, void>)ptr)(a[0], a[1]);
                    return null;
                },
                (3, true) => a =>
                {
                    ((delegate*<object?, object?, object?, void>)ptr)(a[0], a[1], a[2]);
                    return null;
                },
                (4, true) => a =>
                {
                    ((delegate*<object?, object?, object?, object?, void>)ptr)(a[0], a[1], a[2], a[3]);
                    return null;
                },
                (5, true) => a =>
                {
                    ((delegate*<object?, object?, object?, object?, object?, void>)ptr)(a[0], a[1], a[2], a[3], a[4]);
                    return null;
                },

                (1, false) => a => ((delegate*<object?, object?>)ptr)(a[0]),
                (2, false) => a => ((delegate*<object?, object?, object?>)ptr)(a[0], a[1]),
                (3, false) => a => ((delegate*<object?, object?, object?, object?>)ptr)(a[0], a[1], a[2]),
                (4, false) => a => ((delegate*<object?, object?, object?, object?, object?>)ptr)(a[0], a[1], a[2], a[3]),
                (5, false) => a => ((delegate*<object?, object?, object?, object?, object?, object?>)ptr)(a[0], a[1], a[2], a[3], a[4]),

                _ => throw new NotSupportedException(),
            };

            return new WrappingMethodDelegate(del);
        }
    }

    internal class WrappingMethodDelegate : IMethodDelegate
    {
        private readonly Func<object?[], object?> _delegate;

        public WrappingMethodDelegate(Func<object?[], object?> @delegate)
        {
            _delegate = @delegate;
        }

        public object? Invoke(params object?[] args) => _delegate.Invoke(args);
    }

    internal class ReflectionMethodDelegate : IMethodDelegate
    {
        private readonly MethodInfo _method;

        public ReflectionMethodDelegate(MethodInfo method)
        {
            _method = method;
        }

        public object? Invoke(params object?[] args)
        {
            if (_method.IsStatic)
            {
                _method.Invoke(null, args);
            }
            else
            {
                _method.Invoke(args[0], args.AsSpan(1).ToArray());
            }

            return null;
        }
    }
}
