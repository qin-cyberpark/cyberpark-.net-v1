using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;
using System.Net;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CyberPark.Domain.Core
{
    public partial class Utility
    {
        private static Newtonsoft.Json.JsonSerializerSettings _jsonSerializerSetting;
        static Utility()
        {
            _jsonSerializerSetting = new Newtonsoft.Json.JsonSerializerSettings()
            {
                PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };
        }
        public static string ToJson(object obj, bool compress= false)
        {
            var strJson =  Newtonsoft.Json.JsonConvert.SerializeObject(obj, _jsonSerializerSetting);
            if (compress)
            {
                return Compress(strJson);
            }
            else
            {
                return strJson;
            }
        }
        public static T ToObject<T>(string json, bool compressed = false)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default(T);
            }
            var strJson = json;
            if(compressed){
                strJson = Decompress(json);
            }
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        public static string Compress(string s)
        {
            var bytes = Encoding.Unicode.GetBytes(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }
                return Convert.ToBase64String(mso.ToArray());
            }
        }

        public static string Decompress(string s)
        {
            var bytes = Convert.FromBase64String(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }
                return Encoding.Unicode.GetString(mso.ToArray());
            }
        }

        public static string GetUrl(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string data = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();

                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
            }

            return data;
        }

        public static void GetLocationByAddress(ref string address, out string lat, out string lng)
        {
            lat = null;
            lng = null;
            string url = string.Format(@"http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=true", address.Replace(" ", "%20"));
            string xmlData = GetUrl(url);
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);
            //find new zealand address
            var addrs = xmlDoc.SelectNodes("GeocodeResponse/result");
            bool isNz = false;
            foreach (XmlNode addr in addrs)
            {
                var longNames = addr.SelectNodes("address_component/long_name");
                foreach (XmlNode name in longNames)
                {
                    if ("New Zealand".Equals(name.InnerText))
                    {
                        isNz = true;
                        break;
                    }
                }

                if (isNz)
                {
                    address = addr.SelectSingleNode("formatted_address")?.InnerText;
                    lat = addr.SelectSingleNode("geometry/location/lat")?.InnerText;
                    lng = addr.SelectSingleNode("geometry/location/lng")?.InnerText;
                    break;
                }
            }
        }
        
        public static BroadbandAvailability GetBroadbandAvailabilityByAdderss(string address)
        {
            string lat, lng;
            bool adsl = false, vdsl = false, ufb = false;
            string activeService = null;
            //get location
            Utility.GetLocationByAddress(ref address, out lat, out lng);
            if (string.IsNullOrEmpty(lat) || string.IsNullOrEmpty(lng))
            {
                return new BroadbandAvailability(address);
            }

            //get inform
            string url = string.Format(@"https://chorus-viewer.wivolo.com/viewer-chorus/jsonp/location-details?lat={0}&lng={1}&rooftop=1&debug=1&zoom=1&maplayers=3&search_type=X&address={2}&callback=testCallback"
                                        , lat, lng, address.Replace(" ", "%20"));
            string response = GetUrl(url);
            if(string.IsNullOrEmpty(response) || !response.Contains("\"success\":true"))
            {
                return new BroadbandAvailability(address);
            }
            response = response.Substring(13, response.Length - 15);
            JObject jsonData = JObject.Parse(response);
            //active
            activeService = jsonData["speedDial"]?["active_service"]?["technology"]?.ToString();
            //available
            foreach (var v in jsonData["speedDial"]["available_services"])
            {
                if ("ADSL".Equals(v["service"].ToString()))
                {
                    adsl = true;
                }
                if ("VDSL".Equals(v["service"].ToString()))
                {
                    vdsl = true;
                }
                if ("FIBRE".Equals(v["service"].ToString()))
                {
                    ufb = true;
                }
            }

            return new BroadbandAvailability(address)
            {
                ADSL = adsl,
                VDSL = vdsl,
                UFB = ufb,
                Active = activeService
            };
        }
    }
}
