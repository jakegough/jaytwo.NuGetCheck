using System;

namespace jaytwo.NuGetCheck
{
    public class DefaultConsole : IConsole
    {
        public void WriteLine(string value) => Console.WriteLine(value);
    }
}
