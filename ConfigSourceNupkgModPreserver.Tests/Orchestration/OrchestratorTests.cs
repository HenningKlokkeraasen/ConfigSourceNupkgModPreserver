using System.Collections.Generic;
using System.Linq;
using ConfigSourceNupkgModPreserver.Contracts.Merging;
using ConfigSourceNupkgModPreserver.Contracts.Orchestration;
using ConfigSourceNupkgModPreserver.Contracts.VisualStudioFacade;
using ConfigSourceNupkgModPreserver.Contracts.WindowsFacade;
using ConfigSourceNupkgModPreserver.Contracts.WppTargetsFileHandling;
using ConfigSourceNupkgModPreserver.Implementation.Orchestration;
using Microsoft.Internal.VisualStudio.PlatformUI;
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
        private Mock<IVisualStudioFacade> _vsFacadeMock;
        private Mock<IPrompter> _prompterMock;
        private string _solutionName;
        private string _solutionDir;
        private string _source1A;
        private string _trans1A;
        private string _source1B;
        private string _trans1B;
        private string _source2A;
        private string _trans2A;
        private string _source2B;
        private string _trans2B;

        [SetUp]
        public void SetUp()
        {
            _fileSystemFacade = new Mock<IFileSystemFacade>();
            _wppTargetsFilesReader = new Mock<IWppTargetsFilesReader>();
            _wppTargetsXmlParser = new Mock<IWppTargetsXmlParser>();
            _merger = new Mock<IMerger>();
            _vsFacadeMock = new Mock<IVisualStudioFacade>();
            _prompterMock = new Mock<IPrompter>();

            _merger.Setup(m => m.RunMerge(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new MergeResult
                {
                    Results = new List<ProcessResult>
                    {
                        new ProcessResult()
                    }
                });

            // Setup solution dir
            _solutionName = _fixture.Create<string>();
            _solutionDir = _fixture.Create<string>();
            _fileSystemFacade.Setup(m => m.GetDirectoryName(_solutionName)).Returns(_solutionDir);

            // Setup .wpp.targets files
            var wppTargetsFiles = _fixture.CreateMany<string>(2);
            _wppTargetsFilesReader.Setup(m => m.GetWppTargetsFiles(_solutionDir)).Returns(wppTargetsFiles);

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
            _source1A = _fixture.Create<string>();
            _trans1A = _fixture.Create<string>();
            _fileSystemFacade.Setup(m => m.CombinePath(dir1, info1.ConfigFolder, info1.ConfigFiles.First())).Returns(_source1A);
            _fileSystemFacade.Setup(m => m.CombinePath(dir1, info1.ConfigFiles.First())).Returns(_trans1A);

            // Setup first project second config file
            _source1B = _fixture.Create<string>();
            _trans1B = _fixture.Create<string>();
            _fileSystemFacade.Setup(m => m.CombinePath(dir1, info1.ConfigFolder, info1.ConfigFiles.Last())).Returns(_source1B);
            _fileSystemFacade.Setup(m => m.CombinePath(dir1, info1.ConfigFiles.Last())).Returns(_trans1B);

            // Setup second project first config file
            _source2A = _fixture.Create<string>();
            _trans2A = _fixture.Create<string>();
            _fileSystemFacade.Setup(m => m.CombinePath(dir2, info2.ConfigFolder, info2.ConfigFiles.First())).Returns(_source2A);
            _fileSystemFacade.Setup(m => m.CombinePath(dir2, info2.ConfigFiles.First())).Returns(_trans2A);

            // Setup second project first second file
            _source2B = _fixture.Create<string>();
            _trans2B = _fixture.Create<string>();
            _fileSystemFacade.Setup(m => m.CombinePath(dir2, info2.ConfigFolder, info2.ConfigFiles.Last())).Returns(_source2B);
            _fileSystemFacade.Setup(m => m.CombinePath(dir2, info2.ConfigFiles.Last())).Returns(_trans2B);
        }

        [Test]
        public void When_yes_is_returned_merges_config_file()
        {
            _prompterMock.Setup(m => m.Prompt(_source1A, _trans1A)).Returns(DialogResult.Yes);
            _prompterMock.Setup(m => m.Prompt(_source1B, _trans1B)).Returns(DialogResult.Yes);
            _prompterMock.Setup(m => m.Prompt(_source2A, _trans2A)).Returns(DialogResult.Yes);
            _prompterMock.Setup(m => m.Prompt(_source2B, _trans2B)).Returns(DialogResult.Yes);

            var sut = new Orchestrator(_fileSystemFacade.Object, _wppTargetsFilesReader.Object, _wppTargetsXmlParser.Object, _merger.Object, _vsFacadeMock.Object, _prompterMock.Object);

            sut.RunMerge(_solutionName);

            _merger.Verify(m => m.RunMerge(_source1A, _trans1A, _solutionDir), Times.Once);
            _merger.Verify(m => m.RunMerge(_source1B, _trans1B, _solutionDir), Times.Once);
            _merger.Verify(m => m.RunMerge(_source2A, _trans2A, _solutionDir), Times.Once);
            _merger.Verify(m => m.RunMerge(_source2B, _trans2B, _solutionDir), Times.Once);
        }

        [Test]
        public void When_no_is_returned_does_not_merge()
        {
            _prompterMock.Setup(m => m.Prompt(_source1A, _trans1A)).Returns(DialogResult.Yes);
            _prompterMock.Setup(m => m.Prompt(_source1B, _trans1B)).Returns(DialogResult.No);
            _prompterMock.Setup(m => m.Prompt(_source2A, _trans2A)).Returns(DialogResult.No);
            _prompterMock.Setup(m => m.Prompt(_source2B, _trans2B)).Returns(DialogResult.Yes);

            var sut = new Orchestrator(_fileSystemFacade.Object, _wppTargetsFilesReader.Object, _wppTargetsXmlParser.Object, _merger.Object, _vsFacadeMock.Object, _prompterMock.Object);

            sut.RunMerge(_solutionName);

            _merger.Verify(m => m.RunMerge(_source1A, _trans1A, _solutionDir), Times.Once);
            _merger.Verify(m => m.RunMerge(_source1B, _trans1B, _solutionDir), Times.Never);
            _merger.Verify(m => m.RunMerge(_source2A, _trans2A, _solutionDir), Times.Never);
            _merger.Verify(m => m.RunMerge(_source2B, _trans2B, _solutionDir), Times.Once);
        }

        [Test]
        public void When_cancel_is_returned_stops_merging()
        {
            _prompterMock.Setup(m => m.Prompt(_source1A, _trans1A)).Returns(DialogResult.Yes);
            _prompterMock.Setup(m => m.Prompt(_source1B, _trans1B)).Returns(DialogResult.Cancel);

            var sut = new Orchestrator(_fileSystemFacade.Object, _wppTargetsFilesReader.Object, _wppTargetsXmlParser.Object, _merger.Object, _vsFacadeMock.Object, _prompterMock.Object);

            sut.RunMerge(_solutionName);

            _merger.Verify(m => m.RunMerge(_source1A, _trans1A, _solutionDir), Times.Once);
            _merger.Verify(m => m.RunMerge(_source1B, _trans1B, _solutionDir), Times.Never);
            _merger.Verify(m => m.RunMerge(_source2A, _trans2A, _solutionDir), Times.Never);
            _merger.Verify(m => m.RunMerge(_source2B, _trans2B, _solutionDir), Times.Never);
        }
    }
}
