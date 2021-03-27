#nullable enable
using System;
using System.Threading.Tasks;
using Android.Views;

namespace EBind.Tests.Droid
{
    public static class ViewPostExtensions
    {
        public static Task PostAsync(this View view)
        {
            var tcs = new TaskCompletionSource<object?>();
            view.Post(() => tcs.TrySetResult(null));
            return tcs.Task;
        }

        public static Task PostAsync(this View view, Action action)
        {
            var tcs = new TaskCompletionSource<object?>();
            view.Post(() =>
            {
                try
                {
                    action.Invoke();
                    tcs.TrySetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });
            return tcs.Task;
        }

        public static Task PostAsync(this View view, Func<Task> task)
        {
            var tcs = new TaskCompletionSource<object?>();
            view.Post(async () =>
            {
                try
                {
                    await task().ConfigureAwait(false);

                    tcs.TrySetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });
            return tcs.Task;
        }
    }
}