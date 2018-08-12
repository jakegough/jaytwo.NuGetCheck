using CommandLine;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace jaytwo.NuGetCheck
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var nugetVersionService = new NugetVersionService();
            var console = new DefaultConsole();
            var myProgram = new NugetCheckProgram(nugetVersionService, console);
            return myProgram.Run(args);
        }
    }
}
