using System;

namespace ConfigSourceNupkgModPreserver.Contracts.WindowsFacade
{
    public interface IWindowsShellFacade
    {
        int RunCommand(string command, string arguments, string workingDirectory);
        Tuple<int, string> RunProcess(string processFileName, string arguments, string workingDirectory);
    }
}