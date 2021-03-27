using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace EBind
{
    internal static class DelegateTypeWrapper
    {
        private static readonly ConcurrentDictionary<Type, Func<Action, Delegate>> _wrapperFactoryCache = new();

        public static Delegate Wrap(this Action action, Type delegateType)
        {
            if (delegateType == typeof(EventHandler))
                return new EventHandler((_, __) => action());

            return _wrapperFactoryCache
                    .GetOrAdd(delegateType, CreateDelegateWrapperFactory)
                    .Invoke(action);
        }

        private static Func<Action, Delegate> CreateDelegateWrapperFactory(Type delegateType)
        {
            var handlerMethod = delegateType.GetMethod("Invoke")
                ?? throw new ArgumentException("Provided type is not a delegate type", nameof(delegateType));

            var handlerReturnType = handlerMethod.ReturnType;
            var handlerParams = BuildParametersExpressions(handlerMethod);

            var factoryParameter = Parameter(typeof(Action));

            return Lambda<Func<Action, Delegate>>(
                    Lambda(delegateType,
                        Block(
                            Invoke(factoryParameter),
                            Label(
                                Label(handlerReturnType),
                                Default(handlerReturnType))), handlerParams),
                    factoryParameter)
                .Compile();
        }

        private static ParameterExpression[] BuildParametersExpressions(MethodInfo method)
        {
            var parameterInfos = method.GetParameters();
            var parameters = new ParameterExpression[parameterInfos.Length];

            for (int i = 0; i < parameterInfos.Length; i++)
            {
                parameters[i] = Parameter(parameterInfos[i].ParameterType, parameterInfos[i].Name);
            }

            return parameters;
        }
    }
}
