namespace ConfigSourceNupkgModPreserver.Contracts.VisualStudioFacade
{
    public interface IVisualStudioFacade
    {
        int PromptUser(string title, string message);
        void WriteToDebugPane(string text);
        void WriteToGeneralPane(string text);
    }
}