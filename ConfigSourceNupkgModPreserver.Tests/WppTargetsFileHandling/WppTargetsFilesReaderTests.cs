using System.Collections.Generic;
using System.Linq;
using ConfigSourceNupkgModPreserver.Contracts.WindowsFacade;
using ConfigSourceNupkgModPreserver.Implementation.WppTargetsFileHandling;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace ConfigSourceNupkgModPreserver.Tests.WppTargetsFileHandling
{
    [TestFixture]
    public class WppTargetsFilesReaderTests
    {
        private readonly Fixture _fixture = new Fixture();
        private Mock<IFileSystemFacade> _fileSystemIntegratorMock;
        private WppTargetsFilesReader _sut;
        private string _solutionPath;
        private List<string> _subDirectories;

        [SetUp]
        public void SetUp()
        {
            _solutionPath = _fixture.Create<string>();
            _fileSystemIntegratorMock = new Mock<IFileSystemFacade>();
            _sut = new WppTargetsFilesReader(_fileSystemIntegratorMock.Object);
        }

        [TestFixture]
        public class When_input_is_null : WppTargetsFilesReaderTests
        {
            [SetUp]
            public new void SetUp()
            {
                base.SetUp();
                _solutionPath = null;
            }

            [Test]
            public void _returns_empty_list() => ActAndAssertEmptyNotNull();
        }

        public class When_input_is_not_null : WppTargetsFilesReaderTests
        {
            [SetUp]
            public new void SetUp()
            {
                base.SetUp();
                _solutionPath = _fixture.Create<string>();
            }

            [Test]
            public void GetsSubDirectories()
            {
                var result = _sut.GetWppTargetsFiles(_solutionPath);
                _fileSystemIntegratorMock.Verify(f => f.GetSubDirectories(_solutionPath), Times.Once);
            }
        }

        [TestFixture]
        public class When_subdirectories_is_null : WppTargetsFilesReaderTests
        {
            [SetUp]
            public new void SetUp()
            {
                base.SetUp();
                _solutionPath = _fixture.Create<string>();
                _subDirectories = null;
                _fileSystemIntegratorMock.Setup(f => f.GetSubDirectories(_solutionPath)).Returns(_subDirectories);
            }

            [Test]
            public void _returns_empty_list() => ActAndAssertEmptyNotNull();
        }

        [TestFixture]
        public class When_subdirectories_is_not_null : WppTargetsFilesReaderTests
        {
            [SetUp]
            public new void SetUp()
            {
                base.SetUp();
                _solutionPath = _fixture.Create<string>();
                _subDirectories = _fixture.CreateMany<string>(3).ToList();
                _fileSystemIntegratorMock.Setup(f => f.GetSubDirectories(_solutionPath)).Returns(_subDirectories);
            }

            [Test]
            public void GetsWppTargetsFiles()
            {
                var result = _sut.GetWppTargetsFiles(_solutionPath);
                foreach (var subDirectory in _subDirectories)
                    _fileSystemIntegratorMock.Verify(f => f.GetFiles(subDirectory, $"*{WppTargetsFilesReader.WppTargetsFileExtension}"), Times.Once);
            }
        }

        [TestFixture]
        public class When_wpp_targets_files_dont_exist : WppTargetsFilesReaderTests
        {
            [SetUp]
            public new void SetUp()
            {
                base.SetUp();
                _solutionPath = _fixture.Create<string>();
                _subDirectories = _fixture.CreateMany<string>(3).ToList();
                _fileSystemIntegratorMock.Setup(f => f.GetSubDirectories(_solutionPath)).Returns(_subDirectories);
            }

            [Test]
            public void _returns_empty_list() => ActAndAssertEmptyNotNull();
        }

        [TestFixture]
        public class When_wpp_targets_files_exists : WppTargetsFilesReaderTests
        {
            private string _firstDirFirstFile;
            private string _secondDirSecondFile;
            private string _thirdDirSecondFile;

            [SetUp]
            public new void SetUp()
            {
                base.SetUp();
                _solutionPath = _fixture.Create<string>();
                _subDirectories = _fixture.CreateMany<string>(3).ToList();
                _fileSystemIntegratorMock.Setup(f => f.GetSubDirectories(_solutionPath)).Returns(_subDirectories);
                
                _firstDirFirstFile = _fixture.Create<string>();
                _secondDirSecondFile = _fixture.Create<string>();
                _thirdDirSecondFile = _fixture.Create<string>();

                _fileSystemIntegratorMock.Setup(f => f.GetFiles(_subDirectories[0], $"*{WppTargetsFilesReader.WppTargetsFileExtension}"))
                    .Returns(new List<string> {_firstDirFirstFile, _fixture.Create<string>()});

                _fileSystemIntegratorMock.Setup(f => f.GetFiles(_subDirectories[1], $"*{WppTargetsFilesReader.WppTargetsFileExtension}"))
                    .Returns(new List<string> { string.Empty, _secondDirSecondFile, _fixture.Create<string>() });

                _fileSystemIntegratorMock.Setup(f => f.GetFiles(_subDirectories[2], $"*{WppTargetsFilesReader.WppTargetsFileExtension}"))
                    .Returns(new List<string> { null, _thirdDirSecondFile, _fixture.Create<string>() });
            }

            [Test]
            public void _takes_the_first_nonempty_in_each_directory()
            {
                var result = _sut.GetWppTargetsFiles(_solutionPath);

                result.ShouldBeEquivalentTo(new List<string>
                {
                    _firstDirFirstFile,
                    _secondDirSecondFile,
                    _thirdDirSecondFile
                });
            }
        }
        
        protected void ActAndAssertEmptyNotNull()
        {
            var result = _sut.GetWppTargetsFiles(_solutionPath);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}