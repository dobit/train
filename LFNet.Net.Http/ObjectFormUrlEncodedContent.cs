using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Xml.Serialization;
namespace LFNet.Net.Http
{
    /// <summary>
    /// 对象以表单形式提交
    /// </summary>
    public class UrlEncodedContent : FormUrlEncodedContent
    {
        static Dictionary<Type, PropertyInfo[]> typePropertyInfoses = new Dictionary<Type, PropertyInfo[]>(); 

        public UrlEncodedContent(object obj,bool lcasePropetyName=false)
            : base(GetKeyValuePairs(obj, lcasePropetyName))
        {
            //this.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        }

        private static IEnumerable<KeyValuePair<string, string>> GetKeyValuePairs(object inonObject, bool lcasePropetyName)
        {

           // if (inonObject == null) return nameValueCollection;

            Dictionary<string, object> keyObjects = new Dictionary<string, object>();
            Type type = inonObject.GetType();
            PropertyInfo[] propertyInfos;
            if (typePropertyInfoses.ContainsKey(type))
            {
                propertyInfos = typePropertyInfoses[type];
            }
            else
            {
                propertyInfos = inonObject.GetType().GetProperties().Where(p => p.CanRead).ToArray();
                typePropertyInfoses.Add(type, propertyInfos);
            }
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                string keyname = propertyInfo.Name;
               var attribs = propertyInfo.GetCustomAttributes(typeof(XmlElementAttribute), true);
                if (attribs.Length > 0)
                {
                    var attrib = (XmlElementAttribute)attribs[0];
                    if (string.IsNullOrEmpty(attrib.ElementName)) keyname = attrib.ElementName;
                }
                
                //var attribs = propertyInfo.GetCustomAttributes(typeof(), true);
                //if (attribs.Length > 0)
                //{
                //    var attrib = (XmlElementAttribute)attribs[0];
                //    if (string.IsNullOrEmpty(attrib.ElementName)) keyname = attrib.ElementName;
                //}
               // if ()
                keyObjects.Add(keyname, propertyInfo.GetValue(inonObject, new object[] { }));
            }
            //LFNet.Common.Reflection.ObjectCopier.Copy(inonObject, keyObjects);
            foreach (KeyValuePair<string, object> keyValuePair in keyObjects)
            {
                if (keyValuePair.Value != null)
                {
                    if (keyValuePair.Value is string)
                    {
                        yield return
                            new KeyValuePair<string, string>(lcasePropetyName ? keyValuePair.Key.ToLower() : keyValuePair.Key,keyValuePair.Value.ToString());
                    }
                    else
                    {
                        var value = keyValuePair.Value as IEnumerable;
                        if (value != null)
                        {
                            IEnumerable iEnumerable = value;

                            foreach (var item  in iEnumerable)
                            {
                                yield return
                                    new KeyValuePair<string, string>(lcasePropetyName ? keyValuePair.Key.ToLower() : keyValuePair.Key,item.ToString());
                            }
                        }
                        else
                            yield return
                                new KeyValuePair<string, string>(lcasePropetyName ? keyValuePair.Key.ToLower() : keyValuePair.Key,keyValuePair.Value.ToString());
                    }
                }
                else
                {
                    yield return new KeyValuePair<string, string>(lcasePropetyName ? keyValuePair.Key.ToLower() : keyValuePair.Key,"");
                }
            }
        }
    }

    public static class Extension
    {
        public static UrlEncodedContent ToUrlEncodedContent(this object obj,bool lcasePropetyName=false)
        {
            return new UrlEncodedContent(obj,lcasePropetyName);
        }
    }
}
