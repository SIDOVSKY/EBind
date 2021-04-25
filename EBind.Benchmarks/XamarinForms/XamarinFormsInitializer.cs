using Xamarin.Forms.Internals;

namespace EBind.Benchmarks.XamarinForms
{
    internal static class XamarinFormsInitializer
    {
        public static void Init()
        {
            Xamarin.Forms.Device.PlatformServices = new DummyPlatformServices();
        }
    }
}
