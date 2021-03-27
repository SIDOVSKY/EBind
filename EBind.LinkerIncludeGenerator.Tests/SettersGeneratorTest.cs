using System;
using Xunit;

namespace EBind.LinkerIncludeGenerator.Tests
{
    public class SettersGeneratorTest
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
                    () => obj.Property == obj.ReadOnlyProperty,
                    () => Sample.StaticMethod(obj.Property),
                };
            }
#pragma warning restore CS8321 // Local function is declared but never used

            var includeClass = Type.GetType("EBind.LinkerInclude.Setters");

            Assert.NotNull(includeClass);
        }

        internal class Sample
        {
            public static void StaticMethod(int parameter)
            {
            }

            public int Property { get; set; }

            public int ReadOnlyProperty { get; }
        }
    }
}
