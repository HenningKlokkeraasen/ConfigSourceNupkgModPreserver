﻿using ConfigSourceNupkgModPreserver.Implementation.WppTargetsFileHandling;
using FluentAssertions;
using NUnit.Framework;

namespace ConfigSourceNupkgModPreserver.Tests.WppTargetsFileHandling
{
    [TestFixture]
    public class WppTargetsXmlParserTests
    {
        /// <summary>
        /// Tests correct XML parsing - dependes on LINQ to XML
        /// </summary>
        [Test]
        public void Test()
        {
            var input = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>"
                        + "<Project ToolsVersion=\"4.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">"
                        + "<PropertyGroup>"
                        + "<ConfigFolder>Configurations</ConfigFolder>"
                        + "</PropertyGroup>"
                        + "</Project> ";

            var sut = new WppTargetsXmlParser();
            
            var output = sut.GetConfigFolder(input);

            output.Should().BeEquivalentTo("Configurations");
        }
    }
}