using System.Collections.Generic;
using ConfigSourceNupkgModPreserver.Contracts.Merging;
using ConfigSourceNupkgModPreserver.Contracts.VisualStudioFacade;
using ConfigSourceNupkgModPreserver.Contracts.WindowsFacade;
using Microsoft.Internal.VisualStudio.PlatformUI;

namespace ConfigSourceNupkgModPreserver.Implementation.Merging
{
    public class Merger : IMerger
    {
        public const string TempFileName = "configsourcenupkgmodpreserver.temp.config";

        private readonly IVisualStudioFacade _visualStudioFacade;
        private readonly IWindowsShellFacade _windowsShellFacade;

        public Merger(IVisualStudioFacade visualStudioFacade, IWindowsShellFacade windowsShellFacade)
        {
            _visualStudioFacade = visualStudioFacade;
            _windowsShellFacade = windowsShellFacade;
        }

        /// <summary>
        /// Prompts the VS user if git merge-file should be run, 
        /// and runs the git command if the answer is "yes".
        /// </summary>
        public MergeResult RunMerge(string sourceFileRelativePath, string transformedFileRelativePath, string solutionDir)
        {
            var result = _visualStudioFacade.PromptUser(
                "Merge potentially modified transform config back to source config?", 
                $"Transform config: \n{transformedFileRelativePath}\n\n" +
                $"Source config: \n{sourceFileRelativePath}");
            if (result != DialogResult.Yes)
                return MergeResult.Empty;

            var resultOfCopy = _windowsShellFacade.RunCommand("copy", $"NUL {TempFileName}", solutionDir);
            var resultOfMergeFile = _windowsShellFacade.RunProcess("git.exe", $"merge-file {sourceFileRelativePath} {TempFileName} {transformedFileRelativePath}", solutionDir);
            var resultOfDel = _windowsShellFacade.RunCommand("del", TempFileName, solutionDir);

            return new MergeResult
            {
                Results = new List<ProcessResult>
                {
                    resultOfCopy,
                    resultOfMergeFile,
                    resultOfDel
                }
            };
        }
    }
}
