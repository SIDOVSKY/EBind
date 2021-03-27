#nullable enable

using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;

namespace EBind.Tests.Droid
{
    public class ActivityMonitor<T> : Java.Lang.Object, Application.IActivityLifecycleCallbacks where T : Activity
    {
        public const string ActivityMonitorGuidExtraKey = "ActivityMonitorGuid";

        private readonly string _guid = Guid.NewGuid().ToString();

        private TaskCompletionSource<object?>? _createTcs;
        private TaskCompletionSource<object?>? _destroyTcs;
        private TaskCompletionSource<object?>? _pauseTcs;
        private TaskCompletionSource<object?>? _resumeTcs;
        private TaskCompletionSource<object?>? _startTcs;
        private TaskCompletionSource<object?>? _stopTcs;

        public ActivityMonitor()
        {
            Intent = new Intent(Application.Context, typeof(T))
                .AddFlags(ActivityFlags.NewTask)
                .PutExtra(ActivityMonitorGuidExtraKey, _guid);
        }

        public T? Activity { get; private set; }
        public Intent Intent { get; }

        public void OnActivityCreated(Activity activity, Bundle? savedInstanceState)
        {
            if (!IsMonitorable(activity))
                return;

            Activity = (T)activity;
            _createTcs?.TrySetResult(null);
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
            if (!IsMonitorable(activity))
                return;

            _startTcs?.TrySetResult(null);
        }

        public void OnActivityResumed(Activity activity)
        {
            if (!IsMonitorable(activity))
                return;

            Activity = (T)activity;
            _resumeTcs?.TrySetResult(null);
        }

        public void OnActivityPaused(Activity activity)
        {
            if (!IsMonitorable(activity))
                return;

            Activity = (T)activity;
            _pauseTcs?.TrySetResult(null);
        }

        public void OnActivityStopped(Activity activity)
        {
            if (!IsMonitorable(activity))
                return;

            _stopTcs?.TrySetResult(null);
        }

        public void OnActivityDestroyed(Activity activity)
        {
            if (!IsMonitorable(activity))
                return;

            Activity = null;
            _destroyTcs?.TrySetResult(null);
        }

        public Task WaitForCreateAsync()
        {
            if (_createTcs?.Task.IsCompleted != false)
            {
                _createTcs = new();
            }

            return _createTcs.Task;
        }

        public Task WaitForStartAsync()
        {
            if (_startTcs?.Task.IsCompleted != false)
            {
                _startTcs = new();
            }

            return _startTcs.Task;
        }

        public Task WaitForResumeAsync()
        {
            if (_resumeTcs?.Task.IsCompleted != false)
            {
                _resumeTcs = new();
            }

            return _resumeTcs.Task;
        }

        public Task WaitForPauseAsync()
        {
            if (_pauseTcs?.Task.IsCompleted != false)
            {
                _pauseTcs = new();
            }

            return _pauseTcs.Task;
        }

        public Task WaitForStopAsync()
        {
            if (_stopTcs?.Task.IsCompleted != false)
            {
                _stopTcs = new();
            }

            return _stopTcs.Task;
        }

        public Task WaitForDestroyAsync()
        {
            if (Activity is null)
                return Task.CompletedTask;

            if (_destroyTcs?.Task.IsCompleted != false)
            {
                _destroyTcs = new();
            }

            return _destroyTcs.Task;
        }

        private bool IsMonitorable(Activity activity) =>
            activity == Activity
            || activity.Intent?.GetStringExtra(ActivityMonitorGuidExtraKey) == _guid;
    }
}