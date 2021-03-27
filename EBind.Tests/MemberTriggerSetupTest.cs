using Xunit;

namespace EBind.Tests
{
    public class MemberTriggerSetupTest
    {
        [Fact]
        public void Trigger_Overwrite_Should_Work()
        {
            var setup = new MemberTriggerSetup();
            var memberInfo = typeof(string).GetProperty(nameof(string.Length))!;

            setup.AddTrigger<string, string>(memberInfo, _ => "0", null!, null!);
            var del = setup.FindTriggerDelegate(memberInfo)!;

            var handler = (string)del.CreateHandler(null!);
            Assert.Equal("0", handler);

            setup.AddTrigger<string, string>(memberInfo, _ => "1", null!, null!);
            del = setup.FindTriggerDelegate(memberInfo)!;

            handler = (string)del.CreateHandler(null!);
            Assert.Equal("1", handler);
        }
    }
}
