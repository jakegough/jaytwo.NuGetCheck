using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace jaytwo.NuGetCheck.Tests
{
    public class ProgramTests
    {
        [Fact]
        public async Task Program_Main_returns_success_exit_code_for_happy_path()
        {
            using (var stringWriter = new StringWriter())
            {
                // arrange
                Console.SetOut(stringWriter);

                var args = new[] { "--packageId=xunit", "--minVersion=2.0.0", "--maxVersion=2.0.1" };

                // act
                var result = await Program.Main(args);

                // assert
                Assert.Equal(0, result);
                Assert.Equal("2.0.0", stringWriter.ToString().Trim());
            }
        }

        [Fact]
        public async Task Program_Main_returns_failure_exit_code_for_unhappy_path()
        {
            using (var stringWriter = new StringWriter())
            {
                // arrange
                Console.SetOut(stringWriter);

                var args = new[] { "--packageId=xunit", "--minVersion=999.999.999" };

                // act
                var result = await Program.Main(args);

                // assert
                Assert.Equal(-1, result);
                Assert.Equal("No results.", stringWriter.ToString().Trim());
            }
        }
    }
}
