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

            [Test]
            public void Calls_windowsShell()
            {
                var vsFacadeMock = new Mock<IVisualStudioFacade>();
                var windowsShellMock = new Mock<IWindowsShellFacade>();
                vsFacadeMock.Setup(m => m.PromptUser(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(DialogResult.Yes);

                var sut = new Merger(windowsShellMock.Object);

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
