using System.Net.Http;

namespace LFNet.Net.Http
{
    /// <summary>
    /// жиЪд
    /// </summary>
    public class RetryHandler : DelegatingHandler
    {
        private const int MaxRetries = 3;

        public RetryHandler(HttpMessageHandler innerHandler) : base(innerHandler)
        {
            
        }
        protected override async System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            HttpResponseMessage response;
            for (int i = 0; i < MaxRetries-1; i++)
            {
                response=await  base.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                
            }
            response=await  base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}