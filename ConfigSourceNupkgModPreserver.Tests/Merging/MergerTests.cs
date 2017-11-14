using ConfigSourceNupkgModPreserver.Contracts.VisualStudioFacade;
using ConfigSourceNupkgModPreserver.Contracts.WindowsFacade;
using ConfigSourceNupkgModPreserver.Implementation.Merging;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace ConfigSourceNupkgModPreserver.Tests.Merging
{
    [TestFixture]
    public class MergerTests
    {
        private readonly Fixture _fixture = new Fixture();

        [TestFixture]
        public class When_the_user_says_no : MergerTests
        {
            [Test]
            public void _then_merging_is_not_done()
            {
                var vsFacadeMock = new Mock<IVisualStudioFacade>();
                var windowsShellMock = new Mock<IWindowsShellFacade>();
                vsFacadeMock.Setup(m => m.PromptUser(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(DialogResult.No);

                var sut = new Merger(vsFacadeMock.Object, windowsShellMock.Object);

                sut.RunMerge("", "", "");

                windowsShellMock.Verify(m => m.RunCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                    Times.Never);
                windowsShellMock.Verify(m => m.RunProcess(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                    Times.Never);
            }
        }

        [TestFixture]
        public class When_the_user_says_yes : MergerTests
        {
            [Test]
            public void _then_merging_is_done()
            {
                var vsFacadeMock = new Mock<IVisualStudioFacade>();
                var windowsShellMock = new Mock<IWindowsShellFacade>();
                vsFacadeMock.Setup(m => m.PromptUser(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(DialogResult.Yes);

                var sut = new Merger(vsFacadeMock.Object, windowsShellMock.Object);

                var sourceConfigRelativePath = _fixture.Create<string>();
                var transformedConfigRelativePath = _fixture.Create<string>();
                var solutionDir = _fixture.Create<string>();

                sut.RunMerge(sourceConfigRelativePath, transformedConfigRelativePath, solutionDir);

                windowsShellMock.Verify(m => m.RunCommand("copy", $"NUL {Merger.TempFileName}", solutionDir),
                    Times.Once);
                windowsShellMock.Verify(m => m.RunProcess("git.exe", $"merge-file {sourceConfigRelativePath} {Merger.TempFileName} {transformedConfigRelativePath}", solutionDir),
                    Times.Once);
                windowsShellMock.Verify(m => m.RunCommand("del", $"{Merger.TempFileName}", solutionDir),
                    Times.Once);
            }
        }
    }
}
