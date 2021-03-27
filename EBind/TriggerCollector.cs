using System.Collections.Generic;
using System.Linq.Expressions;

namespace EBind
{
    internal sealed class TriggerCollector : ExpressionVisitor
    {
        private readonly MemberTriggerSetup _memberSetup;
        private readonly ExpressionInterpreter _interpreter;

        private List<Trigger> _visitedTriggers = new(2);

        public TriggerCollector(MemberTriggerSetup memberSetup, ExpressionInterpreter interpreter)
        {
            _memberSetup = memberSetup;
            _interpreter = interpreter;
        }

        public List<Trigger> Parse(Expression expression)
        {
            Visit(expression);
            var triggers = _visitedTriggers;
            _visitedTriggers = new List<Trigger>();
            return triggers;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var triggerDelegate = _memberSetup.FindTriggerDelegate(node.Member);

            if (triggerDelegate != null)
            {
                _visitedTriggers.Add(new Trigger(
                    target: _interpreter.FindMemberObjectOrType(node),
                    triggerDelegate: triggerDelegate));
            }

            return base.VisitMember(node);
        }
    }
}
