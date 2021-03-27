using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace EBind.MethodDelegates
{
    internal static class MethodDelegateCache
    {
        private static readonly ConcurrentDictionary<MethodInfo, IMethodDelegate> s_cache = new();

        public static bool IsSupported(MethodInfo methodInfo)
        {
            var targetArgumentCount = methodInfo.IsStatic ? 0 : 1;
            var parameters = methodInfo.GetParameters();

            if ((targetArgumentCount + parameters.Length) is < 1 or > 5)
                return false;

            for (var i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType.IsByRef)
                    return false;
            }

            return true;
        }

        public static IMethodDelegate Find(MethodInfo methodInfo, Type targetType)
        {
#if __IOS__
            // Delegate.CreateDelegate does not fully support virtual methods in iOS AOT.
            // It throws `System.ExecutionEngineException : Attempting to JIT compile method '(wrapper delegate-invoke) void <Module>:invoke_callvirt_...`
            if (methodInfo.IsVirtual)
                return FindAotCompatible(methodInfo, targetType);
#endif
            return s_cache.GetOrAdd(methodInfo, CreateDelegate);
        }

#if __IOS__
        private static IMethodDelegate FindAotCompatible(MethodInfo methodInfo, Type targetType)
        {
            if (methodInfo.ReflectedType != targetType)
            {
                var parameterTypes = Array.ConvertAll(methodInfo.GetParameters(), p => p.ParameterType);
                methodInfo = targetType.GetMethod(methodInfo.Name, parameterTypes);
            }

            return s_cache.GetOrAdd(methodInfo, Platform.AotCompatibleDelegateFactory.Create);
        }
#endif

        private static IMethodDelegate CreateDelegate(MethodInfo methodInfo)
        {
            var targetType = methodInfo.ReflectedType;
            var targetArgumentCount = methodInfo.IsStatic ? 0 : 1;
            var returnArgumentCount = methodInfo.ReturnType == typeof(void) ? 0 : 1;

            var parameters = methodInfo.GetParameters();

            var typeArguments = new Type[targetArgumentCount + parameters.Length + returnArgumentCount];

            if (targetArgumentCount > 0)
            {
                typeArguments[0] = targetType;
            }

            for (var i = 0; i < parameters.Length; i++)
            {
                typeArguments[targetArgumentCount + i] = parameters[i].ParameterType;
            }

            if (returnArgumentCount > 0)
            {
                typeArguments[typeArguments.Length - 1] = methodInfo.ReturnType;
            }

            var type = (
                arity: targetArgumentCount + parameters.Length,
                returnsVoid: returnArgumentCount == 0,
                valueTypeInstance: !methodInfo.IsStatic && targetType.IsValueType) switch
            {
                (1, true, true) => typeof(ValueInstanceAction<>),
                (2, true, true) => typeof(ValueInstanceAction<,>),
                (3, true, true) => typeof(ValueInstanceAction<,,>),
                (4, true, true) => typeof(ValueInstanceAction<,,,>),
                (5, true, true) => typeof(ValueInstanceAction<,,,,>),
                (1, true, false) => typeof(ActionDelegate<>),
                (2, true, false) => typeof(ActionDelegate<,>),
                (3, true, false) => typeof(ActionDelegate<,,>),
                (4, true, false) => typeof(ActionDelegate<,,,>),
                (5, true, false) => typeof(ActionDelegate<,,,,>),
                (1, false, true) => typeof(ValueInstanceFunction<,>),
                (2, false, true) => typeof(ValueInstanceFunction<,,>),
                (3, false, true) => typeof(ValueInstanceFunction<,,,>),
                (4, false, true) => typeof(ValueInstanceFunction<,,,,>),
                (5, false, true) => typeof(ValueInstanceFunction<,,,,,>),
                (1, false, false) => typeof(FunctionDelegate<,>),
                (2, false, false) => typeof(FunctionDelegate<,,>),
                (3, false, false) => typeof(FunctionDelegate<,,,>),
                (4, false, false) => typeof(FunctionDelegate<,,,,>),
                (5, false, false) => typeof(FunctionDelegate<,,,,,>),
                _ => throw new NotSupportedException(),
            };

            var genericType = type.MakeGenericType(typeArguments);

            return (IMethodDelegate)Activator.CreateInstance(genericType, methodInfo);
        }
    }
}
