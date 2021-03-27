using System;
using System.Linq.Expressions;
using System.Reflection;
using EBind.Collections;
using EBind.MethodDelegates;
using EBind.PropertyAccessors;

namespace EBind
{
    internal class ExpressionInterpreter
    {
        private readonly LinkedMap _expressionTargets = new();

        public object FindMemberObjectOrType(MemberExpression expression)
        {
            ref var target = ref _expressionTargets.GetOrAddValueRef(expression);

            if (target is not null)
                return target;

            switch (expression.Expression)
            {
                case ConstantExpression constExpr:
                    return target = constExpr.Value;

                case null: // Target is static
                    return target = expression.Member switch
                    {
                        PropertyInfo property => property.ReflectedType,
                        FieldInfo field => field.ReflectedType,
                        _ => throw new InvalidOperationException(
                            $"Unexpected member type: {expression.Member.MemberType}"),
                    };

                case MemberExpression memberExpr:
                    var memberObject = FindMemberObjectOrType(memberExpr);

                    return target = memberExpr.Member switch
                    {
                        PropertyInfo { CanRead: true } property => property.GetValue(memberObject, null),
                        FieldInfo field => field.GetValue(memberObject),
                        _ => throw new InvalidOperationException(
                            $"Unexpected member type: {memberExpr.Member.MemberType}"),
                    } ?? throw new NullReferenceException(
                            $"Object reference `{memberExpr.Member.Name}` in expression " +
                            $"`{expression.ToReadableString()}` not set to an instance of an object.");

                default:
                    throw new InvalidOperationException(
                        $"Unexpected target expression type: {expression.Expression.GetType()}");
            }
        }

        public Func<T?>? TryBuildGetter<T>(Expression expression)
        {
            switch (expression)
            {
                case ConstantExpression constantExpression:
                    var value = (T)constantExpression.Value;
                    return () => value;

                case MemberExpression memberExpression:
                    return TryBuildMemberGetter<T>(memberExpression);

                case BinaryExpression { NodeType: ExpressionType.Coalesce } coalesceExpression:
                    return TryBuildCoalesceGetter<T>(coalesceExpression);

                case BinaryExpression { NodeType: ExpressionType.AndAlso } andAlsoExpression:
                    return TryBuildAndAlsoGetter(andAlsoExpression) as Func<T>;

                case BinaryExpression { NodeType: ExpressionType.OrElse } orElseExpression:
                    return TryBuildOrElseGetter(orElseExpression) as Func<T>;

                case BinaryExpression { NodeType: ExpressionType.Equal } equalExpression:
                    return TryBuildEqualGetter(equalExpression) as Func<T>;

                case BinaryExpression { NodeType: ExpressionType.Add } addExpression:
                    return TryBuildPlusGetter<T>(addExpression);

                case UnaryExpression { NodeType: ExpressionType.Not } negationExpression
                when typeof(T) == typeof(bool):
                    return TryBuildNegationGetter(negationExpression) as Func<T>;

                case UnaryExpression { NodeType: ExpressionType.Convert } convertExpression
                when convertExpression.Type == typeof(T):
                    return TryBuildConvertGetter<T>(convertExpression);

                case MethodCallExpression methodCall:
                    return TryBuildMethodGetter<T>(methodCall);
            }

            return null;
        }

        private Func<T?>? TryBuildCoalesceGetter<T>(BinaryExpression expression)
        {
            var leftGetter = TryBuildGetter<T>(expression.Left);
            if (leftGetter == null)
                return null;

            var rightGetter = TryBuildGetter<T>(expression.Right);
            if (rightGetter == null)
                return null;

            return () => leftGetter() ?? rightGetter();
        }

        private Func<bool>? TryBuildAndAlsoGetter(BinaryExpression expression)
        {
            var leftGetter = TryBuildGetter<bool>(expression.Left);
            if (leftGetter == null)
                return null;

            var rightGetter = TryBuildGetter<bool>(expression.Right);
            if (rightGetter == null)
                return null;

            return () => leftGetter() && rightGetter();
        }

        private Func<bool>? TryBuildOrElseGetter(BinaryExpression expression)
        {
            var leftGetter = TryBuildGetter<bool>(expression.Left);
            if (leftGetter == null)
                return null;

            var rightGetter = TryBuildGetter<bool>(expression.Right);
            if (rightGetter == null)
                return null;

            return () => leftGetter() || rightGetter();
        }

        private Func<bool>? TryBuildEqualGetter(BinaryExpression expression)
        {
            var leftGetter = TryBuildGetter<object>(expression.Left);
            if (leftGetter == null)
                return null;

            var rightGetter = TryBuildGetter<object>(expression.Right);
            if (rightGetter == null)
                return null;

            return () => Equals(leftGetter(), rightGetter());
        }

        private Func<T?>? TryBuildPlusGetter<T>(BinaryExpression expression)
        {
            if (typeof(T) != typeof(string))
                return null;

            var leftGetter = TryBuildGetter<string>(expression.Left);
            if (leftGetter == null)
                return null;

            var rightGetter = TryBuildGetter<string>(expression.Right);
            if (rightGetter == null)
                return null;

            return new Func<string>(() => leftGetter() + rightGetter()) as Func<T>;
        }

        private Func<bool>? TryBuildNegationGetter(UnaryExpression expression)
        {
            var negationOperandGetter = TryBuildGetter<bool>(expression.Operand);
            if (negationOperandGetter == null)
                return null;

            return () => !negationOperandGetter();
        }

        private Func<T?>? TryBuildConvertGetter<T>(UnaryExpression expression)
        {
            var convertOperandGetter = TryBuildGetter<object>(expression.Operand);
            if (convertOperandGetter == null)
                return null;

            if (typeof(T).IsValueType)
            {
                var method = expression.Method;

                if (method is null)
                {
                    return () => (T)Convert.ChangeType(convertOperandGetter(), typeof(T));
                }

                var convertDelegate = MethodDelegateCache.Find(method, method.ReflectedType);
                return () => (T)convertDelegate.Invoke(convertOperandGetter())!;
            }

            return () => (T)convertOperandGetter();
        }

        private Func<T?>? TryBuildMethodGetter<T>(MethodCallExpression expression)
        {
            var method = expression.Method;

            if (!MethodDelegateCache.IsSupported(method))
                return null;

            var argGetters = TryBuildArgumentGetters(expression);

            if (argGetters is null)
                return null;

            var methodDelegate = MethodDelegateCache.Find(method, expression.Object?.Type ?? method.ReflectedType);

            return () => (T)methodDelegate.Invoke(Array.ConvertAll(argGetters, a => a.Invoke()))!;
        }

        public Action? TryBuildMethodDelegate(MethodCallExpression expression)
        {
            var method = expression.Method;

            if (!MethodDelegateCache.IsSupported(method))
                return null;

            var argGetters = TryBuildArgumentGetters(expression);

            if (argGetters is null)
                return null;

            var methodDelegate = MethodDelegateCache.Find(method, expression.Object?.Type ?? method.ReflectedType);

            return () => methodDelegate.Invoke(Array.ConvertAll(argGetters, a => a.Invoke()));
        }

        private Func<object?>[]? TryBuildArgumentGetters(MethodCallExpression expression)
        {
            var targetCount = expression.Object == null ? 0 : 1;

            var argGetters = new Func<object?>[targetCount + expression.Arguments.Count];

            if (targetCount > 0)
            {
                var objGetter = TryBuildGetter<object?>(expression.Object!);

                if (objGetter is null)
                    return null;

                argGetters[0] = objGetter;
            }

            for (int i = 0; i < expression.Arguments.Count; i++)
            {
                var arg = TryBuildGetter<object?>(expression.Arguments[i]);

                if (arg is null)
                    return null;

                argGetters[targetCount + i] = arg;
            }

            return argGetters;
        }

        public Func<TResult?>? TryBuildMemberGetter<TResult>(MemberExpression memberExpr)
        {
            var target = FindMemberObjectOrType(memberExpr);

            switch (memberExpr.Member)
            {
                case FieldInfo f:
                    return () => (TResult)f.GetValue(target);

                case PropertyInfo p when p.CanRead:
                    var targetType = memberExpr.Expression?.Type // null for static
                        ?? p.ReflectedType;
                    var accessor = PropertyAccessorCache.Find(p, targetType);
                    return () => (TResult)accessor.Get(target);

                default:
                    return null;
            }
        }

        public Action<T>? TryBuildSetter<T>(Expression expr, ISetterProvider? customSetterProvider = null)
        {
            if (expr is UnaryExpression { NodeType: ExpressionType.Convert } convertExpression
                && convertExpression.Operand.Type is Type { IsPrimitive: true } convertType)
            {
                if ((convertExpression.Method is not null && convertExpression.Method.Name != "op_Implicit")
                    || !typeof(IConvertible).IsAssignableFrom(typeof(T)))
                {
                    return null;
                }

                var convertSetter = TryBuildSetter<object>(convertExpression.Operand, customSetterProvider);
                if (convertSetter == null)
                    return null;

                return x =>
                {
                    var value = Convert.ChangeType(x, convertType);
                    convertSetter(value);
                };
            }

            if (expr is not MemberExpression memberExpression)
                return null;

            var target = FindMemberObjectOrType(memberExpression);

            switch (memberExpression.Member)
            {
                case MemberInfo m when customSetterProvider?[m] is Setter customSetter:
                    return x => customSetter(target, x);

                case FieldInfo f:
                    return x => f.SetValue(target, x);

                case PropertyInfo p when p.CanWrite:
                    var targetType = memberExpression.Expression?.Type // null for static
                        ?? p.ReflectedType;
                    var accessor = PropertyAccessorCache.Find(p, targetType);
                    return x => accessor.Set(target, x);

                default:
                    return null;
            }
        }
    }
}
