using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jaytwo.NuGetCheck
{
    public static class NuGetVersionExtensions
    {
        public static IEnumerable<NuGetVersion> WhereMajorVersionEqualTo(this IEnumerable<NuGetVersion> versions, NuGetVersion version)
        {
            return versions.Where(x => x.Major == version.Major);
        }

        public static IEnumerable<NuGetVersion> WhereMajorMinorVersionEqualTo(this IEnumerable<NuGetVersion> versions, NuGetVersion version)
        {
            return versions.Where(x => x.Major == version.Major).Where(x => x.Minor == version.Minor);
        }

        public static IEnumerable<NuGetVersion> Foo(this IEnumerable<NuGetVersion> versions, NuGetVersion version, bool? sameMinorVersion, bool? sameMajorVersion)
        {
            IEnumerable<NuGetVersion> result = new List<NuGetVersion>(versions);

            if (sameMinorVersion ?? false)
            {
                result = result.WhereMajorMinorVersionEqualTo(version);
            }

            if (sameMajorVersion ?? false)
            {
                result = result.WhereMajorVersionEqualTo(version);
            }

            return result;
        }
    }
}
