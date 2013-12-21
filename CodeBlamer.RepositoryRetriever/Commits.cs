using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace CodeBlamer.RepositoryRetriever
{
    public class Commits
    {
        public string Author { get; set; }
        public string SHA { get; set; }
        public DateTime Date { get; set; }
    }
}
