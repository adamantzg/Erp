using System.Web;
using System.Web.Optimization;
using company.Common;

namespace backend
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                      "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/globalize").Include("~/Scripts/globalize.js"));
            bundles.Add(new ScriptBundle("~/bundles/utils").Include("~/Scripts/lodash.js").Include("~/Scripts/functions.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.js*", "~/Scripts/jquery.validate.unobtrusive.js", "~/Scripts/jquery.validate.globalize.js"));

            
            bundles.Add(new ScriptBundle("~/bundles/knockout").Include("~/Scripts/knockout-{version}.js").Include("~/Scripts/knockout.mapping*").Include("~/Scripts/knockout.validation*"));
            bundles.Add(new ScriptBundle("~/bundles/plupload").Include("~/Scripts/plupload/plupload.full.js"));
            bundles.Add(new ScriptBundle("~/bundles/plupload2").Include("~/Scripts/plupload-2.0.0/js/moxie.js").Include("~/Scripts/plupload-2.0.0/js/plupload.js"));
            bundles.Add(new ScriptBundle("~/bundles/angular-simple").Include("~/Scripts/angular.js"));
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                      "~/Scripts/angular.js", "~/Scripts/angular-sanitize.js","~/Scripts/angular-ui-router.js", "~/Scripts/angular-animate.js",
                      "~/Scripts/angular-ui/ui-utils.js", "~/Scripts/angular-ui/ui-utils-ieshiv.js"));

            bundles.Add(new ScriptBundle("~/bundles/angularmodel").Include("~/Scripts/angularjs-cheklist-model.js"));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-multiselect").Include("~/Scripts/jquery-multiselect/jquery.multiselect.js"));
            bundles.Add(new ScriptBundle("~/bundles/angular-multiselect").Include("~/Scripts/angularjs-dropdown-multiselect/angularjs-dropdown-multiselect.js"));


            bundles.Add(new StyleBundle("~/Content/DealerInfo/css").Include("~/Content/DealerInfo/main.css"));
            
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap*"));

            bundles.Add(new ScriptBundle("~/bundles/ang-bootstrap").Include("~/Scripts/angular-ui/ui-bootstrap-tpls.js"));

            
            bundles.Add(new StyleBundle("~/simple-content/css").Include("~/content/style-simple.css", "~/content/bootstrap.css", "~/content/font-awesome.css"));

            bundles.Add(new StyleBundle("~/Content/css/dist").Include("~/Content/dist/main_style.css", "~/Content/dist/style.css"));

            bundles.Add(new ScriptBundle("~/bundles/lightbox").Include(
                        "~/Scripts/lightbox/js/lightbox.js"));
            bundles.Add(new StyleBundle("~/Content/lightbox/css").Include("~/Scripts/lightbox/css/lightbox.css", "~/Scripts/ligthbox/css/screen.css"));

            


            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include("~/Content/themes/base/jquery-ui.css"));
            bundles.Add(new StyleBundle("~/Content/themes/smoothness/css").Include("~/Content/themes/smoothness/jquery-ui.css"));
            //BundleTable.EnableOptimizations = true;

            bundles.Add(new ScriptBundle("~/bundles/DataTable").Include(
                        "~/Scripts/DataTables/jquery.dataTables.js",
                        "~/Scripts/DataTables/dataTables.select.js"
                        ));
            bundles.Add(new ScriptBundle("~/bundles/AngularDataTable").Include(
                    "~/Scripts/angular-datatables/js/angular-datatables.js"
                    ));
            bundles.Add(new StyleBundle("~/Content/DataTables/style/css").Include(
                "~/Content/DataTables/css/dataTables.bootstrap.css", new CssRewriteUrlTransform())
                .Include("~/Content/DataTables/css/select.dataTables.css", new CssRewriteUrlTransform())
                .Include("~/Content/DataTables/css/select.bootstrap.css", new CssRewriteUrlTransform())
                .Include("~/Content/DataTables/css/jQuery.datatables.css", new CssRewriteUrlTransform())
                .Include("~/Scripts/angular-datatables/css/angular-datatables.css", new CssRewriteUrlTransform())
                );
            bundles.Add(new StyleBundle("~/Content/stylesUSA/style/css").Include(              
              "~/Content/morris/morris.css",
              "~/Content/style-fixed-director.css",
              "~/Content/style-crosswater.css"
               ));
            bundles.Add(new ScriptBundle("~/bundles/extLibUSA").Include(
                    "~/Scripts/lodash.js",
                    "~/Scripts/angular-plupload/angular-plupload.js",
                    "~/Scripts/imagepaste/paste.js",
                    "~/Scripts/functions.js",
                    "~/Scripts/bootstrap.min.js"
                  ));

            bundles.Add(new ScriptBundle("~/bundles/DealerUSA").Include(
                "~/AngularApps/DealerUSA/app.js",                
                "~/AngularApps/DealerUSA/dealers/dealerListCtrl8.js"
                
                ));
            bundles.Add(new ScriptBundle("~/bundles/app")
                .Include("~/AngularApps/InspectionCASimple/app.js")
                .Include("~/AngularApps/InspectionCASimple/Inspections/createInspectionCACtrl.js")
                .Include("~/AngularApps/InspectionCASimple/Recheck/listRechecksController.js")
                .Include("~/AngularApps/InspectionCASimple/Recheck/editRecheckController.js")
                .Include("~/AngularApps/InspectionCASimple/Resolved/listResolvedController.js")
                .Include("~/AngularApps/InspectionCASimple/Resolved/detailResolvedController.js")
                .Include("~/AngularApps/InspectionCASimple/Navigation/navigationController.js")
                .Include("~/AngularApps/InspectionCASimple/Services/factoryInspectionCASample.js")
                .Include("~/AngularApps/InspectionCASimple/Services/factoryStorage.js"));

        }
    }
}