﻿using BenchmarkDotNet.Attributes;
using EBind.Test.Models;
using GalaSoft.MvvmLight.Helpers;
using MugenMvvmToolkit;
using MugenMvvmToolkit.Binding;
using MvvmCross.Binding.BindingContext;
using ReactiveUI;

namespace EBind.Benchmarks
{
    public class Comparison_Creation : IViewFor<NotifyPropertyChangedEventObject>
    {
        private readonly MvxBindingContextOwner _mvxContextOwner = new MvxBindingContextOwner();

        private NotifyPropertyChangedEventObject? _target;
        private NotifyPropertyChangedEventObject? _source;

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

        [GlobalSetup]
        public void Setup()
        {
            MvvmCrossInitializer.Init();
            MugenToolkitInitializer.Init();

            _target = new NotifyPropertyChangedEventObject();
            _source = new NotifyPropertyChangedEventObject
            {
                IntValue = 1
            };
        }

        [Benchmark(Baseline = true)]
        public void EBind()
        {
            _target!.IntValue = 0;

            var b = new EBinding()
            {
                () => _target.IntValue == _source!.IntValue,
            };

            b.Dispose();
        }

        [Benchmark]
        public void Mugen()
        {
            _target!.IntValue = 0;

            var b = _target.Bind(() => t => t.IntValue)
                .To(_source, () => (s, _) => s!.IntValue)
                .OneWay()
                .Build();

            b.Dispose();
        }

        [Benchmark]
        public void MvvmLight()
        {
            _target!.IntValue = 0;

            var b = new Binding<int, int>(
                _source, () => _source!.IntValue,
                _target, () => _target.IntValue,
                BindingMode.OneWay);

            b.Detach();
        }

        [Benchmark]
        public void MvvmCross()
        {
            _target!.IntValue = 0;

            _mvxContextOwner.CreateBinding(_target)
                .For(x => x.IntValue)
                .To<NotifyPropertyChangedEventObject>(obj => obj.IntValue)
                .OneWay()
                .Apply();

            _mvxContextOwner.ClearAllBindings();
        }

        [Benchmark]
        public void ReactiveUI()
        {
            _target!.IntValue = 0;

            var b = this.OneWayBind(_source,
                x => x.IntValue,
                v => v._target!.IntValue);

            b.Dispose();
        }

        [Benchmark]
        public void PraeclarumBind()
        {
            _target!.IntValue = 0;

            var left = 0;
            var b = Praeclarum.Bind.Binding.Create(() => left == _source!.IntValue);

            b.Unbind();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _target = null;
            _source = null;
        }
    }
}
