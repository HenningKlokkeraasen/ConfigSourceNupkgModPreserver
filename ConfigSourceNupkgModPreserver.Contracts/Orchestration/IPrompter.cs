namespace ConfigSourceNupkgModPreserver.Contracts.Orchestration
{
    public interface IPrompter
    {
        int Prompt(string sourceFile, string targetFile);
    }
}
