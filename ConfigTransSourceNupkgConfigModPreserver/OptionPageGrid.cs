using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace ConfigTransSourceNupkgConfigModPreserver
{
    /// <inheritdoc />
    /// <summary>
    /// VS-wide options. Improvement: should be possible to set per solution, since different solutions might have different setup.
    /// </summary>
    internal class OptionPageGrid : DialogPage
    {
        [Category("Config Trans Source Nupkg Config Mod Preserver")]
        [DisplayName("Source config file relative path")]
        [Description("This is the source config file, that will have unhandled Nupkg modifications merged into it")]
        public string SourceConfigRelativePath { get; set; } = "MyProject\\Configurations\\web.config";

        [Category("Config Trans Source Nupkg Config Mod Preserver")]
        [DisplayName("Transformed config file relative path")]
        [Description("This is the transformed config file, that NuGet packages modify")]
        public string TransformedConfigRelativePath { get; set; } = "MyProject\\web.config";
    }
}
