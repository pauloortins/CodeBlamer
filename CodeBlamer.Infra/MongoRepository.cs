using System.Collections.Generic;
using System.Linq;
using CodeBlamer.Infra.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace CodeBlamer.Infra
{
    public class MongoRepository
    {
        private const string _connectionString = "mongodb://localhost";
        
        private MongoDatabase GetDatabase()
        {
            var client = new MongoClient(_connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("codeblamerdb");
            return database;
        }
        
        public void InsertProject(Project project)
        {
            var database = GetDatabase();
            var collection = database.GetCollection<Project>("projects");

            collection.Insert(project);
        }

        public List<Project> GetProjects()
        {
            var database = GetDatabase();
            var collection = database.GetCollection<Project>("projects");
            return collection.FindAllAs<Project>().ToList();
        }

        public void InsertUrl(string url)
        {
            var database = GetDatabase();
            var collection = database.GetCollection<RepositoryUrl>("urls");
            collection.Insert(new RepositoryUrl() {Url = url});
        }

        public RepositoryUrl GetUrl()
        {
            var database = GetDatabase();
            var collection = database.GetCollection<RepositoryUrl>("urls");
            return collection.FindOne();
        }

        public void DeleteUrl(RepositoryUrl url)
        {
            var database = GetDatabase();
            var collection = database.GetCollection<RepositoryUrl>("urls");
            collection.Remove(Query.EQ("_id", url.Id));
        }

        public void SaveMetrics(RepositoryUrl repositoryUrl, string commit, List<NewModule> modules)
        {
            var projects = GetDatabase().GetCollection<Project>("projects");

            var project = projects.FindOne(Query.EQ("RepositoryUrl", repositoryUrl.Url));
            var commitProject = project.Commits.First(x => x.SHA == commit);
            commitProject.Modules = modules;

            projects.Save(project);
        }
    }
}
