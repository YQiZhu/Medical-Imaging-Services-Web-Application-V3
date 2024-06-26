﻿using System.Web;
using System.Web.Optimization;

namespace FIT5032_PortfolioV3
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/bootstrap-datepicker.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/fullcalendar").Include(
                      "~/Scripts/lib/jquery.min.js",
                      "~/Scripts/lib/moment.min.js",
                      "~/Scripts/fullcalendar.js",
                      "~/Scripts/calendar.js"));

            bundles.Add(new ScriptBundle("~/bundles/mapbox").Include(
                      "~/Scripts/location.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/bootstrap-datepicker.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                "~/Scripts/DataTables/jquery.dataTables.min.js",
                "~/Scripts/DataTables/dataTables.bootstrap.min.js" 
                ));

            bundles.Add(new StyleBundle("~/Content/datatables").Include(
                "~/Content/DataTables/css/dataTables.bootstrap.min.css" 
                ));

            bundles.Add(new StyleBundle("~/bundles/fullcalendar-css").Include(
                "~/Content/fullcalendar.min.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/fullcalendar-js").Include(
                        "~/Scripts/fullcalendar.min.js"));

        }
    }
}
