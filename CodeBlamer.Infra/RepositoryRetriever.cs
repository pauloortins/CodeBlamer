using System.IO;
using System.Linq;
using CodeBlamer.Infra.Models;
using LibGit2Sharp;

namespace CodeBlamer.Infra
{
    public class RepositoryRetriever
    {
        public void AddRepository(RepositoryUrl repositoryUrl)
        {
            CloneProject(repositoryUrl);

            var commits = GetCommits(repositoryUrl);
            InsertCommits(repositoryUrl.Url, commits);
        }

        private void CloneProject(RepositoryUrl repositoryUrl)
        {
            var pathResolver = new PathResolver(repositoryUrl.Url);
            Repository.Clone(repositoryUrl.Url, pathResolver.GetRepositoryPath());
        }

        private IQueryable<Commit> GetCommits(RepositoryUrl repositoryUrl)
        {
            var pathResolver = new PathResolver(repositoryUrl.Url);
            var repository = new Repository(pathResolver.GetRepositoryPath());
            return repository.Commits.OrderByDescending(x => x.Author.When.UtcDateTime).Take(1).AsQueryable();
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

        private void InsertCommits(string repositoryUrl, IQueryable<Commit> commits)
        {
            var project = new Project()
                {
                    RepositoryUrl = repositoryUrl,
                    RepositoryAuthor = GetRepositoryAuthor(repositoryUrl),
                    RepositoryName = GetRepositoryName(repositoryUrl)
                };

            var colCommits = commits.Take(1).Select(x => new Commits { Author = x.Author.Name, Date = x.Author.When.UtcDateTime, SHA = x.Sha, RepositoryUrl = repositoryUrl}).ToList();

            new MongoRepository().InsertProject(project, colCommits);
        }

        public void GenerateSpecificVersion(RepositoryUrl repositoryUrl, string commit)
        {
            var pathResolver = new PathResolver(repositoryUrl.Url, commit);

            var repositoryFolder = pathResolver.GetRepositoryPath();
            var repository = new Repository(repositoryFolder + "/.git");
            repository.Checkout(commit);

            var commitFolder = pathResolver.GetVersionPath();
            Directory.CreateDirectory(commitFolder);

            DirectoryCopy(repositoryFolder, commitFolder, true);

            repository.Checkout(repository.Branches["master"]);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
