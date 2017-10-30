//------------------------------------------------------------------------------
// <copyright file="ConfigTransSourceNupkgConfigModPreserver.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using NuGet.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

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
        private IVsPackageInstallerProjectEvents packageInstallerProjectEvents;
        private IVsPackageInstallerEvents packageInstallerEvents;
        
        private string currentBatchId;
        private Dictionary<string, string> packagesMetadata;
        
        /// <summary>
        /// ConfigTransSourceNupkgConfigModPreserver GUID string.
        /// </summary>
        public const string PackageGuidString = "36fa07a8-d764-4bbc-93af-858e6584bea8";

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigTransSourceNupkgConfigModPreserver"/> class.
        /// </summary>
        public ConfigTransSourceNupkgConfigModPreserver()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            var componentModel = (IComponentModel)GetService(typeof(SComponentModel));
            packageInstallerEvents = componentModel.GetService<IVsPackageInstallerEvents>();
            packageInstallerProjectEvents = componentModel.GetService<IVsPackageInstallerProjectEvents>();

            packagesMetadata = new Dictionary<string, string>();

            BindNuGetPackageEvents();
        }

        #endregion

        private void BindNuGetPackageEvents()
        {
            packageInstallerProjectEvents.BatchStart += (projectMetadata) =>
            {
                // preserve current batch id or project name to compare with batch end event
                currentBatchId = projectMetadata.BatchId;
                Console.WriteLine("Current Project Name : " + projectMetadata.ProjectName);
            };

            packageInstallerEvents.PackageInstalled += (metadata) =>
            {
                // package being insalled in current project
                // Save package metadata to use at batch end event
                if (packagesMetadata.ContainsKey(metadata.Id))
                    packagesMetadata[metadata.Id] = "installed";
                else
                    packagesMetadata.Add(metadata.Id, "installed");
            };

            packageInstallerEvents.PackageUninstalled += (metadata) =>
            {
                // package being uninstalled in current project
                // Save package metadata to use at batch end event
                if (packagesMetadata.ContainsKey(metadata.Id))
                    packagesMetadata[metadata.Id] = "uninstalled";
                else
                    packagesMetadata.Add(metadata.Id, "uninstalled");
            };

            packageInstallerProjectEvents.BatchEnd += (projectMetadata) =>
            {
                if (currentBatchId == projectMetadata.BatchId)
                {
                    // Now you can use your packages metadata saved during packageinstalled or packageuninstalled events
                    foreach (var packageName in packagesMetadata.Keys)
                    {
                        Console.WriteLine($"Package {packageName} was {packagesMetadata[packageName]}");
                    }
                }
            };
        }
    }
}
