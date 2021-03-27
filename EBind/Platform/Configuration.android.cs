using System;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using static Android.Views.View;
using static Android.Widget.AdapterView;
using static Android.Widget.CalendarView;
using static Android.Widget.CompoundButton;
using static Android.Widget.DatePicker;
using static Android.Widget.NumberPicker;
using static Android.Widget.RatingBar;
using static Android.Widget.SearchView;
using static Android.Widget.SeekBar;
using static Android.Widget.TimePicker;

namespace EBind.Platform
{
    /// <summary>
    /// Android specific configuration for initialization and execution of bindings.
    /// Provides triggers, dispatchers, and the default settings.
    /// Has some pre-configured triggers – see the README file for details.
    /// </summary>
    public class Configuration : EBind.Configuration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        public Configuration()
        {
            ConfigureProperties();

            ConfigureEvents();
        }

        private void ConfigureEvents()
        {
            ConfigureTrigger<View>(
                nameof(View.Click),
                (o, h) => o.Click += h,
                (o, h) => o.Click -= h);

            ConfigureTrigger<View, LongClickEventArgs>(
                nameof(View.LongClick),
                (o, h) => o.LongClick += h,
                (o, h) => o.LongClick -= h);

            ConfigureTrigger<View, FocusChangeEventArgs>(
                nameof(View.FocusChange),
                (o, h) => o.FocusChange += h,
                (o, h) => o.FocusChange -= h);

            // EditText
            ConfigureTrigger<TextView, TextChangedEventArgs>(
                nameof(TextView.TextChanged),
                (o, h) => o.TextChanged += h,
                (o, h) => o.TextChanged -= h);

            // CheckBox
            ConfigureTrigger<CompoundButton, CheckedChangeEventArgs>(
                nameof(CompoundButton.CheckedChange),
                (o, h) => o.CheckedChange += h,
                (o, h) => o.CheckedChange -= h);

            ConfigureTrigger<SeekBar, ProgressChangedEventArgs>(
                nameof(SeekBar.ProgressChanged),
                (o, h) => o.ProgressChanged += h,
                (o, h) => o.ProgressChanged -= h);

            ConfigureTrigger<SearchView, QueryTextChangeEventArgs>(
                nameof(SearchView.QueryTextChange),
                (o, h) => o.QueryTextChange += h,
                (o, h) => o.QueryTextChange -= h);

            ConfigureTrigger<RatingBar, RatingBarChangeEventArgs>(
                nameof(RatingBar.RatingBarChange),
                (o, h) => o.RatingBarChange += h,
                (o, h) => o.RatingBarChange -= h);

            // Spinner
            ConfigureTrigger<AdapterView, ItemSelectedEventArgs>(
                nameof(AdapterView.ItemSelected),
                (o, h) => o.ItemSelected += h,
                (o, h) => o.ItemSelected -= h);

            ConfigureTrigger<AdapterView, ItemClickEventArgs>(
                nameof(AdapterView.ItemClick),
                (o, h) => o.ItemClick += h,
                (o, h) => o.ItemClick -= h);

            ConfigureTrigger<AdapterView, ItemLongClickEventArgs>(
                nameof(AdapterView.ItemLongClick),
                (o, h) => o.ItemLongClick += h,
                (o, h) => o.ItemLongClick -= h);

            ConfigureTrigger<NumberPicker, ValueChangeEventArgs>(
                nameof(NumberPicker.ValueChanged),
                (o, h) => o.ValueChanged += h,
                (o, h) => o.ValueChanged -= h);

            ConfigureTrigger<DatePicker, DateChangedEventArgs>(
                nameof(DatePicker.DateChanged),
                (o, h) => o.DateChanged += h,
                (o, h) => o.DateChanged -= h);

            ConfigureTrigger<TimePicker, TimeChangedEventArgs>(
                nameof(TimePicker.TimeChanged),
                (o, h) => o.TimeChanged += h,
                (o, h) => o.TimeChanged -= h);

            ConfigureTrigger<CalendarView, DateChangeEventArgs>(
                nameof(CalendarView.DateChange),
                (o, h) => o.DateChange += h,
                (o, h) => o.DateChange -= h);
        }

        private void ConfigureProperties()
        {
            // EditText
            ConfigureTrigger<TextView, TextChangedEventArgs, string?>(
                o => o.Text,
                (o, h) => o.TextChanged += h,
                (o, h) => o.TextChanged -= h);

            // CheckBox
            ConfigureTrigger<CompoundButton, CheckedChangeEventArgs, bool>(
                o => o.Checked,
                (o, h) => o.CheckedChange += h,
                (o, h) => o.CheckedChange -= h);

            ConfigureTrigger<SeekBar, ProgressChangedEventArgs, int>(
                o => o.Progress,
                (o, h) => o.ProgressChanged += h,
                (o, h) => o.ProgressChanged -= h);

            ConfigureTrigger<SearchView, QueryTextChangeEventArgs, string?>(
                o => o.Query,
                (o, h) => o.QueryTextChange += h,
                (o, h) => o.QueryTextChange -= h,
                setter: (o, v) => o.SetQuery(v, submit: false));

            ConfigureTrigger<RatingBar, RatingBarChangeEventArgs, float>(
                o => o.Rating,
                (o, h) => o.RatingBarChange += h,
                (o, h) => o.RatingBarChange -= h);

            // Spinner
            ConfigureTrigger<AdapterView, ItemSelectedEventArgs, int>(
                o => o.SelectedItemPosition,
                (o, h) => o.ItemSelected += h,
                (o, h) => o.ItemSelected -= h,
                setter: (o, v) => o.SetSelection(v));

            ConfigureTrigger<NumberPicker, ValueChangeEventArgs, int>(
                o => o.Value,
                (o, h) => o.ValueChanged += h,
                (o, h) => o.ValueChanged -= h);

            ConfigureTrigger<DatePicker, DateChangedEventArgs, DateTime>(
                o => o.DateTime,
                (o, h) =>
                {
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    {
                        o.DateChanged += h;
                    }
                    else
                    {
                        o.Init(o.Year, o.Month, o.DayOfMonth, new OnDateChangedListener(h));
                    }
                },
                (o, h) =>
                {
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    {
                        o.DateChanged += h;
                    }
                    else
                    {
                        o.Init(o.Year, o.Month, o.DayOfMonth, null);
                    }
                });

            ConfigureTrigger<TimePicker, TimeChangedEventArgs, int>(
                o => o.Hour,
                (o, h) => o.TimeChanged += h,
                (o, h) => o.TimeChanged -= h);

            ConfigureTrigger<TimePicker, TimeChangedEventArgs, int>(
                o => o.Minute,
                (o, h) => o.TimeChanged += h,
                (o, h) => o.TimeChanged -= h);

#pragma warning disable CS0618 // Type or member is obsolete
            ConfigureTrigger<TimePicker, TimeChangedEventArgs, Java.Lang.Integer?>(
                o => o.CurrentHour,
                (o, h) => o.TimeChanged += h,
                (o, h) => o.TimeChanged -= h);

            ConfigureTrigger<TimePicker, TimeChangedEventArgs, Java.Lang.Integer?>(
                o => o.CurrentMinute,
                (o, h) => o.TimeChanged += h,
                (o, h) => o.TimeChanged -= h);
#pragma warning restore CS0618 // Type or member is obsolete

            ConfigureTrigger<CalendarView, DateChangeEventArgs, long>(
                o => o.Date,
                (o, h) => o.DateChange += h,
                (o, h) => o.DateChange -= h);
        }

        private class OnDateChangedListener : Java.Lang.Object, IOnDateChangedListener
        {
            private readonly EventHandler<DateChangedEventArgs> _handler;

            public OnDateChangedListener(EventHandler<DateChangedEventArgs> handler)
            {
                _handler = handler;
            }

            public void OnDateChanged(DatePicker? view, int year, int monthOfYear, int dayOfMonth)
            {
                _handler.Invoke(view, new DateChangedEventArgs(year, monthOfYear, dayOfMonth));
            }
        }
    }
}
