using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeBlamer.Infra.Models.Metrics
{
    public class StyleCopMetrics : Metric
    {
        public List<StyleCopIssue> Violations { get; set; }
        public int NumberOfIssues
        {
            get { return Violations.Count; }
        }

        public StyleCopMetrics()
        {
        }

        public StyleCopMetrics(XElement element)
        {
            Violations = new List<StyleCopIssue>();

            var violationsXml = element.Element("Violations");
            foreach (var violationElement in violationsXml.Elements("Violation"))
            {
                var violation = new StyleCopIssue();
                violation.RuleId = violationElement.Attribute("RuleId").Value;
                violation.RuleGroup = violationElement.Attribute("RuleGroup") != null
                                          ? violationElement.Attribute("RuleGroup").Value
                                          : string.Empty;
                violation.Kind = violationElement.Attribute("Kind").Value;
                violation.RuleNamespace = violationElement.Attribute("RuleNamespace").Value;
                violation.Rule = violationElement.Attribute("Rule").Value;
                violation.LineNumber = Int32.Parse(violationElement.Attribute("LineNumber").Value);
                violation.Message = violationElement.Attribute("Message").Value;

                Violations.Add(violation);
            }
        }
    }
}
