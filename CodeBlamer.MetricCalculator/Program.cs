using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeBlamer.Infra;

namespace CodeBlamer.MetricCalculator
{
    class Program
    {
        public static void Main(string[] args)
        {
            Run();
        }

        private static void Run()
        {
            var mongo = new MongoRepository();
            var repository = new RepositoryRetriever();

            while (true)
            {
                var repositoryUrl = mongo.GetUrl();

                if (repositoryUrl == null)
                {
                    WriteToConsole("Url not found");
                    Thread.Sleep(1000);
                    continue;
                }

                WriteToConsole(string.Format("{0} found", repositoryUrl.Url));

                WriteToConsole("Cloning Repository");
                repository.AddRepository(repositoryUrl.Url);

                WriteToConsole("Generating Versions");
                GenerateAllVersions(repositoryUrl.Url);

                WriteToConsole("Calculating Metrics");
                CalculateMetrics(repositoryUrl.Url);                

                mongo.DeleteUrl(repositoryUrl);
            }
        }

        private static void CalculateMetrics(string repositoryUrl)
        {
            var mongo = new MongoRepository();
            var repository = new RepositoryRetriever();
            var projects = mongo.GetProjects().First(x => x.ReposiroryUrl.Equals(repositoryUrl));

            projects.Commits.ForEach(x =>
                {
                    var solutionFolder = repository.GetCommitFolder(repositoryUrl, x.SHA);
                    var solution = new Solution(solutionFolder, x.SHA);
                    solution.Build();
                    solution.CalculateMetrics();
                });
        }

        private static void WriteToConsole(string msg)
        {
            Console.WriteLine("{0} - {1}", DateTime.Now, msg);
        }

        private static void GenerateAllVersions(string repositoryUrl)
        {
            var mongo = new MongoRepository();
            var projects = mongo.GetProjects().First(x => x.ReposiroryUrl.Equals(repositoryUrl));

            projects.Commits.ForEach(x => GenerateVersion(repositoryUrl, x.SHA));
        }

        private static void GenerateVersion(string repositoryUrl, string commit)
        {
            var repository = new Infra.RepositoryRetriever();
            repository.GenerateSpecificVersion(repositoryUrl,commit);
        }
    }
}
