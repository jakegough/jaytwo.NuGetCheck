using System.Linq;
using System.Threading.Tasks;
using CommandLine;

namespace jaytwo.NuGetCheck
{
    public class NugetCheckProgram
    {
        private readonly INugetVersionService _nugetVersionService;
        private readonly IConsole _console;

        public NugetCheckProgram()
            : this(new NugetVersionService(), new DefaultConsole())
        {
        }

        internal NugetCheckProgram(INugetVersionService nugetVersionService, IConsole console)
        {
            _nugetVersionService = nugetVersionService;
            _console = console;
        }

        public async Task<int> RunAsync(string[] args)
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

            return await parseResult.MapResult(
                options => RunWithOptionsAsync(options),
                errors => Task.FromResult(1)
            );
        }

        private async Task<int> RunWithOptionsAsync(RunOptions options)
        {
            var versions = await _nugetVersionService.GetPackageVersionsAsync(
                packageId: options.PackageId,
                minVersion: options.MinVersion,
                maxVersion: options.MaxVersion);

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
