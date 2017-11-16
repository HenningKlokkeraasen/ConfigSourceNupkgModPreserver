using System.Linq;
using ConfigSourceNupkgModPreserver.Contracts.Merging;
using ConfigSourceNupkgModPreserver.Contracts.VisualStudioFacade;
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
        private readonly IVisualStudioFacade _visualStudioFacade;

        public Orchestrator(IFileSystemFacade fileSystemFacade, IWppTargetsFilesReader wppTargetsFilesReader, IWppTargetsXmlParser wppTargetsXmlParser, IMerger merger, IVisualStudioFacade visualStudioFacade)
        {
            _fileSystemFacade = fileSystemFacade;
            _wppTargetsFilesReader = wppTargetsFilesReader;
            _wppTargetsXmlParser = wppTargetsXmlParser;
            _merger = merger;
            _visualStudioFacade = visualStudioFacade;
        }

        public void RunMerge(string solutionName)
        {
            var solutionDir = _fileSystemFacade.GetDirectoryName(solutionName);

            // Grab each .wpp.targets file in the solution
            var wppTargetsFiles = _wppTargetsFilesReader.GetWppTargetsFiles(solutionDir);

            foreach (var wppTargetsFile in wppTargetsFiles)
                MergeAllConfigFilesDefinedInWppTargetsFile(wppTargetsFile, solutionDir);
        }

        private void MergeAllConfigFilesDefinedInWppTargetsFile(string wppTargetsFile, string solutionDir)
        {
            var xml = _fileSystemFacade.ReadAllText(wppTargetsFile);
            var info = _wppTargetsXmlParser.GetInfo(xml);

            if (string.IsNullOrEmpty(info?.ConfigFolder) || info.ConfigFiles == null || !info.ConfigFiles.Any())
                return;

            var projectFolder = _fileSystemFacade.GetDirectoryName(wppTargetsFile);

            // Grab each config file defined in the .wpp.targets file
            foreach (var configFile in info.ConfigFiles)
                MergeConfigFiles(configFile, info, projectFolder, solutionDir);
        }

        private void MergeConfigFiles(string configFile, WppTargetsInfo info, string projectFolder, string solutionDir)
        {
            // The source config file is assumed placed in the ConfigFolder specified in the .wpp.targets file
            var sourcePath = _fileSystemFacade.CombinePath(projectFolder, info.ConfigFolder, configFile);

            // The tranformed config file is assumed placed in the root of the project
            var transformedPath = _fileSystemFacade.CombinePath(projectFolder, configFile);

            var mergeResult = _merger.RunMerge(sourcePath, transformedPath, solutionDir);

            if (mergeResult == null || mergeResult == MergeResult.Empty || mergeResult.Results == null)
                return;

            foreach (var result in mergeResult.Results)
                WriteProcessResultToVsWindow(result);
        }

        private void WriteProcessResultToVsWindow(ProcessResult result)
        {
            // Assume error if something has been written to STDERR. Do not rely on ExitCode - no standard definition of what is an error
            if (!string.IsNullOrEmpty(result.Error))
                _visualStudioFacade.WriteToGeneralPane($"Process {result.NameOfProcessOrCommand} exited with exit code {result.ExitCode} and error: {result.Error}");
#if DEBUG
            if (!string.IsNullOrEmpty(result.Output))
                _visualStudioFacade.WriteToDebugPane($"(DEBUG) Process {result.NameOfProcessOrCommand} exited with exit code {result.ExitCode} and stdout: {result.Output}");
#endif

        }
    }
}
