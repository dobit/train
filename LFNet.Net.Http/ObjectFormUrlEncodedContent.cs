using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using LFNet.Common.Reflection;

namespace LFNet.Net.Http
{
    /// <summary>
    /// 对象以表单形式提交
    /// </summary>
    public class ObjectFormUrlEncodedContent : FormUrlEncodedContent
    {
        public ObjectFormUrlEncodedContent(object obj,bool lcasePropetyName=false)
            : base(GetKeyValuePairs(obj, lcasePropetyName))
        {
            //this.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        }

        private static IEnumerable<KeyValuePair<string, string>> GetKeyValuePairs(object obj, bool lcasePropetyName)
        {
            Dictionary<string, object> keyObjects = new Dictionary<string, object>();
            LFNet.Common.Reflection.ObjectCopier.Copy(obj, keyObjects);
            foreach (KeyValuePair<string, object> keyValuePair in keyObjects)
            {
                if (keyValuePair.Value == null)
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
}
