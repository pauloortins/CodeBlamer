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
        public FxCopMetrics(XElement element)
        {
            
        }
    }

    public class StyleCopMetrics
    {
        public StyleCopMetrics(XElement element)
        {
            
        }
    }
}
