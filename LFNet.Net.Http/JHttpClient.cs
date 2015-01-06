using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LFNet.Net.Http
{
    /// <summary>
    /// 扩展Httpclient
    /// </summary>
    public class JHttpClient : HttpClient
    {
        public JHttpClient()
        {
            
        }

        public JHttpClient(HttpMessageHandler handler)
            : base(handler)
        {
            
        }

        public JHttpClient(HttpMessageHandler handler,bool disposeHandler ):base(handler,disposeHandler)
        {
            
        }

        /// <summary>
        /// 将一个对象以表单形式发送出去
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="obj"></param>
        /// <param name="lcaseNames"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<HttpResponseMessage> PostAsync(System.Uri requestUri, UrlEncodedContent content)
        {
            return await base.PostAsync(requestUri,content);
        }
        /// <summary>
        /// 将一个对象以表单形式发送出去
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="obj"></param>
        /// <param name="lcaseNames"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<HttpResponseMessage> PostAsync(System.Uri requestUri,  UrlEncodedContent content, CancellationToken cancellationToken)
        {
            return await base.PostAsync(requestUri, content,cancellationToken);
        }

        /// <summary>
        /// 将一个对象以表单形式发送出去
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="obj"></param>
        /// <param name="lcaseNames"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<HttpResponseMessage> PostAsync(string requestUri, UrlEncodedContent content)
        {
           
            return await base.PostAsync(requestUri, content);
        }

        /// <summary>
        /// 将一个对象以表单形式发送出去
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="obj"></param>
        /// <param name="lcaseNames"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<HttpResponseMessage> PostAsync(string requestUri, UrlEncodedContent content, CancellationToken cancellationToken)
        {
            return await base.PostAsync(requestUri, content, cancellationToken);
        }


        public async Task<string> GetStringAsync(string requestUri, UrlEncodedContent content)
        {
            string query = content.ReadAsStringAsync().Result;
            string url = requestUri.Contains("?") ? requestUri + "&" + query : requestUri + "?" + query;
            return await base.GetStringAsync(url);
        }


        public async Task<HttpResponseMessage> GetAsync(string requestUri, UrlEncodedContent content)
        {
            string query = content.ReadAsStringAsync().Result;
            string url = requestUri.Contains("?") ? requestUri + "&" + query : requestUri + "?" + query;
            return await base.GetAsync(url);
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri, UrlEncodedContent content, CancellationToken cancellationToken)
        {
            string query = content.ReadAsStringAsync().Result;
            string url = requestUri.Contains("?") ? requestUri + "&" + query : requestUri + "?" + query;
            return await base.GetAsync(url, cancellationToken);
        }
    }
    
    
}