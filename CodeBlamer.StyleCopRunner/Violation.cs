using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using StyleCop;
using StyleCop.CSharp;

namespace CodeBlamer.StyleCopRunner
{
    public class Violation
    {
        public string ClassName { get; set; }
        public string FullName { get; set; }
        public string Declaration { get; set; }
        public string RuleId { get; set; }
        public string Kind { get; set; }
        public string RuleNamespace { get; set; }
        public string Rule { get; set; }
        public string LineNumber { get; set; }
        public string RuleGroup { get; set; }
        public string Message { get; set; }
        public string Namespace { get; set; }

        public Violation(ViolationEventArgs eventArgs)
        {
            var className = eventArgs.Element.Document.SourceCode.Name;

            var csElement = (CsElement)eventArgs.Element;
            var fullName = csElement.FullNamespaceName;
            var declarationName = csElement.Declaration.Name;

            ClassName = className;
            FullName = fullName;
            Declaration = declarationName;
            RuleId = eventArgs.Violation.Rule.CheckId;
            Kind = csElement.GetType().Name;
            RuleNamespace = eventArgs.Violation.Rule.Namespace;
            Rule = eventArgs.Violation.Rule.Name;
            LineNumber = eventArgs.Violation.Line.ToString();
            RuleGroup = eventArgs.Violation.Rule.RuleGroup;
            Message = eventArgs.Violation.Rule.Context;
            Namespace = GetNameSpace(fullName, className, declarationName, csElement.GetType().Name);
        }

        private string GetNameSpace(string fullName, string className, string declarationName, string type)
        {
            fullName = fullName.Replace("Root.", string.Empty);
            if (type == "Namespace")
            {
                return fullName;
            }

            return fullName.Replace("." + className.Split('.')[0], "").Replace("." + declarationName, "");
        }

        public XElement ToXmlElement()
        {
            var violation = new XElement("Violation");
            violation.SetAttributeValue("ClassName", ClassName);
            violation.SetAttributeValue("FullName", FullName);
            violation.SetAttributeValue("Declaration", Declaration);
            violation.SetAttributeValue("RuleId", RuleId);
            violation.SetAttributeValue("Kind", Kind);
            violation.SetAttributeValue("RuleNamespace", RuleNamespace);
            violation.SetAttributeValue("Rule", Rule);
            violation.SetAttributeValue("LineNumber", LineNumber);
            violation.SetAttributeValue("RuleGroup", RuleGroup);
            violation.SetAttributeValue("Message", Message);
            violation.SetAttributeValue("NameSpace", GetNameSpace(FullName, ClassName, Declaration, Kind));

            return violation;
        }
    }
}
