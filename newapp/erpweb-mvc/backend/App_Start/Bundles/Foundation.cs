using System.Web.Optimization;

namespace backend
{
    public static class Foundation
    {
        public static Bundle Styles()
        {
            return new StyleBundle("~/Content/foundation/styles").Include(
                       "~/Content/foundation/foundation.css",
                       //"~/Content/foundation/foundation.mvc.css",
                       "~/Content/foundation/app.css");
        }

        //public static Bundle Scripts()
        //{
        //    return new ScriptBundle("~/bundles/foundation").Include(
        //              "~/Scripts/vendor/jquery.js",
        //              "~/Scripts/jquery.cookie.js");
        //}
    }
}