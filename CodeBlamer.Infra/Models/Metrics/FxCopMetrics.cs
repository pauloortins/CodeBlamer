using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeBlamer.Infra.Models.Metrics
{
    public class FxCopMetrics : Metric
    {
        public List<FxCopIssue> Messages { get; set; }
        public int NumberOfIssues
        {
            get { return Messages.Count; }
        }

        public FxCopMetrics()
        {

        }

        public FxCopMetrics(XElement element)
        {
            Messages = new List<FxCopIssue>();

            var messages = element.Element("Messages");
            if (messages != null)
            {
                foreach (var xElement in messages.Elements("Message"))
                {
                    var message = new FxCopIssue();
                    message.TypeName = xElement.Attribute("TypeName").Value;
                    message.Category = xElement.Attribute("Category").Value;
                    message.CheckId = xElement.Attribute("CheckId").Value;
                    message.FixCategory = xElement.Attribute("FixCategory").Value;

                    var issue = xElement.Element("Issue");
                    message.Certainty = Int32.Parse(issue.Attribute("Certainty").Value);
                    message.Level = issue.Attribute("Level").Value;
                    message.LineNumber = issue.Attribute("Line") != null ? Int32.Parse(issue.Attribute("Line").Value) : 0;

                    Messages.Add(message);
                }
            }
        }
    }
}
