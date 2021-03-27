using System.Linq.Expressions;

namespace EBind
{
    internal static class ExpressionExtensions
    {
        /// <summary>
        /// Translates the given expression to a source-code string.
        /// </summary>
        /// <remarks>
        /// CPU-intensive operation, not designed for usage in performance-sensitive areas.
        /// Added to make exceptions clearer.
        /// </remarks>
        /// <returns>
        /// Source code representation of an expression. <br/>
        /// Default textual representation in case of an error. <br/>
        /// 'null' for null
        /// </returns>
        public static string ToReadableString(this Expression? expression)
        {
            if (expression is null)
                return "null";

            try
            {
                return AgileObjects.ReadableExpressions.ExpressionExtensions.ToReadableString(expression);
            }
            catch
            {
                return expression.ToString();
            }
        }
    }
}
