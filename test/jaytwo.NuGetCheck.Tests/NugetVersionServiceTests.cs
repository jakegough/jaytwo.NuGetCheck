using System;
using System.Threading.Tasks;
using NuGet.Versioning;
using Xunit;

namespace jaytwo.NuGetCheck.Tests
{
    public class NugetVersionServiceTests
    {
        [Fact]
        public async Task GetPackageVersionsAsync_returns_versions_greater_than_or_equal_to_minVersion()
        {
            // arrange
            var versionService = new NugetVersionService();
            var package = "xunit";
            var minVersion = "2.0.0";
            var minSemanticVersion = NuGetVersion.Parse(minVersion);

            // act
            var versions = await versionService.GetPackageVersionsAsync(package, minVersion, null);

            // assert
            Assert.NotEmpty(versions);
            Assert.All(versions, version =>
            {
                var semanticVarsion = NuGetVersion.Parse(version);
                Assert.True(semanticVarsion >= minSemanticVersion);
            });
        }

        [Fact]
        public async Task GetPackageVersionsAsync_returns_versions_less_than_or_equal_to_maxVersion()
        {
            // arrange
            var versionService = new NugetVersionService();
            var package = "xunit";
            var maxVersion = "2.0.0";
            var maxSemanticVersion = NuGetVersion.Parse(maxVersion);

            // act
            var versions = await versionService.GetPackageVersionsAsync(package, null, maxVersion);

            // assert
            Assert.NotEmpty(versions);
            Assert.All(versions, version =>
            {
                var semanticVarsion = NuGetVersion.Parse(version);
                Assert.True(semanticVarsion < maxSemanticVersion);
            });
        }

        [Fact]
        public async Task GetPackageVersionsAsync_returns_versions_between_minVersion_and_maxVersion()
        {
            // arrange
            var versionService = new NugetVersionService();
            var package = "xunit";
            var minVersion = "1.0.0";
            var minSemanticVersion = NuGetVersion.Parse(minVersion);
            var maxVersion = "2.0.0";
            var maxSemanticVersion = NuGetVersion.Parse(maxVersion);

            // act
            var versions = await versionService.GetPackageVersionsAsync(package, minVersion, maxVersion);

            // assert
            Assert.NotEmpty(versions);
            Assert.All(versions, version =>
            {
                var nuGetVersion = NuGetVersion.Parse(version);
                Assert.True(nuGetVersion >= minSemanticVersion);
                Assert.True(nuGetVersion < maxSemanticVersion);
            });
        }
    }
}
