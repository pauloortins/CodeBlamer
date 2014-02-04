using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBlamer.Infra.Models.Metrics;

namespace CodeBlamer.Infra.Models.CodeElements
{
    public class CodeElement
    {
        public string Name { get; set; }
        public PowerMetrics PowerMetrics { get; set; }
        public FxCopMetrics FxCopMetrics { get; set; }
        public StyleCopMetrics StyleCopMetrics { get; set; }

        public void AddMetrics(Metric metric)
        {
            if (metric is PowerMetrics)
            {
                PowerMetrics = (PowerMetrics)metric;
            }
            else if (metric is FxCopMetrics)
            {
                FxCopMetrics = (FxCopMetrics)metric;
            }
            else if (metric is StyleCopMetrics)
            {
                StyleCopMetrics = (StyleCopMetrics)metric;
            }
        }
    }
}
