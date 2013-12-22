using MongoDB.Bson;

namespace CodeBlamer.Infra
{
    public class RepositoryUrl
    {
        public ObjectId Id { get; set; }
        public string Url { get; set; }

        public string GetRepositoryPath()
        {
            return "E:/CodeBlamer/Repositories/" + Url.Replace("https://github.com/", string.Empty);
        }

        public string GetVersionPath(string commit)
        {
            return GetRepositoryPath().Replace("Repositories", "Versions") + "/" + commit;
        }

        public string GetBuildPath(string commit)
        {
            return GetRepositoryPath().Replace("Repositories", "Builds") + "/" + commit;
        }

        public string GetResultPath(string commit, string projectName)
        {
            return GetRepositoryPath().Replace("Repositories", "Results") + "/" + commit + "/" + projectName;
        }
     }
}
