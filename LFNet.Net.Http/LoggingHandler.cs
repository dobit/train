using System.Diagnostics;
using System.Net.Http;

namespace LFNet.Net.Http
{
    /// <summary>
    /// ≤∂◊Ωhttp»’÷æ
    /// </summary>
    public class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(HttpMessageHandler innerHandler):base(innerHandler)
        {
            
        }

        protected override async System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            Debug.WriteLine("Request:");
            Debug.WriteLine(request.ToString());
            if (request.Content != null)
            {
                Debug.WriteLine(await  request.Content.ReadAsStringAsync());
            }
            HttpResponseMessage response= await base.SendAsync(request, cancellationToken);
            Debug.WriteLine("Response:");
            Debug.WriteLine(response.ToString());
            if (response.Content != null)
            {
                Debug.WriteLine(await response.Content.ReadAsStringAsync());
            }
            return response;
        }
    }
}