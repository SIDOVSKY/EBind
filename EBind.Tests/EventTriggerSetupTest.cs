using System;
using EBind.EventBindings;
using EBind.Test.Models;
using Xunit;

namespace EBind.Tests
{
    public class EventTriggerSetupTest
    {
        [Fact]
        public void FindTriggerDelegate_Should_Give_Closest_In_Type_Hierarchy()
        {
            var setup = new EventTriggerSetup();
            const string eventName = "Test";

            var handlerFactories = new Func<Action, string>[]
            {
                _ => "0",
                _ => "1",
                _ => "2",
                _ => "3",
                _ => "4",
            };

            setup.AddTrigger<bool, string>(eventName, handlerFactories[4], null!, null!);
            setup.AddTrigger<HierarchyObject1, string>(eventName, handlerFactories[1], null!, null!);
            setup.AddTrigger<HierarchyObject0, string>(eventName, handlerFactories[0], null!, null!);
            setup.AddTrigger<byte, string>(eventName, handlerFactories[4], null!, null!);
            setup.AddTrigger<HierarchyObject3, string>(eventName, handlerFactories[3], null!, null!);
            setup.AddTrigger<HierarchyObject2, string>(eventName, handlerFactories[2], null!, null!);
            setup.AddTrigger<int, string>(eventName, handlerFactories[4], null!, null!);

            var del = setup.FindTriggerDelegate(eventName, typeof(HierarchyObject3))!;
            var handler = (string)del.CreateHandler(null!);

            Assert.Equal("3", handler);
        }

        [Fact]
        public void Trigger_Overwrite_Should_Work()
        {
            var setup = new EventTriggerSetup();
            const string eventName = "Test";

            setup.AddTrigger<int, string>(eventName, _ => "0", null!, null!);
            var del = setup.FindTriggerDelegate(eventName, typeof(int))!;

            var handler = (string)del.CreateHandler(null!);
            Assert.Equal("0", handler);

            setup.AddTrigger<int, string>(eventName, _ => "1", null!, null!);
            del = setup.FindTriggerDelegate(eventName, typeof(int))!;

            handler = (string)del.CreateHandler(null!);
            Assert.Equal("1", handler);
        }
    }
}
