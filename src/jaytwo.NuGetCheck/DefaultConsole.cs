using System;
using System.Collections.Generic;
using System.Text;

namespace jaytwo.NuGetCheck
{
    public class DefaultConsole : IConsole
    {
        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }
    }
}
