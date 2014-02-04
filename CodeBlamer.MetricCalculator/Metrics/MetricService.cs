using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBlamer.Infra.Extensions;
using CodeBlamer.Infra.Models;
using CodeBlamer.Infra.Models.Metrics;

namespace CodeBlamer.MetricCalculator.Metrics
{
    public abstract class MetricService
    {
        protected readonly PathResolver PathResolver;

        protected MetricService(PathResolver pathResolver)
        {
            PathResolver = pathResolver;
        }

        public void CalculateMetrics(FileInfo dll)
        {
            var metricsOutputFolder = PathResolver.GetResultPath();
            metricsOutputFolder.CreateDirectory();

            var cdCommand = string.Format("E: && cd {0}", PathResolver.GetVersionPath());
            var metricsCommand = GenerateMetricsCommand(dll);

            CommandExtensions.Run(new string[] { cdCommand, metricsCommand });
        }

        public abstract Metric.MetricsSource GetMetricSource();
        public abstract string GenerateResultPath(FileInfo dll);
        public abstract string GenerateMetricsCommand(FileInfo dll);
    }
}
