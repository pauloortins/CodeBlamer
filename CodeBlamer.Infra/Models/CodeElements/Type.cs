using System.Collections.Generic;
using CodeBlamer.Infra.Models.Metrics;

namespace CodeBlamer.Infra.Models.CodeElements
{
    public class Type : CodeElement
    {
        public Type()
        {
            Members = new List<Member>();
        }

        public List<Member> Members { get; set; }
    }
}
