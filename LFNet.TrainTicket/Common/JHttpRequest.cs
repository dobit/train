using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace LFNet.TrainTicket
{
    public class JHttpClient
    {
        static  Dictionary<Type,PropertyInfo[]> typePropertyInfoses=new Dictionary<Type, PropertyInfo[]>(); 
        public WebProxy WebProxy { get; set; }
        public CookieContainer Cookie { get; set; }
        public JHttpClient():this(null)
        {
           
            
        }
        public JHttpClient(WebProxy webProxy)
        {
            WebProxy = webProxy;
            Cookie=new CookieContainer();
        }

        public  string GetString(string url,object inonObject,string referer)
        {
            HttpRequest httpRequest = HttpRequest.Create(CombineToUrl(url,ObjectToForms(inonObject)), referer, Cookie, HttpMethod.GET, "", WebProxy);
            return httpRequest.GetString();
        }

        public T GetJsonObject<T>(string url, object inonObject, string referer)
        {
            HttpRequest httpRequest = HttpRequest.Create(CombineToUrl(url, ObjectToForms(inonObject)), referer, Cookie, HttpMethod.GET, "", WebProxy);
            return httpRequest.GetJsonObject<T>();
        }

        public string PostToString(string url, object inonObject, string referer)
        {
            HttpRequest httpRequest = HttpRequest.Create(url, referer, Cookie, ObjectToForms(inonObject), WebProxy);
            return httpRequest.GetString();
        }

        public T PostToJsonObject<T>(string url, object inonObject, string referer)
        {
            HttpRequest httpRequest = HttpRequest.Create(url, referer, Cookie, ObjectToForms(inonObject), WebProxy);
            return httpRequest.GetJsonObject<T>();
        }

        private NameValueCollection ObjectToForms(object inonObject)
        {
            
            System.Collections.Specialized.NameValueCollection nameValueCollection = new NameValueCollection();
            if (inonObject == null) return nameValueCollection;
            Type type = inonObject.GetType();
            PropertyInfo[] propertyInfos;
            if (typePropertyInfoses.ContainsKey(type))
            {
                propertyInfos = typePropertyInfoses[type];
            }
            else
            {
                propertyInfos = inonObject.GetType().GetProperties().Where(p=>p.CanRead).ToArray();
                typePropertyInfoses.Add(type, propertyInfos);
            }
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                nameValueCollection.Add(propertyInfo.Name, propertyInfo.GetValue(inonObject, new object[] { }).ToString());
            }

            return nameValueCollection;
        }


        private string CombineToUrl(string url, NameValueCollection nv)
        {
            if (nv.Count == 0) return url;
            StringBuilder sb = new StringBuilder();
            foreach (var key in nv.AllKeys)
            {
                sb.AppendFormat("&{0}={1}", key, Common.HtmlUtil.UrlEncode(nv[key]));
            }
            if (url.Contains("?")) url += sb.ToString();
            else
            {
                url +="?"+ sb.Remove(0, 1).ToString();
            }
            return url ;
        }
    }
}