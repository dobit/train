using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MSScriptControl;
using Newtonsoft.Json;
using Microsoft.JScript;
using Microsoft.JScript.Vsa;
namespace LFNet.TrainTicket
{
    public static class Utils
    {
        /// <summary>
        /// ��Html�л�ȡ��ֵ
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
        /// ����url�����ʽ������
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
                case CardType.�������֤:
                    return "1";

                case CardType.һ�����֤:
                    return "2";
                case CardType.�۰�ͨ��֤:
                    return "C";
                case CardType.̨��ͨ��֤:
                    return "G";
                case CardType.����:
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
        /// ��ȡ��λ����Ʊ��Ϣ
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
            if (seatType == SeatType.����) return wz;
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
    }
}