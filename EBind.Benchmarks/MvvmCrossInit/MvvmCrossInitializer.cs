using System;
using MvvmCross.Base;
using MvvmCross.Binding;
using MvvmCross.IoC;
using MvvmCross.Logging;

namespace EBind.Benchmarks
{
    internal static class MvvmCrossInitializer
    {
        public static void Init()
        {
            MvxIoCProvider.Initialize();
            MvxIoCProvider.Instance.RegisterSingleton<IMvxLogProvider>(() => new DummyLogProvider());
            MvxIoCProvider.Instance.RegisterSingleton<IMvxMainThreadAsyncDispatcher>(() => new DummyMainThreadAsyncDispatcher());
            new MvxBindingBuilder().DoRegistration();
        }

        public class DummyLogProvider : IMvxLogProvider
        {
            public IMvxLog GetLogFor(Type type) => new DummyLog();

            public IMvxLog GetLogFor<T>() => new DummyLog();

            public IMvxLog GetLogFor(string name) => new DummyLog();

            public IDisposable OpenMappedContext(string key, string value)
            {
                throw new NotImplementedException();
            }

            public IDisposable OpenNestedContext(string message)
            {
                throw new NotImplementedException();
            }

            private class DummyLog : IMvxLog
            {
                public bool IsLogLevelEnabled(MvxLogLevel logLevel) => false;

                public bool Log(
                    MvxLogLevel logLevel,
                    Func<string> messageFunc,
                    Exception? exception = null,
                    params object[] formatParameters) => false;
            }
        }

        public class DummyMainThreadAsyncDispatcher : MvxMainThreadAsyncDispatcher
        {
            public override bool IsOnMainThread => true;

            public override bool RequestMainThreadAction(Action action, bool maskExceptions = true)
            {
                action();
                return true;
            }
        }
    }
}
