#nullable enable
using System;
using UIKit;

namespace EBind.Tests.iOS
{
    public class ViewControllerFixture<T> : IDisposable where T : UIViewController
    {
        protected T ViewController { get; }

        private UIViewController? _previousViewController;

        public ViewControllerFixture(Func<T>? viewControllerFactory = null)
        {
            ViewController = viewControllerFactory?.Invoke() ?? (T)Activator.CreateInstance(typeof(T));
            _previousViewController = UIApplication.SharedApplication.Delegate.GetWindow().RootViewController;
            UIApplication.SharedApplication.Delegate.GetWindow().RootViewController = ViewController;
            ViewController.LoadViewIfNeeded();
        }

        public void Dispose()
        {
            UIApplication.SharedApplication.Delegate.GetWindow().RootViewController = _previousViewController;
            _previousViewController = null;
        }
    }
}