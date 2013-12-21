using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using LibGit2Sharp;

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
            new RepositoryRetriever.RepositoryRetriever().AddRepository(repositoryUrl);
            return View("Index");
        }
    }
}
