using System.Linq;
using ConfigSourceNupkgModPreserver.Contracts.Merging;
using ConfigSourceNupkgModPreserver.Contracts.Orchestration;
using ConfigSourceNupkgModPreserver.Contracts.VisualStudioFacade;
using ConfigSourceNupkgModPreserver.Contracts.WindowsFacade;
using ConfigSourceNupkgModPreserver.Contracts.WppTargetsFileHandling;
using Microsoft.Internal.VisualStudio.PlatformUI;

namespace ConfigSourceNupkgModPreserver.Implementation.Orchestration
{
    public class Orchestrator
    {
        private readonly IFileSystemFacade _fileSystemFacade;
        private readonly IWppTargetsFilesReader _wppTargetsFilesReader;
        private readonly IWppTargetsXmlParser _wppTargetsXmlParser;
        private readonly IMerger _merger;
        private readonly IVisualStudioFacade _visualStudioFacade;
        private readonly IPrompter _prompter;

        public Orchestrator(IFileSystemFacade fileSystemFacade, IWppTargetsFilesReader wppTargetsFilesReader, IWppTargetsXmlParser wppTargetsXmlParser, IMerger merger, IVisualStudioFacade visualStudioFacade, IPrompter prompter)
        {
            _fileSystemFacade = fileSystemFacade;
            _wppTargetsFilesReader = wppTargetsFilesReader;
            _wppTargetsXmlParser = wppTargetsXmlParser;
            _merger = merger;
            _visualStudioFacade = visualStudioFacade;
            _prompter = prompter;
        }

        public void RunMerge(string solutionName)
        {
            var solutionDir = _fileSystemFacade.GetDirectoryName(solutionName);

            // Grab each .wpp.targets file in the solution
            var wppTargetsFiles = _wppTargetsFilesReader.GetWppTargetsFiles(solutionDir);

            foreach (var wppTargetsFile in wppTargetsFiles)
                if (!MergeAllConfigFilesDefinedInWppTargetsFile(wppTargetsFile, solutionDir))
                    return;
        }

        /// <summary>
        /// </summary>
        /// <returns>True if merging should continue, False if user presses Cancel</returns>
        private bool MergeAllConfigFilesDefinedInWppTargetsFile(string wppTargetsFile, string solutionDir)
        {
            var xml = _fileSystemFacade.ReadAllText(wppTargetsFile);
            var info = _wppTargetsXmlParser.GetInfo(xml);

            if (string.IsNullOrEmpty(info?.ConfigFolder) || info.ConfigFiles == null || !info.ConfigFiles.Any())
                return false;

            var projectFolder = _fileSystemFacade.GetDirectoryName(wppTargetsFile);

            // Grab each config file defined in the .wpp.targets file
            foreach (var configFile in info.ConfigFiles)
                if (!MergeConfigFiles(configFile, info, projectFolder, solutionDir))
                    return false;

            return true;
        }

        /// <summary>
        /// Prompts the VS user if git merge-file should be run, 
        /// and runs the git command if the answer is "yes".
        /// </summary>
        /// <returns>True if merging should continue, False if user presses Cancel</returns>
        private bool MergeConfigFiles(string configFile, WppTargetsInfo info, string projectFolder, string solutionDir)
        {
            // The source config file is assumed placed in the ConfigFolder specified in the .wpp.targets file
            var sourcePath = _fileSystemFacade.CombinePath(projectFolder, info.ConfigFolder, configFile);

            // The tranformed config file is assumed placed in the root of the project
            var transformedPath = _fileSystemFacade.CombinePath(projectFolder, configFile);

            var answer = _prompter.Prompt(sourcePath, transformedPath);

            if (answer == DialogResult.No)
                return true;

            if (answer == DialogResult.Cancel)
                return false;

            if (answer != DialogResult.Yes)
                return false;

            var mergeResult = _merger.RunMerge(sourcePath, transformedPath, solutionDir);
            
            foreach (var result in mergeResult.Results)
                WriteProcessResultToVsWindow(result);

            return true;
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
