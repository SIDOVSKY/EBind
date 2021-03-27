using System;
using BenchmarkDotNet.Attributes;
using EBind.Test.Models;

namespace EBind.Benchmarks
{
    public class EBind_Trigger
    {
        private readonly NotifyPropertyChangedEventObject _npcObject = new();
        private readonly PropertyChangedEventObject _pcObject = new();

        private EBinding? _binding;

        [GlobalSetup(Target = nameof(EqualityProperties_NotifyPropertyChanged))]
        public void Setup()
        {
            var target = new NotifyPropertyChangedEventObject();

            _binding = new EBinding()
            {
                () => target.IntValue == _npcObject.IntValue,
            };
        }

        [Benchmark(Description = "a.Prop == b.Prop // INPC")]
        public void EqualityProperties_NotifyPropertyChanged()
        {
            ChangeIntValue(_npcObject);
        }

        [GlobalSetup(Target = nameof(EqualityProperties_Handler))]
        public void SetupHandler()
        {
            var target = new PropertyChangedEventObject();

            var configuration = new Configuration();
            configuration.ConfigureTrigger<PropertyChangedEventObject, string>(
                x => x.StringValue,
                (o, h) => o.StringValueChanged += h,
                (o, h) => o.StringValueChanged -= h);

            _binding = new EBinding(configuration)
            {
                () => target.StringValue == _pcObject.StringValue,
            };
        }

        [Benchmark(Description = "a.Prop == b.Prop // EventHandler")]
        public void EqualityProperties_Handler()
        {
            ChangeStringValue(_pcObject);
        }

        [GlobalSetup(Target = nameof(Action_Instance_0_Arguments))]
        public void SetupAction_Instance_0_Arguments()
        {
            _binding = new EBinding()
            {
                () => _npcObject.IntValue.GetHashCode()
            };
        }

        [Benchmark(Description = "a.Method()")]
        public void Action_Instance_0_Arguments()
        {
            ChangeIntValue(_npcObject);
        }

        [GlobalSetup(Target = nameof(Action_Static_1_Argument_Method))]
        public void SetupAction_Static_1_Argument_Method()
        {
            _binding = new EBinding()
            {
                () => Convert.ToString(_npcObject.IntValue.GetHashCode())
            };
        }

        [Benchmark(Description = "a.Method(b.Prop.Method())")]
        public void Action_Static_1_Argument_Method()
        {
            ChangeIntValue(_npcObject);
        }

        [GlobalSetup(Target = nameof(Equality_Static_1_Argument))]
        public void SetupEquality_Static_1_Argument()
        {
            var target = new NotifyPropertyChangedEventObject();

            _binding = new EBinding()
            {
                () => target.StringValue == MethodInvoker.ConvertStatic(_npcObject.IntValue),
            };
        }

        [Benchmark(Description = "a.Prop == Static.Method(b.Prop)")]
        public void Equality_Static_1_Argument()
        {
            ChangeIntValue(_npcObject);
        }

        [GlobalSetup(Target = nameof(Equality_Instance_1_Argument))]
        public void SetupEquality_Instance_1_Argument()
        {
            var target = new NotifyPropertyChangedEventObject();
            var instanceMethodInvoker = new MethodInvoker();

            _binding = new EBinding()
            {
                () => target.StringValue == instanceMethodInvoker.Convert(_npcObject.IntValue),
            };
        }

        [Benchmark(Description = "a.Prop == b.Method(c.Prop)")]
        public void Equality_Instance_1_Argument()
        {
            ChangeIntValue(_npcObject);
        }

        [GlobalSetup(Target = nameof(Equality_Instance_2_Arguments))]
        public void SetupEquality_Instance_2_Arguments()
        {
            var target = new NotifyPropertyChangedEventObject();
            var instanceMethodInvoker = new MethodInvoker();

            _binding = new EBinding()
            {
                () => target.StringValue == instanceMethodInvoker.Convert(_npcObject.IntValue, 1),
            };
        }

        [Benchmark(Description = "a.Prop == b.Method(c.Prop, d.Prop)")]
        public void Equality_Instance_2_Arguments()
        {
            ChangeIntValue(_npcObject);
        }

        [GlobalSetup(Target = nameof(Equality_Instance_3_Arguments))]
        public void SetupEquality_Instance_3_Arguments()
        {
            var target = new NotifyPropertyChangedEventObject();
            var instanceMethodInvoker = new MethodInvoker();

            _binding = new EBinding()
            {
                () => target.StringValue == instanceMethodInvoker.Convert(_npcObject.IntValue, 1, 2),
            };
        }

        [Benchmark(Description = "a.Prop == b.Method(c.Prop, d.Prop, e.Prop)")]
        public void Equality_Instance_3_Arguments()
        {
            ChangeIntValue(_npcObject);
        }

        [GlobalSetup(Target = nameof(EqualityComplexAction))]
        public void SetupEqualityComplexAction()
        {
            var target = new NotifyPropertyChangedEventObject();

            _binding = new EBinding()
            {
                () => target.StringValue == (_npcObject.StringValue + " ").Trim(),
            };
        }

        [Benchmark(Description = "a.Prop == (b.Prop + c.Prop).Method()")]
        public void EqualityComplexAction()
        {
            ChangeStringValue(_npcObject);
        }

        [GlobalSetup(Target = nameof(NullCoalescing))]
        public void SetupNullCoalescing()
        {
            var target = new NotifyPropertyChangedEventObject();
            var nullValueNPC = new NotifyPropertyChangedEventObject
            {
                StringValue = null,
            };

            _binding = new EBinding
            {
                () => target.StringValue == (nullValueNPC.StringValue ?? _npcObject.StringValue)
            };
        }

        [Benchmark(Description = "a.Prop == (b.Prop ?? c.Prop)")]
        public void NullCoalescing()
        {
            ChangeStringValue(_npcObject);
        }

        [GlobalSetup(Target = nameof(ConditionalAnd))]
        public void SetupConditionalAnd()
        {
            var target = new NotifyPropertyChangedEventObject();
            var trueValueNPC = new NotifyPropertyChangedEventObject
            {
                BoolValue = true,
            };

            _binding = new EBinding
            {
                () => target.BoolValue == (trueValueNPC.BoolValue && _npcObject.BoolValue)
            };
        }

        [Benchmark(Description = "a.Prop == (b.Prop && c.Prop)")]
        public void ConditionalAnd()
        {
            _npcObject.BoolValue = !_npcObject.BoolValue;
        }

        [GlobalSetup(Target = nameof(ConditionalOr))]
        public void SetupConditionalOr()
        {
            var target = new NotifyPropertyChangedEventObject();
            var falseValueNPC = new NotifyPropertyChangedEventObject
            {
                BoolValue = false,
            };

            _binding = new EBinding
            {
                () => target.BoolValue == (falseValueNPC.BoolValue || _npcObject.BoolValue)
            };
        }

        [Benchmark(Description = "a.Prop == (b.Prop || c.Prop)")]
        public void ConditionalOr()
        {
            _npcObject.BoolValue = !_npcObject.BoolValue;
        }

        [GlobalSetup(Target = nameof(LogicalNegation))]
        public void SetupLogicalNegation()
        {
            var target = new NotifyPropertyChangedEventObject();

            _binding = new EBinding
            {
                () => target.BoolValue == !_npcObject.BoolValue
            };
        }

        [Benchmark(Description = "a.Prop == !b.Prop")]
        public void LogicalNegation()
        {
            _npcObject.BoolValue = !_npcObject.BoolValue;
        }

        [GlobalSetup(Target = nameof(Equals))]
        public void SetupEquals()
        {
            var target = new NotifyPropertyChangedEventObject();
            var zeroValueNPC = new NotifyPropertyChangedEventObject();

            _binding = new EBinding
            {
                () => target.BoolValue == (zeroValueNPC.IntValue == _npcObject.IntValue)
            };
        }

        [Benchmark(Description = "a.Prop == (b.Prop == c.Prop)")]
        public void Equals()
        {
            ChangeIntValue(_npcObject);
        }

        [GlobalSetup(Target = nameof(Enum))]
        public void SetupEnum()
        {
            var target = new NotifyPropertyChangedEventObject();

            _binding = new EBinding
            {
                () => target.EnumValue == _npcObject.EnumValue
            };
        }

        [Benchmark(Description = "a.Enum == b.Enum")]
        public void Enum()
        {
            ChangeEnumValue(_npcObject);
        }

        [GlobalSetup(Target = nameof(Conversion))]
        public void SetupConversion()
        {
            var target = new NotifyPropertyChangedEventObject();

            _binding = new EBinding
            {
                () => target.FloatValue == _npcObject.IntValue
            };
        }

        [Benchmark(Description = "a.Float == b.Int")]
        public void Conversion()
        {
            ChangeIntValue(_npcObject);
        }

        [GlobalSetup(Target = nameof(Plus))]
        public void SetupPlus()
        {
            var target = new NotifyPropertyChangedEventObject();
            var constStringNPC = new NotifyPropertyChangedEventObject
            {
                StringValue = "const_"
            };

            _binding = new EBinding
            {
                () => target.StringValue == constStringNPC.StringValue + _npcObject.StringValue
            };
        }

        [Benchmark(Description = "a.Prop == b.Prop + c.Prop")]
        public void Plus()
        {
            ChangeStringValue(_npcObject);
        }

        [GlobalSetup(Target = nameof(StringInterpolation))]
        public void SetupStringInterpolation()
        {
            var target = new NotifyPropertyChangedEventObject();
            var constStringNPC = new NotifyPropertyChangedEventObject
            {
                StringValue = "const"
            };

            _binding = new EBinding
            {
                () => target.StringValue == $"{constStringNPC.StringValue}_{_npcObject.StringValue}"
            };
        }

        [Benchmark(Description = "a.Prop == $\"{b.Prop}_{c.Prop}\"")]
        public void StringInterpolation()
        {
            ChangeStringValue(_npcObject);
        }

        private void ChangeIntValue(NotifyPropertyChangedEventObject obj)
        {
            if (obj.IntValue <= 0)
            {
                obj.IntValue++;
            }
            else
            {
                obj.IntValue--;
            }
        }

        private void ChangeStringValue(NotifyPropertyChangedEventObject obj)
        {
            obj.StringValue = obj.StringValue?.Length == 0
                ? "NotEmpty"
                : string.Empty;
        }

        private void ChangeEnumValue(NotifyPropertyChangedEventObject obj)
        {
            obj.EnumValue = obj.EnumValue == DayOfWeek.Monday
                ? DayOfWeek.Tuesday
                : DayOfWeek.Monday;
        }

        private void ChangeStringValue(PropertyChangedEventObject obj)
        {
            obj.StringValue = obj.StringValue.Length == 0
                ? "NotEmpty"
                : string.Empty;
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _binding!.Dispose();
        }
    }
}
