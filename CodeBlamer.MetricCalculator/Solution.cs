using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CodeBlamer.Infra;
using CodeBlamer.Infra.Extensions;

namespace CodeBlamer.MetricCalculator
{
    class Solution
    {
        public string SolutionPath { get; set; }
        public List<string> Projects { get; set; }
        private readonly RepositoryUrl _repositoryUrl;
        private readonly string _commit;

        public Solution(RepositoryUrl repositoryUrl, string commit)
        {
            _repositoryUrl = repositoryUrl;
            _commit = commit;

            var solutionFiles = GetVersionPath().SearchFor("*.sln");
            SolutionPath = solutionFiles[0].FullName;
            Projects = GetProjects();
        }

        public string GetVersionPath()
        {
            return _repositoryUrl.GetVersionPath(_commit);
        }

        public string GetBuildPath()
        {
            return _repositoryUrl.GetBuildPath(_commit);
        }

        public string GetResultPath(string projectName)
        {
            return _repositoryUrl.GetResultPath(_commit, projectName);
        }

        private List<string> GetProjects()
        {
            var content = File.ReadAllText(SolutionPath);
            Regex projReg = new Regex("Project\\(\"\\{[\\w-]*\\}\"\\) = \"([\\w _]*.*)\", \"(.*\\.(cs|vcx|vb)proj)\"" , RegexOptions.Compiled);
            var matches = projReg.Matches(content).Cast<Match>();
            var projects = matches.Select(x => x.Groups[1].Value).ToList();
            return projects;
        }       

        public void Build()
        {
            //strCommandParameters are parameters to pass to program
            var parameters = "\"{0}\" /t:build /m:4 /nr:true /p:OutputPath={1} /nologo";

            "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\MSBuild.exe".Run(string.Format(parameters, SolutionPath, GetBuildPath()));
        }

        public void CalculateMetrics()
        {
            Projects.ForEach(CalculateMetricsForProject);
        }

        private void CalculateMetricsForProject(string projectName)
        {
            var parameters = "/file:\"{0}\" /out:\"{1}\"";
            var metricsOutputFolder = GetResultPath(projectName);

            metricsOutputFolder.CreateDirectory();

            var metricsOutputPath = metricsOutputFolder + "\\result.xml";

            var file = GetBuildPath().SearchForInSurface(projectName + ".*")[0];

            var dllPath = file.FullName;

            "C:\\Program Files (x86)\\Microsoft Visual Studio 11.0\\Team Tools\\Static Analysis Tools\\FxCop\\Metrics.exe".Run(
                           string.Format(parameters, dllPath, metricsOutputPath));
        }
        
    }
}