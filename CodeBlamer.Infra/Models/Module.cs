using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeBlamer.Infra.Models
{
    public class Module : Node
    {
        public List<Namespace> Namespaces { get; set; }

        public Module(XElement element) : base(element)
        {
            var namespaces = element.Element("Namespaces").Elements("Namespace");
            Namespaces = namespaces.Select(x => new Namespace(x)).ToList();
        }
    }
}
