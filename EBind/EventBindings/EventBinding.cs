using System;
using System.Reflection;

namespace EBind.EventBindings
{
    internal class EventBinding : IEBinding
    {
        private readonly string _eventName;
        private readonly TriggerDelegate? _triggerDelegate;
        private readonly EventInfo? _eventInfo;

        private object? _eventHandler;
        private object? _target;

        public EventBinding(
            object target,
            string eventName,
            Action action,
            BindFlag flags,
            EventTriggerSetup eventTriggerSetup)
        {
            _target = target;
            _eventName = eventName;

            if (flags.HasFlag(BindFlag.OneTime))
            {
                action += Dispose;
            }

            var targetType = target.GetType();

            _triggerDelegate = eventTriggerSetup.FindTriggerDelegate(eventName, targetType);

            if (_triggerDelegate != null)
            {
                _eventHandler = _triggerDelegate.CreateHandler(action);
                _triggerDelegate.Subscribe(_target, _eventHandler);
            }
            else
            {
                _eventInfo = targetType.GetEvent(eventName) ?? throw new ArgumentException(
                    $"Event [{eventName} has not been configured for type [{targetType.Name}].\n" +
                    $"Please do that in {nameof(Configuration)} " +
                    $"(e. g. {nameof(EBinding)}.{nameof(EBinding.DefaultConfiguration)})", eventName);
                _eventHandler = _eventInfo.AddUniversalHandler(_target, action);

                EBinding.ReportWarning(
                    $"Used reflection to add handler for [{targetType.Name}.{eventName}].\n" +
                    $"Please configure a trigger in {nameof(Configuration)} to improve performance " +
                    "and avoid linking events away.");
            }

            EBinding.ReportInfo("Added [{0}] handler for {1}", _eventName, _target);
        }

        public void Dispose()
        {
            if (_eventHandler == null)
                return;

            if (_triggerDelegate != null)
            {
                _triggerDelegate.Unsubscribe(_target!, _eventHandler);
            }
            else if (_eventInfo != null)
            {
                _eventInfo.RemoveEventHandler(_target!, (Delegate)_eventHandler);
            }

            EBinding.ReportInfo("Removed handler [{0}] for {1}", _eventName, _target!);

            _target = null;
            _eventHandler = null;
        }
    }
}
