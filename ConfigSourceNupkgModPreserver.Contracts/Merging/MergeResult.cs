using System.Collections.Generic;
using System.Linq;

namespace ConfigSourceNupkgModPreserver.Contracts.Merging
{
    public class MergeResult
    {
        public IEnumerable<ProcessResult> Results { get; set; }

        public static MergeResult Empty => new MergeResult
        {
            Results = Enumerable.Empty<ProcessResult>()
        };
    }
}