namespace ConfigSourceNupkgModPreserver.Contracts.Merging
{
    public interface IMerger
    {
        MergeResult RunMerge(string sourceFileRelativePath, string transformedFileRelativePath, string solutionDir);
    }
}