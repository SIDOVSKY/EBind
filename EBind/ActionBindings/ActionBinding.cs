using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EBind.ActionBindings
{
    internal abstract class ActionBinding : ExpressionBinding
    {
        protected ActionBinding(Configuration configuration) : base(configuration)
        {
        }

        public static ActionBinding Create(
                Expression<Action> expr,
                BindFlag flags,
                Configuration configuration)
            => new ActionBinding<Action>(
                expr,
                fallbackActionFactory: static e => e.Compile(true),
                flags,
                configuration);

        public static new ActionBinding Create<T>(
                Expression<Func<T>> expr,
                BindFlag flags,
                Configuration configuration)
            => new ActionBinding<Func<T>>(
                expr,
                fallbackActionFactory: static e =>
                {
                    var funcDelegate = e.Compile(true);
                    return () => funcDelegate.Invoke();
                },
                flags,
                configuration);
    }

    internal sealed class ActionBinding<TDelegate> : ActionBinding where TDelegate : Delegate
    {
        private readonly ExpressionInterpreter _interpreter = new();
        private readonly List<Trigger>? _triggers;

        public ActionBinding(
            Expression<TDelegate> expr,
            Func<Expression<TDelegate>, Action> fallbackActionFactory,
            BindFlag flags,
            Configuration configuration) : base(configuration)
        {
            if (expr.Body is not MethodCallExpression methodCallExpression)
                throw new NotSupportedException();

            var action = _interpreter.TryBuildMethodDelegate(methodCallExpression) ?? fallbackActionFactory(expr);

            if (!flags.HasFlag(BindFlag.NoInitialTrigger))
            {
                PerformAction(action);

                if (flags.HasFlag(BindFlag.OneTime))
                {
                    return;
                }
            }

            _triggers = new TriggerCollector(configuration.MemberTriggerSetup, _interpreter).Parse(expr);

            Subscribe(_triggers, () =>
            {
                PerformAction(action);

                if (flags.HasFlag(BindFlag.OneTime))
                {
                    Unsubscribe(_triggers);
                }
            });
        }

        public override void Dispose()
        {
            Unsubscribe(_triggers);
            base.Dispose();
        }

        private void PerformAction(Action action)
        {
            if (Configuration.ActionDispatchDelegate == null)
                action.Invoke();
            else
                Configuration.ActionDispatchDelegate.Invoke(action);
        }
    }
}
