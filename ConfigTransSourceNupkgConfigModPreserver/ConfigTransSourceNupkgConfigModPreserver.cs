using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using NuGet.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Process = System.Diagnostics.Process;

namespace ConfigTransSourceNupkgConfigModPreserver
{
    /// <inheritdoc />
    /// <summary>
    /// Implements the package exposed by this assembly.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(ConfigTransSourceNupkgConfigModPreserver.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [ProvideOptionPage(typeof(OptionPageGrid), "Extensions", "Config Trans Source Nupkg Config Mod Preserver", 0, 0, true)]
    public sealed class ConfigTransSourceNupkgConfigModPreserver : Package
    {
        private IVsPackageInstallerProjectEvents _packageInstallerProjectEvents;
        private IVsUIShell _vsUiShell;
        private string _currentBatchId;
        
        public const string PackageGuidString = "36fa07a8-d764-4bbc-93af-858e6584bea8";
        
        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            var componentModel = (IComponentModel)GetService(typeof(SComponentModel));
            _packageInstallerProjectEvents = componentModel.GetService<IVsPackageInstallerProjectEvents>();
            _vsUiShell = (IVsUIShell)GetService(typeof(SVsUIShell));

            BindNuGetPackageEvents();
        }

        private void BindNuGetPackageEvents()
        {
            _packageInstallerProjectEvents.BatchStart += projectMetadata =>
            {
                _currentBatchId = projectMetadata.BatchId;
            };
            
            _packageInstallerProjectEvents.BatchEnd += projectMetadata =>
            {
                if (_currentBatchId != projectMetadata.BatchId)
                    return;

                var result = PromptUser();
                if (result != DialogResult.Yes)
                    return;

                const string tempFileName = "temp.web.config";
                var sourceFileName = SourceConfigRelativePath;
                var modifiedFileName = TransformedConfigRelativePath;
                RunCommand("copy", $"NUL {tempFileName}");
                RunProcess("git.exe", $"merge-file {sourceFileName} {tempFileName} {modifiedFileName}");
                RunCommand("del", tempFileName);
            };
        }

        private int PromptUser()
        {
            var clsid = Guid.Empty;
            int result;
            ErrorHandler.ThrowOnFailure(_vsUiShell.ShowMessageBox(
                0,
                ref clsid,
                "Merge potentially transformed web.config back to source web.config?",
                string.Empty,
                string.Empty,
                0,
                OLEMSGBUTTON.OLEMSGBUTTON_YESNO,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                OLEMSGICON.OLEMSGICON_QUERY,
                0,
                out result));
            return result;
        }

        private void RunProcess(string processFileName, string arguments)
        {
            var processStartInfo = MakeProcessStartInfo(processFileName);
            processStartInfo.Arguments = arguments;

            var process = MakeProcess(processStartInfo);

            var stderrStr = process.StandardError.ReadToEnd();
            var stdoutStr = process.StandardOutput.ReadToEnd();

            process.WaitForExit();
            
            var exitCode = process.ExitCode;

            if (!stderrStr.Equals(string.Empty))
                WriteToWindow(stderrStr);
            if (!stdoutStr.Equals(string.Empty))
                WriteToWindow(stderrStr);

            process.Close();
        }

        private static void WriteToWindow(string text)
        {
            var outWindow = GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;

            var generalPaneGuid = VSConstants.GUID_OutWindowDebugPane; // GUID_OutWindowGeneralPane
            IVsOutputWindowPane generalPane;
            outWindow.GetPane(ref generalPaneGuid, out generalPane);

            generalPane.OutputString(text);
            generalPane.Activate();
        }

        private void RunCommand(string command, string arguments)
        {
            var processStartInfo = MakeProcessStartInfo("cmd.exe");
            processStartInfo.RedirectStandardInput = true;

            var process = MakeProcess(processStartInfo);

            process.StandardInput.WriteLine($"{command} {arguments}");
            process.StandardInput.Flush();
            process.StandardInput.Close();
            
            process.WaitForExit();

            var exitCode = process.ExitCode;

            Console.WriteLine(process.StandardOutput.ReadToEnd());
        }

        private static Process MakeProcess(ProcessStartInfo processStartInfo)
        {
            var process = new Process
            {
                StartInfo = processStartInfo
            };
            process.Start();
            return process;
        }

        private ProcessStartInfo MakeProcessStartInfo(string processFileName)
        {
            var processStartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = processFileName
            };

            var dte = (DTE) GetService(typeof(DTE));
            var solutionDir = Path.GetDirectoryName(dte.Solution.FullName);
            processStartInfo.WorkingDirectory = solutionDir;

            return processStartInfo;
        }

        public string SourceConfigRelativePath => OptionsPage.SourceConfigRelativePath;

        public string TransformedConfigRelativePath => OptionsPage.TransformedConfigRelativePath;

        private OptionPageGrid OptionsPage => (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
    }
}
