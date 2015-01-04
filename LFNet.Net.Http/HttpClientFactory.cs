using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace LFNet.Net.Http
{
    public static class HttpClientFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="referrer"></param>
        /// <param name="userAgent"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static JHttpClient Create(string referrer = "", string userAgent = "", CookieContainer cookie = null)
        {
            //RulesString("&User-Agents", true) 
            //BindPref("fiddlerscript.ephemeral.UserAgentString")
            //RulesStringValue(0,"Netscape &3", "Mozilla/3.0 (Win95; I)")
            //RulesStringValue(1,"WinPhone7", "Mozilla/4.0 (compatible: MSIE 7.0; Windows Phone OS 7.0; Trident/3.1; IEMobile/7.0; SAMSUNG; SGH-i917)")
            //RulesStringValue(2,"WinPhone8.1", "Mozilla/5.0 (Mobile; Windows Phone 8.1; Android 4.0; ARM; Trident/7.0; Touch; rv:11.0; IEMobile/11.0; NOKIA; Lumia 520) like iPhone OS 7_0_3 Mac OS X AppleWebKit/537 (KHTML, like Gecko) Mobile Safari/537")
            //RulesStringValue(3,"&Safari5 (Win7)", "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.21.1 (KHTML, like Gecko) Version/5.0.5 Safari/533.21.1")
            //RulesStringValue(4,"Safari7 (Mac)", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9) AppleWebKit/537.71 (KHTML, like Gecko) Version/7.0 Safari/537.71")
            //RulesStringValue(5,"iPad", "Mozilla/5.0 (iPad; CPU OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A403 Safari/8536.25")
            //RulesStringValue(6,"iPhone6", "Mozilla/5.0 (iPhone; CPU iPhone OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A405 Safari/8536.25")
            //RulesStringValue(7,"IE &6 (XPSP2)", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1)")
            //RulesStringValue(8,"IE &7 (Vista)", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; SLCC1)")
            //RulesStringValue(9,"IE 8 (Win2k3 x64)", "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; WOW64; Trident/4.0)")
            //RulesStringValue(10,"IE &8 (Win7)", "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0)")
            //RulesStringValue(11,"IE 9 (Win7)", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)")
            //RulesStringValue(12,"IE 10 (Win8)", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)")
            //RulesStringValue(13,"IE 11 (Surface2)", "Mozilla/5.0 (Windows NT 6.3; ARM; Trident/7.0; Touch; rv:11.0) like Gecko")
            //RulesStringValue(14,"IE 11 (Win8.1)", "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko")
            //RulesStringValue(15,"IE 12 (Win10)", "Mozilla/5.0 (Windows NT 6.4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36 Edge/12.0")
            //RulesStringValue(16,"&Opera", "Opera/9.80 (Windows NT 6.2; WOW64) Presto/2.12.388 Version/12.17")
            //RulesStringValue(17,"&Firefox 3.6", "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.7) Gecko/20100625 Firefox/3.6.7")
            //RulesStringValue(18,"&Firefox 33", "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:33.0) Gecko/20100101 Firefox/33.0")
            //RulesStringValue(19,"&Firefox Phone", "Mozilla/5.0 (Mobile; rv:18.0) Gecko/18.0 Firefox/18.0")
            //RulesStringValue(20,"&Firefox (Mac)", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.8; rv:24.0) Gecko/20100101 Firefox/24.0")
            //RulesStringValue(21,"Chrome", "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.62 Safari/537.36")
            //RulesStringValue(22,"ChromeBook", "Mozilla/5.0 (X11; CrOS armv7l 2913.260.0) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.99 Safari/537.11")
            //RulesStringValue(23,"GoogleBot Crawler", "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)")
            //RulesStringValue(24,"Kindle Fire (Silk)", "Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10_6_3; en-us; Silk/1.0.22.79_10013310) AppleWebKit/533.16 (KHTML, like Gecko) Version/5.0 Safari/533.16 Silk-Accelerated=true")
            //RulesStringValue(25,"&Custom...", "%CUSTOM%")
            
            JHttpClient httpClient =cookie == null?new JHttpClient() : new JHttpClient(new HttpClientHandler {CookieContainer = cookie,UseCookies=true});
            if(string.IsNullOrEmpty(referrer))httpClient.DefaultRequestHeaders.Referrer = new Uri(referrer);
            if (string.IsNullOrEmpty(referrer)) httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
            return new JHttpClient();
        }




        
    }
}