using System.Collections.Generic;
using CodeBlamer.Infra.Models.Metrics;

namespace CodeBlamer.Infra.Models.CodeElements
{
    public class Module : CodeElement
    {
        public Module()
        {
            Namespaces = new List<Namespace>();
        }
        
        public List<Namespace> Namespaces { get; set; }        
    }
}
