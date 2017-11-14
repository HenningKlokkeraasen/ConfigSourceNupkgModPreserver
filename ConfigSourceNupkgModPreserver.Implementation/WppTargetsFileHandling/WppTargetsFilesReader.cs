using System.Collections.Generic;
using System.Linq;
using ConfigSourceNupkgModPreserver.Contracts.WindowsFacade;
using ConfigSourceNupkgModPreserver.Contracts.WppTargetsFileHandling;

namespace ConfigSourceNupkgModPreserver.Implementation.WppTargetsFileHandling
{
    public class WppTargetsFilesReader : IWppTargetsFilesReader
    {
        public const string WppTargetsFileExtension = ".wpp.targets";

        private readonly IFileSystemFacade _fileGetter;

        public WppTargetsFilesReader(IFileSystemFacade fileGetter)
        {
            _fileGetter = fileGetter;
        }

        /// <summary>
        /// Grabs the first file with extension .wpp.targets in each subdirectory (one level) below the directory
        /// </summary>
        /// <param name="directory">E.g. c:\source\MyApp\</param>
        public IEnumerable<string> GetWppTargetsFiles(string directory)
        {
            if (string.IsNullOrEmpty(directory))
                return Enumerable.Empty<string>();

            var subDirectories = _fileGetter.GetSubDirectories(directory);
            if (subDirectories == null)
                return Enumerable.Empty<string>();

            var result = new List<string>();
            foreach (var subDirectory in subDirectories)
            {
                var files = _fileGetter.GetFiles(subDirectory, $"*{WppTargetsFileExtension}");
                var fileList = files as IList<string> ?? files.ToList();
                if (!fileList.Any())
                    continue;
                
                result.Add(fileList.FirstOrDefault(f => !string.IsNullOrEmpty(f)));
            }

            return result;
        }
    }
}