using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace CodeBlamer.Infra
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

        public string GetFolderPath(string repositoryUrl)
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

        public string GetCommitFolder(string repositoryUrl, string commit)
        {
            return GetFolderPath(repositoryUrl).Replace("Repositories", "Versions") + "/" + commit;
        }

        public void GenerateSpecificVersion(string repositoryUrl, string commit)
        {
            var repositoryFolder = GetFolderPath(repositoryUrl);
            var repository = new Repository(repositoryFolder + "/.git");
            repository.Checkout(commit);

            var commitFolder = GetCommitFolder(repositoryUrl, commit);
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
