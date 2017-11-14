using ConfigSourceNupkgModPreserver.Contracts.Merging;
using ConfigSourceNupkgModPreserver.Contracts.VisualStudioFacade;
using ConfigSourceNupkgModPreserver.Contracts.WindowsFacade;
using Microsoft.Internal.VisualStudio.PlatformUI;

namespace ConfigSourceNupkgModPreserver.Implementation.Merging
{
    public class Merger : IMerger
    {
        public const string TempFileName = "temp.web.config";

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
        public void RunMerge(string sourceFileRelativePath, string transformedFileRelativePath, string solutionDir)
        {
            var result = _visualStudioFacade.PromptUser(
                "Merge potentially transformed web.config back to source web.config?", 
                $"Transformed web.config: \n\t{transformedFileRelativePath}\n\n" +
                $"Source web.config: \n\t{sourceFileRelativePath}");
            if (result != DialogResult.Yes)
                return;

            var processExitCode1 = _windowsShellFacade.RunCommand("copy", $"NUL {TempFileName}", solutionDir);
            var processExitCode2AndMessage = _windowsShellFacade.RunProcess("git.exe", $"merge-file {sourceFileRelativePath} {TempFileName} {transformedFileRelativePath}", solutionDir);
            var processExitCode3 = _windowsShellFacade.RunCommand("del", TempFileName, solutionDir);

            if (processExitCode2AndMessage != null && !processExitCode2AndMessage.Item2.Equals(string.Empty))
                _visualStudioFacade.WriteToDebugPane(processExitCode2AndMessage.Item2);
        }
    }
}
