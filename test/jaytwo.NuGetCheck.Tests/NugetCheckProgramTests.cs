using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace jaytwo.NuGetCheck.Tests
{
    public class NugetCheckProgramTests
    {
        [Fact]
        public void Run_returns_negative_1_when_no_package_results()
        {
            // arrange
            var packageId = "anything";
            
            var mockConsole = new Mock<IConsole>();

            var mockNugetVersionService = new Mock<INugetVersionService>();
            mockNugetVersionService
                .Setup(x => x.GetPackageVersionsAsync(packageId, null, null))
                .ReturnsAsync(new string[] { });

            var program = new NugetCheckProgram(mockNugetVersionService.Object, mockConsole.Object);
            var args = new[] { $"--packageId={packageId}" };

            // act
            var result = program.Run(args);

            // assert
            Assert.Equal(-1, result);
            mockNugetVersionService.VerifyAll();
        }

        [Fact]
        public void Run_returns_1_when_invalid_args()
        {
            // arrange
            var mockConsole = new Mock<IConsole>();
            var mockNugetVersionService = new Mock<INugetVersionService>();            
            var program = new NugetCheckProgram(mockNugetVersionService.Object, mockConsole.Object);
            var args = new[] { $"--foo=bar" };

            // act
            var result = program.Run(args);

            // assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void Run_writes_to_console_when_invalid_args()
        {
            // arrange
            var mockConsole = new Mock<IConsole>();
            var mockNugetVersionService = new Mock<INugetVersionService>();
            var program = new NugetCheckProgram(mockNugetVersionService.Object, mockConsole.Object);
            var args = new[] { $"--foo=bar" };

            // act
            var result = program.Run(args);

            // assert
            Assert.Equal(1, result);
        }
    }
}
