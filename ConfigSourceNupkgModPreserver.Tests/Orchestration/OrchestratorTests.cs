using System.Linq;
using ConfigSourceNupkgModPreserver.Contracts.Merging;
using ConfigSourceNupkgModPreserver.Contracts.WindowsFacade;
using ConfigSourceNupkgModPreserver.Contracts.WppTargetsFileHandling;
using ConfigSourceNupkgModPreserver.Implementation.Orchestration;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace ConfigSourceNupkgModPreserver.Tests.Orchestration
{
    [TestFixture]
    public class OrchestratorTests
    {
        private readonly Fixture _fixture = new Fixture();
        private Mock<IFileSystemFacade> _fileSystemFacade;
        private Mock<IWppTargetsFilesReader> _wppTargetsFilesReader;
        private Mock<IWppTargetsXmlParser> _wppTargetsXmlParser;
        private Mock<IMerger> _merger;

        [Test]
        public void MergesEachConfigFile()
        {
            _fileSystemFacade = new Mock<IFileSystemFacade>();
            _wppTargetsFilesReader = new Mock<IWppTargetsFilesReader>();
            _wppTargetsXmlParser = new Mock<IWppTargetsXmlParser>();
            _merger = new Mock<IMerger>();

            // Setup solution dir
            var solutionName = _fixture.Create<string>();
            var solutionDir = _fixture.Create<string>();
            _fileSystemFacade.Setup(m => m.GetDirectoryName(solutionName)).Returns(solutionDir);

            // Setup .wpp.targets files
            var wppTargetsFiles = _fixture.CreateMany<string>(2);
            _wppTargetsFilesReader.Setup(m => m.GetWppTargetsFiles(solutionDir)).Returns(wppTargetsFiles);

            // Setup first project (directory)
            var xml1 = _fixture.Create<string>();
            var info1 = new WppTargetsInfo
            {
                ConfigFolder = _fixture.Create<string>(),
                ConfigFiles = _fixture.CreateMany<string>(2)
            };
            var dir1 = _fixture.Create<string>();
            _fileSystemFacade.Setup(m => m.ReadAllText(wppTargetsFiles.First())).Returns(xml1);
            _wppTargetsXmlParser.Setup(m => m.GetInfo(xml1)).Returns(info1);
            _fileSystemFacade.Setup(m => m.GetDirectoryName(wppTargetsFiles.First())).Returns(dir1);

            // Setup second project (directory)
            var xml2 = _fixture.Create<string>();
            var info2 = new WppTargetsInfo
            {
                ConfigFolder = _fixture.Create<string>(),
                ConfigFiles = _fixture.CreateMany<string>(2)
            };
            var dir2 = _fixture.Create<string>();
            _fileSystemFacade.Setup(m => m.ReadAllText(wppTargetsFiles.Last())).Returns(xml2);
            _wppTargetsXmlParser.Setup(m => m.GetInfo(xml2)).Returns(info2);
            _fileSystemFacade.Setup(m => m.GetDirectoryName(wppTargetsFiles.Last())).Returns(dir2);

            // Setup first project first config file
            var source1a = _fixture.Create<string>();
            var trans1a = _fixture.Create<string>();
            _fileSystemFacade.Setup(m => m.CombinePath(dir1, info1.ConfigFolder, info1.ConfigFiles.First())).Returns(source1a);
            _fileSystemFacade.Setup(m => m.CombinePath(dir1, info1.ConfigFiles.First())).Returns(trans1a);

            // Setup first project second config file
            var source1b = _fixture.Create<string>();
            var trans1b = _fixture.Create<string>();
            _fileSystemFacade.Setup(m => m.CombinePath(dir1, info1.ConfigFolder, info1.ConfigFiles.Last())).Returns(source1b);
            _fileSystemFacade.Setup(m => m.CombinePath(dir1, info1.ConfigFiles.Last())).Returns(trans1b);

            // Setup second project first config file
            var source2a = _fixture.Create<string>();
            var trans2a = _fixture.Create<string>();
            _fileSystemFacade.Setup(m => m.CombinePath(dir2, info2.ConfigFolder, info2.ConfigFiles.First())).Returns(source2a);
            _fileSystemFacade.Setup(m => m.CombinePath(dir2, info2.ConfigFiles.First())).Returns(trans2a);

            // Setup second project first second file
            var source2b = _fixture.Create<string>();
            var trans2b = _fixture.Create<string>();
            _fileSystemFacade.Setup(m => m.CombinePath(dir2, info2.ConfigFolder, info2.ConfigFiles.Last())).Returns(source2b);
            _fileSystemFacade.Setup(m => m.CombinePath(dir2, info2.ConfigFiles.Last())).Returns(trans2b);

            var sut = new Orchestrator(_fileSystemFacade.Object, _wppTargetsFilesReader.Object, _wppTargetsXmlParser.Object, _merger.Object);

            sut.RunMerge(solutionName);

            _merger.Verify(m => m.RunMerge(source1a, trans1a, solutionDir), Times.Once);
            _merger.Verify(m => m.RunMerge(source1b, trans1b, solutionDir), Times.Once);
            _merger.Verify(m => m.RunMerge(source2a, trans2a, solutionDir), Times.Once);
            _merger.Verify(m => m.RunMerge(source2b, trans2b, solutionDir), Times.Once);
        }
    }
}
