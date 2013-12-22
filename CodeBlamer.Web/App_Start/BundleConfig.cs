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

            bundles.Add(new ScriptBundle("~/bundles/aciTree")
                .Include("~/Scripts/lib/aciTree/jquery.aciPlugin.min.js")
                .Include("~/Scripts/lib/aciTree/jquery.aciTree.min.js")
                );

            bundles.Add(new StyleBundle("~/css/aciTree")
                    .IncludeDirectory("~/Content/css/lib/aciTree","*.css")
                );
        }
    }
}