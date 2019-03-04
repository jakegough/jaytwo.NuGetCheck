using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using NuGet.Versioning;

namespace jaytwo.NuGetCheck
{
    public class NugetCheckProgram
    {
        private readonly INugetVersionSource _nugetVersionSource;
        private readonly TextWriter _standardOut;
        private readonly TextWriter _standardError;


        public NugetCheckProgram()
            : this(new NugetVersionSource(), Console.Out, Console.Error)
        {
        }

        internal NugetCheckProgram(INugetVersionSource nugetVersionSource, TextWriter standardOut, TextWriter standardError)
        {
            _nugetVersionSource = nugetVersionSource;
            _standardOut = standardOut;
            _standardError = standardError;
        }

        public int Run(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "nugetcheck";
            app.HelpOption("-?|-h|--help");
            app.Out = _standardOut;
            app.Error = _standardError;

            var packageIdArgument = app.Argument("[package id]", "NuGet package id to query against");
            var lessThanOption = app.Option("-lt <version>", "Show only versions less than this value", CommandOptionType.SingleValue);
            var lessThanOrEqualToOption = app.Option("-lte <version>", "Show only versions less or equal to than this value", CommandOptionType.SingleValue);
            var greaterThanOption = app.Option("-gt <version>", "Show only versions greater than this value", CommandOptionType.SingleValue);
            var greaterThanOrEqualToOption = app.Option("-gte <version>", "Show only versions greater or equal to than this value", CommandOptionType.SingleValue);
            var sameMajorVersionOption = app.Option("--same-major", "foo", CommandOptionType.NoValue);
            var sameMinorVersionOption = app.Option("--same-minor", "foo", CommandOptionType.NoValue);

            app.OnExecute(() =>
            {
                return RunWithOptionsAsync(new RunOptions()
                {
                    PackageId = packageIdArgument.Value,
                    GreaterThan = greaterThanOption.HasValue() ? greaterThanOption.Value() : null,
                    GreaterThanOrEqualTo = greaterThanOrEqualToOption.HasValue() ? greaterThanOrEqualToOption.Value() : null,
                    LessThan = lessThanOption.HasValue() ? lessThanOption.Value() : null,
                    LessThanOrEqualTo = lessThanOrEqualToOption.HasValue() ? lessThanOrEqualToOption.Value() : null,
                    SameMajorVersion = sameMajorVersionOption.HasValue(),
                    SameMinorVersion = sameMinorVersionOption.HasValue(),
                });
            });

            try
            {
                return app.Execute(args);
            }
            catch (Exception ex)
            {
                _standardError.WriteLine(ex.Message);
                return 1;
            }
        }

        private async Task<int> RunWithOptionsAsync(RunOptions options)
        {
            var allVersions = await _nugetVersionSource.GetPackageVersionsAsync(options.PackageId);

            var filteredVersions = ApplyFiltersFromOptions(allVersions, options).ToList();

            if (filteredVersions.Any())
            {
                foreach (var version in filteredVersions)
                {
                    _standardOut.WriteLine(version.ToString());
                }

                return 0;
            }
            else
            {
                _standardOut.WriteLine("No results.");
                return -1;
            }
        }

        internal static IEnumerable<NuGetVersion> ApplyFiltersFromOptions(IEnumerable<NuGetVersion> versions, RunOptions options)
        {
            IEnumerable<NuGetVersion> result = new List<NuGetVersion>(versions);

            if (NuGetVersion.TryParse(options.GreaterThan, out NuGetVersion greaterThanVersion))
            {
                result = result.Where(x => x > greaterThanVersion);
                result = ApplyMajorMinorVersionFilterFromOptions(result, greaterThanVersion, options);
            }

            if (NuGetVersion.TryParse(options.GreaterThanOrEqualTo, out NuGetVersion greaterThanOrEqualToVersion))
            {
                result = result.Where(x => x >= greaterThanOrEqualToVersion);
                result = ApplyMajorMinorVersionFilterFromOptions(result, greaterThanOrEqualToVersion, options);
            }

            if (NuGetVersion.TryParse(options.LessThan, out NuGetVersion lessThanVersion))
            {
                result = result.Where(x => x < lessThanVersion);
                result = ApplyMajorMinorVersionFilterFromOptions(result, lessThanVersion, options);
            }

            if (NuGetVersion.TryParse(options.LessThanOrEqualTo, out NuGetVersion lessThanOrEqualToVersion))
            {
                result = result.Where(x => x <= lessThanOrEqualToVersion);
                result = ApplyMajorMinorVersionFilterFromOptions(result, lessThanOrEqualToVersion, options);
            }

            return result;
        }

        internal static IEnumerable<NuGetVersion> ApplyMajorMinorVersionFilterFromOptions(IEnumerable<NuGetVersion> versions, NuGetVersion version, RunOptions options)
        {
            return ApplyMajorMinorVersionFilter(versions, version, options.SameMinorVersion, options.SameMajorVersion);
        }

        internal static IEnumerable<NuGetVersion> ApplyMajorMinorVersionFilter(IEnumerable<NuGetVersion> versions, NuGetVersion version, bool? sameMinorVersion, bool? sameMajorVersion)
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
