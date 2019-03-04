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

            var mockNugetVersionService = new Mock<INugetVersionSource>();
            mockNugetVersionService
                .Setup(x => x.GetPackageVersionsAsync(packageId))
                .ReturnsAsync(new NuGetVersion[] { });

            using (var mockStandardOut = new StringWriter())
            using (var mockStandardError = new StringWriter())
            {
                var program = new NugetCheckProgram(mockNugetVersionService.Object, mockStandardOut, mockStandardError);
                var args = new[] { packageId };

                // act
                var result = program.Run(args);

                // assert
                Assert.Equal(-1, result);
                mockNugetVersionService.VerifyAll();
            }
        }

        [Fact]
        public void Run_returns_1_when_invalid_args()
        {
            // arrange
            var mockNugetVersionService = new Mock<INugetVersionSource>();

            using (var mockStandardOut = new StringWriter())
            using (var mockStandardError = new StringWriter())
            {
                var program = new NugetCheckProgram(mockNugetVersionService.Object, mockStandardOut, mockStandardError);
                var args = new[] { $"--foo=bar" };

                // act
                var result = program.Run(args);

                // assert
                Assert.Equal(1, result);
            }
        }

        [Fact]
        public void Run_writes_to_console_when_invalid_args()
        {
            // arrange
            using (var mockStandardOut = new StringWriter())
            using (var mockStandardError = new StringWriter())
            {
                var mockNugetVersionService = new Mock<INugetVersionSource>();
                var program = new NugetCheckProgram(mockNugetVersionService.Object, mockStandardOut, mockStandardError);
                var args = new[] { $"--foo=bar" };

                // act
                var result = program.Run(args);

                // assert
                Assert.Equal(1, result);

                var output = mockStandardError.ToString();
                Assert.Contains("--foo=bar", output);
            }
        }

        [Theory]
        [InlineData(new[] { "abc", "-lt", "1.1.0" }, new[] { "0.1.0-beta1", "0.1.0-beta2", "0.1.0", "1.0.0-beta", "1.0.0", "1.1.0-beta" })]
        [InlineData(new[] { "abc", "-lt", "1.1.0", "--same-major" }, new[] { "1.0.0-beta", "1.0.0", "1.1.0-beta" })]
        [InlineData(new[] { "abc", "-lt", "1.1.0", "--same-minor" }, new[] { "1.1.0-beta" })]
        [InlineData(new[] { "abc", "-lte", "1.1.0" }, new[] { "0.1.0-beta1", "0.1.0-beta2", "0.1.0", "1.0.0-beta", "1.0.0", "1.1.0-beta", "1.1.0" })]
        [InlineData(new[] { "abc", "-lte", "1.1.0", "--same-major" }, new[] { "1.0.0-beta", "1.0.0", "1.1.0-beta", "1.1.0" })]
        [InlineData(new[] { "abc", "-lte", "1.1.0", "--same-minor" }, new[] { "1.1.0-beta", "1.1.0" })]
        [InlineData(new[] { "abc", "-gt", "2.0.0-beta" }, new[] { "2.0.0", "2.1.0-beta", "2.1.0", "3.0.0", "3.1.0-beta", "3.1.0" })]
        [InlineData(new[] { "abc", "-gt", "2.0.0-beta", "--same-major" }, new[] { "2.0.0", "2.1.0-beta", "2.1.0" })]
        [InlineData(new[] { "abc", "-gt", "2.0.0-beta", "--same-minor" }, new[] { "2.0.0" })]
        [InlineData(new[] { "abc", "-gte", "2.0.0-beta" }, new[] { "2.0.0-beta", "2.0.0", "2.1.0-beta", "2.1.0", "3.0.0", "3.1.0-beta", "3.1.0" })]
        [InlineData(new[] { "abc", "-gte", "2.0.0-beta", "--same-major" }, new[] { "2.0.0-beta", "2.0.0", "2.1.0-beta", "2.1.0" })]
        [InlineData(new[] { "abc", "-gte", "2.0.0-beta", "--same-minor" }, new[] { "2.0.0-beta", "2.0.0" })]
        public void Run_filters_properly(string[] args, string[] expectedVersions)
        {
            // arrange
            var mockNugetVersionService = new Mock<INugetVersionSource>();
            mockNugetVersionService
                .Setup(x => x.GetPackageVersionsAsync("abc"))
                .ReturnsAsync(new NuGetVersion[] {
                    new NuGetVersion("0.1.0-beta1"),
                    new NuGetVersion("0.1.0-beta2"),
                    new NuGetVersion("0.1.0"),
                    new NuGetVersion("1.0.0-beta"),
                    new NuGetVersion("1.0.0"),
                    new NuGetVersion("1.1.0-beta"),
                    new NuGetVersion("1.1.0"),
                    new NuGetVersion("2.0.0-beta"),
                    new NuGetVersion("2.0.0"),
                    new NuGetVersion("2.1.0-beta"),
                    new NuGetVersion("2.1.0"),
                    new NuGetVersion("3.0.0"),
                    new NuGetVersion("3.1.0-beta"),
                    new NuGetVersion("3.1.0"),
                });

            using (var mockStandardOut = new StringWriter())
            using (var mockStandardError = new StringWriter())
            {
                var program = new NugetCheckProgram(mockNugetVersionService.Object, mockStandardOut, mockStandardError);

                // act
                var outputCode = program.Run(args);

                // assert
                Assert.Equal(0, outputCode);
                var outputLines = mockStandardOut.ToString().Split("\n").Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim()).ToArray();
                Assert.Equal(expectedVersions, outputLines);
            }
        }
    }
}
