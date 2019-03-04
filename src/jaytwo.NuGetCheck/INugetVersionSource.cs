using NuGet.Versioning;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jaytwo.NuGetCheck
{
    public interface INugetVersionSource
    {
        Task<IList<NuGetVersion>> GetPackageVersionsAsync(string packageId);
    }
}
