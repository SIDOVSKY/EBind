using EBind.Test.Models;
using EBind.Tests.Models;
using Xunit;

namespace EBind.Tests
{
    public class EventBindingTest
    {
        [Fact]
        public void Configured_EventBinding_Should_Work()
        {
            var config = new Configuration();
            config.ConfigureTrigger<PropertyChangedEventObject>(
                nameof(PropertyChangedEventObject.StringValueChanged),
                (o, h) => o.StringValueChanged += h,
                (o, h) => o.StringValueChanged -= h);

            var obj = new PropertyChangedEventObject();
            var result = false;

            new EBinding(config)
            {
                (obj, nameof(obj.StringValueChanged), () => result = true)
            };

            Assert.False(result);

            obj.StringValue = "new value";
            Assert.True(result);
        }

        [Fact]
        public void EventBinding_StopWorking_OnDispose()
        {
            var config = new Configuration();
            config.ConfigureTrigger<PropertyChangedEventObject>(
                nameof(PropertyChangedEventObject.StringValueChanged),
                (o, h) => o.StringValueChanged += h,
                (o, h) => o.StringValueChanged -= h);

            var obj = new PropertyChangedEventObject();
            var result = false;

            var b = new EBinding(config)
            {
                (obj, nameof(obj.StringValueChanged), () => result = true)
            };

            Assert.False(result);

            obj.StringValue = "new value";
            Assert.True(result);

            b.Dispose();
            result = false;
            obj.StringValue = "next value";
            Assert.False(result);
        }

        [Fact]
        public void Unconfigured_EventBinding_Should_Work_WithReflection()
        {
            var obj = new PropertyChangedEventObject();
            var result = false;

            new EBinding
            {
                (obj, nameof(obj.StringValueChanged), () => result = true)
            };

            Assert.False(result);

            obj.StringValue = "new value";
            Assert.True(result);
        }

        [Fact]
        public void Command_In_EventBinding_Should_Work()
        {
            var config = new Configuration();
            config.ConfigureTrigger<PropertyChangedEventObject>(
                nameof(PropertyChangedEventObject.StringValueChanged),
                (o, h) => o.StringValueChanged += h,
                (o, h) => o.StringValueChanged -= h);

            var obj = new PropertyChangedEventObject();
            var command = new ExecuteCountCommand();

            new EBinding(config)
            {
                (obj, nameof(obj.StringValueChanged), command)
            };

            obj.StringValue = "1";
            obj.StringValue = "2";

            Assert.Equal(2, command.InvokeCount);
        }
    }
}
