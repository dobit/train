using System.Net.Http;

namespace LFNet.Net.Http
{
    /// <summary>
    /// ��չHttpclient
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
        /// ��һ�������Ա���ʽ���ͳ�ȥ
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="obj"></param>
        /// <param name="lcaseNames"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<HttpResponseMessage> PostAsync(System.Uri requestUri,object obj,bool lcaseNames=false)
        {
            return await base.PostAsync(requestUri, new ObjectFormUrlEncodedContent(obj,lcaseNames));
        }

        /// <summary>
        /// ��һ�������Ա���ʽ���ͳ�ȥ
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="obj"></param>
        /// <param name="lcaseNames"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<HttpResponseMessage> PostAsync(string requestUri, object obj, bool lcaseNames = false)
        {
            return await base.PostAsync(requestUri, new ObjectFormUrlEncodedContent(obj, lcaseNames));
        }


    }
    
    
}