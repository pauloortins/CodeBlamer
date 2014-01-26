using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlamer.Infra.Models
{
    public class PathResolver
    {
        private readonly string _url;
        private readonly string _commit;

        public PathResolver(string url, string commit = "")
        {
            _url = url;
            _commit = commit;
        }

        public string GetRepositoryPath()
        {
            return "E:/CodeBlamer/Repositories/" + _url.Replace("https://github.com/", string.Empty);
        }

        public string GetVersionPath()
        {
            return GetRepositoryPath().Replace("/", "\\").Replace("Repositories", "Versions") + "\\" + _commit;
        }

        public string GetBuildPath()
        {
            return GetRepositoryPath().Replace("/", "\\").Replace("Repositories", "Builds") + "\\" + _commit;
        }

        public string GetResultPath()
        {
            return GetRepositoryPath().Replace("/", "\\").Replace("Repositories", "Results") + "\\" + _commit;
        }

        public string GetResultXmlPath(FileInfo dll)
        {
            return GetResultPath() + "\\" + dll.DirectoryName.Replace(GetVersionPath(), "").Replace("\\", "").Replace("objDebug", "") + ".xml";
        }
    }
}
