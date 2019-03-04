using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NuGet.Versioning;
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

            var mockNugetVersionService = new Mock<INugetVersionSource>();
            mockNugetVersionService
                .Setup(x => x.GetPackageVersionsAsync(packageId))
                .ReturnsAsync(new NuGetVersion[] { });

            var program = new NugetCheckProgram(mockNugetVersionService.Object, mockConsole.Object);
            var args = new[] { packageId };

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
            var mockNugetVersionService = new Mock<INugetVersionSource>();
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
            mockConsole.Setup(x => x.WriteLine(It.Is<string>(param => param.Contains("--foo=bar"))));

            var mockNugetVersionService = new Mock<INugetVersionSource>();
            var program = new NugetCheckProgram(mockNugetVersionService.Object, mockConsole.Object);
            var args = new[] { $"--foo=bar" };

            // act
            var result = program.Run(args);

            // assert
            Assert.Equal(1, result);
            mockConsole.VerifyAll();
        }

        [Theory]
        [InlineData("abc", "-lte", "0.1.0")]
        [InlineData("abc", "-lte", "0.1.0", "--same-major")]
        [InlineData("abc", "-lte", "0.1.0", "--same-minor")]
        [InlineData("abc", "-gte", "1.0.0", "-lt", "2.0.0")]
        [InlineData("abc", "-gte", "1.0.0", "-lte", "1.0.0")]
        public void Run_filters_properly(params string[] args)
        {
            using (var stringWriter = new StringWriter())
            {
                var mockConsole = new Mock<IConsole>();
                mockConsole
                    .Setup(x => x.WriteLine(It.IsAny<string>()))
                    .Callback(stringWriter.WriteLine);

                var mockNugetVersionService = new Mock<INugetVersionSource>();
                mockNugetVersionService
                    .Setup(x => x.GetPackageVersionsAsync("abc"))
                    .ReturnsAsync(new NuGetVersion[] {
                        new NuGetVersion("0.1.0-beta1"),
                        new NuGetVersion("0.1.0-beta2"),
                        new NuGetVersion("0.1.0"),
                        new NuGetVersion("0.2.0-beta1"),
                        new NuGetVersion("0.2.0-beta2"),
                        new NuGetVersion("0.2.0"),
                        new NuGetVersion("1.0.0-beta1"),
                        new NuGetVersion("1.0.0-beta2"),
                        new NuGetVersion("1.0.0"),
                        new NuGetVersion("1.1.0-beta1"),
                        new NuGetVersion("1.1.0-beta2"),
                        new NuGetVersion("1.1.0"),
                        new NuGetVersion("1.2.0-beta1"),
                        new NuGetVersion("1.2.0-beta2"),
                        new NuGetVersion("1.2.0"),
                        new NuGetVersion("1.3.0-beta1"),
                        new NuGetVersion("1.3.0-beta2"),
                        new NuGetVersion("1.3.0"),
                        new NuGetVersion("1.0.0-beta1"),
                        new NuGetVersion("1.0.0-beta2"),
                        new NuGetVersion("2.0.0"),
                        new NuGetVersion("2.1.0-beta1"),
                        new NuGetVersion("2.1.0-beta2"),
                        new NuGetVersion("2.1.0"),
                        new NuGetVersion("2.2.0-beta1"),
                        new NuGetVersion("2.2.0-beta2"),
                        new NuGetVersion("2.2.0"),
                        new NuGetVersion("2.3.0-beta1"),
                        new NuGetVersion("2.3.0-beta2"),
                        new NuGetVersion("2.3.0"),
                    });

                var program = new NugetCheckProgram(mockNugetVersionService.Object, mockConsole.Object);

                // arrange
                Console.SetOut(stringWriter);

                // act
                var outputCode = program.Run(args);

                // assert
                Assert.Equal(0, outputCode);

                var outputLines = stringWriter.ToString().Split("\n").Select(x => x.Trim());
                Assert.NotEmpty(outputLines);
            }
        }
    }
}
