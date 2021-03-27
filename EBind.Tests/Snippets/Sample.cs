using System;
using System.ComponentModel;
using System.Windows.Input;

namespace EBind.Tests.Snippets
{
    internal abstract class Sample
    {
        protected abstract View view { get; }
        protected abstract ViewModel vm { get; }

        protected void Setup()
        {
            EBinding.DefaultConfiguration.AssignmentDispatchDelegate = Dispatcher.RunOnUiThread;
            EBinding.DefaultConfiguration.ActionDispatchDelegate = Dispatcher.RunOnUiThread;

            // begin-snippet: Configure-Member-Trigger
            EBinding.DefaultConfiguration.ConfigureTrigger<View, string>(
                v => v.Text,
                (v, h) => v.TextEditedEventHandler += h,
                (v, h) => v.TextEditedEventHandler -= h);

            EBinding.DefaultConfiguration.ConfigureTrigger<View, View.TextEditedEventArgs, string>(
                v => v.Text,
                (v, h) => v.TextEditedGenericEventHandler += h,
                (v, h) => v.TextEditedGenericEventHandler -= h);

            EBinding.DefaultConfiguration.ConfigureTrigger<View, Action<string>, string>(
                v => v.Text,
                trigger => _ => trigger(),
                (v, h) => v.TextEditedCustomEventHandler += h,
                (v, h) => v.TextEditedCustomEventHandler -= h);
            // end-snippet

            // begin-snippet: Configure-Event-Trigger
            EBinding.DefaultConfiguration.ConfigureTrigger<View>(
                nameof(View.TextEditedEventHandler),
                (v, h) => v.TextEditedEventHandler += h,
                (v, h) => v.TextEditedEventHandler -= h);

            EBinding.DefaultConfiguration.ConfigureTrigger<View, View.TextEditedEventArgs>(
                nameof(View.TextEditedGenericEventHandler),
                (v, h) => v.TextEditedGenericEventHandler += h,
                (v, h) => v.TextEditedGenericEventHandler -= h);

            EBinding.DefaultConfiguration.ConfigureTrigger<View, Action<string>>(
                nameof(View.TextEditedCustomEventHandler),
                trigger => _ => trigger(),
                (v, h) => v.TextEditedCustomEventHandler += h,
                (v, h) => v.TextEditedCustomEventHandler -= h);
            // end-snippet

            // begin-snippet: Configure-Custom-Event-Trigger
            EBinding.DefaultConfiguration.ConfigureTrigger<View, View.GestureRecognizer>(
                "CustomEventForGesture",
                trigger => new View.GestureRecognizer(trigger),
                (v, h) => v.AddGestureRecognizer(h),
                (v, h) => v.RemoveGestureRecognizer(h));
            // end-snippet
        }

        protected void Bind()
        {
            // begin-snippet: Bind
            var binding = new EBinding
            {
                () => view.Text == vm.Text,
                () => view.Text == vm.Description.Title.Text,
                () => view.Text == (vm.Text ?? vm.FallbackText),
                () => view.Visible == !vm.TextVisible,
                () => view.Visible == (vm.TextVisible == vm.ImageVisible),
                () => view.Visible == (vm.TextVisible && vm.ImageVisible),
                () => view.Visible == (vm.TextVisible || vm.ImageVisible),
                () => view.FullName == $"{vm.FirstName} {vm.LastName}",
                () => view.FullName == vm.FirstName + " " + vm.LastName,
                () => view.Timestamp == Converter.DateTimeToEpoch(vm.DateTime),

                BindFlag.TwoWay,
                () => view.Text == vm.Text,
                () => view.SliderValueFloat == vm.AgeInt,

                BindFlag.OneTime | BindFlag.NoInitialTrigger,
                () => view.ShowImage(vm.ImageUri),
                () => Dispatcher.RunOnUiThread(() => view.ShowImage(vm.ImageUri)),

                BindFlag.OneTime,
                (view, nameof(view.Click), vm.OnViewClicked),
                (view, nameof(view.TextEditedEventHandler), () => vm.OnViewTextEdited(view.Text)),
                (view, "CustomEventForGesture", vm.OnViewClicked),

                (view, nameof(view.Click), vm.ViewClickCommand),
                (view, nameof(view.Click), () => vm.ViewClickCommand.TryExecute(view.Text)),

                new UserExtensions.CustomEBinding(),
            };

            binding.Dispose();
            // end-snippet
        }
    }

    internal abstract class View
    {
        public abstract bool Visible { get; set; }
        public abstract float SliderValueFloat { get; set; }
        public abstract long Timestamp { get; set; }
        public abstract string FullName { get; set; }
        public abstract string Text { get; set; }

        public abstract void ShowImage(string uri);

        public abstract event EventHandler TextEditedEventHandler;

        public abstract event EventHandler<TextEditedEventArgs> TextEditedGenericEventHandler;

        public abstract event Action<string> TextEditedCustomEventHandler;

        public abstract event EventHandler Click;

        public abstract void AddGestureRecognizer(GestureRecognizer gestureRecognizer);

        public abstract void RemoveGestureRecognizer(GestureRecognizer gestureRecognizer);

        public class TextEditedEventArgs : EventArgs { }

        public class GestureRecognizer
        {
            public GestureRecognizer(Action action)
            {
            }
        }
    }

    internal abstract class ViewModel : INotifyPropertyChanged
    {
        public abstract DateTime DateTime { get; }
        public abstract bool TextVisible { get; }
        public abstract bool ImageVisible { get; }
        public abstract int AgeInt { get; set; }
        public abstract string FirstName { get; set; }
        public abstract string LastName { get; set; }
        public abstract string Text { get; set; }
        public abstract string FallbackText { get; set; }
        public abstract string ImageUri { get; }
        public abstract IDescriptionViewModel Description { get; }

        public abstract ICommand ViewClickCommand { get; }

        public abstract void OnViewClicked();
        public abstract bool OnViewTextEdited(string text);

        public abstract event PropertyChangedEventHandler? PropertyChanged;

        public interface IDescriptionViewModel : INotifyPropertyChanged
        {
            ITitleViewModel Title { get; }

            public interface ITitleViewModel : INotifyPropertyChanged
            {
                string Text { get; }
            }
        }
    }

    internal static class UserExtensions
    {
        // begin-snippet: Custom-EBinding
        public class CustomEBinding : IEBinding
        {
            public void Dispose()
            {
            }
        }

        public static void Add(this EBinding bindingHolder, CustomEBinding binding,
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                if (binding == null)
                    throw new ArgumentNullException(nameof(binding));

                if (bindingHolder.CurrentFlag == BindFlag.OneTime)
                    throw new NotSupportedException();

                bindingHolder.Add(binding);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error in entry {bindingHolder.Count} at line #{sourceLineNumber}", ex);
            }
        }
        // end-snippet
    }

    internal static class Dispatcher
    {
        public static void RunOnUiThread(Action action)
        {
        }
    }

    internal static class Converter
    {
        public static long DateTimeToEpoch(DateTime dateTime) => default;
    }
}
