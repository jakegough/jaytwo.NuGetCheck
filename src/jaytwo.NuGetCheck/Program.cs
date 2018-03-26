using CommandLine;
using System;
using System.Threading.Tasks;

namespace jaytwo.NuGetCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<RunOptions>(args)
                .WithParsed<RunOptions>(opts => RunOptionsAndReturnExitCode(opts));
        }

        static void RunOptionsAndReturnExitCode(RunOptions options)
        {
            Task.Run(async () => { await RunOptionsAndReturnExitCodeAsync(options); })
                .GetAwaiter()
                .GetResult();
        }

        static async Task RunOptionsAndReturnExitCodeAsync(RunOptions options)
        {
            INugetVersionService versionService = new NugetVersionService();
            var versions = await versionService.GetPackageVersionsAsync(options.PackageId, options.MinVersion, options.MaxVersion);

            foreach (var version in versions)
            {
                Console.WriteLine(version);
            }
        }
    }
}
