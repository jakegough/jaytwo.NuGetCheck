using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jaytwo.NuGetCheck
{
    public class NugetCheckProgram
    {
        private readonly INugetVersionService _nugetVersionService;
        private readonly IConsole _console;

        public NugetCheckProgram(INugetVersionService service, IConsole console)
        {
            _nugetVersionService = service;
            _console = console;
        }

        public int Run(string[] args)
        {
            ParserResult<RunOptions> parseResult;

            try
            {
                parseResult = Parser.Default.ParseArguments<RunOptions>(args);
            }
            catch
            {
                return 1;
            }

            return parseResult.MapResult(
                    options => RunWithOptions(options),
                    errors => 1
                );
        }

        private int RunWithOptions(RunOptions options)
        {
            var versions = _nugetVersionService
                .GetPackageVersionsAsync(
                    options.PackageId,
                    options.MinVersion,
                    options.MaxVersion)
                .RunSync();

            if (versions.Any())
            {
                foreach (var version in versions)
                {
                    _console.WriteLine(version);
                }

                return 0;
            }
            else
            {
                _console.WriteLine("No results.");
                return -1;
            }
        }
    }
}
