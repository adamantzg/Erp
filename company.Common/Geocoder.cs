using System;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;

namespace company.Common
{
    public static class Geocoder
    {

        //private static string _GoogleMapsKey = Properties.Settings.Default.GoogleMap_Key;
    
        /// Google.com Geocoder
        /// Url request to
        /// http://maps.google.com/maps/geo?q=your address&output=xml&key=xxxxxxxxxxxxxx
        public static Geolocation? ResolveAddress(string query)
        {
            //if (string.IsNullOrEmpty(_GoogleMapsKey))
            //    _GoogleMapsKey = Properties.Settings.Default.GoogleMap_Key;

            string url = $"https://maps.googleapis.com/maps/api/geocode/xml?address={HttpUtility.UrlEncode(query)}&sensor=false&key={Properties.Settings.Default.MapsKey}"; 
            
            XmlNode coords = null;
            try
            {
                string xmlString = GetUrl(url);
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(xmlString);
                XmlNamespaceManager xnm = new XmlNamespaceManager(xd.NameTable);
                coords = xd.GetElementsByTagName("location")[0];
            }
            catch
            {
                
            }
            Geolocation? gl = null;
            if (coords != null)
            {
                string[] coordinateArray = new string[] { coords.ChildNodes[0].InnerText, coords.ChildNodes[1].InnerText };
                if (coordinateArray.Length >= 2)
                {
                    gl = new Geolocation(Convert.ToDecimal(coordinateArray[0],System.Globalization.CultureInfo.InvariantCulture), Convert.ToDecimal(coordinateArray[1],System.Globalization.CultureInfo.InvariantCulture));
                }
            }

            return gl;
        }

        public static Geolocation? ResolveAddress(string address, string city, string state, string postcode, string country)
        {
            return ResolveAddress(address + "," + city + "," + state + "," + postcode + " " + country);
        }

    
    
        // Retrieve a Url via WebClient
        private static string GetUrl(string url)
        {
            string result = string.Empty;
            WebClient Client = new WebClient();
            using (Stream strm = Client.OpenRead(url))
            {
                StreamReader sr = new StreamReader(strm);
                result = sr.ReadToEnd();
            }

            return result;
        }
    }

    public struct Geolocation
    {
        public decimal Lat;

        public decimal Lon;

        public Geolocation(decimal lat, decimal lon)
        {
            Lat = lat;
            Lon = lon;
        }

        public override string ToString()
        {
            return "Latitude: " + Lat.ToString() + " Longitude: " + Lon.ToString();
        }
    
        public string ToQueryString()
        {
            return "+to:" + Lat + "%2B" + Lon;
        }
    }
    
}