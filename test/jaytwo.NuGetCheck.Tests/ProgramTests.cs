using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace jaytwo.NuGetCheck.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void Program_Main_returns_success_exit_code_for_happy_path()
        {
            using (var stringWriter = new StringWriter())
            {
                // arrange
                Console.SetOut(stringWriter);

                var args = new[] { "--packageId=xunit", "--minVersion=2.0.0", "--maxVersion=2.0.1" };

                // act
                var result = Program.Main(args);

                // assert
                Assert.Equal(0, result);
                Assert.Equal("2.0.0", stringWriter.ToString().Trim());
            }
        }

        [Fact]
        public void Program_Main_returns_failure_exit_code_for_unhappy_path()
        {
            using (var stringWriter = new StringWriter())
            {
                // arrange
                Console.SetOut(stringWriter);
                
                var args = new[] { "--packageId=xunit", "--minVersion=999.999.999" };

                // act
                var result = Program.Main(args);

                // assert
                Assert.Equal(-1, result);
                Assert.Equal("No results.", stringWriter.ToString().Trim());
            }
        }
    }
}
