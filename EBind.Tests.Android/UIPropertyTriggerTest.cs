using System;
using Android.App;
using Android.Widget;
using EBind.Test.Models;
using Xunit;

namespace EBind.Tests.Droid
{
    public class UIPropertyTriggerTest
    {
        [UIFact]
        public void TextView_Text_TwoWay()
        {
            var vm = new NotifyPropertyChangedEventObject
            {
                StringValue = "Hello",
            };
            var textField = new TextView(Application.Context);

            var binding = new EBinding
            {
                BindFlag.TwoWay,
                () => textField.Text == vm.StringValue,
            };

            Assert.Equal("Hello", textField.Text);

            textField.Text = "Goodbye";

            Assert.Equal("Goodbye", vm.StringValue);
        }

        [UIFact]
        public void CompoundButton_Checked_TwoWay()
        {
            var vm = new NotifyPropertyChangedEventObject
            {
                BoolValue = true,
            };
            var checkBox = new CheckBox(Application.Context);

            var binding = new EBinding
            {
                BindFlag.TwoWay,
                () => checkBox.Checked == vm.BoolValue,
            };

            Assert.True(checkBox.Checked);

            checkBox.Checked = false;

            Assert.False(vm.BoolValue);
        }

        [UIFact]
        public void SeekBar_Progress_TwoWay()
        {
            var vm = new NotifyPropertyChangedEventObject
            {
                IntValue = 1
            };
            var seekBar = new SeekBar(Application.Context);

            var binding = new EBinding
            {
                BindFlag.TwoWay,
                () => seekBar.Progress == vm.IntValue,
            };

            Assert.Equal(1, seekBar.Progress);

            seekBar.Progress = 2;

            Assert.Equal(2, vm.IntValue);
        }

        [UIFact]
        public void SearchView_Query_TwoWay()
        {
            var vm = new NotifyPropertyChangedEventObject
            {
                StringValue = "Hello",
            };
            var searchView = new SearchView(Application.Context);

            var binding = new EBinding
            {
                BindFlag.TwoWay,
                () => searchView.Query == vm.StringValue,
            };

            Assert.Equal("Hello", searchView.Query);

            searchView.SetQuery("Goodbye", submit: true);

            Assert.Equal("Goodbye", vm.StringValue);
        }

        [UIFact]
        public void RatingBar_Rating_TwoWay()
        {
            var vm = new NotifyPropertyChangedEventObject
            {
                FloatValue = 1,
            };
            var ratingBar = new RatingBar(Application.Context)
            {
                NumStars = 5,
            };

            var binding = new EBinding
            {
                BindFlag.TwoWay,
                () => ratingBar.Rating == vm.FloatValue,
            };

            Assert.Equal(1, ratingBar.Rating);

            ratingBar.Rating = 2;

            Assert.Equal(2, vm.FloatValue);
        }

        [UIFact]
        public void NumberPicker_Value_TwoWay()
        {
            var vm = new NotifyPropertyChangedEventObject
            {
                IntValue = 1,
            };
            var numberPicker = new NumberPicker(Application.Context)
            {
                MaxValue = 10,
            };
            numberPicker.Layout(0, 0, 100, 100);

            var binding = new EBinding
            {
                BindFlag.TwoWay,
                () => numberPicker.Value == vm.IntValue,
            };

            Assert.Equal(1, numberPicker.Value);

            numberPicker.ScrollBy(0, -30);

            Assert.Equal(2, vm.IntValue);
        }

        [UIFact]
        public void DatePicker_DateTime_TwoWay()
        {
            var initialDate = DateTime.UtcNow.Date;
            var finalDate = initialDate.AddDays(2);

            var rightDate = initialDate;
            var datePicker = new DatePicker(Application.Context);

            var binding = new EBinding
            {
                BindFlag.TwoWay,
                () => datePicker.DateTime == rightDate,
            };

            Assert.Equal(initialDate, datePicker.DateTime);

            datePicker.DateTime = finalDate;

            Assert.Equal(finalDate, rightDate);
        }

        [UIFact]
        public void TimePicker_Hour_TwoWay()
        {
            var vm = new NotifyPropertyChangedEventObject
            {
                IntValue = 1,
            };

            var timePicker = new TimePicker(Application.Context);

            var binding = new EBinding
            {
                BindFlag.TwoWay,
                () => timePicker.Hour == vm.IntValue,
            };

            Assert.Equal(1, timePicker.Hour);

            timePicker.Hour = 2;

            Assert.Equal(2, vm.IntValue);
        }

        [UIFact]
        public void TimePicker_Minute_TwoWay()
        {
            var vm = new NotifyPropertyChangedEventObject
            {
                IntValue = 1,
            };

            var timePicker = new TimePicker(Application.Context);

            var binding = new EBinding
            {
                BindFlag.TwoWay,
                () => timePicker.Minute == vm.IntValue,
            };

            Assert.Equal(1, timePicker.Minute);

            timePicker.Minute = 2;

            Assert.Equal(2, vm.IntValue);
        }
    }
}