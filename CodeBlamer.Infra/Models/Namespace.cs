using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeBlamer.Infra.Models
{
    public class Namespace : Node
    {
        public List<Type> Types { get; set; }

        public Namespace(XElement element) : base(element)
        {
            var types = element.Element("Types").Elements("Type");
            Types = types.Select(x => new Type(x)).ToList();
        }
    }
}
