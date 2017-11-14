using System.Linq;
using ConfigSourceNupkgModPreserver.Implementation.WppTargetsFileHandling;
using FluentAssertions;
using NUnit.Framework;

namespace ConfigSourceNupkgModPreserver.Tests.WppTargetsFileHandling
{
    [TestFixture]
    public class WppTargetsXmlParserTests
    {
        private const string Input = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>"
                                     + "<Project ToolsVersion=\"4.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">"
                                     + "<PropertyGroup>"
                                     + "<ConfigFolder>Configurations</ConfigFolder>"
                                     + "</PropertyGroup>"
                                     + "<ItemGroup>"
                                     + "<ConfigName Include=\"Web\"><Ext>config</Ext></ConfigName>"
                                     + "<ConfigName Include=\"ConnectionStrings\"><Ext>config</Ext></ConfigName>"
                                     + "</ItemGroup>"
                                     + "</Project> ";

        /// <summary>
        /// Tests correct XML parsing - dependes on LINQ to XML
        /// </summary>
        [Test]
        public void GrabsConfigFolder()
        {
            var sut = new WppTargetsXmlParser();
            
            var output = sut.GetInfo(Input);

            output.ConfigFolder.Should().BeEquivalentTo("Configurations");
        }

        /// <summary>
        /// Tests correct XML parsing - dependes on LINQ to XML
        /// </summary>
        [Test]
        public void GrabsConfigFiles()
        {
            var sut = new WppTargetsXmlParser();

            var output = sut.GetInfo(Input);

            output.ConfigFiles.First().Should().BeEquivalentTo("Web.config");
            output.ConfigFiles.Last().Should().BeEquivalentTo("ConnectionStrings.config");
        }
    }
}
