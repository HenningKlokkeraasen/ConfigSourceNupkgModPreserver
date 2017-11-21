using System.Collections.Generic;
using ConfigSourceNupkgModPreserver.Contracts.Merging;
using ConfigSourceNupkgModPreserver.Contracts.WindowsFacade;

namespace ConfigSourceNupkgModPreserver.Implementation.Merging
{
    public class Merger : IMerger
    {
        public const string TempFileName = "configsourcenupkgmodpreserver.temp.config";

        private readonly IWindowsShellFacade _windowsShellFacade;

        public Merger(IWindowsShellFacade windowsShellFacade)
        {
            _windowsShellFacade = windowsShellFacade;
        }

        /// <summary>
        /// Runs git merge-file
        /// </summary>
        public MergeResult RunMerge(string sourceFileRelativePath, string transformedFileRelativePath, string solutionDir)
        {
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
