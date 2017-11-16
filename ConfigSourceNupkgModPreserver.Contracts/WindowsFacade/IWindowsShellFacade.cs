using ConfigSourceNupkgModPreserver.Contracts.Merging;

namespace ConfigSourceNupkgModPreserver.Contracts.WindowsFacade
{
    public interface IWindowsShellFacade
    {
        ProcessResult RunCommand(string command, string arguments, string workingDirectory);
        ProcessResult RunProcess(string processFileName, string arguments, string workingDirectory);
    }
}