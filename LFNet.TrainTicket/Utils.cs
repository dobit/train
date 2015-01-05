using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MSScriptControl;
using Newtonsoft.Json;
using Microsoft.JScript;
using Microsoft.JScript.Vsa;
using LFNet.Net.Http;
namespace LFNet.TrainTicket
{
    public static class Utils
    {
        public const string UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
        public static LFNet.Net.Http.JHttpClient GetHttpClient(this Account account, string referrer)
        {
            return HttpClientFactory.Create(referrer, UserAgent, account.Cookie);
        }

        /// <summary>
        /// 从Html中获取表单值
        /// </summary>
        /// <param name="htmlcontent"></param>
        /// <returns></returns>
        public static  NameValueCollection GetForms(string htmlcontent)
        {
            NameValueCollection forms=new NameValueCollection();
            Regex re = new Regex("<input\\s+[^>]*?name=(?<q>['\"]{0,1})(?<name>[^>\\s'\"]*)\\k<q>\\s+[^>]*?value=(?<q2>['\"]{0,1})(?<value>[^>\\s'\"]*)\\k<q2>", RegexOptions.IgnoreCase);
            MatchCollection mc = re.Matches(htmlcontent);
            foreach (Match ma in mc)
            {
                forms[ma.Groups["name"].ToString()] = ma.Groups["value"].ToString();
            }
            re = new Regex("<input\\s+[^>]*?value=(?<q2>['\"]{0,1})(?<value>[^>\\s'\"]*)\\k<q2>\\s+[^>]*?name=(?<q>['\"]{0,1})(?<name>[^>\\s'\"]*)\\k<q>", RegexOptions.IgnoreCase);
             mc = re.Matches(htmlcontent);
            foreach (Match ma in mc)
            {
                forms[ma.Groups["name"].ToString()] = ma.Groups["value"].ToString();
            }
            return forms;
        }

        /// <summary>
        /// 返回url编码格式的请求串
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        public static string ToQueryString(this NameValueCollection forms)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var key in forms.AllKeys)
            {
                sb.AppendFormat("&{0}={1}", Common.HtmlUtil.UrlEncode(key), Common.HtmlUtil.UrlEncode(forms[key]));
            }
            if(sb.Length>0)
            return sb.Remove(0, 1).ToString();
            else
            {
                return "";
            }
        }

        public static string ToSeatTypeValue(this SeatType seatType)
        {
            
            foreach (TextValue textValue in Global.GetSeatTypes())
            {
                if(textValue.Text.Equals(seatType.ToString()))
                {
                    return textValue.Value.ToString();
                }
            }
            return "";
        }

        public static string ToCardTypeValue(this CardType cardType)
        {
            switch (cardType)
            {
                case CardType.二代身份证:
                    return "1";

                case CardType.一代身份证:
                    return "2";
                case CardType.港澳通行证:
                    return "C";
                case CardType.台湾通行证:
                    return "G";
                case CardType.护照:
                    return "B";
                default:
                    throw new ArgumentOutOfRangeException("cardType");
            }
        }

        public static T ToJsonObject<T>(this string jsonString)
        {
            try
            {
                string str = jsonString;
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


        /// <summary>
        /// 获取座位的余票信息
        /// </summary>
        /// <param name="seatType"></param>
        /// <returns></returns>
        public static int GetRealSeatNumber(string YpInfoDetail, SeatType seatType)
        {
            int i = 0;
            int wz = 0;
            while (i < YpInfoDetail.Length)
            {
                string s = YpInfoDetail.Substring(i, 10);
                SeatType c_seat = (SeatType)s[0];
                string numStr = s.Substring(6, 4);
                int count = int.Parse(numStr);
                if (count >= 3000)
                {
                    count -= 3000;
                    wz = count ;
                }
                else
                {
                    if (seatType == c_seat) return count;
                }
                i += 10;
            }
            if (seatType == SeatType.无座) return wz;
            else
            {
                return 0;
            }

        }

        public static dynamic ExcuteJScript(string js)
        {
            //Microsoft.JScript.JSParser jsParser=new JSParser(new Context());
            //ScriptBlock scriptBlock = jsParser.Parse();

            ScriptControlClass scriptControl = new ScriptControlClass();
            scriptControl.Language ="javascript";

            scriptControl.Reset();
            
           return scriptControl.Eval(js);

        }

        /// <summary>
        /// bin216加密
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Bin216(string s)
        {
            string o = "";
            string n="";
            s += "";
           // char b = ""; 0;// "";
            for (int i = 0 ; i < s.Length; i++) {
               int b = s[i];//.charCodeAt(i);
                n = b.ToString("x2");//.toString(16);
                o += n.Length < 2 ? "0" + n : n;
            }
            return o;
        }

     
        public static string Encode32(string input) {
          string  keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
             input = Common.HtmlUtil.UrlEncode(input); 
            var output = ""; 
            uint chr1, chr2, chr3;
            uint enc1, enc2, enc3, enc4; 
            var i = 0;
            do
            {
                chr1 = input[i++]; 
                chr2 = input[i++];
                chr3 = input[i++]; 
                enc1 = chr1 >> 2; 
                enc2 = ((chr1 & 3) << 4) | (chr2 >> 4); 
                enc3 = ((chr2 & 15) << 2) | (chr3 >> 6); 
                enc4 = chr3 & 63;
                if (chr2<'0'||chr2>'0')  //isNaN(chr2))
                {
                    enc3 = enc4 = (char)64;
                } else if (chr3<'0'||chr3>'0')//isNaN(chr3))
                {
                    enc4 = (char)64;
                } 
                output = output + keyStr[enc1] + keyStr[enc2] + keyStr[enc3] + keyStr[enc4]; 
                chr1 = chr2 = chr3 = 'a';
                enc1 = enc2 = enc3 = enc4 ='a';
            } while (i < input.Length); 
            return output;
        }

    }
}