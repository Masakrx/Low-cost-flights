﻿using System.Web;
using System.Web.Optimization;

namespace Low_cost_flights
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                         "~/Scripts/jquery-ui-{version}.js",
                         "~/Scripts/tokenize2.js",
                         "~/Scripts/chosen.jquery.min.js",
                         "~/Scripts/date.js",
                         "~/Scripts/quanitity.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/daterangepickerJS").Include(
               "~/Scripts/moment.min.js",
               "~/Scripts/daterangepicker.js"
           ));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/umd/popper.min.js",
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/bootstrap-multiselect.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-multiselect.css",
                      "~/Content/site.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/tokenize2.css",
                      "~/Content/chosen.min.css",
                      "~/Content/PagedList.css",
                      "~/Content/quantity.css"));

            bundles.Add(new StyleBundle("~/bundles/daterangepickerCSS").Include(
                "~/Content/daterangepicker.css"
            ));
        }
    }
}
