using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBlamer.Infra.Extensions;
using CodeBlamer.Infra.Models;

namespace CodeBlamer.MetricCalculator.Metrics
{
    public class StyleCopService : MetricService
    {
        public StyleCopService(PathResolver pathResolver) : base(pathResolver)
        {
        }

        public override MetricsSource GetMetricSource()
        {
            return MetricsSource.StyleCop;
        }

        public override string GenerateResultPath(FileInfo dll)
        {
            return PathResolver.GetResultXmlPath(dll).Replace(".xml", "-styleCop.xml");
        }

        public override string GenerateMetricsCommand(FileInfo dll)
        {
            var projFilePath = dll.Directory.Parent.Parent.SearchFor("*.csproj")[0].FullName;

            var parameters = " -proj {0} -out {1}";
            var metricsOutputPath = GenerateResultPath(dll);

            var exePath =
                @"C:\Users\paulo_000\Documents\GitHub\CodeBlamer\CodeBlamer.StyleCopRunner\bin\Debug\CodeBlamer.StyleCopRunner.exe";

            var metricsCommand =
            "\"" + exePath + "\"" +
                           string.Format(parameters, projFilePath, metricsOutputPath);

            return metricsCommand;
        }
    }
}
