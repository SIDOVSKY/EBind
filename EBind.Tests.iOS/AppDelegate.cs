#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using EBind.Tests.iOS;
using Foundation;
using Microsoft.DotNet.XHarness.TestRunners.Common;
using Microsoft.DotNet.XHarness.TestRunners.Xunit;
using UIKit;
using Xamarin.Essentials;
using Xunit.Runner;
using Xunit.Sdk;

[assembly: Preserve]

// All mtouch `--argument`s with prefix `-app-arg:` are passed to the managed `Main` method (here),
// see https://stackoverflow.com/a/9525233, http://docs.go-mono.com/?link=man%3amtouch(1) @ "PASSING ARGUMENTS TO THE APPLICATION".
// XHarness support them and should be called as follows:
// xharness apple test [OPTIONS] -- [RUNTIME ARGUMENTS] -app-arg:xharness
if (args.Contains("xharness"))
    UIApplication.Main(args, null, nameof(XHarnessAppDelegate));
else
    UIApplication.Main(args, null, nameof(AppDelegate));

namespace EBind.Tests.iOS
{
    [Register(nameof(AppDelegate))]
    public class AppDelegate : RunnerAppDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // We need this to ensure the execution assembly is part of the app bundle
            AddExecutionAssembly(typeof(ExtensibilityPointFactory).Assembly);

            AddTestAssembly(Assembly.GetExecutingAssembly());
            AddTestAssembly(typeof(EBindingTest).Assembly);

            return base.FinishedLaunching(app, options);
        }
    }

    [Register(nameof(XHarnessAppDelegate))]
    public class XHarnessAppDelegate : UIApplicationDelegate
    {
        public override UIWindow? Window { get; set; }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            Window = new UIWindow(UIScreen.MainScreen.Bounds)
            {
                RootViewController = new ViewController()
            };
            Window.MakeKeyAndVisible();

            return true;
        }

        private class ViewController : UIViewController
        {
            public override async void ViewDidLoad()
            {
                base.ViewDidLoad();

                var entryPoint = new EntryPoint();

                await entryPoint.RunAsync();
            }
        }

        private class EntryPoint : iOSApplicationEntryPoint
        {
            protected override bool LogExcludedTests => true;

            protected override int? MaxParallelThreads => Environment.ProcessorCount;

            protected override IDevice Device { get; } = new TestDevice();

            protected override IEnumerable<TestAssemblyInfo> GetTestAssemblies()
            {
                yield return new TestAssemblyInfo(typeof(EBindingTest).Assembly, typeof(EBindingTest).Assembly.Location);
                yield return new TestAssemblyInfo(Assembly.GetExecutingAssembly(), Assembly.GetExecutingAssembly().Location);
            }

            protected override void TerminateWithSuccess()
            {
                Console.WriteLine("Exiting test run with success");

                var s = new ObjCRuntime.Selector("terminateWithSuccess");
                UIApplication.SharedApplication.PerformSelector(s, UIApplication.SharedApplication, 0);
            }
        }

        private class TestDevice : IDevice
        {
            public string BundleIdentifier => AppInfo.PackageName;

            public string UniqueIdentifier => Guid.NewGuid().ToString("N");

            public string Name => DeviceInfo.Name;

            public string Model => DeviceInfo.Model;

            public string SystemName => DeviceInfo.Platform.ToString();

            public string SystemVersion => DeviceInfo.VersionString;

            public string Locale => CultureInfo.CurrentCulture.Name;
        }
    }
}
