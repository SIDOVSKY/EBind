using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using EBind.ActionBindings;
using EBind.EventBindings;

namespace EBind
{
    /// <summary>
    /// Represetation of a binding created and used with <see cref="EBinding"/>.
    /// </summary>
    public interface IEBinding : IDisposable
    {
    }

    /// <summary>
    /// The main API to work with the library. A facade for binding creation and configuration. <br/>
    /// Acts as a container for created bindings.
    /// When this object is disposed, it will destroy all the bindings contained in it.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "No native resources are used")]
    public class EBinding : IEBinding, IReadOnlyCollection<IEBinding>
    {
        private readonly List<IEBinding> _bindings = new List<IEBinding>(4);

        /// <summary>
        /// Bindings initializer in a collection initializer format.
        /// </summary>
        /// <remarks>
        /// Usage cheatsheet:
        /// <code language="c#">
        /// var binding = new EBinding<br/>
        /// {<br/>
        /// ⠀⠀() => view.Text == vm.Text,<br/>
        /// ⠀⠀() => view.Text == vm.Decription.Title.Text,<br/>
        /// ⠀⠀() => view.Text == (vm.Text ?? vm.FallbackText),<br/>
        /// ⠀⠀() => view.Visible == !vm.TextVisible,<br/>
        /// ⠀⠀() => view.Visible == (vm.TextVisible == vm.ImageVisible),<br/>
        /// ⠀⠀() => view.Visible == (vm.TextVisible &amp;&amp; vm.ImageVisible),<br/>
        /// ⠀⠀() => view.Visible == (vm.TextVisible || vm.ImageVisible),<br/>
        /// ⠀⠀() => view.FullName == $"{vm.FirstName} {vm.LastName}",<br/>
        /// ⠀⠀() => view.FullName == vm.FirstName + " " + vm.LastName,<br/>
        /// ⠀⠀() => view.Timestamp == Converter.DateTimeToEpoch(vm.DateTime),<br/>
        /// ⠀⠀<br/>
        /// ⠀⠀BindFlag.TwoWay,<br/>
        /// ⠀⠀() => view.Text == vm.Text,<br/>
        /// ⠀⠀() => view.SliderValueFloat == vm.AgeInt,<br/>
        /// ⠀⠀<br/>
        /// ⠀⠀BindFlag.OneTime | BindFlag.NoInitialTrigger,<br/>
        /// ⠀⠀() => view.ShowImage(vm.ImageUri),<br/>
        /// ⠀⠀() => Dispatcher.RunOnUiThread(() => view.ShowImage(vm.ImageUri)),<br/>
        /// ⠀⠀<br/>
        /// ⠀⠀BindFlag.OneTime,<br/>
        /// ⠀⠀(view, nameof(view.Click), vm.OnViewClicked),<br/>
        /// ⠀⠀(view, nameof(view.TextEditedEventHandler), () => vm.OnViewTextEdited(view.Text)),<br/>
        /// ⠀⠀(view, "CustomEventForGesture", vm.OnViewClicked),<br/>
        /// ⠀⠀<br/>
        /// ⠀⠀(view, nameof(view.Click), vm.ViewClickCommand),<br/>
        /// ⠀⠀(view, nameof(view.Click), () => vm.ViewClickCommand.TryExecute(view.Text)),<br/>
        /// ⠀⠀<br/>
        /// ⠀⠀new UserExtensions.CustomEBinding(),<br/>
        /// };<br/>
        /// <br/>
        /// binding.Dispose();
        /// </code>
        /// </remarks>
        /// <param name="configuration">
        /// Specific configuration for this binding setup.
        /// <see cref="DefaultConfiguration"/> is used when nothing is specified.
        /// </param>
        // U+2800 BRAILLE PATTERN BLANK ('⠀') is used instead of indent spaces in the code sample block
        // because whitespaces are trimmed off.
        public EBinding(Configuration? configuration = null)
        {
            Configuration = configuration ?? DefaultConfiguration;

            CurrentFlag = Configuration.DefaultFlag;
        }

        /// <summary>
        /// Configuration used by <see cref="EBinding"/> when the corresponding parameter is not specified.
        /// </summary>
        public static Configuration DefaultConfiguration { get; set; } =
#if NETSTANDARD
            new Configuration();
#else
            new Platform.Configuration();
#endif

        /// <summary>
        /// Configuration for initialization and execution of bindings.
        /// </summary>
        public Configuration Configuration { get; }

        /// <summary>
        /// Gets the number of bindings.
        /// </summary>
        /// <returns>The number of bindings.</returns>
        public int Count => _bindings.Count;

        /// <summary>
        /// Current options for binding construction.
        /// </summary>
        public BindFlag CurrentFlag { get; private set; }

        /// <summary>
        /// Sets the flag for construction of subsequent bindings.
        /// </summary>
        /// <param name="nextFlag"></param>
        public void Add(BindFlag nextFlag)
        {
            CurrentFlag = nextFlag;
        }

        /// <summary>
        /// Collection input for custom bindings.<br/>
        /// Custom binding can be initialized in a <see href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/object-and-collection-initializers#collection-initializers:~:text=an%20extension%20method">
        /// collection initializer extension method</see> with <see cref="CurrentFlag"/> and <see cref="Configuration"/>.
        /// </summary>
        /// <remarks>
        /// It's recommended to decorate exceptions during binding creation with additional information about
        /// the binding position (<see cref="Count"/>) and its line number
        /// (<see cref="CallerLineNumberAttribute"/>) as debuggers dont highlight that.<br/>
        /// <br/>
        /// Example:
        /// <code language="c#">
        /// public static void Add(this EBinding bindingHolder, CustomEBinding binding, [CallerLineNumber] int sourceLineNumber = 0)<br/>
        /// {<br/>
        /// ⠀⠀try<br/>
        /// ⠀⠀{<br/>
        /// ⠀⠀⠀⠀if (binding == null)<br/>
        /// ⠀⠀⠀⠀⠀⠀throw new ArgumentNullException(nameof(binding));<br/>
        /// ⠀⠀⠀⠀<br/>
        /// ⠀⠀⠀⠀if (bindingHolder.CurrentFlag == BindFlag.OneTime)<br/>
        /// ⠀⠀⠀⠀⠀⠀throw new NotSupportedException();<br/>
        /// ⠀⠀<br/>
        /// ⠀⠀⠀⠀bindingHolder.Add(binding);<br/>
        /// ⠀⠀}<br/>
        /// ⠀⠀catch (Exception ex)<br/>
        /// ⠀⠀{<br/>
        /// ⠀⠀⠀⠀throw new Exception($"Error in entry {bindingHolder.Count} at line #{sourceLineNumber}", ex);<br/>
        /// ⠀⠀}<br/>
        /// }<br/>
        /// </code>
        /// </remarks>
        // U+2800 BRAILLE PATTERN BLANK ('⠀') is used instead of indent spaces in the code sample block
        // because whitespaces are trimmed off.
        public void Add(IEBinding binding)
        {
            if (binding == null)
                throw new ArgumentNullException(nameof(binding), $"Entry #{Count}");

            _bindings.Add(binding);
        }

        /// <summary>
        /// Collection initializer for "equality" and "action" binding expressions.
        /// </summary>
        /// <remarks>
        /// Format:
        /// <code language="c#">
        /// () => view.Text == vm.Text,<br/>
        /// () => view.ShowImage(vm.ImageUri),
        /// </code>
        /// </remarks>
        /// <param name="specification"><see cref="LambdaExpression"/> that describes the binding.</param>
        /// <param name="sourceLineNumber">A compiler-supplied binding location used for exception indication.</param>
        public void Add<T>(Expression<Func<T>> specification,
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                if (specification == null)
                    throw new ArgumentNullException(nameof(specification));

                Add(ExpressionBinding.Create(specification, CurrentFlag, Configuration));
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error in entry {Count} at line #{sourceLineNumber}:\n`{specification.ToReadableString()}`", ex);
            }
        }

        /// <summary>
        /// Collection initializer for "action" binding expressions.
        /// </summary>
        /// <remarks>
        /// Format:
        /// <code language="c#">
        /// () => view.ShowImage(vm.ImageUri),
        /// </code>
        /// </remarks>
        /// <param name="specification"><see cref="LambdaExpression"/> that describes the binding.</param>
        /// <param name="sourceLineNumber">A compiler-supplied binding location used for exception indication.</param>
        public void Add(Expression<Action> specification,
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                if (specification == null)
                    throw new ArgumentNullException(nameof(specification));

                Add(ActionBinding.Create(specification, CurrentFlag, Configuration));
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error in entry {Count} at line #{sourceLineNumber}:\n`{specification.ToReadableString()}`", ex);
            }
        }

        /// <summary>
        /// Collection initializer for binding delegates to events.<br/>
        /// Custom events are configured in <see cref="Configuration.ConfigureTrigger{TTarget, TEventHandler, TMember}(Expression{Func{TTarget, TMember}}, Func{Action, TEventHandler}, Action{TTarget, TEventHandler}, Action{TTarget, TEventHandler}, Action{TTarget, TMember}?)"/>.
        /// </summary>
        /// <remarks>
        /// Format:
        /// <code language="c#">
        /// (view, nameof(view.Click), vm.OnViewClicked),
        /// </code>
        /// </remarks>
        /// <typeparam name="T">Type of the event target.</typeparam>
        /// <param name="eventBindElements">A tuple of parameters for the event binding.</param>
        /// <param name="sourceLineNumber">A compiler-supplied binding location used for exception indication.</param>
        public void Add<T>((T target, string eventName, Action action) eventBindElements,
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                var (target, eventName, action) = eventBindElements;

                if (target == null)
                    throw new ArgumentNullException(nameof(target));

                if (eventName == null)
                    throw new ArgumentNullException(nameof(eventName));

                if (action == null)
                    throw new ArgumentNullException(nameof(action));

                Add(new EventBinding(target, eventName, action, CurrentFlag, Configuration.EventTriggerSetup));
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error in entry {Count} at line #{sourceLineNumber}", ex);
            }
        }

        /// <summary>
        /// Collection initializer for binding commands to events.<br/>
        /// Custom events are configured in <see cref="Configuration.ConfigureTrigger{TTarget, TEventHandler, TMember}(Expression{Func{TTarget, TMember}}, Func{Action, TEventHandler}, Action{TTarget, TEventHandler}, Action{TTarget, TEventHandler}, Action{TTarget, TMember}?)"/>.
        /// </summary>
        /// <remarks>
        /// Format:
        /// <code language="c#">
        /// (view, nameof(view.Click), vm.ViewClickCommand),
        /// </code>
        /// </remarks>
        /// <typeparam name="T">Type of the event target.</typeparam>
        /// <param name="eventBindElements">A tuple of parameters for the event binding.</param>
        /// <param name="sourceLineNumber">A compiler-supplied binding location used for exception indication.</param>
        public void Add<T>((T target, string eventName, ICommand command) eventBindElements,
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                var (target, eventName, command) = eventBindElements;

                if (target == null)
                    throw new ArgumentNullException(nameof(target));

                if (eventName == null)
                    throw new ArgumentNullException(nameof(eventName));

                if (command == null)
                    throw new ArgumentNullException(nameof(command));

                Add(new EventBinding(
                    target, eventName, () => command.TryExecute(), CurrentFlag, Configuration.EventTriggerSetup));
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error in entry {Count} at line #{sourceLineNumber}", ex);
            }
        }

        /// <summary>
        /// Unbind, disable bindings.
        /// Unsubscribes all the listeners of value changes, removes references to itself from the binding targets.
        /// Becomes free for GC.
        /// This cannot be undone.
        /// </summary>
        public void Dispose()
        {
            foreach (var b in _bindings)
            {
                b.Dispose();
            }
            _bindings.Clear();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="EBinding"/> collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the <see cref="EBinding"/> collection.</returns>
        public IEnumerator<IEBinding> GetEnumerator() => _bindings.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _bindings.GetEnumerator();

        [Conditional("DEBUG")]
        internal static void ReportInfo(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Debug.WriteLine(message, "[EBIND]");
        }

        internal static void ReportWarning(string message)
        {
            Debug.WriteLine(message, "[EBIND WARNING]");
        }
    }
}
