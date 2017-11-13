using System;
using System.Diagnostics;
using System.IO;

namespace ConfigTransSourceNupkgConfigModPreserver.Code
{
    public class WindowsProcessIntegrator
    {
        /// <summary>
        /// </summary>
        /// <returns>Process exit code</returns>
        public static int RunCommand(string command, string arguments, string solutionDirectory)
        {
            var processStartInfo = MakeProcessStartInfo("cmd.exe", solutionDirectory);
            processStartInfo.RedirectStandardInput = true;

            var process = MakeAndStartProcess(processStartInfo);

            process.StandardInput.WriteLine($"{command} {arguments}");
            process.StandardInput.Flush();
            process.StandardInput.Close();

            process.WaitForExit();

            var exitCode = process.ExitCode;

            Console.WriteLine(process.StandardOutput.ReadToEnd());

            return exitCode;
        }

        /// <summary>
        /// </summary>
        /// <returns>Process exit code and STDERR or STDOUT message if applicable</returns>
        public static Tuple<int, string> RunProcess(string processFileName, string arguments, string solutionDirectory)
        {
            var processStartInfo = MakeProcessStartInfo(processFileName, solutionDirectory);
            processStartInfo.Arguments = arguments;

            var process = MakeAndStartProcess(processStartInfo);

            var stderrStr = process.StandardError.ReadToEnd();
            var stdoutStr = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            var exitCode = process.ExitCode;

            var message = !stderrStr.Equals(string.Empty)
                ? stderrStr
                : !stdoutStr.Equals(string.Empty)
                    ? stdoutStr
                    : string.Empty;

            process.Close();

            return new Tuple<int, string>(exitCode, message);
        }

        private static Process MakeAndStartProcess(ProcessStartInfo processStartInfo)
        {
            var process = new Process
            {
                StartInfo = processStartInfo
            };
            process.Start();
            return process;
        }

        private static ProcessStartInfo MakeProcessStartInfo(string processFileName, string solutionDirectory)
        {
            var processStartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = processFileName,
                WorkingDirectory = solutionDirectory
            };
            return processStartInfo;
        }
    }
}
