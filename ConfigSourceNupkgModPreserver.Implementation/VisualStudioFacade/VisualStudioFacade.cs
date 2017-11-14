using System;
using ConfigSourceNupkgModPreserver.Contracts.VisualStudioFacade;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace ConfigSourceNupkgModPreserver.Implementation.VisualStudioFacade
{
    public class VisualStudioFacade : IVisualStudioFacade
    {
        private readonly IVsUIShell _vsUiShell;
        private readonly IVsOutputWindow _vsOutputWindow;

        public VisualStudioFacade(IVsUIShell shell, IVsOutputWindow outputWindow)
        {
            _vsUiShell = shell;
            _vsOutputWindow = outputWindow;
        }

        /// <summary>
        /// Forces a Yes/No dialog prompt to the VS user
        /// </summary>
        /// <returns>An int defined in a DialogResult const</returns>
        public int PromptUser(string title, string message)
        {
            var clsid = Guid.Empty;
            int result;
            ErrorHandler.ThrowOnFailure(_vsUiShell.ShowMessageBox(
                0,
                ref clsid,
                title,
                message,
                string.Empty,
                0,
                OLEMSGBUTTON.OLEMSGBUTTON_YESNO,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                OLEMSGICON.OLEMSGICON_QUERY,
                0,
                out result));
            return result;
        }

        public void WriteToDebugPane(string text) => WriteToPane(text, VSConstants.GUID_OutWindowDebugPane);

        public void WriteToGeneralPane(string text) => WriteToPane(text, VSConstants.GUID_OutWindowGeneralPane);

        private void WriteToPane(string text, Guid paneGuid)
        {
            IVsOutputWindowPane pane;
            _vsOutputWindow.GetPane(ref paneGuid, out pane);

            pane.OutputString(text);
            pane.Activate();
        }
    }
}
