using System;
using NuGet.VisualStudio;

namespace ConfigSourceNupkgModPreserver.Implementation.VisualStudioFacade
{
    public class NuGetFacade
    {
        private readonly IVsPackageInstallerProjectEvents _packageInstallerProjectEvents;
        private string _currentBatchId;

        public NuGetFacade(IVsPackageInstallerProjectEvents packageInstallerProjectEvents)
        {
            _packageInstallerProjectEvents = packageInstallerProjectEvents;
        }

        /// <summary>
        /// Binds the callback to the BatchEnd event of IVsPackageInstallerProjectEvents (NuGet)
        /// More info: https://stackoverflow.com/questions/40478003/visual-studio-extension-event-after-nuget-install-uninstall
        /// https://docs.microsoft.com/nb-no/nuget/visual-studio-extensibility/nuget-api-in-visual-studio
        /// </summary>
        /// <param name="callback"></param>
        public void BindNuGetPackageEvents(Action callback)
        {
            _packageInstallerProjectEvents.BatchStart += projectMetadata =>
            {
                _currentBatchId = projectMetadata.BatchId;
            };

            _packageInstallerProjectEvents.BatchEnd += projectMetadata =>
            {
                if (_currentBatchId != projectMetadata.BatchId)
                    return;

                callback?.Invoke();
            };
        }
    }
}
