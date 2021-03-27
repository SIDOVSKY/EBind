using System;
using EBind.Test.Models;
using Xunit;

namespace EBind.Tests
{
    public class EqualityBindingTest
    {
        [Fact]
        public void Const_To_Const_Is_Not_Supported()
        {
            const string left = "left";
            const string right = "right";

            _ = Assert.Throws<Exception>(() => new EBinding
            {
                () => left == right
            });
        }

        [Fact]
        public void Method_To_Const_Is_Not_Supported()
        {
            const int left = 42;

            _ = Assert.Throws<Exception>(() => new EBinding
            {
                () => left == StaticMethods.Return33()
            });
        }

        [Fact]
        public void Static_Method_To_Local_Should_Work()
        {
            int left = 0;

            _ = new EBinding
            {
                () => left == StaticMethods.Return33()
            };

            Assert.Equal(33, left);
        }

        [Fact]
        public void Prop_To_Local_Should_Work()
        {
            int left = 69;
            var right = new PlainObject
            {
                IntProperty = 42,
            };

            _ = new EBinding
            {
                () => left == right.IntProperty
            };

            Assert.Equal(left, right.IntProperty);
            Assert.Equal(42, left);
        }

        [Fact]
        public void Static_Prop_To_Local_Should_Work()
        {
            int left = 69;

            _ = new EBinding
            {
                () => left == StaticProperties.Int33
            };

            Assert.Equal(StaticProperties.Int33, left);
            Assert.Equal(33, left);
        }

        [Fact]
        public void Method_To_Local_Should_Work()
        {
            int left = 42;

            _ = new EBinding
            {
                () => left == StaticMethods.Return33()
            };

            Assert.Equal(33, left);
        }

        [Fact]
        public void Object_To_Null_Should_Work()
        {
            PlainObject? left = null;
            var right = new PlainObject();

            _ = new EBinding
            {
                () => left == right
            };

            Assert.Same(left, right);
            Assert.NotNull(left);
        }

        [Fact]
        public void Null_To_Object_Should_Work()
        {
            var left = new PlainObject();
            PlainObject? right = null;

            _ = new EBinding
            {
                () => left == right
            };

            Assert.Same(left, right);
            Assert.Null(left);
        }

        [Fact]
        public void Const_To_Property_Should_Work()
        {
            var left = new PlainObject
            {
                IntProperty = 42,
            };
            const int right = 1001;

            _ = new EBinding
            {
                () => left.IntProperty == right
            };

            Assert.Equal(left.IntProperty, right);
            Assert.Equal(1001, left.IntProperty);
        }

        [Fact]
        public void NullCoalescing_Should_Work()
        {
            var left = "";
            var first = new NotifyPropertyChangedEventObject
            {
                StringValue = "first",
            };
            var second = new NotifyPropertyChangedEventObject
            {
                StringValue = "second",
            };

            _ = new EBinding
            {
                () => left == (first.StringValue ?? second.StringValue)
            };

            Assert.Equal("first", left);

            first.StringValue = null;

            Assert.Equal("second", left);

            second.StringValue = "newSecond";

            Assert.Equal("newSecond", left);
        }

        [Fact]
        public void Conditional_And_Should_Work()
        {
            var left = false;
            var first = new NotifyPropertyChangedEventObject
            {
                BoolValue = true,
            };
            var second = new NotifyPropertyChangedEventObject
            {
                BoolValue = true,
            };

            _ = new EBinding
            {
                () => left == (first.BoolValue && second.BoolValue)
            };

            Assert.True(left);

            second.BoolValue = false;

            Assert.False(left);
        }

        [Fact]
        public void Conditional_Or_Should_Work()
        {
            var left = false;
            var first = new NotifyPropertyChangedEventObject
            {
                BoolValue = true,
            };
            var second = new NotifyPropertyChangedEventObject
            {
                BoolValue = true,
            };

            _ = new EBinding
            {
                () => left == (first.BoolValue || second.BoolValue)
            };

            Assert.True(left);

            first.BoolValue = false;
            second.BoolValue = false;

            Assert.False(left);
        }

        [Fact]
        public void String_Concatenation_Via_Plus_Should_Work()
        {
            var left = string.Empty;
            var first = new NotifyPropertyChangedEventObject
            {
                StringValue = "Hello",
            };
            var second = new NotifyPropertyChangedEventObject
            {
                StringValue = "World",
            };

            _ = new EBinding
            {
                () => left == first.StringValue + second.StringValue
            };

            Assert.Equal("HelloWorld", left);

            first.StringValue = "Good";
            second.StringValue = "Bye";

            Assert.Equal("GoodBye", left);
        }

        [Fact]
        public void Logical_Negation_Should_Work()
        {
            var left = false;
            var right = new NotifyPropertyChangedEventObject
            {
                BoolValue = false,
            };

            _ = new EBinding
            {
                () => left == !right.BoolValue
            };

            Assert.True(left);

            right.BoolValue = true;

            Assert.False(left);
        }

        [Fact]
        public void Implicit_Convert_Should_Work_Two_Way()
        {
            var left = new NotifyPropertyChangedEventObject();

            var right = new NotifyPropertyChangedEventObject
            {
                IntValue = 42,
            };

            _ = new EBinding
            {
                BindFlag.TwoWay,
                () => left.FloatValue == right.IntValue
            };

            Assert.Equal(42f, left.FloatValue);

            right.IntValue = 33;

            Assert.Equal(33f, left.FloatValue);

            left.FloatValue = 12;

            Assert.Equal(12, right.IntValue);
        }

        [Fact]
        public void Explicit_Convert_Should_Work_One_Way()
        {
            var left = new NotifyPropertyChangedEventObject();

            var right = new IntWrapper
            {
                Value = 42,
            };

            _ = new EBinding
            {
                () => left.IntValue == (int)right,
            };

            Assert.Equal(42f, left.IntValue);
        }

        [Fact]
        public void Equality_Should_Work()
        {
            var left = false;
            var first = new NotifyPropertyChangedEventObject
            {
                IntValue = 42,
            };
            var second = new NotifyPropertyChangedEventObject
            {
                IntValue = 42,
            };

            _ = new EBinding
            {
                () => left == (first.IntValue == second.IntValue)
            };

            Assert.True(left);

            first.IntValue = 33;

            Assert.False(left);

            second.IntValue = 33;

            Assert.True(left);
        }

        [Fact]
        public void Enum_Equality_Binding_Should_Work()
        {
            var left = DayOfWeek.Sunday;
            var right = new NotifyPropertyChangedEventObject
            {
                EnumValue = DayOfWeek.Monday,
            };

            _ = new EBinding
            {
                () => left == right.EnumValue
            };

            Assert.Equal(DayOfWeek.Monday, left);

            right.EnumValue = DayOfWeek.Tuesday;

            Assert.Equal(DayOfWeek.Tuesday, left);
        }

        [Fact]
        public void Static_1_Argument_Should_Work()
        {
            var left = string.Empty;
            var right = new NotifyPropertyChangedEventObject
            {
                IntValue = 42,
            };

            _ = new EBinding
            {
                () => left == MethodInvoker.ConvertStatic(right.IntValue)
            };

            Assert.Equal("42", left);

            right.IntValue = 33;

            Assert.Equal("33", left);
        }

        [Fact]
        public void InstanceMethod_1_Argument_Should_Work()
        {
            var left = string.Empty;
            var right = new NotifyPropertyChangedEventObject
            {
                IntValue = 42,
            };
            var converter = new MethodInvoker();

            _ = new EBinding
            {
                () => left == converter.Convert(right.IntValue)
            };

            Assert.Equal("42", left);

            right.IntValue = 33;

            Assert.Equal("33", left);
        }

        [Fact]
        public void InstanceMethod_2_Arguments_Should_Work()
        {
            var left = string.Empty;
            var right = new NotifyPropertyChangedEventObject
            {
                IntValue = 42,
            };
            var right2 = 0;
            var converter = new MethodInvoker();

            _ = new EBinding
            {
                () => left == converter.Convert(right.IntValue, right2)
            };

            Assert.Equal("42", left);

            right.IntValue = 33;

            Assert.Equal("33", left);
        }

        [Fact]
        public void InstanceMethod_3_Arguments_Should_Work()
        {
            var left = string.Empty;
            var right = new NotifyPropertyChangedEventObject
            {
                IntValue = 42,
            };
            var right2 = 0;
            var right3 = 0;
            var converter = new MethodInvoker();

            _ = new EBinding
            {
                () => left == converter.Convert(right.IntValue, right2, right3)
            };

            Assert.Equal("42", left);

            right.IntValue = 33;

            Assert.Equal("33", left);
        }
    }
}
