using System.Collections.Generic;
using System.Linq;
using CodeBlamer.Infra.Models;
using CodeBlamer.Infra.Models.CodeElements;
using LibGit2Sharp;
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

        public void InsertProject(Project project, List<Commits> commits)
        {
            var database = GetDatabase();
            var projects = database.GetCollection<Project>("projects");

            projects.Insert(project);

            var colCommits = database.GetCollection<Commits>("commits");
            colCommits.InsertBatch(commits);
        }

        public List<Commits> GetCommits()
        {
            var database = GetDatabase();
            var collection = database.GetCollection<Commits>("commits");
            return collection.FindAllAs<Commits>().ToList();
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

        public void SaveMetrics(RepositoryUrl repositoryUrl, string commitSHA, List<Module> modules)
        {
            var colCommits = GetDatabase().GetCollection<Commits>("commits");

            var commit = colCommits.FindOne(Query.EQ("SHA", commitSHA));
            commit.Modules = modules;

            colCommits.Save(commit);
        }
    }
}
