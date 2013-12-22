using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlamer.Infra.Extensions
{
    public static class DirectoryExtensions
    {
        public static List<FileInfo> SearchFor(this DirectoryInfo directoryInfo, string searchString)
        {
            var solutionFiles = directoryInfo.GetFiles(searchString).ToList();
            var subDirs = directoryInfo.GetDirectories();

            foreach (var subDir in subDirs)
            {
                solutionFiles.AddRange(subDir.SearchFor(searchString));
            }

            return solutionFiles;
        }

        public static List<FileInfo> SearchFor(this string path, string searchString)
        {
            return new DirectoryInfo(path).SearchFor(searchString);
        }

        public static List<FileInfo> SearchForInSurface(this string path, string searchString)
        {
            return new DirectoryInfo(path).GetFiles(searchString).ToList();
        }

        public static void CreateDirectory(this string path)
        {
            Directory.CreateDirectory(path);
        }
    }
}
