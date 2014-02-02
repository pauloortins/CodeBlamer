using System;
using System.Collections.Generic;

namespace CodeBlamer.Infra.Models
{
    public class Commits
    {
        public Commits()
        {
            Modules = new List<NewModule>();
        }

        public string Author { get; set; }
        public string SHA { get; set; }
        public DateTime Date { get; set; }
        public List<NewModule> Modules { get; set; }
    }
}
