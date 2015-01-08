using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace LFNet.TrainTicket
{
    /// <summary>
    /// 创建一个Http请求
    /// </summary>
    public class HttpRequest
    {
        private readonly Action<Stream> _writeFileAction;

        public static HttpRequest Create(string url, string referer, CookieContainer cookie, HttpMethod method, string postStr, WebProxy proxy = null, bool isXMLHttpRequest=false, NameValueCollection headers=null)
        {
            return new HttpRequest(url, referer, cookie, method, postStr, isXMLHttpRequest, headers) { WebProxy = proxy };
        }
        public static HttpRequest Create(string url, string referer, CookieContainer cookie, NameValueCollection forms,  WebProxy proxy = null,bool isXMLHttpRequest=false,NameValueCollection headers=null)
        {
            return new HttpRequest(url, referer, cookie, forms,isXMLHttpRequest,headers) { WebProxy = proxy };
        }
        public static HttpRequest Create(string url, string referer, CookieContainer cookie, NameValueCollection forms, string file, WebProxy proxy = null)
        {
            return new HttpRequest(url, referer, cookie, forms, file) { WebProxy = proxy };
        }
        public string Url { get; set; }
        public string Referer { get; set; }
        public HttpMethod Method { get; set; }
        public string PostStr { get; set; }
        public bool IsXmlHttpRequest { get; set; }
        public NameValueCollection Headers { get; set; }
        public CookieContainer Cookie { get; set; }
        public NameValueCollection Forms { get; set; }
        public string File { get; set; }

        public bool UploadFile { get; set; }

        public WebProxy WebProxy { get; set; }
        /// <summary>
        /// 设置发送数据的类型
        /// 默认 application/x-www-form-urlencoded
        /// </summary>
        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        public HttpRequest(string url, string referer, CookieContainer cookie, HttpMethod method, string postStr, bool isXMLHttpRequest,NameValueCollection headers)
        {
            Url = url;
            Referer = referer;
            Method = method;
            PostStr = postStr;
            IsXmlHttpRequest = isXMLHttpRequest;
            Headers = headers;
            Cookie = cookie;
        }
        public HttpRequest(string url, string referer, CookieContainer cookie, NameValueCollection forms, bool isXMLHttpRequest, NameValueCollection headers)
         {
             Url = url;
             Referer = referer;
             Method = HttpMethod.POST;
             StringBuilder sb=new StringBuilder();
             foreach (var key in forms.AllKeys)
             {
                 sb.AppendFormat("&{0}={1}", key, Common.HtmlUtil.UrlEncode(forms[key]));
             }
             
             PostStr = sb.Remove(0,1).ToString();
             Cookie = cookie;
            IsXmlHttpRequest = isXMLHttpRequest;
            Headers = headers;
         }
        public HttpRequest(string url, string referer, CookieContainer cookie, NameValueCollection forms, string filename)
        {
            Url = url;
            Referer = referer;
            Cookie = cookie;
            Forms = forms;
            File = filename;
            UploadFile = true;
            Method = HttpMethod.POST;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="referer">引用页地址</param>
        /// <param name="cookie">cookie</param>
        /// <param name="method">请求方式</param>
        /// <param name="forms">提交的表单</param>
        /// <param name="filename">上传的文件</param>
        /// <param name="writeFileAction"></param>
        public HttpRequest(string url, string referer, CookieContainer cookie, HttpMethod method = HttpMethod.GET, NameValueCollection forms = null, string filename = "", Action<Stream> writeFileAction = null)
        {
            Url = url;
            Referer = referer;
            Cookie = cookie;
            Forms = forms ?? new NameValueCollection();
            File = filename;
            UploadFile = string.IsNullOrEmpty(filename);
            Method = string.IsNullOrEmpty(filename) ? method : HttpMethod.POST;
            _writeFileAction = writeFileAction;

        }
        public bool SaveToFile(string localFile)
        {
            try
            {
                Stream stream = GetStream();
                using (var fs = new FileStream(localFile, FileMode.CreateNew))
                {
                    stream.CopyTo(fs);
                    fs.Flush();
                    fs.Dispose();
                }
                stream.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetString()
        {
            return GetString(Encoding.UTF8);
        }

        public string GetString(Encoding encoding)
        {
            try
            {
                using (StreamReader streamReader = new StreamReader(GetStream(), encoding))
                {
                    string ret = streamReader.ReadToEnd();
                    return ret;
                }
            }catch
            {
                return "";
            }


        }

        public T GetJsonObject<T>()
        {
            return GetJsonObject<T>(Encoding.UTF8);
        }

        public T GetJsonObject<T>(Encoding encoding)
        {
            try
            {
                string str = GetString(encoding);
                int pos = str.IndexOf('{');
                string jsonStr = str.Substring(pos, str.LastIndexOf('}') - pos + 1);
                JsonReader jsonReader = new JsonTextReader(new StringReader(jsonStr));

                JsonSerializer jsonSerializer = new JsonSerializer();

                return jsonSerializer.Deserialize<T>(jsonReader);
            }
            catch
            {
                return default(T);
            }
        }

        public T GetXmlObject<T>() where T : class
        {
            return GetXmlObject<T>(Encoding.UTF8);
        }

        public T GetXmlObject<T>(Encoding encoding) where T : class
        {
            try
            {
                System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(new StringReader(GetString(encoding))) as T;

            }
            catch
            {
                return default(T);
            }
        }

        public Stream GetStream()
        {
            try
            {
                return GetResponse().GetResponseStream();

            }
            catch(WebException webException)
            {
               if( webException.Status==WebExceptionStatus.Timeout ||webException.Status==WebExceptionStatus.SendFailure)
               {
                   return GetResponse().GetResponseStream();
               }
               throw;
            }
           
            
        }

        private int redirectCnt = 0;
        private string _contentType="application/x-www-form-urlencoded";

        public WebResponse GetResponse()
        {

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Url);
            webRequest.CookieContainer = Cookie;
            webRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
            
            webRequest.Referer = Referer;
            webRequest.Method = Method.ToString();
            webRequest.Timeout =30000;
            webRequest.AllowAutoRedirect = false;
            webRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            webRequest.KeepAlive = true;
            if(IsXmlHttpRequest)
            {
                webRequest.Headers["x-requested-with"] = "XMLHttpRequest";
            }
            if (Headers != null)
                foreach (string key in Headers.Keys)
                {
                    webRequest.Headers[key] = Headers[key];
                }
            if(WebProxy!=null)
            {
                webRequest.Proxy = WebProxy;
            }

            if (Method == HttpMethod.POST)
            {
                if (!UploadFile)
                {
                    if(!string.IsNullOrEmpty(ContentType))
                        webRequest.ContentType = ContentType;
                    Stream requestStream = webRequest.GetRequestStream();
                    byte[] postData = Encoding.UTF8.GetBytes(PostStr);
                    requestStream.Write(postData, 0, postData.Length);
                    requestStream.Close();
                }
                else
                {
                    string boundary = "------------" + DateTime.Now.Ticks.ToString("x");
                    const string line = "\r\n";
                    webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

                    StringBuilder sb = new StringBuilder();
                    foreach (string key in Forms.Keys)
                    {
                        sb.Append("--" + boundary).Append(line);
                        sb.Append("Content-Disposition: form-data; name=\"" + key + "\"");
                        sb.Append(line);
                        sb.Append(line);
                        sb.Append(Forms[key]).Append(line);
                    }
                    sb.Append("--" + boundary).Append(line);
                    sb.Append("Content-Disposition: form-data; name=\"Filedata\"; ");
                    string filename = File;
                    if (filename.Contains(":"))
                    {
                        filename = Path.GetFileName(filename);
                    }
                    sb.Append("filename=\"").Append(filename).Append("\"").Append(line);
                    sb.Append("Content-Type: application/octet-stream").Append(line);
                    sb.Append(line);

                    byte[] requestData = Encoding.UTF8.GetBytes(sb.ToString());
                    Stream requestStream = webRequest.GetRequestStream();
                    requestStream.Write(requestData, 0, requestData.Length);
                    if (_writeFileAction != null)
                    {
                        _writeFileAction(requestStream);
                    }
                    else
                    {
                        using (FileStream fileStream = new FileStream(File, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {

                            requestStream.Write(requestData, 0, requestData.Length);
                            fileStream.CopyTo(requestStream);

                        }
                    }
                    byte[] endData = Encoding.UTF8.GetBytes("/r/n--" + boundary + "--");
                    requestStream.Write(endData, 0, endData.Length);
                    requestStream.Close();

                }
            }
          var   response=webRequest.GetResponse() as HttpWebResponse;
            string location = response.Headers[HttpResponseHeader.Location];
           
            if (!string.IsNullOrEmpty(location))
            {
                if (redirectCnt > 5)
                {
                    response.Close();
                    throw new Exception("跳转次数过多" + location);
                }
                this.Url = new Uri(new Uri(Url), location).ToString();
                this.Method = HttpMethod.GET;
                redirectCnt++;
                return GetResponse();
            }
            else

                return response;
        }

        
    }
}