using System;
using System.Collections.Generic;
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
            int enc1, enc2, enc3, enc4; 
            var i = 0;
            do
            {
                chr1 = input[i++]; 
                chr2 =(uint) (i < input.Length? input[i++]:0);
                chr3 = (uint) (i < input.Length ? input[i++] : 0); 
                enc1 = (int) (chr1 >> 2); 
                enc2 = (int) (((chr1 & 3) << 4) | (chr2 >> 4)); 
                enc3 = (int) (((chr2 & 15) << 2) | (chr3 >> 6)); 
                enc4 = (int) (chr3 & 63);
                if (chr2<'0'||chr2>'9')  //isNaN(chr2))
                {
                    enc3 = enc4 = 64;
                } else if (chr3<'0'||chr3>'9')//isNaN(chr3))
                {
                    enc4 = 64;
                }
                output = output + keyStr[enc1] + keyStr[enc2] + keyStr[enc3] + keyStr[enc4]; 
                chr1 = chr2 = chr3 = 'a';
                enc1 = enc2 = enc3 = enc4 ='a';
            } while (i < input.Length); 
            return output;
        }

    }

    public class Base32
    {
        uint delta = 0x9E3779B8;
            public string longArrayToString(uint[] data, bool includeLength) {
                int length = data.Length;
                int n = (length - 1) << 2;
                if (includeLength) {
                    var m = data[length - 1];
                    if ((m < n - 3) || (m > n))
                        return null; 
                    n = (int) m;
                }
                string[] dataStrings=new string[length];
                for (var i = 0; i < length; i++)
                {
                    var d = data[i];
                    dataStrings[i] =""+ (char) (d & 0xff) + (char) (d >> 8 & 0xff) + (char) (d >> 16 & 0xff) +
                                     (char) (d >> 24 & 0xff);
                        //String.fromCharCode(data[i] & 0xff, data[i] >>> 8 & 0xff, data[i] >>> 16 & 0xff, data[i] >>> 24 & 0xff);
                }
                if (includeLength) {
                    return string.Join("",dataStrings).Substring(0, n);
                } else {
                     return string.Join("",dataStrings);
                }
            }
            public uint[] stringToLongArray(string str, bool includeLength) {
                var length = str.Length;

                var len = length/4;
                if (length > len*4) len++;
                var result =new uint[includeLength?len+1:len];
               // var result = new List<int>();
                for (var i = 0; i < length; i += 4)
                {
                    var d = (uint)str[i];
                    for (var j = 1; j < 4 && i + j < length; j++)
                    {
                        int m = (2*4*j);
                        d = (uint) (d | str[i + j] << m) ;
                    }


                    result[i >> 2] = d;// (int)(str[i] | str[i + 1] << 8 | str[i + 2] << 16 | str[i + 3] << 24);
                }
                if (includeLength)
                {
                    result[result.Length-1] = (uint) length;
                } 
                return result;
            }
           
        
        public string encrypt(string str,string key) { 
            if (str == "") {
                 return "";
            }
                var v = stringToLongArray(str, true);
                var k = stringToLongArray(key, false);
                if (k.Length < 4)
                {
                    var temp = new uint[4];
                    Array.Copy(k,temp,k.Length);
                    // k.Length = 4;
                    k = temp;
                }
                var n = v.Length - 1;
            var z =(uint) v[n];
            var y = (uint)v[0];
            uint mx=0;
           // var e;
            int p=0;
            var q = Math.Floor(6 + 52.0/(n + 1));
            uint sum = 0;
                while (0 < q--) {
                    sum = sum +(delta & 0xffffffff); 
                  var  e = sum >> 2 & 3;
                    for ( p = 0; p < n; p++) {
                        y =(uint) v[p + 1];
                        mx =( (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));

                        z = v[p] = v[p] + (mx & 0xffffffff);
                    }
                    y = v[0];
                    mx =  (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z);
                    z = v[n] = v[n] + (mx & 0xffffffff);
                }
                return longArrayToString(v, false);
            }

    }
}