using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EBind.ActionBindings;
using EBind.EqualityBindings;

namespace EBind
{
    internal abstract class ExpressionBinding : IEBinding
    {
        protected Configuration Configuration { get; }

        public static ExpressionBinding Create<T>(
            Expression<Func<T>> expr,
            BindFlag flag,
            Configuration configuration)
        {
            if (expr.Body.NodeType == ExpressionType.Equal)
            {
                return EqualityBinding.Create(expr, flag, configuration);
            }
            else if (expr.Body.NodeType == ExpressionType.Call)
            {
                return ActionBinding.Create(expr, flag, configuration);
            }

            throw new NotSupportedException("Only equality and action call binding expressions are supported.\n" +
                $"`{expr.Body.ToReadableString()}` is an expression of type {expr.Body.NodeType}");
        }

        protected ExpressionBinding(Configuration configuration)
        {
            Configuration = configuration;
        }

        public virtual void Dispose()
        {
        }

        private protected void Subscribe(IEnumerable<Trigger> triggers, Action action)
        {
            foreach (var trigger in triggers)
            {
                trigger.Subscribe(action);
            }
        }

        private protected void Unsubscribe(IEnumerable<Trigger>? triggers)
        {
            if (triggers == null)
                return;

            foreach (var trigger in triggers)
            {
                trigger.Unsubscribe();
            }
        }
    }
}
