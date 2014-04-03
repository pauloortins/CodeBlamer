using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CodeBlamer.Infra;
using CodeBlamer.Infra.Extensions;
using CodeBlamer.Infra.Models;
using CodeBlamer.Infra.Models.CodeElements;
using CodeBlamer.MetricCalculator.Metrics;

namespace CodeBlamer.MetricCalculator
{
    class Solution
    {
        public string SolutionPath { get; set; }
        public List<string> Projects { get; set; }
        private readonly RepositoryUrl _repositoryUrl;
        private readonly string _commit;
        private readonly PathResolver _pathResolver;
        private readonly List<MetricService> _metricServices;

        public Solution(RepositoryUrl repositoryUrl, string commit)
        {
            _repositoryUrl = repositoryUrl;
            _commit = commit;
            _pathResolver = new PathResolver(repositoryUrl.Url, commit);

            _metricServices = new List<MetricService>()
                {
                    new PowerMetricsService(_pathResolver)                    
                };

            var solutionFiles = GetVersionPath().SearchFor("*.sln");
            SolutionPath = solutionFiles[0].FullName;
            Projects = GetProjects();
        }

        public string GetSolutionName()
        {
            var solutionFiles = GetVersionPath().SearchFor("*.sln");
            return solutionFiles[0].Name;
        }

        public string GetVersionPath()
        {
            return _pathResolver.GetVersionPath();
        }

        public string GetBuildPath()
        {
            return _pathResolver.GetBuildPath();
        }

        public string GetResultPath()
        {
            return _pathResolver.GetResultPath();
        }

        public string GetResultXmlPath(FileInfo fileInfo)
        {
            return _pathResolver.GetResultXmlPath(fileInfo);
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
            var parameters = "\"{0}\" /t:build /m:4 /nr:true /nologo /p:CustomAfterMicrosoftCommonTargets=C:\\CodeBlamer\\filter.targets";
            var path = GetVersionPath();

            var cdCommand = string.Format("C: && cd {0}", path);
            var buildCommand = "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\MSBuild.exe " + string.Format(parameters, GetSolutionName());

            CommandExtensions.Run(new string[] {cdCommand, buildCommand});

            var files = new DirectoryInfo(GetVersionPath()).SearchFor("System.Core.dll");
            files.ForEach(x => x.Delete());
        }

        public void CalculateMetrics()
        {
            var compiledDlls = GetCompiledDlls();
            compiledDlls.ForEach(CalculateMetricsForDll);
        }

        private void CalculateMetricsForDll(FileInfo dll)
        {
            _metricServices.ForEach(x => x.CalculateMetrics(dll));
        }

        private List<FileInfo> GetCompiledDlls()
        {
            var path = GetVersionPath();

            var directories = from subdirectory in Directory.GetDirectories(path, "obj", SearchOption.AllDirectories) 
                              select subdirectory + "/Debug";

            return directories.SelectMany<string, FileInfo>(x =>
                {
                    var dlls = x.SearchForInSurface("*.dll");
                    var exes = x.SearchForInSurface("*.exe");
                    dlls.AddRange(exes);

                    return dlls;
                }).ToList();
            
        }

        public void SaveMetrics()
        {
            var modules = GetCompiledDlls().Select(GetMetricsForDll).ToList();
            var mongo = new MongoRepository();
            mongo.SaveMetrics(_repositoryUrl, _commit, modules);
        }
        
        private Module GetMetricsForDll(FileInfo fileInfo)
        {
            var result = new MetricResults();

            _metricServices.ForEach(service =>
                {
                    var document = XDocument.Load(service.GenerateResultPath(fileInfo));
                    var moduleXml = document.Root.Descendants("Module").FirstOrDefault();
                    var projFileName = fileInfo.Directory.Parent.Parent.SearchFor("*.csproj")[0].Name;
                    moduleXml.Attribute("Name").SetValue(projFileName);

                    result.AddModule(moduleXml, service.GetMetricSource());
                });

            return result.GetModule();
        }
    }
}