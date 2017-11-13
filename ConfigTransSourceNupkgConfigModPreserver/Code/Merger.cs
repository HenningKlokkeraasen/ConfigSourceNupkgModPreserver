using Microsoft.Internal.VisualStudio.PlatformUI;

namespace ConfigTransSourceNupkgConfigModPreserver.Code
{
    public class Merger
    {
        private readonly VisualStudioIntegrator _visualStudioIntegrator;

        public Merger(VisualStudioIntegrator visualStudioIntegrator)
        {
            _visualStudioIntegrator = visualStudioIntegrator;
        }

        public void RunMerge(string sourceConfigRelativePath, string transformedConfigRelativePath, string solutionDir)
        {
            var result = _visualStudioIntegrator.PromptUser(
                "Merge potentially transformed web.config back to source web.config?", 
                $"Transformed web.config: \n\t{transformedConfigRelativePath}\n\n" +
                $"Source web.config: \n\t{sourceConfigRelativePath}");
            if (result != DialogResult.Yes)
                return;

            const string tempFileName = "temp.web.config";
            var sourceFileName = sourceConfigRelativePath;
            var modifiedFileName = transformedConfigRelativePath;
            var processExitCode1 = WindowsProcessIntegrator.RunCommand("copy", $"NUL {tempFileName}", solutionDir);
            var processExitCode2AndMessage = WindowsProcessIntegrator.RunProcess("git.exe", $"merge-file {sourceFileName} {tempFileName} {modifiedFileName}", solutionDir);
            var processExitCode3 = WindowsProcessIntegrator.RunCommand("del", tempFileName, solutionDir);

            if (!processExitCode2AndMessage.Item2.Equals(string.Empty))
                _visualStudioIntegrator.WriteToDebugPane(processExitCode2AndMessage.Item2);
        }
    }
}
