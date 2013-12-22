using System.IO;
using System.Linq;
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
            Repository.Clone(repositoryUrl.Url, repositoryUrl.GetRepositoryPath());
        }

        private IQueryableCommitLog GetCommits(RepositoryUrl repositoryUrl)
        {
            var repository = new Repository(repositoryUrl.GetRepositoryPath());
            return repository.Commits;
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
            var project = new Project()
                {
                    ReposiroryUrl = repositoryUrl,
                    RepositoryAuthor = GetRepositoryAuthor(repositoryUrl),
                    RepositoryName = GetRepositoryName(repositoryUrl)
                };

            project.Commits =
                commits.Select(x => new Commits {Author = x.Author.Name, Date = x.Author.When.UtcDateTime, SHA = x.Sha}).ToList();

            new MongoRepository().InsertProject(project);
        }

        public void GenerateSpecificVersion(RepositoryUrl repositoryUrl, string commit)
        {
            var repositoryFolder = repositoryUrl.GetRepositoryPath();
            var repository = new Repository(repositoryFolder + "/.git");
            repository.Checkout(commit);

            var commitFolder = repositoryUrl.GetVersionPath(commit);
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
