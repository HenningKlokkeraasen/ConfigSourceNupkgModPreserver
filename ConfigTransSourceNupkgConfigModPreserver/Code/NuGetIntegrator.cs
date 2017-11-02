using System;
using NuGet.VisualStudio;

namespace ConfigTransSourceNupkgConfigModPreserver.Code
{
    public class NuGetIntegrator
    {
        private readonly IVsPackageInstallerProjectEvents _packageInstallerProjectEvents;
        private string _currentBatchId;

        public NuGetIntegrator(IVsPackageInstallerProjectEvents packageInstallerProjectEvents)
        {
            _packageInstallerProjectEvents = packageInstallerProjectEvents;
        }

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

                callback();
            };
        }
    }
}
