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
            var csElement = (CsElement)eventArgs.Element;
            var fullName = csElement.FullNamespaceName;

            ClassName = GetClassName(csElement);
            FullName = fullName;
            Declaration = GetDeclaration(csElement);
            RuleId = eventArgs.Violation.Rule.CheckId;
            Kind = csElement.GetType().Name;
            RuleNamespace = eventArgs.Violation.Rule.Namespace;
            Rule = eventArgs.Violation.Rule.Name;
            LineNumber = eventArgs.Violation.Line.ToString();
            RuleGroup = eventArgs.Violation.Rule.RuleGroup;
            Message = eventArgs.Violation.Rule.Context;
            Namespace = GetNameSpace(csElement);
        }

        private string GetDeclaration(CsElement codeElement)
        {
            if (codeElement.GetType().Name == "Method")
            {
                var methodDeclaration = (Method)codeElement;

                var parameters = string.Join(",", methodDeclaration.Parameters.Select(x => x.Type.Text));
                var returnType = methodDeclaration.ReturnType != null ? " : " + methodDeclaration.ReturnType.Text : string.Empty;

                var declaration = methodDeclaration.Declaration.Name + "(" + parameters + ")" + returnType;

                return declaration.Replace(",", ", ");
            }

            if (codeElement.GetType().Name == "Constructor")
            {
                var constructorDeclaration = (Constructor)codeElement;
                var parameters = string.Join(",", constructorDeclaration.Parameters.Select(x => x.Type.Text));
                var declaration = constructorDeclaration.Declaration.Name + "(" + parameters + ")";

                return declaration.Replace(",", ", ");
            }

            if (codeElement.GetType().Name == "Accessor")
            {
                var accessor = (Accessor)codeElement;
                var property = (Property)codeElement.Parent;
                var parameters = string.Join(", ", accessor.Parameters.Select(x => x.Type.Text));
                var returnType = accessor.ReturnType != null ? " : " + accessor.ReturnType.Text : string.Empty;

                var declaration = property.Declaration.Name + "." + accessor.Declaration.Name + "(" + parameters + ")" + returnType;
                return declaration.Replace(",", ", ");
            }

            return codeElement.Declaration.Name.Replace(",", ", ");
        }

        private string GetClassName(CsElement codeElement)
        {
            if (codeElement.GetType().Name == "DocumentRoot" ||
                codeElement.GetType().Name == "Namespace" ||
                codeElement.GetType().Name == "AssemblyOrModuleAttribute")
            {
                return string.Empty;
            }

            if (codeElement.GetType().Name == "UsingDirective")
            {
                return "UsingDirective";
            }

            if (codeElement.GetType().Name == "Class" || codeElement.GetType().Name == "Interface" || codeElement.GetType().Name == "Enum")
            {
                return codeElement.Declaration.Name;
            }

            return GetClassName((CsElement)codeElement.Parent);
        }

        private string GetNameSpace(CsElement codeElement)
        {
            while (codeElement.GetType().Name != "Namespace" && codeElement.GetType().Name != "DocumentRoot")
            {
                codeElement = (CsElement)codeElement.Parent;
            }

            return codeElement.FullNamespaceName.Replace("Root.", string.Empty);
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
            violation.SetAttributeValue("NameSpace", Namespace);

            return violation;
        }
    }
}
