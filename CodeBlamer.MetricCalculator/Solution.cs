using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CodeBlamer.Infra;

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

            var solutionFiles = SearchInFolder(new DirectoryInfo(GetVersionPath()));
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

        private List<FileInfo> SearchInFolder(DirectoryInfo directoryInfo)
        {
            var solutionFiles = directoryInfo.GetFiles("*.sln").ToList();
            var subDirs = directoryInfo.GetDirectories();

            foreach (var subDir in subDirs)
            {
                solutionFiles.AddRange(SearchInFolder(subDir));
            }

            return solutionFiles;
        }

        public void Build()
        {
            //strCommandParameters are parameters to pass to program
            var parameters = "\"{0}\" /t:build /m:4 /nr:true /p:OutputPath={1} /nologo";

            RunExternalExe("C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\MSBuild.exe",
                           string.Format(parameters, SolutionPath, GetBuildPath()));
            
        }

        public void CalculateMetrics()
        {
            Projects.ForEach(CalculateMetricsForProject);
        }

        private void CalculateMetricsForProject(string projectName)
        {
            var parameters = "/file:\"{0}\" /out:\"{1}\"";
            var metricsOutputFolder = GetResultPath(projectName);

            Directory.CreateDirectory(metricsOutputFolder);

            var metricsOutputPath = metricsOutputFolder + "\\result.xml";

            var file = new DirectoryInfo(GetBuildPath()).GetFiles(projectName + ".*")[0];

            var dllPath = file.FullName;

            RunExternalExe("C:\\Program Files (x86)\\Microsoft Visual Studio 11.0\\Team Tools\\Static Analysis Tools\\FxCop\\Metrics.exe",
                           string.Format(parameters, dllPath, metricsOutputPath));
        }

        public string RunExternalExe(string filename, string arguments = null)
        {
            var process = new Process();

            process.StartInfo.FileName = filename;
            if (!string.IsNullOrEmpty(arguments))
            {
                process.StartInfo.Arguments = arguments;
            }

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            var stdOutput = new StringBuilder();
            process.OutputDataReceived += (sender, args) => stdOutput.Append(args.Data);

            string stdError = null;
            try
            {
                process.Start();
                process.BeginOutputReadLine();
                stdError = process.StandardError.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                throw new Exception("OS error while executing " + Format(filename, arguments) + ": " + e.Message, e);
            }

            if (process.ExitCode == 0)
            {
                return stdOutput.ToString();
            }
            
            var message = new StringBuilder();

            if (!string.IsNullOrEmpty(stdError))
            {
                message.AppendLine(stdError);
            }

            if (stdOutput.Length != 0)
            {
                message.AppendLine("Std output:");
                message.AppendLine(stdOutput.ToString());
            }

            throw new Exception(Format(filename, arguments) + " finished with exit code = " + process.ExitCode + ": " + message);
        }

        private string Format(string filename, string arguments)
        {
            return "'" + filename +
                   ((string.IsNullOrEmpty(arguments)) ? string.Empty : " " + arguments) +
                   "'";
        }
    }
}