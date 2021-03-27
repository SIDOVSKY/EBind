using System;
using EBind.Test.Models;
using Xunit;

namespace EBind.Tests
{
    public class ChangeTrackingTest
    {
        private readonly Configuration _configuration = new();

        public ChangeTrackingTest()
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
        public void Multiple_Trigger_Properties()
        {
            var objA = new PropertyChangedEventObject
            {
                StringValue = "Hello",
            };
            var objB = new PropertyChangedEventObject
            {
                StringValue = "World",
            };
            var left = "";

            _ = new EBinding(_configuration)
            {
                () => left == objA.StringValue + ", " + objB.StringValue
            };

            Assert.Equal("Hello, World", left);

            objA.StringValue = "Goodbye";

            Assert.Equal("Goodbye, World", left);

            objB.StringValue = "Mars";

            Assert.Equal("Goodbye, Mars", left);
        }

        [Fact]
        public void Multiple_For_Same_Trigger()
        {
            var obj = new PropertyChangedEventObject
            {
                StringValue = "Hello",
            };
            var leftA = "";
            var leftB = "";

            _ = new EBinding(_configuration)
            {
                () => leftA == obj.StringValue,
                () => leftB == obj.StringValue + "..."
            };

            Assert.Equal("Hello", leftA);
            Assert.Equal("Hello...", leftB);

            obj.StringValue = "Goodbye";

            Assert.Equal("Goodbye", leftA);
            Assert.Equal("Goodbye...", leftB);
        }

        [Fact]
        public void Prop_To_Local_NotifyPropertyChanged()
        {
            var obj = new NotifyPropertyChangedEventObject
            {
                StringValue = "Hello",
            };
            var left = "";

            _ = new EBinding
            {
                () => left == obj.StringValue
            };

            Assert.Equal(1, obj.PropertyChangedCount);

            Assert.Equal(obj.StringValue, left);

            obj.StringValue = "Goodbye";

            Assert.Equal("Goodbye", left);
        }

        [Fact]
        public void Prop_To_Local_EventHandler()
        {
            var obj = new PropertyChangedEventObject
            {
                StringValue = "Hello",
            };
            var left = "";

            _ = new EBinding(_configuration)
            {
                () => left == obj.StringValue
            };

            Assert.Equal(1, obj.StringValueChangedCount);

            Assert.Equal(obj.StringValue, left);

            obj.StringValue = "Goodbye";

            Assert.Equal(obj.StringValue, left);
        }

        [Fact]
        public void Instance_Prop_To_Static_Prop_EventHandler()
        {
            var obj = new PropertyChangedEventObject
            {
                StringValue = "Hello",
            };

            _ = new EBinding(_configuration)
            {
                () => StaticProperties.StringEmpty == obj.StringValue
            };

            Assert.Equal(1, obj.StringValueChangedCount);

            Assert.Equal("Hello", StaticProperties.StringEmpty);

            obj.StringValue = "Goodbye";

            Assert.Equal("Goodbye", StaticProperties.StringEmpty);
        }
    }
}
