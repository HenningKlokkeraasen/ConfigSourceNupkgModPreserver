using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConfigSourceNupkgModPreserver.Contracts.WindowsFacade;

namespace ConfigSourceNupkgModPreserver.Implementation.WindowsFacade
{
    public class FileSystemFacade : IFileSystemFacade
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

        public string CombinePath(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path1) || string.IsNullOrEmpty(path2))
                return string.Empty;

            return Path.Combine(path1, path2);
        }

        public string CombinePath(params string[] paths)
        {
            if (paths == null || !paths.Any())
                return string.Empty;

            return Path.Combine(paths);
        }
    }
}