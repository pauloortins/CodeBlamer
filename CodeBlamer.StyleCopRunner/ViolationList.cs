using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeBlamer.StyleCopRunner
{
    public class ViolationList
    {
        private XDocument _document;

        public ViolationList()
        {
            _document = new XDocument();



            var header = new XElement("Modules");
            _document.AddFirst(header);

            var module = new XElement("Module");
            module.SetAttributeValue(XName.Get("Name"), "FluentValidation.Mvc3");

            _document.Descendants().First().Add(module);
        }

        public void AddViolation(Violation violation)
        {
            if (violation.Namespace.Contains("[assembly :"))
                return;

            var module = _document.Descendants(XName.Get("Module")).First();

            AddNodeIfNotExists(module, "Violations");
            AddNodeIfNotExists(module, "Namespaces");

            if (violation.Namespace == "Root")
            {
                var violations = module.Element("Violations");
                violations.Add(violation.ToXmlElement());
                return;
            }


            var namespaces = module.Element(XName.Get("Namespaces"));

            AddNodeIfNotExists(namespaces, "Namespace", violation.Namespace);
            var namespaceNode = namespaces.Elements().First(x => x.Attribute("Name").Value == violation.Namespace);
            AddNodeIfNotExists(namespaceNode, "Violations");

            if (violation.Kind == "Namespace"  || violation.ClassName == "")
            {
                var violations = namespaceNode.Element("Violations");
                violations.Add(violation.ToXmlElement());
                return;
            }

            AddNodeIfNotExists(namespaceNode, "Types");

            var types = namespaceNode.Element("Types");

            AddNodeIfNotExists(types, "Type", violation.ClassName);

            var type = types.Elements().First(x => x.Attribute("Name").Value == violation.ClassName);
            AddNodeIfNotExists(type, "Violations");

            if (violation.Kind == "Class" || violation.Kind == "Interface" || violation.Kind == "Enum")
            {
                var violations = type.Element("Violations");
                violations.Add(violation.ToXmlElement());
                return;
            }

            AddNodeIfNotExists(type, "Members");

            var members = type.Element("Members");

            AddNodeIfNotExists(members, "Member", violation.Declaration);

            var member = members.Elements().First(x => x.Attribute("Name").Value == violation.Declaration);

            AddNodeIfNotExists(member, "Violations");

            var typeViolations = member.Element("Violations");
            typeViolations.Add(violation.ToXmlElement());
        }

        private void AddNodeIfNotExists(XElement destination, string tagName, string nodeName = "")
        {
            var descendantsWithSameTagName = destination.Descendants().Where(x => x.Name == tagName);

            if (string.IsNullOrEmpty(nodeName))
            {
                if (!descendantsWithSameTagName.Any())
                {
                    destination.Add(new XElement(tagName));
                }
            }
            else
            {
                if (descendantsWithSameTagName.All(x => x.Attribute("Name").Value != nodeName))
                {
                    var newNode = new XElement(tagName);
                    newNode.SetAttributeValue(XName.Get("Name"), nodeName);
                    destination.Add(newNode);
                }
            }
        }

        public void Save(string fileName)
        {
            _document.Save(fileName);
        }
    }
}
