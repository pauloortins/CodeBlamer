using System;
using System.Collections.Generic;
using CodeBlamer.Infra.Models.CodeElements;
using MongoDB.Bson;

namespace CodeBlamer.Infra.Models
{
    public class Commits
    {
        public Commits()
        {
            Modules = new List<Module>();
        }

        public ObjectId Id { get; set; }
        public string RepositoryUrl { get; set; }
        public string Author { get; set; }
        public string SHA { get; set; }
        public DateTime Date { get; set; }
        public List<Module> Modules { get; set; }
    }
}
