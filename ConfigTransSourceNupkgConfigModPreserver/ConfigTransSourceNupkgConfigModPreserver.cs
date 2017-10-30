//------------------------------------------------------------------------------
// <copyright file="ConfigTransSourceNupkgConfigModPreserver.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using NuGet.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Internal.VisualStudio.PlatformUI;

namespace ConfigTransSourceNupkgConfigModPreserver
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(ConfigTransSourceNupkgConfigModPreserver.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    public sealed class ConfigTransSourceNupkgConfigModPreserver : Package
    {
        private IVsPackageInstallerProjectEvents _packageInstallerProjectEvents;
        private IVsUIShell _vsUiShell;
        private string _currentBatchId;

        /// <summary>
        /// ConfigTransSourceNupkgConfigModPreserver GUID string.
        /// </summary>
        public const string PackageGuidString = "36fa07a8-d764-4bbc-93af-858e6584bea8";
        
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            var componentModel = (IComponentModel)GetService(typeof(SComponentModel));
            _packageInstallerProjectEvents = componentModel.GetService<IVsPackageInstallerProjectEvents>();
            _vsUiShell = (IVsUIShell)GetService(typeof(SVsUIShell));

            BindNuGetPackageEvents();
        }

        #endregion

        private void BindNuGetPackageEvents()
        {
            _packageInstallerProjectEvents.BatchStart += projectMetadata =>
            {
                // preserve current batch id or project name to compare with batch end event
                _currentBatchId = projectMetadata.BatchId;
            };
            
            _packageInstallerProjectEvents.BatchEnd += projectMetadata =>
            {
                if (_currentBatchId == projectMetadata.BatchId)
                {
                    var result = PromptUser();
                    if (result == DialogResult.Yes)
                    {
                        // ...
                    }                    
                }
            };
        }

        private int PromptUser()
        {
            var clsid = Guid.Empty;
            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(_vsUiShell.ShowMessageBox(
                0,
                ref clsid,
                "Merge web.config back to source web.config?",
                "...",
                string.Empty,
                0,
                OLEMSGBUTTON.OLEMSGBUTTON_YESNO,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                OLEMSGICON.OLEMSGICON_QUERY,
                0,
                out result));
            return result;
        }
    }
}
