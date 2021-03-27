#nullable enable

using System.Threading.Tasks;
using Android.App;
using Xunit;

namespace EBind.Tests.Droid
{
    public class ActivityFixture<T> : IAsyncLifetime where T : Activity
    {
        public T? Activity => ActivityMonitor.Activity;

        public ActivityMonitor<T> ActivityMonitor { get; } = new();

        public Task InitializeAsync()
        {
            ((Application)Application.Context).RegisterActivityLifecycleCallbacks(ActivityMonitor);
            Application.Context.StartActivity(ActivityMonitor.Intent);
            return ActivityMonitor.WaitForResumeAsync();
        }

        public async Task DisposeAsync()
        {
            Activity?.Finish();
            await ActivityMonitor.WaitForDestroyAsync().ConfigureAwait(false);
            ((Application)Application.Context).UnregisterActivityLifecycleCallbacks(ActivityMonitor);
        }
    }
}