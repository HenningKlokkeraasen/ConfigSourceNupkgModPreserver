using System.Collections.Generic;

namespace ConfigTransSourceNupkgConfigModPreserver.Contracts
{
    public interface IFileSystemIntegrator
    {
        string GetDirectoryName(string fileFullName);
        IEnumerable<string> GetSubDirectories(string dir);
        IEnumerable<string> GetFiles(string dir, string extension);
        string ReadAllText(string path);
    }
}
