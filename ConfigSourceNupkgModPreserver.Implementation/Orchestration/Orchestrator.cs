using ConfigSourceNupkgModPreserver.Contracts.Merging;
using ConfigSourceNupkgModPreserver.Contracts.WindowsFacade;
using ConfigSourceNupkgModPreserver.Contracts.WppTargetsFileHandling;

namespace ConfigSourceNupkgModPreserver.Implementation.Orchestration
{
    public class Orchestrator
    {
        private readonly IFileSystemFacade _fileSystemFacade;
        private readonly IWppTargetsFilesReader _wppTargetsFilesReader;
        private readonly IWppTargetsXmlParser _wppTargetsXmlParser;
        private readonly IMerger _merger;

        public Orchestrator(IFileSystemFacade fileSystemFacade, IWppTargetsFilesReader wppTargetsFilesReader, IWppTargetsXmlParser wppTargetsXmlParser, IMerger merger)
        {
            _fileSystemFacade = fileSystemFacade;
            _wppTargetsFilesReader = wppTargetsFilesReader;
            _wppTargetsXmlParser = wppTargetsXmlParser;
            _merger = merger;
        }

        public void RunMerge(string solutionName)
        {
            var solutionDir = _fileSystemFacade.GetDirectoryName(solutionName);

            var wppTargetsFiles = _wppTargetsFilesReader.GetWppTargetsFiles(solutionDir);

            foreach (var wppTargetsFile in wppTargetsFiles)
            {
                var xml = _fileSystemFacade.ReadAllText(wppTargetsFile);
                var configFolder = _wppTargetsXmlParser.GetConfigFolder(xml);

                var projectFolder = _fileSystemFacade.GetDirectoryName(wppTargetsFile);

                var sourceWebConfigPath = _fileSystemFacade.CombinePath(projectFolder, configFolder, "web.config");
                var transformedWebConfigPath = _fileSystemFacade.CombinePath(projectFolder, "web.config");

                _merger.RunMerge(sourceWebConfigPath, transformedWebConfigPath, solutionDir);
            }
        }
    }
}
