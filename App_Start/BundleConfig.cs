using System.Web;
using System.Web.Optimization;

namespace PublicSite
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/sd").Include(
                        "~/Scripts/Standard/sd.common.js",
                        "~/Scripts/Standard/sd.ajax.js",
                        "~/Scripts/Standard/sd.extensions.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/Standard/jquery.validate.min.js",
                        "~/Scripts/Standard/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/Standard/jquery.unobtrusive-ajax.min.js",
                        "~/Scripts/Standard/expressive.annotations.validate.js"));

            bundles.Add(new ScriptBundle("~/bundles/multiselect").Include(
                        "~/Scripts/Standard/bootstrap-multiselect.js",
                        "~/Scripts/Standard/bootstrap-multiselect-collapsible-groups.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/ModernBusiness/css/modern-business.css",
                      "~/Content/custom.css",
                      "~/Content/bootstrap-multiselect.css"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}
