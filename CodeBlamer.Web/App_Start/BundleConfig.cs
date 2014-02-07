using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace CodeBlamer.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(
                new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/lib/jquery/jquery-2.0.3.js")
            );

            bundles.Add(
                new ScriptBundle("~/bundles/bootstrap")
                .Include("~/Scripts/lib/bootstrap/bootstrap.js")
            );

            bundles.Add(
                new ScriptBundle("~/bundles/dataTables")
                .Include("~/Scripts/lib/dataTables/jquery.dataTables.js")
                .Include("~/Scripts/lib/dataTables/dataTables.bootstrap.js")
            );

            bundles.Add(new ScriptBundle("~/bundles/aciTree")
                .Include("~/Scripts/lib/aciTree/jquery.aciPlugin.min.js")
                .Include("~/Scripts/lib/aciTree/jquery.aciTree.min.js")
                );

            bundles.Add(new ScriptBundle("~/bundles/app")
                .IncludeDirectory("~/Scripts/app/util", "*.js")
                .IncludeDirectory("~/Scripts/app/view", "*.js")
                );

            bundles.Add(new ScriptBundle("~/bundles/d3js")
                .Include("~/Scripts/lib/d3js/d3.js")
                .Include("~/Scripts/lib/d3js/d3.geom.js")
                .Include("~/Scripts/lib/d3js/d3.layout.js")
                );

            bundles.Add(new StyleBundle("~/css/aciTree")
                    .IncludeDirectory("~/Content/css/lib/aciTree","*.css")
                );
            
            bundles.Add(new StyleBundle("~/css/bootstrap")
                    .IncludeDirectory("~/Content/css/lib/bootstrap", "*.css")
                );

            bundles.Add(new StyleBundle("~/css/dataTables")
                    .IncludeDirectory("~/Content/css/lib/dataTables", "*.css")
                );

            bundles.Add(new StyleBundle("~/css/site")
                    .Include("~/Content/css/style.css")
                );
        }
    }
}