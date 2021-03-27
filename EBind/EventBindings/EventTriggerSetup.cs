using System;
using System.Collections.Generic;

namespace EBind.EventBindings
{
    internal class EventTriggerSetup
    {
        private static readonly TypeHierarchyComparer s_hierarchyComparer = new();

        private readonly Dictionary<string, SortedDictionary<Type, TriggerDelegate>> _eventTriggerDelegates = new();

        public void AddTrigger<TTarget, TEventHandler>(
            string eventName,
            Func<Action, TEventHandler> eventHandlerTriggerFactory,
            Action<TTarget, TEventHandler> subscribe,
            Action<TTarget, TEventHandler> unsubscribe) where TEventHandler : class
        {
            var triggerConfig = new TriggerDelegate(
                eventHandlerTriggerFactory.Invoke,
                (t, h) => subscribe((TTarget)t, (TEventHandler)h),
                (t, h) => unsubscribe((TTarget)t, (TEventHandler)h));

            if (!_eventTriggerDelegates.TryGetValue(eventName, out var triggerDelegates))
            {
                _eventTriggerDelegates.Add(eventName, triggerDelegates = new(s_hierarchyComparer));
            }

            var targetType = typeof(TTarget);

            if (triggerDelegates.Count > 0 && triggerDelegates.ContainsKey(targetType))
            {
                EBinding.ReportWarning(
                    $"Event [{targetType.Name}.{eventName}] trigger configuration has been overriden");
            }

            triggerDelegates[targetType] = triggerConfig;
        }

        public TriggerDelegate? FindTriggerDelegate(string eventName, Type targetType)
        {
            if (!_eventTriggerDelegates.TryGetValue(eventName, out var triggers))
                return null;

            foreach (var trigger in triggers)
            {
                if (trigger.Key.IsAssignableFrom(targetType))
                    return trigger.Value;
            }

            return null;
        }
    }
}
