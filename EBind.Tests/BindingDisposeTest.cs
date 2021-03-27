using System;
using EBind.Test.Models;
using Xunit;

namespace EBind.Tests
{
    public class BindingDisposeTest
    {
        private readonly Configuration _configuration = new();

        public BindingDisposeTest()
        {
            _configuration.ConfigureTrigger<PropertyChangedEventObject, string>(
                x => x.StringValue,
                (o, h) => o.StringValueChanged += h,
                (o, h) => o.StringValueChanged -= h);

            _configuration.ConfigureTrigger<ActionEventObject, Action<string, string>, string>(
                x => x.StringValue,
                t => (_, __) => t.Invoke(),
                (o, h) => o.StringValueChanged += h,
                (o, h) => o.StringValueChanged -= h);

            _configuration.ConfigureTrigger<FuncEventObject, Func<string, bool>, string>(
                x => x.StringValue,
                t => _ =>
                {
                    t.Invoke();
                    return false;
                },
                (o, h) => o.StringValueChanged += h,
                (o, h) => o.StringValueChanged -= h);
        }

        [Fact]
        public void Action_Handler()
        {
            var obj = new ActionEventObject
            {
                StringValue = "Hello",
            };
            var left = "";

            var b = new EBinding(_configuration)
            {
                () => left == obj.StringValue
            };

            Assert.Equal(1, obj.StringValueChangedCount);

            Assert.Equal(obj.StringValue, left);

            obj.StringValue = "Goodbye";

            Assert.Equal("Goodbye", left);

            b.Dispose();

            Assert.Equal(0, obj.StringValueChangedCount);

            obj.StringValue = "Hello Again";

            Assert.Equal("Goodbye", left);
        }

        [Fact]
        public void EventHandler()
        {
            var obj = new PropertyChangedEventObject
            {
                StringValue = "Hello",
            };
            var left = "";

            var b = new EBinding(_configuration)
            {
                () => left == obj.StringValue
            };

            Assert.Equal(1, obj.StringValueChangedCount);

            Assert.Equal(obj.StringValue, left);

            obj.StringValue = "Goodbye";

            Assert.Equal(obj.StringValue, left);

            b.Dispose();

            Assert.Equal(0, obj.StringValueChangedCount);

            obj.StringValue = "Hello Again";

            Assert.Equal("Goodbye", left);
        }

        [Fact]
        public void Func_Handler()
        {
            var obj = new FuncEventObject
            {
                StringValue = "Hello",
            };
            var left = "";

            var b = new EBinding(_configuration)
            {
                () => left == obj.StringValue
            };

            Assert.Equal(1, obj.StringValueChangedCount);

            Assert.Equal(obj.StringValue, left);

            obj.StringValue = "Goodbye";

            Assert.Equal("Goodbye", left);

            b.Dispose();

            Assert.Equal(0, obj.StringValueChangedCount);

            obj.StringValue = "Hello Again";

            Assert.Equal("Goodbye", left);
        }

        [Fact]
        public void Multiple_Handlers()
        {
            var obj = new PropertyChangedEventObject
            {
                StringValue = "Hello",
            };
            var leftA = "";
            var leftB = "";

            var bA = new EBinding(_configuration)
            {
                () => leftA == obj.StringValue
            };

            var bB = new EBinding(_configuration)
            {
                () => leftB == obj.StringValue + "..."
            };

            Assert.Equal(2, obj.StringValueChangedCount);

            Assert.Equal("Hello", leftA);
            Assert.Equal("Hello...", leftB);

            obj.StringValue = "Goodbye";

            Assert.Equal("Goodbye", leftA);
            Assert.Equal("Goodbye...", leftB);

            bA.Dispose();

            Assert.Equal(1, obj.StringValueChangedCount);

            obj.StringValue = "Hello Again";

            Assert.Equal("Goodbye", leftA);
            Assert.Equal("Hello Again...", leftB);

            bB.Dispose();

            Assert.Equal(0, obj.StringValueChangedCount);

            obj.StringValue = "Goodbye Again";

            Assert.Equal("Goodbye", leftA);
            Assert.Equal("Hello Again...", leftB);
        }

        [Fact]
        public void NotifyPropertyChanged_Handler()
        {
            var obj = new NotifyPropertyChangedEventObject
            {
                StringValue = "Hello",
            };
            var left = "";

            var b = new EBinding
            {
                () => left == obj.StringValue
            };

            Assert.Equal(1, obj.PropertyChangedCount);

            Assert.Equal(obj.StringValue, left);

            obj.StringValue = "Goodbye";

            Assert.Equal("Goodbye", left);

            b.Dispose();

            Assert.Equal(0, obj.PropertyChangedCount);

            obj.StringValue = "Hello Again";

            Assert.Equal("Goodbye", left);
        }
    }
}
