using System;
using System.Collections.Generic;

namespace CodeBlamer.Infra.Models
{
    public class Commits
    {
        public Commits()
        {
            Modules = new List<Module>();
        }

        public string Author { get; set; }
        public string SHA { get; set; }
        public DateTime Date { get; set; }
        public List<Module> Modules { get; set; }
    }
}
