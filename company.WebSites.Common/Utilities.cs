using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Web.Security;
using Newtonsoft.Json;
using asaq2.Model;
using asaq2.Model.DAL;
using Utilities = asaq2.Common.Utilities;

namespace asaq2.WebSites.Common
{
    public class WebUtilities
    {
        public const int WebFilesDefaultImage = 1;

        public static string AddSuffixToFileName(string fileName, string suffix)
        {
            return Utilities.AppendSuffixToFileName(fileName, suffix);
        }

        public static string JsonSerialize(object o)
        {
            var s = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.None
                };

            return JsonConvert.SerializeObject(o, s).Replace("\\n", "");
        }
        //public static string JsonSerialize(object o)
        //{
        //    var s = new JsonSerializerSettings
        //    {
        //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        //        PreserveReferencesHandling = PreserveReferencesHandling.None
        //    };

        //    return JsonConvert.SerializeObject(o, s).Replace("\\n", "");
        //}

        public static T Deserialize<T>(string s)
        {
            var ser = new JavaScriptSerializer();
            return ser.Deserialize<T>(s);
        }

        public static string Serialize(object param)
        {
            var ser = new JavaScriptSerializer();
            return ser.Serialize(param);
        }

        public static string encrypt(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                return Convert.ToBase64String(MachineKey.Protect(Encoding.Unicode.GetBytes(text),"WC"));
            }
            else
            {
                return null;
            }
        }

        public static string decrypt(string text)
        {
            string result = null;
            if (!String.IsNullOrEmpty(text))
            {
                try
                {
                    result = Encoding.Unicode.GetString(MachineKey.Unprotect(Convert.FromBase64String(text),"WC"));
                }
                catch
                {
                    //Empty catch to prevent http errors
                }

            }
            return result;
        }

        public static string GetSiteUrl()
        {
            return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
        }

        public static string GetCountryCodeFromIp()
        {
            var isStaging = HttpContext.Current.Request.Url.Host.ToLower().Contains("bathroomsourcing.com");
            //test;

#if DEBUG
            return "HR"; //Properties.Settings.Default.DebugCountryIp;
#else

            if(isStaging)
                return "UK";

            return string.Empty + HttpContext.Current.Request["HTTP_CF_IPCOUNTRY"];
#endif

        }

        public static Web_product_new CheckFilesExist(string imageRootUrl,Web_product_new product, bool useSiteCode = false)
        {
            var removeFileTypes = Properties.Settings.Default.RemoveFilesType.Split(',').Select(int.Parse).ToList();
            if (product == null)
                return null;
            var site = Web_siteDAL.GetById(product.web_site_id);
            var types = Web_product_file_typeDAL.GetForSite(product.web_site_id);
            if (imageRootUrl.Contains("WC"))
            {
                product.WebFiles.RemoveAll(r => removeFileTypes.Contains(r.file_type??0));
            }
            if (site != null && product.WebFiles != null )
            {
                foreach (var file in product.WebFiles)
                {
                    var type = types.FirstOrDefault(t => t.id == file.file_type);
                    if (type != null)
                    {
                        var path =
                            HttpContext.Current.Server.MapPath(Path.Combine(imageRootUrl, useSiteCode ? site.code : "", type.path,
                                                                            file.name)).Replace("dwg","pdf");
                        file.DoesFileExist = File.Exists(path);
                        if (!string.IsNullOrEmpty(type.previewpath))
                        {
                            var previewPath = HttpContext.Current.Server.MapPath(Path.Combine(imageRootUrl, useSiteCode ? site.code : "", type.previewpath,
                                                                            file.name));
                            file.DoesPreviewFileExist = File.Exists(previewPath);
                        }
                    }
                }
            }
            return product;
        }

        public static bool CheckDealerVisibility(Dealer dealer, string[] saleCountries, DateTime saleStartDate, DateTime saleEndDate)
        {
            return dealer.hide_1 == 1 &&
                   (!saleCountries.Contains(GetCountryCodeFromIp()) || !DateTime.Now.Between(saleStartDate, saleEndDate) || dealer.sales_registered_2017);
        }

        public static bool CheckHostName(System.Web.HttpRequest request, string hostNameToCheck)
        {
            bool hostNameCheck = false;

            string ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            try
            {
                if (string.IsNullOrEmpty(ip))
                {
                    ip = request.ServerVariables["REMOTE_ADDR"];
                }
                else
                {
                    ip = ip.Split(',')
                        .Last()
                        .Trim();
                }

                IPHostEntry entry = Dns.GetHostEntry(ip);

                if (entry != null)
                {
                    if (entry.HostName.ToLower().Contains(hostNameToCheck))
                        hostNameCheck = true;
                }
            }
            catch (Exception)
            {
                //unknown host or some ip with no name
                //no need to log this exception
            }

            return hostNameCheck;
        }
        /// <summary>
        ///     Check is request comming from indexing bot page.
        /// </summary>
        /// <param name="userAgent"></param>
        /// <param name="indexingBot"></param>
        /// <returns>boolean true or false</returns>
        public static bool CheckIsBot(string userAgent, string indexingBot)
        {
            if (indexingBot.Contains(',') && !string.IsNullOrEmpty(userAgent))
            {
                var arr = indexingBot.Split(',');
                return arr.Any(userAgent.ToLower().Contains);
            }
            else
            {

                if (string.IsNullOrEmpty(userAgent))
                {
                    return false;

                }
                else if (userAgent.ToLower().Contains(indexingBot))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


    }
}