using System.Collections.Generic;
using CodeBlamer.Infra.Models.Metrics;

namespace CodeBlamer.Infra.Models.CodeElements
{
    public class Namespace : CodeElement
    {
        public Namespace()
        {
            Types = new List<Type>();
        }

        public List<Type> Types { get; set; }
    }
}
