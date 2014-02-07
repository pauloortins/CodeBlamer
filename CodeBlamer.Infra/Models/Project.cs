using System.Collections.Generic;
using MongoDB.Bson;

namespace CodeBlamer.Infra.Models
{
    public class Project
    {
        public ObjectId Id { get; set; }
        public string RepositoryAuthor { get; set; }
        public string RepositoryName { get; set; }
        public string RepositoryUrl { get; set; }
    }
}
