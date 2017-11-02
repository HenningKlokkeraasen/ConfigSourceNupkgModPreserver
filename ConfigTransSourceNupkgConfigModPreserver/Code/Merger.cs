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

        public void RunMerge(string sourceConfigRelativePath, string transformedConfigRelativePath, string solutionName)
        {
            var result = _visualStudioIntegrator.PromptUser(
                "Merge potentially transformed web.config back to source web.config?", 
                "File paths are configured under Tools -> Options -> Extensions");
            if (result != DialogResult.Yes)
                return;

            const string tempFileName = "temp.web.config";
            var sourceFileName = sourceConfigRelativePath;
            var modifiedFileName = transformedConfigRelativePath;
            var processExitCode1 = WindowsProcessIntegrator.RunCommand("copy", $"NUL {tempFileName}", solutionName);
            var processExitCode2AndMessage = WindowsProcessIntegrator.RunProcess("git.exe", $"merge-file {sourceFileName} {tempFileName} {modifiedFileName}", solutionName);
            var processExitCode3 = WindowsProcessIntegrator.RunCommand("del", tempFileName, solutionName);

            if (!processExitCode2AndMessage.Item2.Equals(string.Empty))
                _visualStudioIntegrator.WriteToDebugPane(processExitCode2AndMessage.Item2);
        }
    }
}
