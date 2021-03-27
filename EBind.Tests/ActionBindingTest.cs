using System;
using EBind.Test.Models;
using Xunit;

namespace EBind.Tests
{
    public class ActionBindingTest
    {
        [Fact]
        public void Instance_1_Argument_Void_Return_Should_Work()
        {
            var obj = new NotifyPropertyChangedEventObject
            {
                StringValue = "Hello"
            };
            var counter = new MethodCallCounter();

            new EBinding()
            {
                () => counter.Action(obj.StringValue),
            };

            Assert.Equal(1, counter.Count);

            obj.StringValue = "GoodBye";
            Assert.Equal(2, counter.Count);
        }

        [Fact]
        public void HighOrder_Action_Should_Work()
        {
            var obj = new NotifyPropertyChangedEventObject
            {
                StringValue = "Hello"
            };

            var result = "";
            var setResult = new Func<string, string>(t => result = t);

            new EBinding()
            {
                () => LambdaInvoker.Invoke(() => setResult(obj.StringValue))
            };

            Assert.Equal("Hello", result);

            obj.StringValue = "GoodBye";
            Assert.Equal("GoodBye", result);
        }

        [Fact]
        public void Multiple_Static_Ref_Method_Bindings_Should_Work()
        {
            var parameter = "param";

            _ = new EBinding
            {
                () => StaticMethods.Append0(ref parameter),
                () => StaticMethods.Append0(ref parameter),
                () => StaticMethods.Append0(ref parameter)
            };

            Assert.Equal("param000", parameter);
        }
    }
}
