using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace CodeBlamer.RepositoryRetriever
{
    public class Project
    {
        public ObjectId Id { get; set; }
        public string RepositoryAuthor { get; set; }
        public string RepositoryName { get; set; }
        public string ReposiroryUrl { get; set; }
        public List<Commits> Commits { get; set; }
    }
}
