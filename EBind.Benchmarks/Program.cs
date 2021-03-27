using System.Reflection;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;

IConfig config =
#if DEBUG
    new DebugInProcessConfig()
#else
    DefaultConfig.Instance
#endif
    .AddDiagnoser(MemoryDiagnoser.Default)
    //.AddJob(new BenchmarkDotNet.Jobs.Job
    //{
    //    Run = { RunStrategy = BenchmarkDotNet.Engines.RunStrategy.ColdStart, LaunchCount = 100, IterationCount = 1 }
    //})
    .WithOption(ConfigOptions.JoinSummary, true)
    .WithOrderer(new JoinedSummaryOrdererByType(SummaryOrderPolicy.FastestToSlowest));

BenchmarkSwitcher
    .FromAssembly(Assembly.GetExecutingAssembly())
    .Run(args, config);
