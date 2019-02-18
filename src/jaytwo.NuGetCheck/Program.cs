using System.Threading.Tasks;

namespace jaytwo.NuGetCheck
{
    public class Program
    {
        public static Task<int> Main(string[] args) => new NugetCheckProgram().RunAsync(args);
    }
}
