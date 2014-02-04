using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeBlamer.Infra.Models.Metrics
{
    public abstract class Metric
    {
        public enum MetricsSource
        {
            PowerMetrics = 0,
            FxCop = 1,
            StyleCop = 2
        }

        public static Metric CreateMetric(XElement element, MetricsSource metricsSource)
        {
            switch (metricsSource)
            {
                case MetricsSource.PowerMetrics:
                    return new PowerMetrics(element);
                case MetricsSource.FxCop:
                    return new FxCopMetrics(element);
                case MetricsSource.StyleCop:
                    return new StyleCopMetrics(element);
            }

            throw new Exception("Metric not exist");
        }
    }
}
