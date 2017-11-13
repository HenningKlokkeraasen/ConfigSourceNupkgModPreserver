using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using ConfigTransSourceNupkgConfigModPreserver.Code;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using NuGet.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

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
    public sealed class ConfigTransSourceNupkgConfigModPreserver : Package
    {
        private IVsPackageInstallerProjectEvents _packageInstallerProjectEvents;
        private IVsUIShell _vsUiShell;
        private DTE _dte;
        private IVsOutputWindow _vsOutputWindow;
        private Merger _merger;

        public const string PackageGuidString = "36fa07a8-d764-4bbc-93af-858e6584bea8";
        
        protected override void Initialize()
        {
            base.Initialize();

            var componentModel = (IComponentModel)GetService(typeof(SComponentModel));
            _packageInstallerProjectEvents = componentModel.GetService<IVsPackageInstallerProjectEvents>();
            _vsUiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            _vsOutputWindow = GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            _dte = (DTE)GetService(typeof(DTE));

            var visualStudioIntegrator = new VisualStudioIntegrator(_vsUiShell, _vsOutputWindow);
            _merger = new Merger(visualStudioIntegrator);
            var nuGetIntegrator = new NuGetIntegrator(_packageInstallerProjectEvents);

            nuGetIntegrator.BindNuGetPackageEvents(RunMerge);
        }

        private void RunMerge()
        {
            var fileSystemIntegrator = new FileSystemIntegrator();
            var wppTargetsFilesReader = new WppTargetsFilesReader(fileSystemIntegrator);
            var wppTargetsFileParser = new WppTargetsXmlParser();

            var solutionDir = fileSystemIntegrator.GetDirectoryName(_dte.Solution.FullName);

            var wppTargetsFiles = wppTargetsFilesReader.GetWppTargetsFiles(solutionDir);

            foreach (var wppTargetsFile in wppTargetsFiles)
            {
                var xml = fileSystemIntegrator.ReadAllText(wppTargetsFile);
                var configFolder = wppTargetsFileParser.GetConfigFolder(xml);

                var projectFolder = fileSystemIntegrator.GetDirectoryName(wppTargetsFile);

                var sourceWebConfigPath = fileSystemIntegrator.CombinePath(projectFolder, configFolder, "web.config");
                var transformedWebConfigPath = fileSystemIntegrator.CombinePath(projectFolder, "web.config");

                _merger.RunMerge(sourceWebConfigPath, transformedWebConfigPath, solutionDir);
            }
        }
    }
}