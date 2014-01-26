using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeBlamer.Infra.Models
{
    public class Node
    {
        public Node(XElement element)
        {
            this.Name = element.Attribute("Name").Value;
            var metrics = new Metrics();
            var metricsXml = element.Element("Metrics");
            foreach (var metric in metricsXml.Elements("Metric"))
            {
                var metricValue = Int32.Parse(metric.Attribute("Value").Value.Replace(".",""));
                switch (metric.Attribute("Name").Value)
                {
                    case "MaintainabilityIndex":
                        metrics.MaintainabilityIndex = metricValue;
                        break;
                    case "CyclomaticComplexity":
                        metrics.CyclomaticComplexity = metricValue;
                        break;
                    case "ClassCoupling":
                        metrics.ClassCoupling = metricValue;
                        break;
                    case "DepthOfInheritance":
                        metrics.DepthOfInheritance = metricValue;
                        break;
                    case "LinesOfCode":
                        metrics.LinesOfCode = metricValue;
                        break;
                }
            }

            this.Metrics = metrics;
        }

        public string Name { get; set; }
        public Metrics Metrics { get; set; }
    }
}
