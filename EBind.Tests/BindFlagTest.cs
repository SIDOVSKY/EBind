using EBind.Test.Models;
using Xunit;

namespace EBind.Tests
{
    public class BindFlagTest
    {
        [Fact]
        public void EqualityBinding_TwoWay()
        {
            var left = new NotifyPropertyChangedEventObject();
            var right = new NotifyPropertyChangedEventObject();

            _ = new EBinding
            {
                BindFlag.TwoWay,
                () => left.IntValue == right.IntValue
            };

            right.IntValue = 1;
            Assert.Equal(1, left.IntValue);

            left.IntValue = 2;
            Assert.Equal(2, right.IntValue);
        }

        [Fact]
        public void EqualityBinding_OneTime()
        {
            var left = new NotifyPropertyChangedEventObject();
            var right = new NotifyPropertyChangedEventObject()
            {
                IntValue = 1
            };

            _ = new EBinding
            {
                BindFlag.OneTime,
                () => left.IntValue == right.IntValue
            };

            right.IntValue = 2;
            right.IntValue = 3;

            Assert.Equal(1, left.IntValue);
        }

        [Fact]
        public void EqualityBinding_OneTime_NoInitialTrigger()
        {
            var left = new NotifyPropertyChangedEventObject();
            var right = new NotifyPropertyChangedEventObject()
            {
                IntValue = 1
            };

            _ = new EBinding
            {
                BindFlag.OneTime | BindFlag.NoInitialTrigger,
                () => left.IntValue == right.IntValue
            };

            right.IntValue = 2;
            right.IntValue = 3;

            Assert.Equal(2, left.IntValue);
        }

        [Fact]
        public void EqualityBinding_OneTime_NoInitialTrigger_TwoWay()
        {
            var left = new NotifyPropertyChangedEventObject();
            var right = new NotifyPropertyChangedEventObject()
            {
                IntValue = 1
            };

            _ = new EBinding
            {
                BindFlag.TwoWay | BindFlag.OneTime | BindFlag.NoInitialTrigger,
                () => left.IntValue == right.IntValue
            };

            left.IntValue = 2;
            left.IntValue = 3;

            Assert.Equal(2, right.IntValue);
        }

        [Fact]
        public void EqualityBinding_NoInitialTrigger()
        {
            var left = new NotifyPropertyChangedEventObject();
            var right = new NotifyPropertyChangedEventObject()
            {
                IntValue = 1
            };

            _ = new EBinding
            {
                BindFlag.NoInitialTrigger,
                () => left.IntValue == right.IntValue
            };

            Assert.Equal(0, left.IntValue);

            right.IntValue = 2;
            Assert.Equal(2, left.IntValue);

            right.IntValue = 3;
            Assert.Equal(3, left.IntValue);
        }

        [Fact]
        public void EqualityBinding_NoInitialTrigger_TwoWay()
        {
            var left = new NotifyPropertyChangedEventObject();
            var right = new NotifyPropertyChangedEventObject()
            {
                IntValue = 1
            };

            _ = new EBinding
            {
                BindFlag.NoInitialTrigger,
                () => left.IntValue == right.IntValue
            };

            Assert.Equal(0, left.IntValue);

            right.IntValue = 2;
            Assert.Equal(2, left.IntValue);

            right.IntValue = 3;
            Assert.Equal(3, left.IntValue);
        }

        [Fact]
        public void ActionBinding_NoInitialTrigger()
        {
            var obj = new NotifyPropertyChangedEventObject();
            var callee = new MethodCallCounter();

            _ = new EBinding
            {
                BindFlag.NoInitialTrigger,
                () => callee.Action(obj.IntValue)
            };

            Assert.Equal(0, callee.Count);

            obj.IntValue = 1;
            Assert.Equal(1, callee.Count);

            obj.IntValue = 2;
            Assert.Equal(2, callee.Count);
        }

        [Fact]
        public void ActionBinding_OneTime()
        {
            var obj = new NotifyPropertyChangedEventObject();
            var callee = new MethodCallCounter();

            _ = new EBinding
            {
                BindFlag.OneTime,
                () => callee.Action(obj.IntValue)
            };

            Assert.Equal(1, callee.Count);

            obj.IntValue = 1;
            obj.IntValue = 2;
            Assert.Equal(1, callee.Count);
        }

        [Fact]
        public void ActionBinding_OneTime_NoInitialTrigger()
        {
            var obj = new NotifyPropertyChangedEventObject();
            var callee = new MethodCallCounter();

            _ = new EBinding
            {
                BindFlag.OneTime | BindFlag.NoInitialTrigger,
                () => callee.Action(obj.IntValue)
            };

            Assert.Equal(0, callee.Count);

            obj.IntValue = 1;
            obj.IntValue = 2;
            Assert.Equal(1, callee.Count);
        }

        [Fact]
        public void EventBinding_OneTime()
        {
            var obj = new NotifyPropertyChangedEventObject();
            var callee = new MethodCallCounter();

            _ = new EBinding
            {
                BindFlag.OneTime,
                (obj, nameof(obj.PropertyChanged), callee.Action)
            };

            Assert.Equal(0, callee.Count);

            obj.IntValue = 1;
            obj.IntValue = 2;
            Assert.Equal(1, callee.Count);
        }
    }
}
