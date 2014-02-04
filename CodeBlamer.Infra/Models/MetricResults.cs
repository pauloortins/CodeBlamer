using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CodeBlamer.Infra.Models.CodeElements;
using CodeBlamer.Infra.Models.Metrics;
using Type = CodeBlamer.Infra.Models.CodeElements.Type;

namespace CodeBlamer.Infra.Models
{    
    public class MetricResults
    {
        private List<Module> _modules = new List<Module>();

        public Module GetModule()
        {
            return _modules.FirstOrDefault();
        }

        public void AddModule(XElement element, Metric.MetricsSource source)
        {
            var moduleName = element.Attribute("Name").Value;
            var module = _modules.FirstOrDefault(x => x.Name == moduleName);

            if (module == null)
            {
                module = new Module();
                module.Name = moduleName;
                _modules.Add(module);
            }

            module.AddMetrics(Metric.CreateMetric(element, source));

            element.Descendants("Namespace").ToList().ForEach(x => AddNamespace(module, x, source));
        }

        private void AddNamespace(Module module, XElement element, Metric.MetricsSource source)
        {
            var namespaceName = element.Attribute("Name").Value;
            var namespaceEl = module.Namespaces.FirstOrDefault(x => x.Name == namespaceName);

            if (namespaceEl == null)
            {
                namespaceEl = new Namespace();
                namespaceEl.Name = namespaceName;
                module.Namespaces.Add(namespaceEl);
            }

            namespaceEl.AddMetrics(Metric.CreateMetric(element, source));

            element.Descendants("Type").ToList().ForEach(x => AddType(namespaceEl, x, source));
        }

        private void AddType(Namespace namespaceEl, XElement element, Metric.MetricsSource source)
        {
            var typeName = element.Attribute("Name").Value;
            var type = namespaceEl.Types.FirstOrDefault(x => x.Name == typeName);

            if (type == null)
            {
                type = new Type();
                type.Name = typeName;
                namespaceEl.Types.Add(type);
            }

            type.AddMetrics(Metric.CreateMetric(element, source));

            element.Descendants("Member").ToList().ForEach(x => AddMember(type, x, source));
        }

        private void AddMember(Type type, XElement element, Metric.MetricsSource source)
        {
            var memberName = element.Attribute("Name").Value;
            var member = type.Members.FirstOrDefault(x => x.Name == memberName);

            if (member == null)
            {
                member = new Member();
                member.Name = memberName;
                type.Members.Add(member);
            }

            member.AddMetrics(Metric.CreateMetric(element, source));
        }
    }                    
}
