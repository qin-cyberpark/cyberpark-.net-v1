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
                              "~/Content/js/iCheck.min.js",
                                "~/Content/js/jquery.scrollUp.min.js",
                                "~/Content/bootstrap/js/bootstrap-confirmation.min.js"));

            //admin js
            bundles.Add(new ScriptBundle("~/bundle/js").Include(
                            "~/Content/js/main.js",
                            "~/Content/js/controller/global-alert.service.js",
                            "~/Content/js/controller/login.controller.js",
                            "~/Content/js/controller/warning.controller.js",
                            "~/Content/js/controller/plan.controller.js",
                            "~/Content/js/controller/customer.controller.js",
                            "~/Content/js/controller/account.controller.js",
                            "~/Content/js/controller/product.controller.js",
                            "~/Content/js/controller/invoice.controller.js",
                            "~/Content/js/controller/transaction.controller.js",
                            "~/Content/js/controller/staff.controller.js",
                            "~/Content/js/controller/external-bill.controller.js",
                            "~/Content/js/controller/adjustment.controller.js"));

            bundles.Add(new StyleBundle("~/css/main").Include(
                     "~/Content/bootstrap/css/bootstrap.min.css",
                     "~/Content/bootstrap/css/skins/all.css",
                     "~/Content/css/main.css"));


        }
    }
}
