using System;

namespace EBind
{
    internal class Trigger
    {
        private readonly object _target;
        private readonly TriggerDelegate _triggerDelegate;

        private object? _handler;

        public Trigger(object target, TriggerDelegate triggerDelegate)
        {
            _target = target;
            _triggerDelegate = triggerDelegate;
        }

        public void Subscribe(Action action)
        {
            _handler = _triggerDelegate.CreateHandler(action);

            _triggerDelegate.Subscribe(_target, _handler);
            EBinding.ReportInfo("Added handler for {0}", _target);
        }

        public void Unsubscribe()
        {
            if (_handler == null)
                return;

            _triggerDelegate.Unsubscribe(_target, _handler);
            EBinding.ReportInfo("Removed handler for {0}", _target);
        }
    }
}
