using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend.Models
{
    public static class SiteSelector
    {
        private static Dictionary<string,string> urls=new Dictionary<string,string>
        {
                {"http://localhost:1535","http://bb-hr.no-ip.org:8083"},
                {"http://bb-hr.no-ip.org:8083","http://bb-hr.no-ip.org:8083"}
        };

        /// <summary>
        /// Get left side url, for localhost, server1
        /// </summary>
        /// <param name="url">left url</param>
        /// <returns>url</returns>
        public static string GetUrl(string url)
        {
            string result;
            if (urls.TryGetValue(url, out result))
            {
                return result;    
            }
            else
            {
                return null;
            }
            
        }
    }
}