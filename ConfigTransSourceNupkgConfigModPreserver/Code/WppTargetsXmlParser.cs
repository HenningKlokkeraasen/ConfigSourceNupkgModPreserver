using System.Xml.Linq;

namespace ConfigTransSourceNupkgConfigModPreserver.Code
{
    public class WppTargetsXmlParser
    {
        private static XNamespace NamespaceMsBuild => "http://schemas.microsoft.com/developer/msbuild/2003";
        private const string ElementPropertyGroup = "PropertyGroup";
        private const string ElementConfigFolder = "ConfigFolder";
        private const string ElementProject = "Project";

        /// <summary>
        /// Parses an XML document specified by 
        /// http://schemas.microsoft.com/developer/msbuild/2003.
        /// The XML document should have the structure 
        /// <Project>
        ///     <PropertyGroup>
        ///         <ConfigFolder>[string]</ConfigFolder>
        ///     </PropertyGroup>
        /// </Project>
        /// </summary>
        /// <returns>The value of the <ConfigFolder></ConfigFolder> elment</returns>
        public string GetConfigFolder(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return string.Empty;

            var configFolder = XDocument.Parse(xml)
                .Element(NamespaceMsBuild + ElementProject)?
                .Element(NamespaceMsBuild + ElementPropertyGroup)?
                .Element(NamespaceMsBuild + ElementConfigFolder);
            
            if (configFolder == null)
                return string.Empty;

            return configFolder.Value;
        }
    }
}