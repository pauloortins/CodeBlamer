using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeBlamer.Infra.Models
{
    public class Member : Node
    {
        public Member(XElement element) : base(element)
        {

        }
    }
}
