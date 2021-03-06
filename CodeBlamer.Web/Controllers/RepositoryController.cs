﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CodeBlamer.Infra.Models;
using CodeBlamer.Infra.Models.CodeElements;
using CodeBlamer.Infra.Models.Metrics;
using CodeBlamer.Web.Models;
using LibGit2Sharp;
using CodeBlamer.Infra;
using Type = CodeBlamer.Infra.Models.CodeElements.Type;

namespace CodeBlamer.Web.Controllers
{
    public class RepositoryController : Controller
    {
        //
        // GET: /Repository/

        public ActionResult Index()
        {
            var viewModel = new RepositoryListViewModel();
            viewModel.Projects = new MongoRepository().GetProjects();
            return View(viewModel);
        }

        public ActionResult Detail(string repositoryUrl)
        {
            var project = new MongoRepository().GetProjects().First(x => x.RepositoryUrl == repositoryUrl);
            var viewModel = new RepositoryDetailViewModel();

            viewModel.RepositoryName = project.RepositoryName;
            viewModel.RepositoryAuthor = project.RepositoryAuthor;
            viewModel.RepositoryUrl = project.RepositoryUrl;

            return View(viewModel);
        }

        public ActionResult AddRepository(string repositoryUrl)
        {
            new MongoRepository().InsertUrl(repositoryUrl);
            return Json(new{});
        }

        [HttpGet]
        public ActionResult GetProjectTree(string repositoryUrl)
        {
            var commits = new MongoRepository().GetCommits().Where(x => x.RepositoryUrl == repositoryUrl);

            var result = commits.OrderByDescending(x => x.Date).First().Modules.OrderBy(x => x.Name).Select(module => new 
                {
                    label =HttpUtility.HtmlEncode(module.Name),
                    open = false,
                    inode = true,
                    node = HttpUtility.HtmlEncode(module.Name),
                    branch = module.Namespaces.OrderBy(x => x.Name).Select(namespaces => new
                        {
                            label = HttpUtility.HtmlEncode(namespaces.Name),
                            open = false,
                            inode = true,
                            node = HttpUtility.HtmlEncode(module.Name + ">" + namespaces.Name),
                            branch = namespaces.Types.OrderBy(x => x.Name).Select(type => new
                                {
                                    label = HttpUtility.HtmlEncode(type.Name),
                                    open = false,
                                    inode = true,
                                    node = HttpUtility.HtmlEncode(module.Name + ">" + namespaces.Name + ">" + type.Name),
                                    branch = type.Members.OrderBy(x => x.Name).Select(member => new
                                        {
                                            label = HttpUtility.HtmlEncode(member.Name),
                                            open = false,
                                            inode = false,
                                            node = HttpUtility.HtmlEncode(module.Name + ">" + namespaces.Name + ">" + type.Name + ">" + member.Name)
                                        })
                                })
                        })
                });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public ActionResult Graph(string repositoryUrl)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Graph2(string repositoryUrl)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Graph3(string repositoryUrl)
        {
            return View();
        }

        [HttpGet]
        public ActionResult GraphJson(string repositoryUrl)
        {

            var commit = new MongoRepository().GetCommits().Where(x => x.RepositoryUrl == "https://github.com/JeremySkinner/FluentValidation.git").OrderByDescending(x => x.Date).First();

            var data = new
                {
                    name = "FluentValidation",
                    children = commit.Modules.Select(module => new
                        {
                            name = module.Name,
                            maintainabilityIndex = module.PowerMetrics.MaintainabilityIndex,
                            children = module.Namespaces.Select(namespaceEl => new
                                {
                                    name = namespaceEl.Name,
                                    maintainabilityIndex = namespaceEl.PowerMetrics.MaintainabilityIndex,
                                    children = namespaceEl.Types.Select(type => new
                                        {
                                            name = type.Name,
                                            maintainabilityIndex = type.PowerMetrics == null ? 0 : type.PowerMetrics.MaintainabilityIndex,
                                            value = type.PowerMetrics == null || type.PowerMetrics.LinesOfCode == 0 ? 1 : type.PowerMetrics.LinesOfCode
                                        })
                                })
                        })
                };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GraphJson2(string repositoryUrl)
        {

            var commit = new MongoRepository().GetCommits().Where(x => x.RepositoryUrl == "https://github.com/JeremySkinner/FluentValidation.git").OrderByDescending(x => x.Date).First();

            var data = new
            {
                name = "FluentValidation",                
                _children = commit.Modules.Select(module => new
                {
                    name = module.Name,                    
                    _children = module.Namespaces.Select(namespaceEl => new
                    {
                        name = "",
                        _children = namespaceEl.Types.Select(type => new
                        {
                            name = "",
                            size = type.PowerMetrics == null || type.PowerMetrics.LinesOfCode == 0 ? 1 : type.PowerMetrics.LinesOfCode
                        })
                    })
                })
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetNodeMetrics(string repositoryUrl, string node)
        {
            var mongo = new MongoRepository();
            var pieces = HttpUtility.HtmlDecode(node).Split('>');
            var commits = mongo.GetCommits().Where(x => x.RepositoryUrl == repositoryUrl).OrderBy(x => x.Date);

            if (pieces.Length == 1)
            {
                var result = commits.Select(commit => ExtractModuleInfo(commit, pieces[0]));
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            
            if (pieces.Length == 2)
            {
                var result = commits.Select(commit => ExtractNamespaceInfo(commit, pieces[0], pieces[1]));
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (pieces.Length == 3)
            {
                var result = commits.Select(commit => ExtractTypeInfo(commit, pieces[0], pieces[1], pieces[2]));
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (pieces.Length == 4)
            {
                var result = commits.Select(commit => ExtractMemberInfo(commit, pieces[0], pieces[1], pieces[2], pieces[3]));
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        private object ExtractModuleInfo(Commits commit, string moduleName)
        {
            var date = commit.Date;
            var module = commit.Modules != null ? commit.Modules.FirstOrDefault(x => x.Name == moduleName) : null;

            return new
                {
                    Date = date,
                    PowerMetrics = module != null ? module.PowerMetrics : new PowerMetrics(),
                    FxCopMetrics = module != null ? module.FxCopMetrics : new FxCopMetrics(),
                    StyleCopMetrics = module != null ? module.StyleCopMetrics : new StyleCopMetrics()
                };
        }

        private object ExtractNamespaceInfo(Commits commit, string moduleName, string namespaceName)
        {
            var date = commit.Date;
            var module = commit.Modules != null ? commit.Modules.FirstOrDefault(x => x.Name == moduleName) : null;
            Namespace namespc = null;

            if (module != null)
            {
                namespc = module.Namespaces.FirstOrDefault(x => x.Name == namespaceName);
            }

            return new
                {
                    Date = date,
                    PowerMetrics = namespc != null ? namespc.PowerMetrics : new PowerMetrics(),
                    FxCopMetrics = namespc != null ? namespc.FxCopMetrics : new FxCopMetrics(),
                    StyleCopMetrics = namespc != null ? namespc.StyleCopMetrics : new StyleCopMetrics()
                };
        }

        private object ExtractTypeInfo(Commits commit, string moduleName, string namespaceName, string typeName)
        {
            var date = commit.Date;
            var module = commit.Modules != null ? commit.Modules.FirstOrDefault(x => x.Name == moduleName) : null;
            Namespace namespc = null;
            Type type = null;

            if (module != null)
            {
                namespc = module.Namespaces.FirstOrDefault(x => x.Name == namespaceName);

                if (namespc != null)
                {
                    type = namespc.Types.FirstOrDefault(x => x.Name == typeName);
                }
            }

            return new
            {
                Date = date,
                PowerMetrics = type != null ? type.PowerMetrics : new PowerMetrics(),
                FxCopMetrics = type != null ? type.FxCopMetrics : new FxCopMetrics(),
                StyleCopMetrics = type != null ? type.StyleCopMetrics : new StyleCopMetrics()
            };
        }

        private object ExtractMemberInfo(Commits commit, string moduleName, string namespaceName, string typeName, string memberName)
        {
            var date = commit.Date;
            var module = commit.Modules != null ? commit.Modules.FirstOrDefault(x => x.Name == moduleName) : null;
            Namespace namespc = null;
            Type type = null;
            Member member = null;

            if (module != null)
            {
                namespc = module.Namespaces.FirstOrDefault(x => x.Name == namespaceName);

                if (namespc != null)
                {
                    type = namespc.Types.FirstOrDefault(x => x.Name == typeName);

                    if (type != null)
                    {
                        member = type.Members.FirstOrDefault(x => x.Name == memberName);
                    }
                }
            }

            return new
            {
                Date = date,
                PowerMetrics = member != null ? member.PowerMetrics : new PowerMetrics(),
                FxCopMetrics = member != null ? member.FxCopMetrics : new FxCopMetrics(),
                StyleCopMetrics = member != null ? member.StyleCopMetrics : new StyleCopMetrics()
            };
        }
    }
}
