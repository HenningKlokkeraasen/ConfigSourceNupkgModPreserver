namespace ConfigSourceNupkgModPreserver.Contracts.Merging
{
    public class ProcessResult
    {
        public string NameOfProcessOrCommand { get; set; }
        public int ExitCode { get; set; }
        public string Error { get; set; }
        public string Output { get; set; }
    }
}