using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jaytwo.NuGetCheck
{
    public interface INugetVersionService
    {
        Task<IList<string>> GetPackageVersionsAsync(string packageId, string minVersion, string maxVersion);
    }
}
