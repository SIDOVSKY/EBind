using System;
using System.ComponentModel;

namespace EBind
{
    internal class TriggerDelegate
    {
        public TriggerDelegate(
            Func<Action, object> handlerFactory,
            Action<object, object> subscribe,
            Action<object, object> unsubscribe)
        {
            CreateHandler = handlerFactory;
            Subscribe = subscribe;
            Unsubscribe = unsubscribe;
        }

        public TriggerDelegate(string notifyPropertyChangedName) : this(
            trigger => new PropertyChangedEventHandler((s, e) =>
            {
                if (e.PropertyName == notifyPropertyChangedName)
                {
                    trigger();
                }
            }),
            (t, h) => ((INotifyPropertyChanged)t).PropertyChanged += (PropertyChangedEventHandler)h,
            (t, h) => ((INotifyPropertyChanged)t).PropertyChanged -= (PropertyChangedEventHandler)h)
        {
        }

        public Func<Action, object> CreateHandler { get; }
        public Action<object, object> Subscribe { get; }
        public Action<object, object> Unsubscribe { get; }
    }
}
