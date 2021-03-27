using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace BenchmarkDotNet.Order
{
    public class JoinedSummaryOrdererByType : DefaultOrderer
    {
        public JoinedSummaryOrdererByType(
            SummaryOrderPolicy summaryOrderPolicy = SummaryOrderPolicy.Default,
            MethodOrderPolicy methodOrderPolicy = MethodOrderPolicy.Declared) : base(summaryOrderPolicy, methodOrderPolicy)
        { }

        public override IEnumerable<BenchmarkCase> GetSummaryOrder(ImmutableArray<BenchmarkCase> benchmarksCases, Summary summary)
        {
            return base.GetSummaryOrder(benchmarksCases, summary).OrderBy(c => c.Descriptor.Type.Name);
        }
    }
}
