using System.Collections.Generic;
using System.Threading.Tasks;

namespace jaytwo.NuGetCheck
{
    public interface INugetVersionService
    {
        Task<IList<string>> GetPackageVersionsAsync(string packageId, string minVersion, string maxVersion);
    }
}
