using MugenMvvmToolkit;
using MugenMvvmToolkit.Binding;
using MugenMvvmToolkit.Infrastructure;

namespace EBind.Benchmarks
{
    internal static class MugenToolkitInitializer
    {
        public static void Init()
        {
            BindingServiceProvider.Initialize();
            ServiceProvider.ReflectionManager = new ExpressionReflectionManager();
            ServiceProvider.AttachedValueProvider = new AttachedValueProvider();
        }
    }
}
