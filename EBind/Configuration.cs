using System;
using System.Linq.Expressions;
using System.Reflection;
using EBind.EventBindings;

namespace EBind
{
    /// <summary>
    /// Delegate for invocation of "action" bindings.
    /// </summary>
    /// <param name="bindingAction">Binding action to dispatch.</param>
    public delegate void ActionHandler(Action bindingAction);

    /// <summary>
    /// Delegate for invocation of "equality" bindings.
    /// </summary>
    /// <param name="bindingValueAssignment">Value assignment to dispatch.</param>
    public delegate void AssignmentHandler(Action bindingValueAssignment);

    /// <summary>
    /// Configuration for initialization and execution of bindings.
    /// Provides triggers, dispatchers, and the default settings.
    /// </summary>
    public class Configuration
    {
        private const string NO_MEMBER_ACCESSORS_ERROR = "Expression does not contain any field or property accessors";

        /// <summary>
        /// Initial flag for all bindings created in the corresponding <see cref="EBinding"/>.
        /// </summary>
        public BindFlag DefaultFlag { get; set; }

        /// <summary>
        /// Wraps method invocation in "action" bindings.<br/>
        /// Useful for forcing a specific thread.
        /// </summary>
        public ActionHandler? ActionDispatchDelegate { get; set; }
        /// <summary>
        /// Wraps value assignment in "equality" bindings.<br/>
        /// Useful for forcing a specific thread.
        /// </summary>
        public AssignmentHandler? AssignmentDispatchDelegate { get; set; }

        internal EventTriggerSetup EventTriggerSetup { get; } = new();
        internal MemberTriggerSetup MemberTriggerSetup { get; } = new();
        internal SetterContainer CustomSetterContainer { get; } = new();

        /// <summary>
        /// Maps the member and the non-generic <see cref="EventHandler"/> event that signals about its changes.
        /// </summary>
        /// <remarks>
        /// Example:
        /// <code language="c#">
        /// Configuration.ConfigureTrigger&lt;View, string&gt;(<br/>
        /// ⠀⠀v => v.Text,<br/>
        /// ⠀⠀(v, h) => v.TextEditedEventHandler += h,<br/>
        /// ⠀⠀(v, h) => v.TextEditedEventHandler -= h);
        /// </code>
        /// </remarks>
        /// <typeparam name="TTarget">Type of the member owner.</typeparam>
        /// <typeparam name="TMember">Type of the member.</typeparam>
        /// <param name="member">Member access expression.</param>
        /// <param name="subscribe">Delegate for event subscription.</param>
        /// <param name="unsubscribe">Delegate for unsubscribing from the event.</param>
        /// <param name="setter">A setter that is defined separately from the property/field.</param>
        // U+2800 BRAILLE PATTERN BLANK ('⠀') is used instead of indent spaces in the code sample block
        // because whitespaces are trimmed off.
        public void ConfigureTrigger<TTarget, TMember>(
            Expression<Func<TTarget, TMember>> member,
            Action<TTarget, EventHandler> subscribe,
            Action<TTarget, EventHandler> unsubscribe,
            Action<TTarget, TMember>? setter = null)
        {
            ConfigureTrigger(member, trigger => (_, __) => trigger(), subscribe, unsubscribe, setter);
        }

        /// <summary>
        /// Maps the member and the generic <see cref="EventHandler{TEventArgs}"/> event that signals about its changes.
        /// </summary>
        /// <remarks>
        /// Example:
        /// <code language="c#">
        /// Configuration.ConfigureTrigger&lt;View, View.TextEditedEventArgs, string&gt;(<br/>
        /// ⠀⠀v => v.Text,<br/>
        /// ⠀⠀(v, h) => v.TextEditedGenericEventHandler += h,<br/>
        /// ⠀⠀(v, h) => v.TextEditedGenericEventHandler -= h);<br/>
        /// </code>
        /// </remarks>
        /// <typeparam name="TTarget">Type of the member owner.</typeparam>
        /// <typeparam name="TEventArgs">Type of the event data in the <see cref="EventHandler{TEventArgs}"/>.</typeparam>
        /// <typeparam name="TMember">Type of the member.</typeparam>
        /// <param name="member">Member access expression.</param>
        /// <param name="subscribe">Delegate for event subscription.</param>
        /// <param name="unsubscribe">Delegate for unsubscribing from the event.</param>
        /// <param name="setter">A setter that is defined separately from the property/field.</param>
        // U+2800 BRAILLE PATTERN BLANK ('⠀') is used instead of indent spaces in the code sample block
        // because whitespaces are trimmed off.
        public void ConfigureTrigger<TTarget, TEventArgs, TMember>(
            Expression<Func<TTarget, TMember>> member,
            Action<TTarget, EventHandler<TEventArgs>> subscribe,
            Action<TTarget, EventHandler<TEventArgs>> unsubscribe,
            Action<TTarget, TMember>? setter = null)
        {
            ConfigureTrigger(member, trigger => (_, __) => trigger(), subscribe, unsubscribe, setter);
        }

        /// <summary>
        /// Maps the member and the event of a custom type that signals about its changes.
        /// </summary>
        /// <remarks>
        /// Example:
        /// <code language="c#">
        /// Configuration.ConfigureTrigger&lt;View, Action&lt;string&gt;, string&gt;(<br/>
        /// ⠀⠀v => v.Text,<br/>
        /// ⠀⠀trigger => _ => trigger(),<br/>
        /// ⠀⠀(v, h) => v.TextEditedCustomEventHandler += h,<br/>
        /// ⠀⠀(v, h) => v.TextEditedCustomEventHandler -= h);<br/>
        /// </code>
        /// </remarks>
        /// <typeparam name="TTarget">Type of the member owner.</typeparam>
        /// <typeparam name="TEventHandler">Type of the event handler.</typeparam>
        /// <typeparam name="TMember">Type of the member.</typeparam>
        /// <param name="member">Member access expression.</param>
        /// <param name="triggerToHandler">A factory of event handlers that invoke the binding trigger.</param>
        /// <param name="subscribe">Delegate for event subscription.</param>
        /// <param name="unsubscribe">Delegate for unsubscribing from the event.</param>
        /// <param name="setter">A setter that is defined separately from the property/field.</param>
        // U+2800 BRAILLE PATTERN BLANK ('⠀') is used instead of indent spaces in the code sample block
        // because whitespaces are trimmed off.
        public void ConfigureTrigger<TTarget, TEventHandler, TMember>(
            Expression<Func<TTarget, TMember>> member,
            Func<Action, TEventHandler> triggerToHandler,
            Action<TTarget, TEventHandler> subscribe,
            Action<TTarget, TEventHandler> unsubscribe,
            Action<TTarget, TMember>? setter = null) where TEventHandler : class
        {
            var memberInfo = ExtractPropertyOrFieldInfo(member)
                ?? throw new ArgumentException(NO_MEMBER_ACCESSORS_ERROR, nameof(member));

            MemberTriggerSetup.AddTrigger(
                memberInfo, triggerToHandler, subscribe, unsubscribe);

            if (setter is not null)
            {
                CustomSetterContainer[memberInfo] = (o, v) => setter((TTarget)o, (TMember)v!);
            }
        }

        /// <summary>
        /// Registers the non-generic <see cref="EventHandler"/> event so that <see cref="EBinding"/>
        /// could find it by an identifier during "event" binding creation.
        /// </summary>
        /// <typeparam name="TTarget">Type of the event owner.</typeparam>
        /// <param name="eventName">Event identifier.</param>
        /// <param name="subscribe">Delegate for event subscription.</param>
        /// <param name="unsubscribe">Delegate for unsubscribing from the event.</param>
        public void ConfigureTrigger<TTarget>(
            string eventName,
            Action<TTarget, EventHandler> subscribe,
            Action<TTarget, EventHandler> unsubscribe)
        {
            ConfigureTrigger(eventName, trigger => (_, __) => trigger(), subscribe, unsubscribe);
        }

        /// <summary>
        /// Registers the generic <see cref="EventHandler{TEventArgs}"/> event so that <see cref="EBinding"/>
        /// could find it by an identifier during "event" binding creation.
        /// </summary>
        /// <typeparam name="TTarget">Type of the event owner.</typeparam>
        /// <typeparam name="TEventArgs">Type of the event data in the <see cref="EventHandler{TEventArgs}"/>.</typeparam>
        /// <param name="eventName">Event identifier.</param>
        /// <param name="subscribe">Delegate for event subscription.</param>
        /// <param name="unsubscribe">Delegate for unsubscribing from the event.</param>
        public void ConfigureTrigger<TTarget, TEventArgs>(
            string eventName,
            Action<TTarget, EventHandler<TEventArgs>> subscribe,
            Action<TTarget, EventHandler<TEventArgs>> unsubscribe)
        {
            ConfigureTrigger(eventName, trigger => (_, __) => trigger(), subscribe, unsubscribe);
        }

        /// <summary>
        /// Registers the event of a custom type so that <see cref="EBinding"/>
        /// could find it by an identifier during "event" binding creation.
        /// </summary>
        /// <typeparam name="TTarget">Type of the event owner.</typeparam>
        /// <typeparam name="TEventHandler">Type of the event handler.</typeparam>
        /// <param name="eventName">Event identifier.</param>
        /// <param name="triggerToHandler">A factory of event handlers that invoke the binding trigger.</param>
        /// <param name="subscribe">Delegate for event subscription.</param>
        /// <param name="unsubscribe">Delegate for unsubscribing from the event.</param>
        public void ConfigureTrigger<TTarget, TEventHandler>(
            string eventName,
            Func<Action, TEventHandler> triggerToHandler,
            Action<TTarget, TEventHandler> subscribe,
            Action<TTarget, TEventHandler> unsubscribe) where TEventHandler : class
        {
            EventTriggerSetup.AddTrigger(
                eventName, triggerToHandler, subscribe, unsubscribe);
        }

        private static MemberInfo? ExtractPropertyOrFieldInfo<TTarget, TMember>(Expression<Func<TTarget, TMember>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            var memberInfo = memberExpression?.Member;

            if (memberInfo?.MemberType != MemberTypes.Property
                && memberInfo?.MemberType != MemberTypes.Field)
            {
                return null;
            }

            return memberInfo;
        }
    }
}
