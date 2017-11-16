using System.Diagnostics;
using ConfigSourceNupkgModPreserver.Contracts.Merging;
using ConfigSourceNupkgModPreserver.Contracts.WindowsFacade;

namespace ConfigSourceNupkgModPreserver.Implementation.WindowsFacade
{
    public class WindowsShellFacade : IWindowsShellFacade
    {
        /// <summary>
        /// Runs a command (with optional arguments) in the workingDirectory,
        /// e.g. copy file.txt file.bak
        /// </summary>
        public ProcessResult RunCommand(string command, string arguments, string workingDirectory)
        {
            var processStartInfo = MakeProcessStartInfo("cmd.exe", workingDirectory);
            processStartInfo.RedirectStandardInput = true;

            var process = Start(processStartInfo);

            process.StandardInput.WriteLine($"{command} {arguments}");
            process.StandardInput.Flush();
            process.StandardInput.Close();
            
            return Finish(process, command);
        }

        /// <summary>
        /// Runs a process (with optional arguments) in the workingDirectory
        /// e.g. git merge-file yours.txt base.txt theirs.txt
        /// </summary>
        public ProcessResult RunProcess(string processFileName, string arguments, string workingDirectory)
        {
            var processStartInfo = MakeProcessStartInfo(processFileName, workingDirectory);
            processStartInfo.Arguments = arguments;

            var process = Start(processStartInfo);
            
            return Finish(process, processFileName);
        }

        private static ProcessResult Finish(Process process, string processFileName)
        {
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            process.WaitForExit();
            var exitCode = process.ExitCode;

            process.Close();

            return new ProcessResult
            {
                NameOfProcessOrCommand = processFileName,
                ExitCode = exitCode,
                Error = error,
                Output = output
            };
        }

        private static Process Start(ProcessStartInfo processStartInfo)
        {
            var process = new Process
            {
                StartInfo = processStartInfo
            };
            process.Start();
            return process;
        }

        private static ProcessStartInfo MakeProcessStartInfo(string processFileName, string workingDirectory) => new ProcessStartInfo
        {
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            FileName = processFileName,
            WorkingDirectory = workingDirectory
        };
    }
}
