using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace CodeBlamer.RepositoryRetriever
{
    public class RepositoryRetriever
    {
        public void AddRepository(string repositoryUrl)
        {
            CloneProject(repositoryUrl);

            var commits = GetCommits(repositoryUrl);
            InsertCommits(repositoryUrl, commits);
        }

        private void CloneProject(string repositoryUrl)
        {
            Repository.Clone(repositoryUrl, GetFolderPath(repositoryUrl));
        }

        private IQueryableCommitLog GetCommits(string repositoryUrl)
        {
            var repository = new Repository(GetFolderPath(repositoryUrl));
            return repository.Commits;
        }

        private string GetFolderPath(string repositoryUrl)
        {
            return "E:/CodeBlamer/Repositories/" + repositoryUrl.Replace("https://github.com/", string.Empty);
        }

        private string GetRepositoryAuthor(string repositoryUrl)
        {
            return
                repositoryUrl.Replace("https://github.com/", string.Empty).Replace(".git", string.Empty).Split('/')[0];
        }

        private string GetRepositoryName(string repositoryUrl)
        {
            return
                repositoryUrl.Replace("https://github.com/", string.Empty).Replace(".git", string.Empty).Split('/')[1];
        }

        private void InsertCommits(string repositoryUrl, IQueryableCommitLog commits)
        {
            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("codeblamerdb");
            var collection = database.GetCollection<Project>("projects");

            var project = new Project()
                {
                    ReposiroryUrl = repositoryUrl,
                    RepositoryAuthor = GetRepositoryAuthor(repositoryUrl),
                    RepositoryName = GetRepositoryName(repositoryUrl)
                };

            project.Commits =
                commits.Select(x => new Commits {Author = x.Author.Name, Date = x.Author.When.UtcDateTime, SHA = x.Sha}).ToList();

            collection.Insert(project);
        }
    }
}
