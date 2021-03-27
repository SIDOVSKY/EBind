using System;
using Foundation;
using UIKit;
using static EBind.Platform.Configuration.ExtraEventNames;

namespace EBind.Platform
{
    /// <summary>
    /// iOS specific configuration for initialization and execution of bindings.
    /// Provides triggers, dispatchers, and the default settings.
    /// Has some pre-configured triggers – see the README file for details.
    /// </summary>
    public class Configuration : EBind.Configuration
    {
        /// <summary>
        /// Collection of identifiers for custom pre-registered events.
        /// </summary>
        /// <remarks>
        /// Supposed to be imported with a `using static` directive.
        /// </remarks>
        public static class ExtraEventNames
        {
            private const string Prefix = "EBind.Platform.Configuration.ExtraEventNames";

            /// <summary>
            /// Identifier of the pre-configured event trigger for <see cref="UIControl.TouchUpInside"/>
            /// and <see cref="UIView.AddGestureRecognizer(UIGestureRecognizer)"/> &lt;- <see cref="UITapGestureRecognizer"/>.
            /// </summary>
            /// <remarks>
            /// Usage:
            /// <code language="c#">
            /// using static EBind.Platform.Configuration.ExtraEventNames;<br/>
            /// ...<br/>
            /// var binding = new EBinding <br/>
            /// {<br/>
            /// ⠀⠀(uiButton, Tap, OnButtonClick),<br/>
            /// ⠀⠀(uiImageView, Tap, OnImageClick),<br/>
            /// };
            /// </code>
            /// </remarks>
            // U+2800 BRAILLE PATTERN BLANK ('⠀') is used instead of indent spaces in the code sample block
            // because whitespaces are trimmed off.
            public const string Tap = Prefix + nameof(Tap);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        public Configuration()
        {
            ConfigureProperties();
            ConfigureExtraEvents();
            ConfigureEvents();
        }

        private void ConfigureExtraEvents()
        {
            ConfigureTrigger<UIControl>(
                Tap,
                (o, h) => o.TouchUpInside += h,
                (o, h) => o.TouchUpInside -= h);

            ConfigureTrigger<UIView, UITapGestureRecognizer>(
                Tap,
                trigger => new UITapGestureRecognizer(trigger),
                (o, h) => o.AddGestureRecognizer(h),
                (o, h) => o.RemoveGestureRecognizer(h));
        }

        private void ConfigureEvents()
        {
            ConfigureTrigger<UIControl>(
                nameof(UIControl.TouchUpInside),
                (o, h) => o.TouchUpInside += h,
                (o, h) => o.TouchUpInside -= h);

            ConfigureTrigger<UIBarButtonItem>(
                nameof(UIBarButtonItem.Clicked),
                (o, h) => o.Clicked += h,
                (o, h) => o.Clicked -= h);

            ConfigureTrigger<UITextField>(
                nameof(UITextField.EditingDidBegin),
                (o, h) => o.EditingDidBegin += h,
                (o, h) => o.EditingDidBegin -= h);

            ConfigureTrigger<UITextField>(
                nameof(UITextField.EditingChanged),
                (o, h) => o.EditingChanged += h,
                (o, h) => o.EditingChanged -= h);

            ConfigureTrigger<UITextField>(
                nameof(UITextField.EditingDidEnd),
                (o, h) => o.EditingDidEnd += h,
                (o, h) => o.EditingDidEnd -= h);

            ConfigureTrigger<UITextView>(
                nameof(UITextView.Changed),
                (o, h) => o.Changed += h,
                (o, h) => o.Changed -= h);

            ConfigureTrigger<UISearchBar, UISearchBarTextChangedEventArgs>(
                 nameof(UISearchBar.TextChanged),
                 (o, h) => o.TextChanged += h,
                 (o, h) => o.TextChanged -= h);

            ConfigureTrigger<UIControl>(
                nameof(UIControl.ValueChanged),
                (o, h) => o.ValueChanged += h,
                (o, h) => o.ValueChanged -= h);

            ConfigureTrigger<UITabBarController, UITabBarSelectionEventArgs>(
                nameof(UITabBarController.ViewControllerSelected),
                (o, h) => o.ViewControllerSelected += h,
                (o, h) => o.ViewControllerSelected -= h);
        }

        private void ConfigureProperties()
        {
            ConfigureTrigger<UITextField, string?>(
                o => o.Text,
                (o, h) => o.EditingChanged += h,
                (o, h) => o.EditingChanged -= h);

            ConfigureTrigger<UITextView, string?>(
                o => o.Text,
                (o, h) => o.Changed += h,
                (o, h) => o.Changed -= h);

            ConfigureTrigger<UISearchBar, UISearchBarTextChangedEventArgs, string?>(
                o => o.Text,
                (o, h) => o.TextChanged += h,
                (o, h) => o.TextChanged -= h);

            ConfigureTrigger<UISlider, float>(
                o => o.Value,
                (o, h) => o.ValueChanged += h,
                (o, h) => o.ValueChanged -= h);

            ConfigureTrigger<UIStepper, double>(
                o => o.Value,
                (o, h) => o.ValueChanged += h,
                (o, h) => o.ValueChanged -= h);

            ConfigureTrigger<UISwitch, bool>(
                o => o.On,
                (o, h) => o.ValueChanged += h,
                (o, h) => o.ValueChanged -= h);

            ConfigureTrigger<UIDatePicker, NSDate>(
                o => o.Date,
                (o, h) => o.ValueChanged += h,
                (o, h) => o.ValueChanged -= h);

            ConfigureTrigger<UISegmentedControl, nint>(
                o => o.SelectedSegment,
                (o, h) => o.ValueChanged += h,
                (o, h) => o.ValueChanged -= h);

            ConfigureTrigger<UITabBarController, UITabBarSelectionEventArgs, nint>(
                o => o.SelectedIndex,
                (o, h) => o.ViewControllerSelected += h,
                (o, h) => o.ViewControllerSelected -= h);

            ConfigureTrigger<UIPageControl, nint>(
                o => o.CurrentPage,
                (o, h) => o.ValueChanged += h,
                (o, h) => o.ValueChanged -= h);
        }
    }
}
