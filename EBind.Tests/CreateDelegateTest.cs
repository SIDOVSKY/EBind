using Xunit;

namespace EBind.Tests
{
    public class CreateDelegateTest
    {
        [Fact]
        public void VirtualProperty()
        {
            const string expected = "SourceProp";

            var target = new Sample();
            var source = new Sample
            {
                IntProp = 1,
                BoolProp = true,
                Prop = expected,
                VirtualProp = expected,
                VirtualPropToOverride = expected,
            };

            var _ = new EBinding
            {
                () => target.IntProp == source.IntProp,
                () => target.BoolProp == source.BoolProp,
                () => target.Prop == source.Prop,
                () => target.VirtualProp == source.VirtualProp,
                () => target.VirtualPropToOverride == source.VirtualPropToOverride,
            };

            Assert.Equal(1, target.IntProp);
            Assert.True(target.BoolProp);
            Assert.Equal(expected, target.Prop);
            Assert.Equal(expected, target.VirtualProp);
            Assert.Equal(expected, target.VirtualPropToOverride);
        }

        [Fact]
        public void VirtualGenericProperty()
        {
            const string expected = "SourceProp";

            var target = new SampleGeneric();
            var source = new SampleGeneric
            {
                Prop = expected,
                VirtualProp = expected,
                VirtualPropToOverride = expected,
            };

            var _ = new EBinding
            {
                () => target.Prop == source.Prop,
                () => target.VirtualProp == source.VirtualProp,
                () => target.VirtualPropToOverride == source.VirtualPropToOverride,
            };

            Assert.Equal(expected, target.Prop);
            Assert.Equal(expected, target.VirtualProp);
            Assert.Equal(expected, target.VirtualPropToOverride);
        }

        // TODO test more variations with parameters of value types and nint
        [Fact]
        public void VirtualMethod()
        {
            var target = new Sample();

            var _ = new EBinding
            {
                () => target.Method(),
            };

            Assert.False(target.BaseMethodCalled);
        }

        private class SampleBase
        {
            public int IntProp { get; set; }
            public bool BoolProp { get; set; }
            public string Prop { get; set; } = "BaseProp";
            public virtual string VirtualProp { get; set; } = "BaseVirtualProp";
            public virtual string VirtualPropToOverride { get; set; } = "BaseVirtualPropToOverride";

            public bool BaseMethodCalled { get; private set; }

            public virtual void Method()
            {
                BaseMethodCalled = true;
            }
        }

        private class SampleGenericBase<T>
        {
            public T? Prop { get; set; }
            public virtual T? VirtualProp { get; set; }
            public virtual T? VirtualPropToOverride { get; set; }
        }

        private class Sample : SampleBase
        {
            public override string VirtualPropToOverride { get; set; } = "OverridenVirtualPropToOverride";

            public override void Method()
            {
            }
        }

        private class SampleGeneric : SampleGenericBase<string>
        {
            public override string? VirtualPropToOverride { get; set; } = "OverridenVirtualPropToOverride";
        }
    }
}
