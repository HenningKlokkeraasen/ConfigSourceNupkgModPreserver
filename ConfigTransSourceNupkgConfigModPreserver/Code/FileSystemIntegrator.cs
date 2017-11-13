using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConfigTransSourceNupkgConfigModPreserver.Contracts;

namespace ConfigTransSourceNupkgConfigModPreserver.Code
{
    public class FileSystemIntegrator : IFileSystemIntegrator
    {
        public string GetDirectoryName(string fileFullName)
        {
            if (string.IsNullOrEmpty(fileFullName))
                return string.Empty;

            return Path.GetDirectoryName(fileFullName);
        }

        public IEnumerable<string> GetSubDirectories(string dir)
        {
            if (string.IsNullOrEmpty(dir))
                return Enumerable.Empty<string>();

            if (string.IsNullOrEmpty(dir))
                return Enumerable.Empty<string>();

            return Directory.GetDirectories(dir);
        }

        public IEnumerable<string> GetFiles(string dir, string extension)
        {
            if (string.IsNullOrEmpty(dir) || string.IsNullOrEmpty(extension))
                return Enumerable.Empty<string>();

            return Directory.GetFiles(dir, extension);
        }

        public string ReadAllText(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            return File.ReadAllText(path);
        }
    }
}