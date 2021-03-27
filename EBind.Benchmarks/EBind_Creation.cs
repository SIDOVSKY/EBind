using System;
using BenchmarkDotNet.Attributes;
using EBind.Test.Models;

namespace EBind.Benchmarks
{
    public class EBind_Creation
    {
        private readonly Configuration _config = new();

        private readonly NotifyPropertyChangedEventObject _npcObject = new();
        private readonly NotifyPropertyChangedEventObject _npcObject2 = new();
        private readonly PropertyChangedEventObject _pcObject = new();
        private readonly Func<int, int> _justReturn = t => t;
        private readonly MethodInvoker _instanceMethodInvoker = new();

        [GlobalSetup]
        public void Setup()
        {
            _config.ConfigureTrigger<PropertyChangedEventObject, string>(
                x => x.StringValue,
                (o, h) => o.StringValueChanged += h,
                (o, h) => o.StringValueChanged -= h);

            _config.ConfigureTrigger<PropertyChangedEventObject>(
                nameof(PropertyChangedEventObject.StringValueChanged),
                (o, h) => o.StringValueChanged += h,
                (o, h) => o.StringValueChanged -= h);
        }

        [Benchmark(Description = "a.Prop == b.Prop // INPC")]
        public void EqualityProperties_NotifyPropertyChanged()
        {
            var b = new EBinding()
            {
                () => _npcObject.IntValue == _npcObject.IntValue,
            };
            b.Dispose();
        }

        [Benchmark(Description = "a.Prop == b.Prop // EventHandler")]
        public void EqualityProperties_Handler()
        {
            var b = new EBinding(_config)
            {
                () => _pcObject.StringValue == _pcObject.StringValue,
            };
            b.Dispose();
        }

        [Benchmark(Description = "a.Prop == Static.Method(b.Prop)")]
        public void Equality_Static_1_Argument()
        {
            var b = new EBinding(_config)
            {
                () => _npcObject.StringValue == MethodInvoker.ConvertStatic(_npcObject.IntValue),
            };
            b.Dispose();
        }

        [Benchmark(Description = "a.Prop == b.Method(c.Prop)")]
        public void Equality_Instance_1_Argument()
        {
            var b = new EBinding(_config)
            {
                () => _npcObject.StringValue == _instanceMethodInvoker.Convert(_npcObject.IntValue),
            };
            b.Dispose();
        }

        [Benchmark(Description = "a.Prop == b.Method(c.Prop, d.Prop)")]
        public void Equality_Instance_2_Arguments()
        {
            var b = new EBinding(_config)
            {
                () => _npcObject.StringValue == _instanceMethodInvoker.Convert(_npcObject.IntValue, 1),
            };
            b.Dispose();
        }

        [Benchmark(Description = "a.Prop == b.Method(c.Prop, d.Prop, e.Prop)")]
        public void Equality_Instance_3_Arguments()
        {
            var b = new EBinding(_config)
            {
                () => _npcObject.StringValue == _instanceMethodInvoker.Convert(_npcObject.IntValue, 1, 2),
            };
            b.Dispose();
        }

        [Benchmark(Description = "a.Prop == (b.Prop + c.Prop).Method()")]
        public void EqualityComplexAction()
        {
            var b = new EBinding(_config)
            {
                () => _npcObject.StringValue == (_npcObject.StringValue + " ").Trim(),
            };
            b.Dispose();
        }

        [Benchmark(Description = "(a, \"nameof(a.Event)\", Method)")]
        public void Event()
        {
            var b = new EBinding(_config)
            {
                (_pcObject, nameof(_pcObject.StringValueChanged), () => { }),
            };
            b.Dispose();
        }

        [Benchmark(Description = "a.Method()")]
        public void Action_Instance_0_Arguments()
        {
            var b = new EBinding()
            {
                () => _npcObject.IntValue.GetHashCode()
            };
            b.Dispose();
        }

        [Benchmark(Description = "a.Method(b.Prop.Method())")]
        public void Action_Static_1_Argument_Method()
        {
            var b = new EBinding()
            {
                () => Convert.ToString(_npcObject.IntValue.GetHashCode())
            };
            b.Dispose();
        }

        [Benchmark(Description = "a.Prop == (b.Prop ?? c.Prop)")]
        public void NullCoalescing()
        {
            var b = new EBinding
            {
                () => _npcObject.StringValue == (_npcObject.StringValue ?? _npcObject2.StringValue)
            };
            b.Dispose();
        }

        [Benchmark(Description = "a.Prop == (b.Prop && c.Prop)")]
        public void ConditionalAnd()
        {
            var b = new EBinding
            {
                () => _npcObject.BoolValue == (_npcObject.BoolValue && _npcObject2.BoolValue)
            };
            b.Dispose();
        }

        [Benchmark(Description = "a.Prop == (b.Prop || c.Prop)")]
        public void ConditionalOr()
        {
            var b = new EBinding
            {
                () => _npcObject.BoolValue == (_npcObject.BoolValue || _npcObject2.BoolValue)
            };
            b.Dispose();
        }

        [Benchmark(Description = "a.Prop == !b.Prop")]
        public void LogicalNegation()
        {
            var b = new EBinding
            {
                () => _npcObject.BoolValue == !_npcObject2.BoolValue
            };
            b.Dispose();
        }

        [Benchmark(Description = "a.Prop == (b.Prop == c.Prop)")]
        public void Equals()
        {
            var b = new EBinding
            {
                () => _npcObject.BoolValue == (_npcObject.IntValue == _npcObject2.IntValue)
            };
            b.Dispose();
        }

        [Benchmark(Description = "a.Enum == b.Enum")]
        public void Enum()
        {
            var b = new EBinding
            {
                () => _npcObject.EnumValue == _npcObject.EnumValue
            };
            b.Dispose();
        }

        [Benchmark(Description = "a.Float == b.Int")]
        public void Conversion()
        {
            var b = new EBinding
            {
                () => _npcObject.FloatValue == _npcObject.IntValue
            };
            b.Dispose();
        }

        [Benchmark(Description = "Static.Method(() => Method(a.Prop))")]
        public void HighOrderAction()
        {
            var b = new EBinding()
            {
                () => LambdaInvoker.Invoke(() => _justReturn(_npcObject.IntValue))
            };
            b.Dispose();
        }

        [Benchmark(Description = "a.Prop == b.Prop + c.Prop")]
        public void Plus()
        {
            var b = new EBinding
            {
                () => _npcObject.StringValue == _npcObject.StringValue + _npcObject.StringValue
            };
            b.Dispose();
        }

        [Benchmark(Description = "a.Prop == $\"{b.Prop}_{c.Prop}\"")]
        public void StringInterpolation()
        {
            var b = new EBinding
            {
                () => _npcObject.StringValue == $"{_npcObject2.StringValue}_{_npcObject2.StringValue}"
            };
            b.Dispose();
        }
    }
}
