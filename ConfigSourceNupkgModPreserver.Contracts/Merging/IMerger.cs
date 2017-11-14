namespace ConfigSourceNupkgModPreserver.Contracts.Merging
{
    public interface IMerger
    {
        void RunMerge(string sourceFileRelativePath, string transformedFileRelativePath, string solutionDir);
    }
}