using System.Collections.Generic;
using System.Xml.Linq;
using ConfigSourceNupkgModPreserver.Contracts.WppTargetsFileHandling;

namespace ConfigSourceNupkgModPreserver.Implementation.WppTargetsFileHandling
{
    public class WppTargetsXmlParser : IWppTargetsXmlParser
    {
        private static XNamespace NamespaceMsBuild => "http://schemas.microsoft.com/developer/msbuild/2003";
        private const string ElementPropertyGroup = "PropertyGroup";
        private const string ElementConfigFolder = "ConfigFolder";
        private const string ElementProject = "Project";
        private const string ElementItemGroup = "ItemGroup";
        private const string ElementConfigName = "ConfigName";
        private const string AttributeInclude = "Include";
        private const string ElementExt = "Ext";

        /// <summary>
        /// Parses an XML document specified by 
        /// http://schemas.microsoft.com/developer/msbuild/2003.
        /// The XML document should have the structure 
        /// <Project>
        ///     <PropertyGroup>
        ///         <ConfigFolder>[string]</ConfigFolder>
        ///     </PropertyGroup>
        ///     <ItemGroup>
        ///         <ConfigName Include="Web">
        ///             <Ext>config</Ext>
        ///         </ConfigName>
        ///     </ItemGroup>
        /// </Project>
        /// </summary>
        /// <returns>The value of the <ConfigFolder></ConfigFolder> elment</returns>
        public WppTargetsInfo GetInfo(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return WppTargetsInfo.Empty;

            var projectElement = XDocument.Parse(xml)
                .Element(NamespaceMsBuild + ElementProject);

            var configFolder = projectElement?
                .Element(NamespaceMsBuild + ElementPropertyGroup)?
                .Element(NamespaceMsBuild + ElementConfigFolder);

            if (configFolder == null)
                return WppTargetsInfo.Empty;

            var configNameElements = projectElement
                .Element(NamespaceMsBuild + ElementItemGroup)?
                .Elements(NamespaceMsBuild + ElementConfigName);

            if (configNameElements == null)
                return WppTargetsInfo.Empty;

            var configFiles = new List<string>();
            foreach (var configNameElement in configNameElements)
            {
                var name = configNameElement.Attribute(AttributeInclude)?.Value;
                var extension = configNameElement.Element(NamespaceMsBuild + ElementExt)?.Value;
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(extension))
                    continue;

                configFiles.Add($"{name}.{extension}");
            }

            return new WppTargetsInfo
            {
                ConfigFolder = configFolder.Value,
                ConfigFiles = configFiles
            };
        }
    }
}