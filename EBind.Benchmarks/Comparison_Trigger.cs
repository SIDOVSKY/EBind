using System;
using BenchmarkDotNet.Attributes;
using EBind.Test.Models;
using GalaSoft.MvvmLight.Helpers;
using MugenMvvmToolkit;
using MugenMvvmToolkit.Binding;
using MvvmCross.Binding.BindingContext;
using ReactiveUI;

namespace EBind.Benchmarks
{
    public class Comparison_Trigger : IViewFor<NotifyPropertyChangedEventObject>
    {
        private NotifyPropertyChangedEventObject? _target;
        private NotifyPropertyChangedEventObject? _source;
        private object? _bindingRef;

        #region ReactiveUI IViewFor
        public NotifyPropertyChangedEventObject? ViewModel
        {
            get => _source;
            set { }
        }
        object? IViewFor.ViewModel
        {
            get => _source;
            set { }
        }
        #endregion

        [GlobalSetup(Target = nameof(EBind))]
        public void EBindSetup()
        {
            _target = new NotifyPropertyChangedEventObject();
            _source = new NotifyPropertyChangedEventObject();

            _bindingRef = new EBinding()
            {
                () => _target.IntValue == _source.IntValue,
            };
        }

        [Benchmark(Baseline = true)]
        public void EBind() => ChangeSourceValue();

        [GlobalSetup(Target = nameof(Mugen))]
        public void MugenSetup()
        {
            MugenToolkitInitializer.Init();

            _target = new NotifyPropertyChangedEventObject();
            _source = new NotifyPropertyChangedEventObject();

            _bindingRef = _target.Bind(() => t => t.IntValue)
                .To(_source, () => (s, _) => s.IntValue)
                .OneWay()
                .Build();
        }

        [Benchmark]
        public void Mugen() => ChangeSourceValue();

        [GlobalSetup(Target = nameof(MvvmLight))]
        public void MvvmLightSetup()
        {
            _target = new NotifyPropertyChangedEventObject();
            _source = new NotifyPropertyChangedEventObject();

            _bindingRef = new Binding<int, int>(
                _source, () => _source.IntValue,
                _target, () => _target.IntValue,
                BindingMode.OneWay);
        }

        [Benchmark]
        public void MvvmLight() => ChangeSourceValue();

        [GlobalSetup(Target = nameof(MvvmCross))]
        public void MvvmCrossSetup()
        {
            MvvmCrossInitializer.Init();

            _target = new NotifyPropertyChangedEventObject();
            _source = new NotifyPropertyChangedEventObject();

            var mvxContextOwner = new MvxBindingContextOwner
            {
                BindingContext =
                {
                    DataContext = _source
                }
            };

            _bindingRef = mvxContextOwner;

            mvxContextOwner.CreateBinding(_target)
                .For(x => x.IntValue)
                .To<NotifyPropertyChangedEventObject>(obj => obj.IntValue)
                .OneWay()
                .Apply();
        }

        [Benchmark]
        public void MvvmCross() => ChangeSourceValue();

        [GlobalSetup(Target = nameof(ReactiveUI))]
        public void ReactiveUiSetup()
        {
            _target = new NotifyPropertyChangedEventObject();
            _source = new NotifyPropertyChangedEventObject();

            _bindingRef = this.OneWayBind(ViewModel,
                x => x.IntValue,
                v => v._target!.IntValue);
        }

        [Benchmark]
        public void ReactiveUI() => ChangeSourceValue();

        [GlobalSetup(Target = nameof(PraeclarumBind))]
        public void PraeclarumBindSetup()
        {
            var left = 0;
            _source = new NotifyPropertyChangedEventObject();

            _bindingRef = Praeclarum.Bind.Binding.Create(() => left == _source.IntValue);
        }

        [Benchmark]
        public void PraeclarumBind() => ChangeSourceValue();

        [GlobalSetup(Target = nameof(XamarinFormsCompiled))]
        public void XamarinFormsCompiledSetup()
        {
            XamarinForms.XamarinFormsInitializer.Init();

            var target = new XamarinForms.SimplestBindableObject();
            _source = new NotifyPropertyChangedEventObject();

            var binding = new Xamarin.Forms.Internals.TypedBinding<NotifyPropertyChangedEventObject, int>(
                getter: s => (s.IntValue, true),
                setter: (s, v) => s.IntValue = v,
                handlers: new[]
                {
                    Tuple.Create<Func<NotifyPropertyChangedEventObject, object>, string>(s => s, nameof(NotifyPropertyChangedEventObject.IntValue))
                })
            {
                Mode = Xamarin.Forms.BindingMode.OneWay,
            };

            target.BindingContext = _source;
            target.SetBinding(XamarinForms.SimplestBindableObject.ValueProperty, binding);

            _bindingRef = target;
        }

        [Benchmark]
        public void XamarinFormsCompiled() => ChangeSourceValue();

        private void ChangeSourceValue()
        {
            if (_source!.IntValue <= 0)
            {
                _source.IntValue++;
            }
            else
            {
                _source.IntValue--;
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _target = null;
            _source = null;
            _bindingRef = null;
        }
    }
}
