using ConfigSourceNupkgModPreserver.Contracts.Orchestration;
using ConfigSourceNupkgModPreserver.Contracts.VisualStudioFacade;

namespace ConfigSourceNupkgModPreserver.Implementation.Orchestration
{
    public class Prompter : IPrompter
    {
        private readonly IVisualStudioFacade _vsFacade;

        public Prompter(IVisualStudioFacade vsFacade)
        {
            _vsFacade = vsFacade;
        }

        public int Prompt(string sourceFileRelativePath, string transformedFileRelativePath)
        {
            var result = _vsFacade.PromptUser(
                "Merge potentially modified transform config back to source config?",
                $"Transform config: \n{transformedFileRelativePath}\n\n" +
                $"Source config: \n{sourceFileRelativePath}");
            return result;
        }
    }
}