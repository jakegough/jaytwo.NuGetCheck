using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace jaytwo.NuGetCheck
{
    public class NugetVersionService : INugetVersionService
    {
        public async Task<IList<string>> GetPackageVersionsAsync(string packageId, string minVersion, string maxVersion)
        {
            var repo = GetRepository();
            var findResource = await repo.GetResourceAsync<FindPackageByIdResource>();
            var packageVersions = await findResource.GetAllVersionsAsync(
                packageId,
                new SourceCacheContext(),
                NullLogger.Instance,
                CancellationToken.None);

            if (SemanticVersion.TryParse(minVersion, out SemanticVersion minSemanticVersion))
            {
                packageVersions = packageVersions.Where(x => x >= minSemanticVersion);
            }

            if (SemanticVersion.TryParse(maxVersion, out SemanticVersion maxSemanticVersion))
            {
                packageVersions = packageVersions.Where(x => x < maxSemanticVersion);
            }

            var result = packageVersions
                .OrderByDescending(x => x)
                .Select(x => x.ToString())
                .ToList();

            return result;
        }

        private SourceRepository GetRepository()
        {
            var root = Environment.CurrentDirectory;
            var sourceRepositoryProvider = new SourceRepositoryProvider(Settings.LoadDefaultSettings(root), Repository.Provider.GetCoreV3());
            var repos = sourceRepositoryProvider.GetRepositories();
            var result = repos.Single(x => x.PackageSource.Name == "nuget.org");
            return result;
        }
    }
}
