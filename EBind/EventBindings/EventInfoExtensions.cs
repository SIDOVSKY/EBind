using System;
using System.Reflection;

namespace EBind.EventBindings
{
    internal static class EventInfoExtensions
    {
        public static Delegate AddUniversalHandler(this EventInfo eventInfo, object target, Action handler)
        {
            var eventHandler = handler.Wrap(eventInfo.EventHandlerType);

            eventInfo.AddEventHandler(target, eventHandler);

            return eventHandler;
        }
    }
}
