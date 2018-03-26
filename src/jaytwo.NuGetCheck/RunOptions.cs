using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace jaytwo.NuGetCheck
{
    public class RunOptions
    {
        [Option("packageId", Required = true, HelpText = "NuGet package id to query against")]
        public string PackageId { get; set; }

        [Option("minVersion", Required = false, HelpText = "Filter by minimum version (inclusive)")]
        public string MinVersion { get; set; }

        [Option("maxVersion", Required = false, HelpText = "Filter by maximum version (exclusive)")]
        public string MaxVersion { get; set; }
    }
}
