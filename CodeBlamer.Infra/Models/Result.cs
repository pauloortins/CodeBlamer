using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeBlamer.Infra.Models
{
    public enum MetricsSource
    {
        PowerMetrics = 0,
        FxCop = 1,
        StyleCop = 2
    }

    public class MetricResults
    {
        private List<NewModule> _modules = new List<NewModule>();

        public NewModule GetModule()
        {
            return _modules.FirstOrDefault();
        }

        public void AddModule(XElement element, MetricsSource source)
        {
            var moduleName = element.Attribute("Name").Value;
            var module = _modules.FirstOrDefault(x => x.Name == moduleName);

            if (module == null)
            {
                module = new NewModule();
                module.Name = moduleName;
                _modules.Add(module);
            }

            switch (source)
            {
                case MetricsSource.PowerMetrics:
                    module.PowerMetrics = new PowerMetrics(element);
                    break;
                case MetricsSource.FxCop:
                    module.FxCopMetrics = new FxCopMetrics(element);
                    break;
                case MetricsSource.StyleCop:
                    module.StyleCopMetrics = new StyleCopMetrics(element);
                    break;
            }

            element.Descendants("Namespace").ToList().ForEach(x => AddNamespace(module, x, source));
        }

        private void AddNamespace(NewModule module, XElement element, MetricsSource source)
        {
            var namespaceName = element.Attribute("Name").Value;
            var namespaceEl = module.Namespaces.FirstOrDefault(x => x.Name == namespaceName);

            if (namespaceEl == null)
            {
                namespaceEl = new NewNamespace();
                namespaceEl.Name = namespaceName;
                module.Namespaces.Add(namespaceEl);
            }

            switch (source)
            {
                case MetricsSource.PowerMetrics:
                    namespaceEl.PowerMetrics = new PowerMetrics(element);
                    break;
                case MetricsSource.FxCop:
                    namespaceEl.FxCopMetrics = new FxCopMetrics(element);
                    break;
                case MetricsSource.StyleCop:
                    namespaceEl.StyleCopMetrics = new StyleCopMetrics(element);
                    break;
            }

            element.Descendants("Type").ToList().ForEach(x => AddType(namespaceEl, x, source));
        }

        private void AddType(NewNamespace namespaceEl, XElement element, MetricsSource source)
        {
            var typeName = element.Attribute("Name").Value;
            var type = namespaceEl.Types.FirstOrDefault(x => x.Name == typeName);

            if (type == null)
            {
                type = new NewType();
                type.Name = typeName;
                namespaceEl.Types.Add(type);
            }

            switch (source)
            {
                case MetricsSource.PowerMetrics:
                    type.PowerMetrics = new PowerMetrics(element);
                    break;
                case MetricsSource.FxCop:
                    type.FxCopMetrics = new FxCopMetrics(element);
                    break;
                case MetricsSource.StyleCop:
                    type.StyleCopMetrics = new StyleCopMetrics(element);
                    break;
            }

            element.Descendants("Member").ToList().ForEach(x => AddMember(type, x, source));
        }

        private void AddMember(NewType type, XElement element, MetricsSource source)
        {
            var memberName = element.Attribute("Name").Value;
            var member = type.Members.FirstOrDefault(x => x.Name == memberName);

            if (member == null)
            {
                member = new NewMember();
                member.Name = memberName;
                type.Members.Add(member);
            }

            switch (source)
            {
                case MetricsSource.PowerMetrics:
                    member.PowerMetrics = new PowerMetrics(element);
                    break;
                case MetricsSource.FxCop:
                    member.FxCopMetrics = new FxCopMetrics(element);
                    break;
                case MetricsSource.StyleCop:
                    member.StyleCopMetrics = new StyleCopMetrics(element);
                    break;
            }
        }
    }

    public class NewModule
    {
        public NewModule()
        {
            Namespaces = new List<NewNamespace>();
        }

        public string Name { get; set; }
        public PowerMetrics PowerMetrics { get; set; }
        public FxCopMetrics FxCopMetrics { get; set; }
        public StyleCopMetrics StyleCopMetrics { get; set; }

        public List<NewNamespace> Namespaces { get; set; }
    }

    public class NewNamespace
    {
        public NewNamespace()
        {
            Types = new List<NewType>();
        }

        public string Name { get; set; }
        public PowerMetrics PowerMetrics { get; set; }
        public FxCopMetrics FxCopMetrics { get; set; }
        public StyleCopMetrics StyleCopMetrics { get; set; }

        public List<NewType> Types { get; set; }
    }

    public class NewType
    {
        public NewType()
        {
            Members = new List<NewMember>();
        }

        public string Name { get; set; }
        public PowerMetrics PowerMetrics { get; set; }
        public FxCopMetrics FxCopMetrics { get; set; }
        public StyleCopMetrics StyleCopMetrics { get; set; }

        public List<NewMember> Members { get; set; }
    }

    public class NewMember
    {
        public string Name { get; set; }
        public PowerMetrics PowerMetrics { get; set; }
        public FxCopMetrics FxCopMetrics { get; set; }
        public StyleCopMetrics StyleCopMetrics { get; set; } 
    }

    public class PowerMetrics
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

    public class FxCopMetrics
    {
        public List<Message> Messages { get; set; }
        public int NumberOfIssues { 
            get { return Messages.Count; }
        }

        public FxCopMetrics()
        {
            
        }

        public FxCopMetrics(XElement element)
        {
            Messages = new List<Message>();

            var messages = element.Element("Messages");
            if (messages != null)
            {
                foreach (var xElement in messages.Elements("Message"))
                {
                    var message = new Message();
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

    public class Message
    {
        public string TypeName { get; set; }
        public string Category { get; set; }
        public string CheckId { get; set; }
        public string FixCategory { get; set; }
        public int Certainty { get; set; }
        public string Level { get; set; }
        public int LineNumber { get; set; }
        public string Description { get; set; }
        
    }

    public class StyleCopMetrics
    {
        public List<Violation> Violations { get; set; }
        public int NumberOfIssues
        {
            get { return Violations.Count; }
        }

        public StyleCopMetrics()
        {
        }

        public StyleCopMetrics(XElement element)
        {
            Violations = new List<Violation>();

            var violationsXml = element.Element("Violations");
            foreach (var violationElement in violationsXml.Elements("Violation"))
            {
                var violation = new Violation();
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

    public class Violation
    {
        public string RuleId { get; set; }
        public string Kind { get; set; }
        public string RuleNamespace { get; set; }
        public string Rule { get; set; }
        public int LineNumber { get; set; }
        public string RuleGroup { get; set; }
        public string Message { get; set; }
    }
}
