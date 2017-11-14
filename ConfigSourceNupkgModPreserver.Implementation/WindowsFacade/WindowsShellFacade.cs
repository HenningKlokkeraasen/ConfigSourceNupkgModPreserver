using System;
using System.Diagnostics;
using ConfigSourceNupkgModPreserver.Contracts.WindowsFacade;

namespace ConfigSourceNupkgModPreserver.Implementation.WindowsFacade
{
    public class WindowsShellFacade : IWindowsShellFacade
    {
        /// <summary>
        /// Runs a command (with optional arguments) in the workingDirectory,
        /// e.g. copy file.txt file.bak
        /// </summary>
        /// <returns>Process exit code</returns>
        public int RunCommand(string command, string arguments, string workingDirectory)
        {
            var processStartInfo = MakeProcessStartInfo("cmd.exe", workingDirectory);
            processStartInfo.RedirectStandardInput = true;

            var process = MakeProcess(processStartInfo);
            StartProcess(process);

            process.StandardInput.WriteLine($"{command} {arguments}");
            process.StandardInput.Flush();
            process.StandardInput.Close();

            var exitCode = Exit(process);

            Console.WriteLine(process.StandardOutput.ReadToEnd());

            return exitCode;
        }

        /// <summary>
        /// Runs a process (with optional arguments) in the workingDirectory
        /// e.g. git merge-file yours.txt base.txt theirs.txt
        /// </summary>
        /// <returns>Process exit code and STDERR or STDOUT message if applicable</returns>
        public Tuple<int, string> RunProcess(string processFileName, string arguments, string workingDirectory)
        {
            var processStartInfo = MakeProcessStartInfo(processFileName, workingDirectory);
            processStartInfo.Arguments = arguments;

            var process = MakeProcess(processStartInfo);
            StartProcess(process);

            var stderrStr = process.StandardError.ReadToEnd();
            var stdoutStr = process.StandardOutput.ReadToEnd();

            var exitCode = Exit(process);

            var message = !stderrStr.Equals(string.Empty)
                ? stderrStr
                : !stdoutStr.Equals(string.Empty)
                    ? stdoutStr
                    : string.Empty;

            process.Close();

            return new Tuple<int, string>(exitCode, message);
        }

        private static void StartProcess(Process process) => process.Start();

        private static int Exit(Process process)
        {
            process.WaitForExit();
            return process.ExitCode;
        }

        private static Process MakeProcess(ProcessStartInfo processStartInfo) => new Process
        {
            StartInfo = processStartInfo
        };

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
