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
                repository.AddRepository(repositoryUrl);

                WriteToConsole("Generating Versions");
                GenerateAllVersions(repositoryUrl);

                WriteToConsole("Calculating Metrics");
                CalculateMetrics(repositoryUrl);                

                mongo.DeleteUrl(repositoryUrl);
            }
        }

        private static void CalculateMetrics(RepositoryUrl repositoryUrl)
        {
            var mongo = new MongoRepository();
            var projects = mongo.GetProjects().First(x => x.ReposiroryUrl.Equals(repositoryUrl.Url));

            projects.Commits.ForEach(x =>
                {
                    var solution = new Solution(repositoryUrl, x.SHA);
                    solution.Build();
                    solution.CalculateMetrics();
                });
        }

        private static void WriteToConsole(string msg)
        {
            Console.WriteLine("{0} - {1}", DateTime.Now, msg);
        }

        private static void GenerateAllVersions(RepositoryUrl repositoryUrl)
        {
            var mongo = new MongoRepository();
            var projects = mongo.GetProjects().First(x => x.ReposiroryUrl.Equals(repositoryUrl.Url));

            projects.Commits.ForEach(x => GenerateVersion(repositoryUrl, x.SHA));
        }

        private static void GenerateVersion(RepositoryUrl repositoryUrl, string commit)
        {
            var repository = new Infra.RepositoryRetriever();
            repository.GenerateSpecificVersion(repositoryUrl,commit);
        }
    }
}
