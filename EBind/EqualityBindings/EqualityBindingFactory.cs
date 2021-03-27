using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace EBind.EqualityBindings
{
    internal abstract class EqualityBindingFactory
    {
        private static readonly ConcurrentDictionary<Type, EqualityBindingFactory> _equalityBindingFactoryCache = new();

        public static EqualityBindingFactory ForValueType(Type bindingValueType) =>
            _equalityBindingFactoryCache.GetOrAdd(bindingValueType, CreateEqualityBindingFactory);

        private static EqualityBindingFactory CreateEqualityBindingFactory(Type bindingValueType) =>
            (EqualityBindingFactory)Activator.CreateInstance(
                typeof(EqualityBindingFactory<>).MakeGenericType(bindingValueType));

        public abstract EqualityBinding Create(Expression left, Expression right, BindFlag flags, Configuration configuration);
    }

    [Platform.Linker.Preserve(AllMembers = true)]
    internal class EqualityBindingFactory<T> : EqualityBindingFactory
    {
        public override EqualityBinding Create(
            Expression left,
            Expression right,
            BindFlag flags,
            Configuration configuration) => new EqualityBinding<T>(left, right, flags, configuration);
    }
}
