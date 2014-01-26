using System.IO;
using MongoDB.Bson;

namespace CodeBlamer.Infra
{
    public class RepositoryUrl
    {
        public ObjectId Id { get; set; }
        public string Url { get; set; }        
     }
}
