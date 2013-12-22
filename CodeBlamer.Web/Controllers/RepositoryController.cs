using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CodeBlamer.Infra.Models;
using LibGit2Sharp;
using CodeBlamer.Infra;
using Type = CodeBlamer.Infra.Models.Type;

namespace CodeBlamer.Web.Controllers
{
    public class RepositoryController : Controller
    {
        //
        // GET: /Repository/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddRepository(string repositoryUrl)
        {
            new MongoRepository().InsertUrl(repositoryUrl);
            return View("Index");
        }

        [HttpGet]
        public ActionResult GetProjectTree(string repositoryUrl)
        {
            var projects = new MongoRepository().GetProjects().First(x => x.RepositoryUrl == "https://github.com/pauloortins/CodeBlamer");

            var result = projects.Commits.OrderByDescending(x => x.Date).First().Modules.Select(module => new 
                {
                    label = module.Name,
                    open = false,
                    inode = true,
                    node = module.Name,
                    branch = module.Namespaces.Select(namespaces => new
                        {
                            label = namespaces.Name,
                            open = false,
                            inode = true,
                            node = module.Name + ">" + namespaces.Name,
                            branch = namespaces.Types.Select(type => new
                                {
                                    label = type.Name,
                                    open = false,
                                    inode = true,
                                    node = module.Name + ">" + namespaces.Name + ">" + type.Name,
                                    branch = type.Members.Select(member => new
                                        {
                                            label = member.Name,
                                            open = false,
                                            inode = false,
                                            node = module.Name + ">" + namespaces.Name + ">" + type.Name + ">" + member.Name,
                                        })
                                })
                        })
                });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetNodeMetrics(string repositoryUrl, string node)
        {
            var mongo = new MongoRepository();
            var pieces = node.Split('>');
            var project = mongo.GetProjects().First(x => x.RepositoryUrl == "https://github.com/pauloortins/CodeBlamer");
            var commits = project.Commits.OrderBy(x => x.Date);

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
                    Metrics = module != null ? module.Metrics : new Metrics()
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
                    Metrics = namespc != null ? namespc.Metrics : new Metrics()
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
                Metrics = type != null ? type.Metrics : new Metrics()
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
                Metrics = member != null ? member.Metrics : new Metrics()
            };
        }
    }
}
