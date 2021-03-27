using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace EBind.EqualityBindings
{
    internal abstract class EqualityBinding : ExpressionBinding
    {
        protected EqualityBinding(Configuration configuration) : base(configuration)
        {
        }

        internal static new EqualityBinding Create<T>(Expression<Func<T>> expr, BindFlag flags, Configuration configuration)
        {
            var equation = (BinaryExpression)expr.Body;
            var left = equation.Left;
            var right = equation.Right;

            if ((left.NodeType == ExpressionType.Constant && right.NodeType == ExpressionType.Call)
                || (left.NodeType == ExpressionType.Call && right.NodeType == ExpressionType.Constant))
            {
                throw new NotSupportedException(
                    $"Cannot bind constant and method call in expression `{expr.Body.ToReadableString()}`");
            }

            // Compiler converts Enums to ints in expression tree lambdas.
            // Expression<Func<DayOfWeek, bool>> _ = x => x == x
            // is compiled to:
            // Expression<Func<DayOfWeek, bool>> _ = x => (int)x == (int)x
            // Here we unwrap that.
            if (left is UnaryExpression { NodeType: ExpressionType.Convert } leftUnary
                && leftUnary.Type == typeof(int)
                && leftUnary.Operand.Type.IsEnum
                && right is UnaryExpression { NodeType: ExpressionType.Convert } rightUnary
                && rightUnary.Type == typeof(int)
                && rightUnary.Operand.Type.IsEnum)
            {
                left = leftUnary.Operand;
                right = rightUnary.Operand;
            }

            return EqualityBindingFactory.ForValueType(left.Type).Create(left, right, flags, configuration);
        }
    }

    internal class EqualityBinding<T> : EqualityBinding
    {
        private readonly ExpressionInterpreter _interpreter = new();
        private readonly BindFlag _flags;

        private readonly Func<T?>? _leftGetter;
        private readonly Action<T?>? _leftSetter;
        private readonly List<Trigger>? _leftTriggers;

        private readonly Func<T?>? _rightGetter;
        private readonly Action<T?>? _rightSetter;
        private readonly List<Trigger>? _rightTriggers;

        private T? _value;

        public EqualityBinding(
            Expression left,
            Expression right,
            BindFlag flags,
            Configuration configuration) : base(configuration)
        {
            _flags = flags;

            _leftSetter = _interpreter.TryBuildSetter<T?>(left, configuration.CustomSetterContainer);

            if (_leftSetter == null)
            {
                EBinding.ReportWarning(
                    $"Value setting to the LEFT side of expression [{left} == {right}] is DISABLED.\n" +
                    "No assignable target member found.");
            }
            else
            {
                _rightGetter = BuildGetter(right);

                _value = _rightGetter.Invoke();

                if (!_flags.HasFlag(BindFlag.NoInitialTrigger))
                {
                    SetValue(_leftSetter, _value);

                    if (_flags.HasFlag(BindFlag.OneTime))
                        return;
                }

                _rightTriggers = new TriggerCollector(configuration.MemberTriggerSetup, _interpreter).Parse(right);
                Subscribe(_rightTriggers, () => OnSideChanged(_rightGetter, _leftSetter));
            }

            if (_flags.HasFlag(BindFlag.TwoWay))
            {
                _rightSetter = _interpreter.TryBuildSetter<T?>(right, configuration.CustomSetterContainer);

                if (_rightSetter == null)
                {
                    EBinding.ReportWarning(
                        $"Value setting to the RIGHT side of expression [{left} == {right}] is DISABLED.\n" +
                        "No assignable target member found.");
                }
                else
                {
                    _leftGetter = BuildGetter(left);

                    if (_leftSetter == null)
                    {
                        _value = _leftGetter.Invoke();

                        if (!_flags.HasFlag(BindFlag.NoInitialTrigger))
                        {
                            SetValue(_rightSetter, _value);

                            if (_flags.HasFlag(BindFlag.OneTime))
                                return;
                        }
                    }

                    _leftTriggers = new TriggerCollector(configuration.MemberTriggerSetup, _interpreter).Parse(left);
                    Subscribe(_leftTriggers, () => OnSideChanged(_leftGetter, _rightSetter));
                }
            }

            if ((_leftTriggers == null || _leftTriggers.Count == 0)
                && (_rightTriggers == null || _rightTriggers.Count == 0))
            {
                EBinding.ReportWarning(
                    $"Could not find any triggering members for binding from expression [{left} == {right}]");
            }
        }

        public override void Dispose()
        {
            UnsubscribeAllTriggers();
            base.Dispose();
        }

        private protected Func<T?> BuildGetter(Expression expression)
        {
            return _interpreter.TryBuildGetter<T>(expression)
                ?? Lambda<Func<T?>>(expression).Compile(true);
        }

        private protected void SetValue(Action<T?> setter, T? value)
        {
            if (Configuration.AssignmentDispatchDelegate == null)
                setter.Invoke(value);
            else
                Configuration.AssignmentDispatchDelegate.Invoke(() => setter.Invoke(value));

            if (_flags.HasFlag(BindFlag.OneTime))
            {
                UnsubscribeAllTriggers();
            }
        }

        private void OnSideChanged(Func<T?> getter, Action<T?> setter)
        {
            var v = getter.Invoke();

            if (v is null && _value is null)
                return;

            if ((v is null && _value is not null)
                || (v is not null && _value is null)
                || (v is not null && _value is not null && !v.Equals(_value)))
            {
                _value = v;
                SetValue(setter, v);
            }
        }

        private void UnsubscribeAllTriggers()
        {
            Unsubscribe(_leftTriggers);
            Unsubscribe(_rightTriggers);
        }
    }
}
