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
    public class PowerMetricsService : MetricService
    {
        public PowerMetricsService(PathResolver pathResolver) : base(pathResolver)
        {
        }

        public override string GenerateResultPath(FileInfo dll)
        {
            return PathResolver.GetResultXmlPath(dll);
        }

        public override string GenerateMetricsCommand(FileInfo dll)
        {
            var parameters = "/file:\"{0}\" /out:\"{1}\" /igc /d:\"{2}\" /gac";
            var metricsOutputPath = GenerateResultPath(dll);
            var dllPath = dll.FullName.Replace("obj", "bin");

            var metricsCommand =
            "\"C:\\Program Files (x86)\\Microsoft Visual Studio 11.0\\Team Tools\\Static Analysis Tools\\FxCop\\Metrics.exe\"" +
                           string.Format(parameters, dllPath, metricsOutputPath, PathResolver.GetVersionPath());

            return metricsCommand;
        }
    }
}
