using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBlamer.Infra.Models;
using CodeBlamer.Infra.Models.Metrics;

namespace CodeBlamer.MetricCalculator.Metrics
{
    public class FxCopService : MetricService
    {
        public FxCopService(PathResolver pathResolver) : base(pathResolver)
        {
        }

        public override Metric.MetricsSource GetMetricSource()
        {
            return Metric.MetricsSource.FxCop;
        }

        public override string GenerateResultPath(FileInfo dll)
        {
            return PathResolver.GetResultXmlPath(dll).Replace(".xml", "-fxCop.xml");
        }

        public override string GenerateMetricsCommand(FileInfo dll)
        {
            var parameters = "/file:\"{0}\" /out:\"{1}\" /igc /d:\"{2}\" /gac";
            var metricsOutputPath = GenerateResultPath(dll);
            var dllPath = dll.FullName.Replace("obj", "bin");

            var metricsCommand =
            "\"C:\\Program Files (x86)\\Microsoft Visual Studio 11.0\\Team Tools\\Static Analysis Tools\\FxCop\\FxCopCmd.exe\"" +
                           string.Format(parameters, dllPath, metricsOutputPath, PathResolver.GetVersionPath());

            return metricsCommand;
        }
    }
}
