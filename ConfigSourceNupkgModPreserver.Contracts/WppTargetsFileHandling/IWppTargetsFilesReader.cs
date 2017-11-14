using System.Collections.Generic;

namespace ConfigSourceNupkgModPreserver.Contracts.WppTargetsFileHandling
{
    public interface IWppTargetsFilesReader
    {
        IEnumerable<string> GetWppTargetsFiles(string directory);
    }
}