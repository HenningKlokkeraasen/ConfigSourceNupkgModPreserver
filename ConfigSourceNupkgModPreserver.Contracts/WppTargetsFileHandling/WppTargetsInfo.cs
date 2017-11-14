using System.Collections.Generic;

namespace ConfigSourceNupkgModPreserver.Contracts.WppTargetsFileHandling
{
    public class WppTargetsInfo
    {
        public string ConfigFolder { get; set; }
        public IEnumerable<string> ConfigFiles { get; set; }

        public static WppTargetsInfo Empty => new WppTargetsInfo();
    }
}