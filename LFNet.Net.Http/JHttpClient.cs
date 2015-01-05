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
        public async System.Threading.Tasks.Task<HttpResponseMessage> PostAsync(System.Uri requestUri, object obj, bool lcaseNames = false)
        {
            return await base.PostAsync(requestUri, new ObjectFormUrlEncodedContent(obj, lcaseNames));
        }
        /// <summary>
        /// 将一个对象以表单形式发送出去
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="obj"></param>
        /// <param name="lcaseNames"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<HttpResponseMessage> PostAsync(System.Uri requestUri, object obj, CancellationToken cancellationToken, bool lcaseNames = false)
        {
            return await base.PostAsync(requestUri, new ObjectFormUrlEncodedContent(obj,lcaseNames),cancellationToken);
        }

        /// <summary>
        /// 将一个对象以表单形式发送出去
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="obj"></param>
        /// <param name="lcaseNames"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<HttpResponseMessage> PostAsync(string requestUri, object obj, bool lcaseNames = false)
        {
            return await base.PostAsync(requestUri, new ObjectFormUrlEncodedContent(obj, lcaseNames));
        }

        /// <summary>
        /// 将一个对象以表单形式发送出去
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="obj"></param>
        /// <param name="lcaseNames"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<HttpResponseMessage> PostAsync(string requestUri, object obj,CancellationToken cancellationToken, bool lcaseNames = false)
        {
            return await base.PostAsync(requestUri, new ObjectFormUrlEncodedContent(obj, lcaseNames), cancellationToken);
        }


        public async Task<string> GetStringAsync(string requestUri, object obj, bool lcaseNames = false)
        {
            string query = new ObjectFormUrlEncodedContent(obj, lcaseNames).ReadAsStringAsync().Result;
            string url = requestUri.Contains("?") ? requestUri + "&" + query : requestUri + "?" + query;
            return await base.GetStringAsync(url);
        }


        public async Task<HttpResponseMessage> GetAsync(string requestUri, object obj, bool lcaseNames = false)
        {
            string query = new ObjectFormUrlEncodedContent(obj, lcaseNames).ReadAsStringAsync().Result;
            string url = requestUri.Contains("?") ? requestUri + "&" + query : requestUri + "?" + query;
            return await base.GetAsync(url);
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri, object obj, CancellationToken cancellationToken, bool lcaseNames = false)
        {
            string query = new ObjectFormUrlEncodedContent(obj, lcaseNames).ReadAsStringAsync().Result;
            string url = requestUri.Contains("?") ? requestUri + "&" + query : requestUri + "?" + query;
            return await base.GetAsync(url, cancellationToken);
        }
    }
    
    
}