using System;
using System.Linq.Expressions;
using System.Windows.Input;
using Xunit;

namespace EBind.Tests
{
    public class EBindingTest
    {
        [Fact]
        public void CollectionInitializer_Exceptions_Contain_Line_Info()
        {
#nullable disable
            AssertThrowsExceptionWithLineInfo(() =>
            {
                try
                {
                    var left = "left";
                    var right = "right";

                    var binding = new EBinding
                    {
                        () => left != right,
                    };
                }
                catch { throw; }
            });

            AssertThrowsExceptionWithLineInfo(() => new EBinding
            {
                null as Expression<Func<bool>>,
            });

            AssertThrowsExceptionWithLineInfo(() => new EBinding
            {
                null as Expression<Action>,
            });

            AssertThrowsExceptionWithLineInfo(() => new EBinding
            {
                (null as string, null, null as Action),
            });

            AssertThrowsExceptionWithLineInfo(() => new EBinding
            {
                ("Target", null, null as Action),
            });

            AssertThrowsExceptionWithLineInfo(() => new EBinding
            {
                ("Target", "EventName", null as Action),
            });

            AssertThrowsExceptionWithLineInfo(() => new EBinding
            {
                (null as string, null, null as ICommand),
            });

            AssertThrowsExceptionWithLineInfo(() => new EBinding
            {
                ("Target", null, null as ICommand),
            });

            AssertThrowsExceptionWithLineInfo(() => new EBinding
            {
                ("Target", "EventName", null as ICommand),
            });
#nullable enable

            static void AssertThrowsExceptionWithLineInfo(Action testCode)
            {
                var exception = Assert.Throws<Exception>(testCode);
                Assert.Contains("line #", exception.ToString());
            }
        }
    }
}
