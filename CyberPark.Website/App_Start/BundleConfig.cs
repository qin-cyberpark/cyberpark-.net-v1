using System.Web;
using System.Web.Optimization;

namespace CyberPark.Website
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundle/js-lib").Include(
                            "~/Content/js/angular.min.js",
                            "~/Content/js/jquery-2.2.0.min.js",
                            "~/Content/bootstrap/js/bootstrap.min.js",
                             "~/Content/js/spin.min.js",
                             "~/Content/js/ladda.min.js",
                              "~/Content/js/angular-ladda.min.js",
                              "~/Content/js/jquery.scrollUp.min.js",
                              "~/Content/bootstrap/js/bootstrap-confirmation.min.js",
                              "~/Content/js/iCheck.min.js"));

            bundles.Add(new ScriptBundle("~/bundle/js").Include(
                            "~/Content/js/main.js",
                            "~/Content/js/controller/global-alert.service.js",
                            "~/Content/js/controller/login.controller.js",
                            "~/Content/js/controller/warning.controller.js",
                            "~/Content/js/controller/application.controller.js",
                            "~/Content/js/controller/available-broadband.controller.js"));

            bundles.Add(new StyleBundle("~/css/main").Include(
                     "~/Content/bootstrap/css/bootstrap.min.css",
                     "~/Content/bootstrap/css/skins/all.css",
                     "~/Content/bootstrap/css/bootstrap-nav-wizard.css",
                     "~/Content/css/ladda-themeless.min.css",
                     "~/Content/css/main.css"));
        }
    }
}
