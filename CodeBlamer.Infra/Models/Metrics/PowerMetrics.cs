using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeBlamer.Infra.Models.Metrics
{
    public class PowerMetrics : Metric
    {
        public int MaintainabilityIndex { get; set; }
        public int CyclomaticComplexity { get; set; }
        public int ClassCoupling { get; set; }
        public int DepthOfInheritance { get; set; }
        public int LinesOfCode { get; set; }

        public PowerMetrics()
        {

        }

        public PowerMetrics(XElement element)
        {
            var metricsXml = element.Element("Metrics");
            foreach (var metric in metricsXml.Elements("Metric"))
            {
                var metricValue = Int32.Parse(metric.Attribute("Value").Value.Replace(".", ""));
                switch (metric.Attribute("Name").Value)
                {
                    case "MaintainabilityIndex":
                        MaintainabilityIndex = metricValue;
                        break;
                    case "CyclomaticComplexity":
                        CyclomaticComplexity = metricValue;
                        break;
                    case "ClassCoupling":
                        ClassCoupling = metricValue;
                        break;
                    case "DepthOfInheritance":
                        DepthOfInheritance = metricValue;
                        break;
                    case "LinesOfCode":
                        LinesOfCode = metricValue;
                        break;
                }
            }
        }
    }
}
