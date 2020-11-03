using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.Security;
//using MVCControlsToolkit.Controls;
using Microsoft.Win32;
using Newtonsoft.Json;
using company.Common;
using erp.Model;
using System.Web.Script.Serialization;
using System.ComponentModel;
using erp.Model.Dal.New;
using backend.App_GlobalResources;
using System.Text.RegularExpressions;
using System.Net.Mime;
using backend.Properties;
using Utilities = company.Common.Utilities;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Data.OleDb;
using System.Web.UI.DataVisualization.Charting;
using erp.DAL.EF;
using erp.DAL.EF.New;

namespace backend
{
    public enum EditMode
    {
        New=0,
        Edit=1
    }

    public static class WebUtilities
    {
        public const string CurrentUserSessionVar = "CurrentUser";
        public const string CurrentExternalUserSessionVar = "CurrentExtUser";

        public const int WebFilesDefaultImage = 1;

        
        public static string AppendToFileName(string fileName, string suffix)
        {
            string fileNameNoExtension, extension;
            fileNameNoExtension = Path.GetFileNameWithoutExtension(fileName);
            extension = Path.GetExtension(fileName);
            return fileNameNoExtension + suffix + extension;
        }

        public static string encrypt(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                return Encoding.Unicode.GetString(MachineKey.Protect(Encoding.Unicode.GetBytes(text)));
            }
            return null;
        }

        public static string decrypt(string text)
        {
            string result = null;
            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    result = Encoding.Unicode.GetString(MachineKey.Unprotect(Encoding.Unicode.GetBytes(text)));
                }
                catch(Exception e)
                {
                    //Empty catch to prevent http errors
                    System.Diagnostics.EventLog.WriteEntry("asaqback", string.Format("Error when decrypting text: {0}. Text: {1}",text,e.ToString()));
                }
                
            }
            return result;
        }


        public static string GetSiteUrl()
        { 
            return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
        }

        
        public static string ArrayToString(string[] array, string separator, bool removeEmpty = true)
        {
            return string.Join(separator, (from a in array where !removeEmpty || !string.IsNullOrEmpty(a?.Trim()) select a).ToArray());  
        }

        

        public static T Deserialize<T>(string s)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            return ser.Deserialize<T>(s);
        }

        public static string Serialize(object param)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            return ser.Serialize(param);
        }

        public static Nullable<T> ToNullable<T>(this string s) where T : struct
        {
            Nullable<T> result = new Nullable<T>();
            try
            {
                if (!string.IsNullOrEmpty(s) && s.Trim().Length > 0)
                {
                    TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
                    result = (T)conv.ConvertFrom(s);
                }
            }
            catch { }
            return result;
        }

        public static IEnumerable<Control> All(this ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                foreach (Control grandChild in control.Controls.All())
                    yield return grandChild;

                yield return control;
            }
        }

        
        
        

        private static long? IpToLong(string ipStr)
        {
            IPAddress ip;
            if (IPAddress.TryParse(ipStr, out ip))
            {
                byte[] bytes = ip.GetAddressBytes();
                return
                       16777216 * (Int64)bytes[0] +
                       65536 * (Int64)bytes[1] +
                       256 * (Int64)bytes[2] +
                       (Int64)bytes[3];
            }
            return null;
        }




        

        
        public static DateTime? ParseDateFromString(string text)
        {
            DateTime retValue;
            if (DateTime.TryParse(text, out retValue))
                return retValue;
            else
            {
                if (DateTime.TryParse(text, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out retValue))
                    return retValue;
                else
                    return null;
            }
        }

       

        
        /// <summary>
        /// Get images from session
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static Dictionary<string, byte[]> GetTempFiles(string prefix = "tempFile_")
        {
            Dictionary<string, byte[]> result = new Dictionary<string, byte[]>();
            foreach (string item in HttpContext.Current.Session.Keys)
            {
                if (item.StartsWith(prefix))
                    result.Add(item.Replace(prefix, ""), (byte[]) HttpContext.Current.Session[item]);
            }
            return result;
        }

        public static string StripHTML(string html)
        {
            return Regex.Replace(html.Replace("&",""), "<[^>]+?>", "");
        }

        public static void SaveTempFile(string fileName, byte[] file, string prefix = "tempFile_")
        {
            HttpContext.Current.Session[prefix + fileName] = file;
        }

        public static void SaveCommentTempFile(string fileName, MemoryStream file)
        {
            HttpContext.Current.Session["commenttempFile_" + fileName] = file;
        }

        //public static void SaveIssueTempFiles(Dictionary<string, MemoryStream> files)
        //{
        //    HttpContext.Current.Session[Properties.Settings.Default.IssueFiles_SessionVar] = files;
        //}

        public static Dictionary<string, MemoryStream> GetCommentTempFiles()
        {
            //return (Dictionary<string, MemoryStream>)HttpContext.Current.Session[Properties.Settings.Default.CommentFiles_SessionVar];
            var result = new Dictionary<string, MemoryStream>();
            foreach (string item in HttpContext.Current.Session.Keys)
            {
                if (item.StartsWith("commenttempFile_"))
                    result.Add(item.Replace("commenttempFile_", ""), (MemoryStream)HttpContext.Current.Session[item]);
            }
            return result;
        }

        public static void SaveCommentTempFiles(Dictionary<string, MemoryStream> files)
        {
            HttpContext.Current.Session[Properties.Settings.Default.CommentFiles_SessionVar] = files;
        }

        internal static void ClearTempFiles(string prefix = "tempFile_")
        {
            List<string> remove = new List<string>();
            foreach (string item in HttpContext.Current.Session.Keys)
            {
                if (item.StartsWith(prefix))
                    remove.Add(item);
            }
            foreach (var item in remove)
            {
                HttpContext.Current.Session.Remove(item);    
            }
        }
        internal static void ClearCommentTempFiles()
        {
            //HttpContext.Current.Session[Properties.Settings.Default.CommentFiles_SessionVar] = null;
            List<string> remove = new List<string>();
            foreach (string item in HttpContext.Current.Session.Keys)
            {
                if (item.StartsWith("commenttempFile_"))
                    remove.Add(item);
            }
            foreach (var item in remove)
            {
                HttpContext.Current.Session.Remove(item);
            }
        }

        public static string GetFileName(object qqfile, HttpRequestBase request)
        {
            //qqfile can be string array for chrome, postedfile class for newer ie, empty for old ie
            if (qqfile is string[] && !string.IsNullOrEmpty(string.Empty + (qqfile as string[])[0]))
                return (qqfile as string[])[0];
            if (qqfile is string && !string.IsNullOrEmpty(string.Empty + qqfile.ToString()))
                return qqfile.ToString();
            //IE, not getting qqfile
            if (request.Files.Count > 0)
                return Path.GetFileName(request.Files[0].FileName);
            return null;
        }

        public static object SaveTempFile(string fileName, HttpRequestBase request, float maxSize, string prefix = "tempFile_")
        {
            object result;
            byte[] fileBytes = GetFileFromRequest(fileName, request, maxSize, out result);
            if (fileBytes != null)
                SaveTempFile(fileName, fileBytes, prefix);
            return result;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="request"></param>
        /// <param name="maxSize"></param>
        /// <param name="stream"></param>
        /// <returns>JSON ready object for fileuploader</returns>
        public static byte[] GetFileFromRequest(string fileName, HttpRequestBase request, float maxSize,
                                                out object retValue)
        {
            Stream fileStream = null;
            bool sizeTooBig = false;
            retValue = null;
            //stream = null;
            byte[] result = null;
            try
            {
                if (request.Files.Count > 0)
                {
                    //old browsers
                    fileStream = request.Files[0].InputStream;
                }
                else
                    fileStream = request.InputStream;
            }
            catch (HttpException)
            {
                //Too big for server settings
                sizeTooBig = true;
            }

            //TODO: Check if file is too big
            if (sizeTooBig || fileStream.Length > maxSize * (Math.Pow(1024, 2)))
            {

                retValue = new { success = false, reason = string.Format(App_GlobalResources.Resources.FileTooBig, maxSize) };
            }
            else
            {
                result = Utilities.FileStreamToBytes(fileStream);
                retValue = new { success = true };
            }
            return result;
        }

        public static object SaveCommentFile(string fileName, HttpRequestBase request, float maxSize)
        {
            Stream fileStream = null;
            bool sizeTooBig = false;
            object retValue = null;
            try
            {
                if (request.Files.Count > 0)
                {
                    //old browsers
                    fileStream = request.Files[0].InputStream;
                }
                else
                    fileStream = request.InputStream;
            }
            catch (HttpException)
            {
                //Too big for server settings
                sizeTooBig = true;
            }

            //TODO: Check if file is too big
            if (sizeTooBig || fileStream.Length > maxSize * (Math.Pow(1024, 2)))
            {

                retValue = new { success = false, reason = string.Format(App_GlobalResources.Resources.FileTooBig, maxSize) };
            }
            else
            {

                MemoryStream ms = new MemoryStream();
                fileStream.CopyTo(ms);
                WebUtilities.SaveCommentTempFile(fileName, ms);

                retValue = new { success = true };
            }
            return retValue;
        }

        public static List<string> GetOpeningStoreTimes()
        {
            return Settings.Default.StoreOpeningTimeValues.Cast<string>().ToList();
        }

        public static List<string> GetClosingStoreTimes()
        {
            return Settings.Default.StoreClosingTimeValues.Cast<string>().ToList();
        }
        

        //public static List<LookupItem> GetOpeningStoreTimes()
        //{
        //    return Settings.Default.StoreOpeningTimeValues.Cast<string>().Select(s=>new LookupItem{id= (s == "closed" ? 0 : int.Parse(s.Split(':')[0])),value = s}).ToList();
        //}

        //public static List<LookupItem> GetClosingStoreTimes()
        //{
        //    return Settings.Default.StoreClosingTimeValues.Cast<string>().Select(s => new LookupItem { id = (s == "closed" ? 0 : int.Parse(s.Split(':')[0])), value = s }).ToList();
        //}

        public static void DeleteTempFile(string name, string prefix = "tempFile_")
        {
            HttpContext.Current.Session.Remove(prefix + name);
        }

        public static byte[] GetTempFile(string name, string prefix = "tempFile_")
        {
           
            if (HttpContext.Current.Session[prefix + name] == null)
                return null;
            
            return (byte[]) HttpContext.Current.Session[prefix + name];
        }

        

        public static string ExtensionToContentType(string extension)
        {
            string contentType = String.Empty;
            RegistryKey key = Registry.ClassesRoot.OpenSubKey("." + extension);
            if (key != null)
                contentType = string.Empty + key.GetValue("Content Type");

            if (string.IsNullOrEmpty(contentType))
                contentType = "application/octet-stream";
            return contentType;
        }

        public static string CorrectDecimal(double? number)
        {
            return number.ToString().Replace(",",".");

        }

        //public static int GetMonthFromNow(int offset)
        //{
        //    string sYear = DateTime.Now.AddMonths(offset).Year.ToString();
        //    return int.Parse(sYear.Substring(sYear.Length - 2, 2)) * 100 + DateTime.Now.AddMonths(offset).Month;
        //}

        

       

        public static string KiloFormat(double num)
        {
            
            if (num >= 1000000)
                return (num / 1000000).ToString("#0.00") + "M";

            if (num >= 1000)
                return (num / 1000).ToString("#0.00") + "K";

            return num.ToString("#,0");
        } 

        public static string GetMIMEType(string filepath)
        {
            var fileInfo = new System.IO.FileInfo(filepath);
            string fileExtension = fileInfo.Extension.ToLower();

            // direct mapping which is fast and ensures these extensions are found
            switch (fileExtension)
            {
                case "htm":
                case "html":
                    return "text/html";
                case "js":
                    return "text/javascript"; // registry may return "application/x-javascript"
            }



            // see if we can find extension info anywhere in the registry
            //Note : there is not a ContentType key under ALL the file types , check Run --> regedit , then extensions !!!

            RegistryPermission regPerm = new RegistryPermission(RegistryPermissionAccess.Read, @"\\HKEY_CLASSES_ROOT");

            // looks for extension with a content type
            RegistryKey rkContentTypes = Registry.ClassesRoot.OpenSubKey(fileExtension);
            if (rkContentTypes != null)
            {
                object key = rkContentTypes.GetValue("Content Type");
                if (key != null)
                    return key.ToString().ToLower();
            }


            // looks for a content type with extension
            // Note : This would be problem if  multiple extensions associate with one content type.
            RegistryKey typeKey = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type");

            foreach (string keyname in typeKey.GetSubKeyNames())
            {
                RegistryKey curKey = typeKey.OpenSubKey(keyname);
                if (curKey != null)
                {
                    object extension = curKey.GetValue("Extension");
                    if (extension != null)
                    {
                        if (extension.ToString().ToLower() == fileExtension)
                        {
                            return keyname;
                        }
                    }
                }
            }

            return "application/octet-stream";
        }

        public static List<Location> GetLocations()
        {
            var result = new List<Location>();
            result.Add(new Location{id=1,Name = App_GlobalResources.Resources.Inspection_Location1_Title});
            result.Add(new Location { id = 2, Name = App_GlobalResources.Resources.Inspection_Location2_Title });
            result.Add(new Location { id = 3, Name = App_GlobalResources.Resources.Inspection_Location3_Title });
            result.Add(new Location { id = 4, Name = App_GlobalResources.Resources.Inspection_Location4_Title });
            return result;
        }

        public static DateTime GetFirstDayOfWeek(DateTime date)
        {
            var firstDayOfWeek = CultureInfo.InvariantCulture.DateTimeFormat.FirstDayOfWeek;

            int offset = firstDayOfWeek - date.DayOfWeek;
            if (offset != 1)
            {
                return date.AddDays(offset);
                
            }
            else
            {
                return date.AddDays(-6);
            }
        }

        public static bool IsEmpty(int? value)
        {
            return value == null || value == 0;
        }

        public static string JsonSerialize(object o)
        {
            var s = new JsonSerializerSettings();
            
            s.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            s.PreserveReferencesHandling = PreserveReferencesHandling.None;
#if DEBUG
            s.Formatting = Formatting.Indented;
#else
            s.Formatting = Formatting.None;
#endif


            return JsonConvert.SerializeObject(o,s).Replace("\\n","");
        }

        

        public static string NumberToText(int n)
        {
            if (n < 0)
                return "Minus " + NumberToText(-n);
            if (n == 0)
                return "";
            if (n <= 19)
                return new string[] {"One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", 
                    "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", 
                    "Seventeen", "Eighteen", "Nineteen"}[n - 1] + " ";
            if (n <= 99)
                return new string[] {"Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", 
                    "Eighty", "Ninety"}[n / 10 - 2] + " " + NumberToText(n % 10);
            if (n <= 199)
                return "One Hundred " + NumberToText(n % 100);
            if (n <= 999)
                return NumberToText(n / 100) + "Hundreds " + NumberToText(n % 100);
            if (n <= 1999)
                return "One Thousand " + NumberToText(n % 1000);
            if (n <= 999999)
                return NumberToText(n / 1000) + "Thousands " + NumberToText(n % 1000);
            if (n <= 1999999)
                return "One Million " + NumberToText(n % 1000000);
            if (n <= 999999999)
                return NumberToText(n / 1000000) + "Millions " + NumberToText(n % 1000000);
            if (n <= 1999999999)
                return "One Billion " + NumberToText(n % 1000000000);
            return NumberToText(n / 1000000000) + "Billions " + NumberToText(n % 1000000000);
        }

        


        public static int GetDateDifference(DateTime? poReqEtd, DateTime? orderdate)
        {
            var diff = poReqEtd - orderdate;
            if (diff == null)
                return 0;
            return Convert.ToInt32(diff.Value.TotalDays);
        }

        public static string GetCurrencyName(int curr_code)
        {
            string result = "USD";
            switch (curr_code)
            {
                case 1:
                    result = "GBP";
                    break;
                case 2:
                    result = "EUR";
                    break;
                default:
                    result = "USD";
                    break;
            }
            return result;
        }

        public static DateTime LastDayOfPreviousWeek(DateTime day)
        {
            return day.AddDays(-1 * (day.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)day.DayOfWeek));
        }

        public static List<SelectListItem> GetSelectListFromNumbers(int from, int to)
        {
            var list = new List<SelectListItem>();
            for (int i = from; i <= to; i++)
            {
                list.Add(new SelectListItem{Text = i.ToString(),Value = i.ToString()});
            }
            return list;
        }

        

        public static String GetIP()
        {
            String ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }

        public static string GetCountryCodeFromIP()
        {
            try
            {
                var countryCode = string.Empty;
                var ipaddress = GetIP();
                if (ipaddress != "::1")
                {
                    using (var client = new WebClient())
                    {

                        var json = client.DownloadString("http://ip-api.com/json/" + ipaddress);
                        var serializer = new JavaScriptSerializer();
                        var o = JObject.Parse(json);
                        foreach (JToken child in o.Children())
                        {
                            var property = child as JProperty;
                            if (property != null && property.Name == "countryCode")
                            {
                                countryCode = property.Value.ToString().ToUpper();
                                break;
                            }
                        }
                    }
                }
                else
                    countryCode = "HR";

                return countryCode;
            }
            catch
            {
                return "GB";
            }

        }

        public static string HandleInspectionFolderUrl(string url, string folderName)
        {
            if (!string.IsNullOrEmpty(folderName))
                return url.Replace("/qc/", "/" + folderName + "/");
            return url;
        }

        public static string DisplayValue(int? val)
        {
            return val != null ? val.ToString() : string.Empty;
        }

        /// <summary>
        /// used for proper pdf rendering
        /// </summary>
        /// <param name="val">int?</param>
        /// <returns>"&nbsp;"</returns>
        public static string DisplayValueForPdf(int? val)
        {
            return val != null ? val.ToString() : WebUtility.HtmlDecode("&nbsp;");
        }

        public static string CombineUrls(string url1, string url2)
        {
            var uri1 = (url1 ?? string.Empty).TrimEnd('/');
            var uri2 = (url2 ?? string.Empty).TrimStart('/');
            return string.Format("{0}/{1}", uri1, uri2);
        }

		public static string CombineUrls(params string[] urls)
		{
			if(urls.Length > 0)
			{
				return string.Join("/", urls.Select(u => u.TrimEnd('/')));
			}
			return string.Empty;
			
		}

		public static OleDbConnection GetOleDbConnection(string connString)
        {
            var conn = new OleDbConnection(connString);
            conn.Open();
            return conn;
        }

        public static MemoryStream StreamChart(Chart chart)
        {
            var ms = new MemoryStream();
            chart.SaveImage(ms, ChartImageFormat.Jpeg);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public static string RenderRazorViewToString(ControllerContext ControllerContext, string ViewName, ViewDataDictionary ViewData, TempDataDictionary TempData)
        {
            string renderedView = string.Empty;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, ViewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                renderedView = sw.GetStringBuilder().ToString();
            }

            return renderedView;
        }

        public static string MapStockOrdersWarehouse(string w, bool useAbbreviations = false)
        {
            if (w == "BO")
                return useAbbreviations ? "CW" : "Crosswater";
            if (w == "IR")
                return useAbbreviations ? "AM" : "Ammara";
            return w;
        }

		public static string MapStockOrdersBrand(string b, bool useAbbreviations = false)
		{
			if (b == "CW")
				return useAbbreviations ? "CW" : "Crosswater";
			if (b == "AM")
				return useAbbreviations ? "AM" : "Ammara";
			return b;
		}

		public static string GetStockStatusText(DateTime? d, string inStockText = "In Stock")
        {
            var status = "Unsure";
            if (d != null) {
                if (d == DateTime.Today) {
                    status = inStockText;
                }
                else {
                    status = $"ETA {d.ToString("d")}";
                }
            }
            return status;
        }

        public static string GetImagesFolder(DateTime? date, string rootFolder)
        {
            var inspRootrelativeFolder = date.ToString("yyyy-MM");
            var fullFolderPath = GetFolderFullPath(inspRootrelativeFolder, rootFolder);
            if (!Directory.Exists(fullFolderPath))
                Directory.CreateDirectory(fullFolderPath);
            return inspRootrelativeFolder;
        }

        public static string GetFolderFullPath(string rootRelativeUrl, string imagesRootFolder)
        {
            return System.Web.HttpContext.Current.Server.MapPath(Path.Combine(imagesRootFolder, rootRelativeUrl));
        }

		public static string DateHeader(OrderDateMode mode)
		{
			var result = "";
			switch (mode)
			{
				case OrderDateMode.Etd:
					result = "ETD";
					break;
				case OrderDateMode.Eta:
					result = "ETA";
					break;
				case OrderDateMode.Sale:
					result = "Sale date";
					break;
				case OrderDateMode.EtaPlusWeek:
					result = "ETA + 1 week";
					break;
				default:
					break;
			}
			return result;
		}

	}

    
    public class JsonNetResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;

            response.ContentType = !String.IsNullOrEmpty(ContentType)
                ? ContentType
                : "application/json";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            // If you need special handling, you can call another form of SerializeObject below
            var serializedObject = WebUtilities.JsonSerialize(Data);
            response.Write(serializedObject);
        }
    }
}