using System.Collections.Generic;
using System.Linq;
using ConfigTransSourceNupkgConfigModPreserver.Contracts;

namespace ConfigTransSourceNupkgConfigModPreserver.Code
{
    public class WppTargetsFilesReader
    {
        public const string WppTargetsFileExtension = ".wpp.targets";

        private readonly IFileSystemIntegrator _fileGetter;

        public WppTargetsFilesReader(IFileSystemIntegrator fileGetter)
        {
            _fileGetter = fileGetter;
        }

        public IEnumerable<string> GetWppTargetsFiles(string solutionFullName)
        {
            if (string.IsNullOrEmpty(solutionFullName))
                return Enumerable.Empty<string>();

            var subDirectories = _fileGetter.GetSubDirectories(solutionFullName);
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