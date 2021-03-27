using System.Reflection;
using Android.App;
using Android.OS;
using Xunit.Runners.UI;
using Xunit.Sdk;

namespace EBind.Tests.Droid
{
    [Activity(
        Label = "@string/app_name",
        MainLauncher = true,
        Theme = "@android:style/Theme.Material.Light")]
    public class MainActivity : RunnerActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            AddTestAssembly(typeof(EBindingTest).Assembly);
            AddTestAssembly(Assembly.GetExecutingAssembly());

            AddExecutionAssembly(typeof(ExtensibilityPointFactory).Assembly);

            base.OnCreate(bundle);
        }
    }
}
