using System;
using EBind.Test.Models;
using Foundation;
using UIKit;
using Xunit;

namespace EBind.Tests.iOS
{
    public class UIPropertyTriggerTest
    {
        [UIFact]
        public void UITextField_Text_TwoWay()
        {
            var vm = new NotifyPropertyChangedEventObject
            {
                StringValue = "Hello"
            };
            var textField = new UITextField();

            var binding = new EBinding
            {
                BindFlag.TwoWay,
                () => textField.Text == vm.StringValue,
            };

            Assert.Equal("Hello", textField.Text);

            textField.Text = string.Empty;
            textField.InsertText("Goodbye");

            Assert.Equal("Goodbye", vm.StringValue);
        }

        [UIFact]
        public void UITextView_Text_TwoWay()
        {
            var vm = new NotifyPropertyChangedEventObject
            {
                StringValue = "Hello"
            };
            var textView = new UITextView();

            var binding = new EBinding
            {
                BindFlag.TwoWay,
                () => textView.Text == vm.StringValue,
            };

            Assert.Equal("Hello", textView.Text);

            textView.Text = string.Empty;
            textView.InsertText("Goodbye");

            Assert.Equal("Goodbye", vm.StringValue);
        }

        [UIFact]
        public void UISearchBar_Text_TwoWay()
        {
            var vm = new NotifyPropertyChangedEventObject
            {
                StringValue = "Hello"
            };
            var searchBar = new UISearchBar();

            var binding = new EBinding
            {
                BindFlag.TwoWay,
                () => searchBar.Text == vm.StringValue,
            };

            Assert.Equal("Hello", searchBar.Text);

            searchBar.Text = "Goodbye";
            searchBar.Delegate.TextChanged(searchBar, searchBar.Text); // searchBar.SearchTextField is from iOS 13
            Assert.Equal("Goodbye", vm.StringValue);
        }

        [UIFact]
        public void UISlider_Value_TwoWay()
        {
            var vm = new NotifyPropertyChangedEventObject
            {
                FloatValue = 1f
            };
            var slider = new UISlider
            {
                MaxValue = 10f
            };

            var binding = new EBinding
            {
                BindFlag.TwoWay,
                () => slider.Value == vm.FloatValue,
            };

            Assert.Equal(1f, slider.Value);

            slider.SetValue(2f, animated: false);
            slider.SendActionForControlEvents(UIControlEvent.ValueChanged);

            Assert.Equal(2f, vm.FloatValue);
        }

        [UIFact]
        public void UIStepper_Value_TwoWay()
        {
            var vm = new NotifyPropertyChangedEventObject
            {
                FloatValue = 1f
            };
            var stepper = new UIStepper();

            var binding = new EBinding
            {
                BindFlag.TwoWay,
                () => stepper.Value == vm.FloatValue,
            };

            Assert.Equal(1f, stepper.Value);

            stepper.Value = 2;
            stepper.SendActionForControlEvents(UIControlEvent.ValueChanged);

            Assert.Equal(2f, vm.FloatValue);
        }

        [UIFact]
        public void UISwitch_On_TwoWay()
        {
            var vm = new NotifyPropertyChangedEventObject
            {
                BoolValue = true
            };
            var switchView = new UISwitch();

            var binding = new EBinding
            {
                BindFlag.TwoWay,
                () => switchView.On == vm.BoolValue,
            };

            Assert.True(switchView.On);

            switchView.SetState(false, animated: false);
            switchView.SendActionForControlEvents(UIControlEvent.ValueChanged);

            Assert.False(vm.BoolValue);
        }

        [UIFact]
        public void UIDatePicker_Date()
        {
            var initialDate = new DateTime(2001, 2, 3, 0, 0, 0, DateTimeKind.Utc);
            var finalDate = new DateTime(2002, 3, 4, 0, 0, 0, DateTimeKind.Utc);
            var targetDate = NSDate.Now;
            var datePicker = new UIDatePicker
            {
                Date = (NSDate)initialDate
            };

            var binding = new EBinding
            {
                () => targetDate == datePicker.Date,
            };

            Assert.Equal(initialDate, (DateTime)targetDate);

            datePicker.SetDate((NSDate)finalDate, animated: false);
            datePicker.SendActionForControlEvents(UIControlEvent.ValueChanged);

            Assert.Equal(finalDate, (DateTime)targetDate);
        }

        [UIFact]
        public void UISegmentedControl_SelectedSegment_TwoWay()
        {
            var vm = new NotifyPropertyChangedEventObject
            {
                IntValue = 1
            };
            var segmentedControl = new UISegmentedControl();
            segmentedControl.InsertSegment("0", 0, animated: false);
            segmentedControl.InsertSegment("1", 1, animated: false);
            segmentedControl.InsertSegment("2", 2, animated: false);

            var binding = new EBinding
            {
                BindFlag.TwoWay,
                () => segmentedControl.SelectedSegment == vm.IntValue,
            };

            Assert.Equal(1, segmentedControl.SelectedSegment);

            segmentedControl.SelectedSegment = 2;
            segmentedControl.SendActionForControlEvents(UIControlEvent.ValueChanged);

            Assert.Equal(2, vm.IntValue);
        }

        [UIFact]
        public void UIPageControl_CurrentPage_TwoWay()
        {
            var vm = new NotifyPropertyChangedEventObject
            {
                IntValue = 1
            };
            var pageControl = new UIPageControl
            {
                Pages = 3
            };

            var binding = new EBinding
            {
                BindFlag.TwoWay,
                () => pageControl.CurrentPage == vm.IntValue,
            };

            Assert.Equal(1, pageControl.CurrentPage);

            pageControl.CurrentPage = 2;
            pageControl.SendActionForControlEvents(UIControlEvent.ValueChanged);

            Assert.Equal(2, vm.IntValue);
        }
    }
}
