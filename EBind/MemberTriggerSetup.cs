using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace EBind
{
    internal class MemberTriggerSetup
    {
        private readonly Dictionary<MemberInfo, TriggerDelegate> _memberTriggerDelegates
            = new(MemberInfoEqualityComparer.Instance);

        public void AddTrigger<TTarget, TEventHandler>(
            MemberInfo memberInfo,
            Func<Action, TEventHandler> eventHandlerTriggerFactory,
            Action<TTarget, TEventHandler> subscribe,
            Action<TTarget, TEventHandler> unsubscribe) where TEventHandler : class
        {
            var triggerDelegate = new TriggerDelegate(
                eventHandlerTriggerFactory.Invoke,
                (t, h) => subscribe((TTarget)t, (TEventHandler)h),
                (t, h) => unsubscribe((TTarget)t, (TEventHandler)h));

            if (_memberTriggerDelegates.ContainsKey(memberInfo))
            {
                var memberHolderType = memberInfo.ReflectedType.Name;
                var memberName = memberInfo.Name;

                EBinding.ReportWarning(
                    $"[{memberHolderType}.{memberName}] trigger configuration has been overriden");
            }

            _memberTriggerDelegates[memberInfo] = triggerDelegate;
        }

        public TriggerDelegate? FindTriggerDelegate(MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo propertyInfo
               && typeof(INotifyPropertyChanged).IsAssignableFrom(propertyInfo.ReflectedType))
            {
                return new TriggerDelegate(propertyInfo.Name);
            }

            _memberTriggerDelegates.TryGetValue(memberInfo, out var triggerConfig);
            return triggerConfig;
        }
    }
}
