using System;
using System.Reflection;
using EBind.Test.Models;
using Xunit;

namespace EBind.Tests
{
    public class ConfigurationTest
    {
        [Fact]
        public void Configure_EventHandler_Member_Trigger()
        {
            var configuration = new Configuration();
            configuration.ConfigureTrigger<TriggerSample, string>(
                x => x.Member,
                (o, h) => o.EventHandler += h,
                (o, h) => o.EventHandler -= h,
                setter: (o, v) => o.BetterSetMember(v));

            var del = configuration.MemberTriggerSetup.FindTriggerDelegate(TriggerSample.MemberInfo);

            Assert.NotNull(del);
        }

        [Fact]
        public void Configure_GenericEventHandler_Member_Trigger()
        {
            var configuration = new Configuration();
            configuration.ConfigureTrigger<TriggerSample, string, string>(
                x => x.Member,
                (o, h) => o.GenericEventHandler += h,
                (o, h) => o.GenericEventHandler -= h,
                setter: (o, v) => o.BetterSetMember(v));

            var del = configuration.MemberTriggerSetup.FindTriggerDelegate(TriggerSample.MemberInfo);

            Assert.NotNull(del);
        }

        [Fact]
        public void Configure_CustomEventHandler_Member_Trigger()
        {
            var configuration = new Configuration();
            configuration.ConfigureTrigger<TriggerSample, TriggerSample.CustomEventHandler, string>(
                x => x.Member,
                trigger => new TriggerSample.CustomEventHandler(trigger),
                (o, h) => o.AddCustomEventHandler(h),
                (o, h) => o.RemoveCustomEventHandler(h),
                setter: (o, v) => o.BetterSetMember(v));

            var del = configuration.MemberTriggerSetup.FindTriggerDelegate(TriggerSample.MemberInfo);

            Assert.NotNull(del);
        }

        [Fact]
        public void Configure_EventHandler_Event_Trigger()
        {
            const string eventName = nameof(Configure_EventHandler_Event_Trigger);

            var configuration = new Configuration();
            configuration.ConfigureTrigger<TriggerSample>(
                eventName,
                (o, h) => o.EventHandler += h,
                (o, h) => o.EventHandler -= h);

            var del = configuration.EventTriggerSetup
                .FindTriggerDelegate(eventName, typeof(TriggerSample));

            Assert.NotNull(del);
        }

        [Fact]
        public void Configure_GenericEventHandler_Event_Trigger()
        {
            const string eventName = nameof(Configure_GenericEventHandler_Event_Trigger);

            var configuration = new Configuration();
            configuration.ConfigureTrigger<TriggerSample, string>(
                eventName,
                (o, h) => o.GenericEventHandler += h,
                (o, h) => o.GenericEventHandler -= h);

            var del = configuration.EventTriggerSetup
                .FindTriggerDelegate(eventName, typeof(TriggerSample));

            Assert.NotNull(del);
        }

        [Fact]
        public void Configure_CustomEventHandler_Event_Trigger()
        {
            const string eventName = nameof(Configure_CustomEventHandler_Event_Trigger);

            var configuration = new Configuration();
            configuration.ConfigureTrigger<TriggerSample, TriggerSample.CustomEventHandler>(
                eventName,
                trigger => new TriggerSample.CustomEventHandler(trigger),
                (o, h) => o.AddCustomEventHandler(h),
                (o, h) => o.RemoveCustomEventHandler(h));

            var del = configuration.EventTriggerSetup
                .FindTriggerDelegate(eventName, typeof(TriggerSample));

            Assert.NotNull(del);
        }

        [Fact]
        public void Configured_Trigger_For_Parent_Class_Should_Work_ForChildren()
        {
            var left = 0;
            var right = new HierarchyObject1
            {
                Property = 1,
            };
            var expectedValue = 42;

            var configuration = new Configuration();
            configuration.ConfigureTrigger<HierarchyObject0, Action<int>, int>(
                x => x.Property,
                t => _ => t.Invoke(),
                (o, h) => o.ChangedProperty += h,
                (o, h) => o.ChangedProperty -= h);

            _ = new EBinding(configuration)
            {
                () => left == right.Property
            };

            right.Property = expectedValue;
            Assert.Equal(expectedValue, left);
        }

        [Fact]
        public void AssignmentDispatchDelegate_Should_Be_Called_If_Set()
        {
            var left = 0;
            var right = new NotifyPropertyChangedEventObject
            {
                IntValue = 1
            };
            var delegateInvokeCount = 0;

            var configuration = new Configuration
            {
                AssignmentDispatchDelegate = a =>
                {
                    delegateInvokeCount++;
                    a();
                }
            };

            _ = new EBinding(configuration)
            {
                () => left == right.IntValue
            };

            Assert.Equal(1, delegateInvokeCount);

            right.IntValue = 2;
            Assert.Equal(2, delegateInvokeCount);
        }

        [Fact]
        public void ActionDispatchDelegate_Should_Be_Called_If_Set()
        {
            var obj = new NotifyPropertyChangedEventObject
            {
                IntValue = 1
            };
            var delegateInvokeCount = 0;

            var configuration = new Configuration
            {
                ActionDispatchDelegate = a =>
                {
                    delegateInvokeCount++;
                    a();
                }
            };

            _ = new EBinding(configuration)
            {
                () => StaticMethods.JustReturn(obj.IntValue)
            };

            Assert.Equal(1, delegateInvokeCount);

            obj.IntValue = 2;
            Assert.Equal(2, delegateInvokeCount);
        }
    }

    public class TriggerSample
    {
        public static MemberInfo MemberInfo { get; } = typeof(TriggerSample).GetProperty(nameof(Member))!;

        public string Member { get; set; } = string.Empty;

        public void BetterSetMember(string value) => Member = value;

        public event EventHandler? EventHandler;

        public event EventHandler<string>? GenericEventHandler;

        public void AddCustomEventHandler(CustomEventHandler handler)
        {
        }

        public void RemoveCustomEventHandler(CustomEventHandler handler)
        {
        }

        public class CustomEventHandler
        {
            public CustomEventHandler(Action trigger)
            {
            }
        }
    }
}
