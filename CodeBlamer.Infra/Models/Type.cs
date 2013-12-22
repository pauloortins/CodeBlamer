using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeBlamer.Infra.Models
{
    public class Type : Node
    {
        public List<Member> Members { get; set; }

        public Type(XElement element) : base(element)
        {
            var members = element.Element("Members").Elements("Member");
            Members = members.Select(x => new Member(x)).ToList();
        }
    }
}
