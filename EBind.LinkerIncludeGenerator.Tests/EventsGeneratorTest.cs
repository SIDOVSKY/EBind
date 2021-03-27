using System;
using Xunit;

namespace EBind.LinkerIncludeGenerator.Tests
{
    public class EventsGeneratorTest
    {
        [Fact]
        public void Should_Include_Code_In_The_Assembly()
        {
#pragma warning disable CS8321 // Local function is declared but never used
            static void Usage()
            {
                var obj = new Sample();

                _ = new EBinding
                {
                    (obj, nameof(obj.Event), () => { }),
                };
            }
#pragma warning restore CS8321 // Local function is declared but never used

            var includeClass = Type.GetType("EBind.LinkerInclude.Events");

            Assert.NotNull(includeClass);
        }

        internal class Sample
        {
            public event EventHandler? Event;
        }
    }
}
