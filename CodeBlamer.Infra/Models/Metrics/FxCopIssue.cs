using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlamer.Infra.Models.Metrics
{
    public class FxCopIssue
    {
        public string TypeName { get; set; }
        public string Category { get; set; }
        public string CheckId { get; set; }
        public string FixCategory { get; set; }
        public int Certainty { get; set; }
        public string Level { get; set; }
        public int LineNumber { get; set; }
        public string Description { get; set; }
    }
}
