using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlamer.Infra.Models.Metrics
{
    public class StyleCopIssue
    {
        public string RuleId { get; set; }
        public string Kind { get; set; }
        public string RuleNamespace { get; set; }
        public string Rule { get; set; }
        public int LineNumber { get; set; }
        public string RuleGroup { get; set; }
        public string Message { get; set; }
    }
}
