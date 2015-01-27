using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LFNet.Configuration;
using LFNet.Net.Http;
using LFNet.TrainTicket.BLL;
using LFNet.TrainTicket.Config;
using LFNet.TrainTicket.Entity;
using LFNet.TrainTicket.Response;

namespace LFNet.TrainTicket.DAL
{
    /// <summary>
    /// ���ݽӿ�
    /// </summary>
    public static class InterfaceProvider
    {
        #region  ����
        /// <summary>
        /// ��ȡվ����Ϣ��URL
        /// </summary>
        public const string StationsUrl =
            "https://kyfw.12306.cn/otn/resources/js/framework/station_name.js?station_version=1.8253";

        /// <summary>
        /// ��½�ύ��ַ
        /// </summary>
        public const string LoginAysnSuggestUrl = "https://kyfw.12306.cn/otn/login/loginAysnSuggest";

        /// <summary>
        /// ��ȡ�����
        /// </summary>
        public const string CheckRandCodeAnsynUrl = "https://kyfw.12306.cn/otn/passcodeNew/checkRandCodeAnsyn";

        /// <summary>
        /// ��ѯҳ��ַ
        /// </summary>
        public const string QueryPageUrl = "https://kyfw.12306.cn/otn/leftTicket/init";

        /// <summary>
        /// ��½ҳ��ַ
        /// </summary>
        public const string LoginPageUrl = "https://kyfw.12306.cn/otn/login/init";

        /// <summary>
        /// �����ύҳ��
        /// </summary>
        public const string OrderPageUrl = "https://kyfw.12306.cn/otn/confirmPassenger/initDc";

        /// <summary>
        /// ��ҳ
        /// </summary>
        public const string HomePage = "http://www.12306.cn";

        /// <summary>
        /// ��Ʊ��ҳ
        /// </summary>
        public const string TicketHomePage = "https://kyfw.12306.cn/otn/";

        /// <summary>
        /// �ҵ�12306ҳ��
        /// </summary>
        public const string InitMy12306PageUrl = "https://kyfw.12306.cn/otn/index/initMy12306";

        /// <summary>
        /// ��Ʊ��ѯҳ��
        /// </summary>
        public const string LeftTicketUrl = "https://kyfw.12306.cn/otn/leftTicket/init";
        #endregion

        #region ���ݻ�ȡ

        /// <summary>
        /// ��ȡ��¼ҳ������
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task<LoginPageResult> GetLoginPageResult(this Client client)
        {
          string content=   await GetStringAsync(client, LoginPageUrl);//, TicketHomePage);
          DynamicJsResult dynamicJsResult=  await GetDynamicJsAction(client, content, LoginPageUrl);
          
            LoginPageResult result=new LoginPageResult();
            result.DynamicJsResult = dynamicJsResult;
            result.RandCodeImage = await InterfaceProvider.GetRandCode(client);
            return result;
        }

        /// <summary>
        /// ��ȡ��������ѯҳ��
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task<PageResult> GetQueryPageResult(this Client client)
        {
            string content = await GetStringAsync(client, QueryPageUrl,InitMy12306PageUrl);
            DynamicJsResult dynamicJsResult = await GetDynamicJsAction(client, content, QueryPageUrl);
            var result = new PageResult();
            result.DynamicJsResult = dynamicJsResult;
            return result;
        }

        /// <summary>
        /// ��ȡ��֤��
        /// </summary>
        /// <param name="client"></param>
        /// <param name="type">0=��¼��1=�µ�,2=�Զ��µ�</param>
        /// <returns></returns>
        public static async Task<Image> GetRandCode(this Client client, int type = 0)
        {
            string url;
            string referurl;// = LoginPageUrl;
             if(type==1)
            {
                url = "https://kyfw.12306.cn/otn/passcodeNew/getPassCodeNew?module=passenger&rand=randp&0." +
                      new Random().Next(10000000, 99999999) +
                      new Random().Next(10000000, 99999999);
                referurl = OrderPageUrl;
            }else 
            {
                url = "https://kyfw.12306.cn/otn/passcodeNew/getPassCodeNew?module=login&rand=sjrand&0." +
                      new Random().Next(10000000, 99999999) +
                      new Random().Next(10000000, 99999999);
                referurl = LoginPageUrl;
            }
             if (type == 2) referurl = QueryPageUrl;
            Stream stream = await GetHttpClient(client, referurl).GetStreamAsync(url);
            return Image.FromStream(stream);
        }

        /// <summary>
        /// �����֤��
        /// </summary>
        /// <param name="randCode">��֤��</param>
        /// <param name="randType">0=��½��1�µ�</param>
        /// <param name="token">��randtype=2������</param>
        /// <returns></returns>
        public static async Task<Response<CheckRandCodeAnsynResponse>> CheckRandCodeAnsyn(this Client client, string randCode, int randType, string token)
        {
            //randCode=6eed&rand=sjrand&randCode_validate=
            //randCode=nsph&rand=randp&_json_att=&REPEAT_SUBMIT_TOKEN=c92104171aee0b7323c8e2466a9d3f8c
            if (randType == 0)
            {

                return await AjaxPostToJsonObjectAsync<Response<CheckRandCodeAnsynResponse>>(client, CheckRandCodeAnsynUrl, new
                {
                    randCode,
                    rand = "sjrand",
                    randCode_validate = ""
                }.ToUrlEncodedContent(), LoginPageUrl);

            }
            else if(randType==1)
            {
                return await AjaxPostToJsonObjectAsync<Response<CheckRandCodeAnsynResponse>>(client, CheckRandCodeAnsynUrl, new
                {
                    randCode,
                    rand = "randp",
                    _json_att = "",
                    REPEAT_SUBMIT_TOKEN = token
                }.ToUrlEncodedContent(), OrderPageUrl);
            }
            else
            {
                return await AjaxPostToJsonObjectAsync<Response<CheckRandCodeAnsynResponse>>(client, CheckRandCodeAnsynUrl, new
                {
                    randCode,
                    rand = "sjrand",
                    randCode_validate = ""
                }.ToUrlEncodedContent(), QueryPageUrl);
            }


        }


        /// <summary>
        /// ��������״̬
        /// </summary>
        /// <returns></returns>
        public static async Task<LoginState> CheckState(this Client client)
        {
           
            string content = await GetHttpClient(client, QueryPageUrl).GetStringAsync(InitMy12306PageUrl);
            if (content.Contains("ϵͳά��"))
            {
                return LoginState.Maintenance;
            }
            if (content.Contains("loginForm") && content.Contains("loginUserDTO.user_name"))
            {
                //IsLogin = false;
                return LoginState.UnLogin;
            }
            else
            {
                //IsLogin = true;
                return LoginState.Login;
            }
        }




        #region ��̬js
        /// <summary>
        /// ҳ�涯̬js���
        /// </summary>
        /// <returns></returns>
        public static async Task<DynamicJsResult> GetDynamicJsAction(this Client client, string content, string pageUrl)
        {
            Regex jsUrlRegex = new Regex(@"<script src=""/otn/dynamicJs/(.*?)""", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string jsurl = new Uri(new Uri(pageUrl), "/otn/dynamicJs/" + jsUrlRegex.Match(content).Groups[1]).ToString();//jsUrlRegex.Replace(content, "$1")).ToString();
            string jsContent = await GetHttpClient(client, pageUrl).GetStringAsync(jsurl);
            Regex jsregex = new Regex(@"(function\sbin216(.*?))function\saj()");
            string js = jsregex.Match(jsContent).Groups[1].ToString();//.Replace(jsContent, "$1");
            Regex keyregex = new Regex(@"var\s*?key\s*?=[""']([A-Za-z0-9+/=]*?)[""'];");
            string key = keyregex.Match(jsContent).Groups[1].ToString(); //Replace(jsContent, "$1");
            int cnt = new Regex(@"value\+='1';").Matches(jsContent).Count;
            string value = "";
            for (int i = 0; i < cnt; i++)
            {
                value += "1";
            }
            value = Utils.ExcuteJScript(js + ";" + "encode32(bin216(Base32.encrypt('" + value + "', '" + key + "')));");

            DynamicJsResult result = new DynamicJsResult();
            result.Key = key;
            result.Value = value;

            
            Regex postUrlRegex = new Regex(@"/otn/dynamicJs/(.*?)'", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            string postUrl = new Uri(new Uri(pageUrl), "/otn/dynamicJs/" + postUrlRegex.Match(jsContent).Groups[1]).ToString();

            Task<string> postDynamicJsStringAsync = PostDynamicJsStringAsync(client, LoginPageUrl, postUrl);
            postDynamicJsStringAsync.DelayToRun(1000);
            return result;

        }

        

        private static async Task<string> GetDynamicJsStringAsync(this Client client, string referrer, string dynamicJsUrl)
        {
            return await  GetStringAsync(client,dynamicJsUrl, referrer);
        }
        private static async Task<string> PostDynamicJsStringAsync(this Client client, string referrer, string dynamicJsUrl)
        {
            return await PostToStringAsync(client, dynamicJsUrl, new StringContent(""), referrer);
        }
        #endregion

        public static async Task<Response<LoginAysnSuggestResponse>> LoginAsynSuggest(this Client client,string userName,string password,string randCode, string key,string value)
        {
            Dictionary<string, string> nameValues = new Dictionary<string, string>()
                {
                    {"loginUserDTO.user_name",userName},
                    {"userDTO.password",password},
                    {"randCode",randCode},
                     {"randCode_validate",""},
                   {key,value},
                   {"myversion","undefined"	},  
                };
            
            return await AjaxPostToJsonObjectAsync<Response<LoginAysnSuggestResponse>>(client, LoginAysnSuggestUrl,
                new FormUrlEncodedContent(nameValues), LoginPageUrl);
        }


        /// <summary>
        /// ��ȡ�˿���Ϣ
        /// </summary>
        /// <returns></returns>
        public static async Task<Response<GetPassengerDTOs>> GetPassengers(this Client client, string submitToken = "")
        {
            HttpContent obj = new StringContent("");
            if (submitToken != "")
                obj = new
                {
                    _json_att = "",
                    REPEAT_SUBMIT_TOKEN = submitToken
                }.ToUrlEncodedContent();
            return await AjaxPostToJsonObjectAsync<Response<GetPassengerDTOs>>(client, "https://kyfw.12306.cn/otn/confirmPassenger/getPassengerDTOs", obj, QueryPageUrl);

        }

        /// <summary>
        /// ����û�״̬
        /// </summary>
        /// <returns></returns>
        public static async Task<Response<CheckUserResponse>> CheckUser(this Client client)
        {
            return
                await
                    AjaxPostToJsonObjectAsync<Response<CheckUserResponse>>(client,
                        "https://kyfw.12306.cn/otn/login/checkUser", new StringContent(""), QueryPageUrl);

        }

        /// <summary>
        /// ��ѯ��־
        /// </summary>
        /// <param name="client"></param>
        /// <param name="trainDate"></param>
        /// <param name="fromStationTeleCode"></param>
        /// <param name="toStationTeleCode"></param>
        /// <param name="type"></param>
        public static async void QueryTLog(this Client client, DateTime trainDate, string fromStationTeleCode,
            string toStationTeleCode, int type = 0)
        {
            string purpose_codes = type == 0 ? "ADULT" : "STUDENT";
            string url = string.Format("https://kyfw.12306.cn/otn/leftTicket/log?leftTicketDTO.train_date={0}&leftTicketDTO.from_station={1}&leftTicketDTO.to_station={2}&purpose_codes={3}"
                , trainDate.ToString("yyyy-MM-dd"), fromStationTeleCode, toStationTeleCode, purpose_codes);

          await   AjaxGetStringAsync(client, url, QueryPageUrl);
        }

        /// <summary>
        /// ��ѯ�г���Ϣ
        /// </summary>
        /// <param name="client"></param>
        /// <param name="trainDate"></param>
        /// <param name="fromStationTeleCode"></param>
        /// <param name="toStationTeleCode"></param>
        /// <param name="type">0=����,1=ѧ��</param>
        /// <returns></returns>
        public static async Task<Response<QueryResponse>> QueryTrainInfos(this Client client, DateTime trainDate, string fromStationTeleCode, string toStationTeleCode, int type = 0)
        {
          
            //GET https://kyfw.12306.cn/otn/leftTicket/queryT?leftTicketDTO.train_date=2015-02-19&leftTicketDTO.from_station=BJP&leftTicketDTO.to_station=NCG&purpose_codes=ADULT HTTP/1.1
            string purpose_codes = type == 0 ? "ADULT" : "STUDENT";
            string url = string.Format("https://kyfw.12306.cn/otn/leftTicket/queryT?leftTicketDTO.train_date={0}&leftTicketDTO.from_station={1}&leftTicketDTO.to_station={2}&purpose_codes={3}"
                , trainDate.ToString("yyyy-MM-dd"), fromStationTeleCode, toStationTeleCode, purpose_codes);

            return await AjaxGetToJsonObjectAsync<Response<QueryResponse>>(client, url, QueryPageUrl);

        }
        /// <summary>
        /// �Զ��ύ
        /// </summary>
        /// <returns></returns>
        public static async Task<Response<AutoSubmitOrderRequestResponse>> AutoSubmitOrderRequest(this Client client, string secretStr, SeatType seatType, DynamicJsResult queryPageDynamicJsResult, AccountInfo account)
        {
            //https://kyfw.12306.cn/otn/confirmPassenger/autoSubmitOrderRequest
            string passengerTicketStr = "";
            string oldPassengerStr = "";
            foreach (Passenger passenger in client.Passengers.Where(p=>p.Checked))
            {
               
                passengerTicketStr += string.Format("{0},{1},{2},{3},{4},{5},{6},N_",
                    seatType.ToSeatTypeValue(),
                    (int)passenger.SeatDetailType,
                    (int)passenger.TicketType, passenger.Name,
                    passenger.CardType.ToCardTypeValue(),
                    passenger.CardNo, passenger.MobileNo);
                oldPassengerStr += string.Format("{0},{1},{2},{3}_", passenger.Name,
                    passenger.CardType.ToCardTypeValue(),
                    passenger.CardNo,(int)passenger.TicketType);
            }
            passengerTicketStr = passengerTicketStr.TrimEnd('_');

            Dictionary<string, string> newforms = new Dictionary<string, string>()
            {
                {queryPageDynamicJsResult.Key,queryPageDynamicJsResult.Value	},   
                {"myversion","undefined"	},  
                {"secretStr",secretStr	},
                {"train_date",	account.TrainDate.ToString("yyyy-MM-dd")},
                //{"back_train_date",account.BackTrainDate.ToString("yyyy-MM-dd")},
                {"tour_flag","dc"},
                {"purpose_codes","ADULT"},
                {"query_from_station_name",account.FromStation},
                {"query_to_station_name",account.ToStation},
                {"cancel_flag","2"},
                {"bed_level_order_num","000000000000000000000000000000"},
                {"passengerTicketStr",passengerTicketStr},
                {"oldPassengerStr",oldPassengerStr},
            };
            FormUrlEncodedContent formUrlEncodedContent=new FormUrlEncodedContent(newforms);
            ;
            HttpContent content = new StringContent(formUrlEncodedContent.ReadAsStringAsync().Result.Replace("cancel_flag", "&cancel_flag"), Encoding.UTF8, "application/x-www-form-urlencoded");

            return await AjaxPostToJsonObjectAsync<Response<AutoSubmitOrderRequestResponse>>(client, "https://kyfw.12306.cn/otn/confirmPassenger/autoSubmitOrderRequest", content, QueryPageUrl);

        }

        /// <summary>
        /// https://dynamic.12306.cn/otsweb/order/querySingleAction.do?method=submutOrderRequest
        /// </summary>
        /// <param name="client"></param>
        /// <param name="secretStr">���ܴ�</param>
        /// <param name="queryPageDynamicJsResult"></param>
        /// <param name="account"></param>
        /// <returns>�õ�ҳ��ı���Ϣ</returns>
        public static async Task<Response<string>> SubmitOrderRequest(this Client client, string secretStr, DynamicJsResult queryPageDynamicJsResult,  AccountInfo account)
        {

            //var config = ConfigFileManager.GetConfig<BuyTicketConfig>();
            //;
            Dictionary<string, string> newforms = new Dictionary<string, string>()
            {
                {queryPageDynamicJsResult.Key,queryPageDynamicJsResult.Value	},   
                {"myversion","undefined"	},  
                {"secretStr",secretStr	},
                {"train_date",	account.TrainDate.ToString("yyyy-MM-dd")},
                {"back_train_date",account.BackTrainDate.ToString("yyyy-MM-dd")},
                {"tour_flag","dc"},
                {"purpose_codes","ADULT"},
                {"query_from_station_name",account.FromStation},
                {"query_to_station_name",account.ToStation},
                {"undefined",""},
            };

            /*_jc_save_czxxcx_toStation=%u5B9C%u6625%u897F%2CYCG; 
             * _jc_save_czxxcx_fromDate=2014-12-25;  
             * _jc_save_fromStation=%u5317%u4EAC%2CBJP;
             * _jc_save_toStation=%u5357%u660C%2CNCG; 
             * _jc_save_fromDate=2015-02-19; 
             * _jc_save_toDate=2014-12-31; 
             * _jc_save_wfdc_flag=dc; 
             * current_captcha_type=C
             * */
            DateTime expires = DateTime.Now.AddDays(365);
            var clientCollection = new CookieCollection()
            {
                //new client("_jc_save_czxxcx_toStation",""),
                //new client("_jc_save_czxxcx_fromDate","2014-12-25"),
                new Cookie("_jc_save_fromStation",LFNet.Common.HtmlUtil.UrlEncode(account.FromStation+","+account.FromStationTeleCode)){Expires = expires },
                new Cookie("_jc_save_toStation",LFNet.Common.HtmlUtil.UrlEncode(account.ToStation+","+account.ToStationTeleCode)){Expires = expires },
                new Cookie("_jc_save_fromDate",account.TrainDate.ToString("yyyy-MM-dd")){Expires = expires },
                new Cookie("_jc_save_toDate",account.BackTrainDate.ToString("yyyy-MM-dd")){Expires = expires },
                new Cookie("_jc_save_wfdc_flag","dc"){Expires = expires },
                // new client("current_captcha_type","C"){Expires = expires },
            };
            client.Cookie.Add(new Uri(ActionUrls.TicketHomePage), clientCollection);
            return await AjaxPostToJsonObjectAsync<Response<string>>(client, "https://kyfw.12306.cn/otn/leftTicket/submitOrderRequest", new FormUrlEncodedContent(newforms), QueryPageUrl);

            ////ת������ҳ��
            //string content = GetHttpClient(ActionUrls.QueryPageUrl).PostAsync("https://kyfw.12306.cn/otn/confirmPassenger/initDc", new UrlEncodedContent(new { _json_att = "" })).Result.Content.ReadAsStringAsync().Result;

            //Dictionary<string, string> dynamicJsAction = GetDynamicJsAction(content, "https://kyfw.12306.cn/otn/confirmPassenger/initDc");
            //Regex regex = new Regex(@"[A-z0-9]{32,}");
            //var matches = regex.Matches(content);
            //string token = matches[0].Groups[1].ToString();
            //string key_check_isChange = matches[1].Groups[1].ToString();
            //string leftTicketStr = matches[2].Groups[1].ToString();
            //dynamicJsAction.Add("REPEAT_SUBMIT_TOKEN", token);
            //dynamicJsAction.Add("key_check_isChange", key_check_isChange);
            //dynamicJsAction.Add("leftTicketStr", leftTicketStr);
            ////���û�ȡ�û���Ϣ
            //GetPassengers(token);
            //return dynamicJsAction;
        }

        /// <summary>
        /// ��ȡ�����ύҳ������
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task<InitDcResult> GetInitDc(this Client client)
        {
          
            string content=await
                    AjaxPostToStringAsync(client, OrderPageUrl,
                        new UrlEncodedContent(new { _json_att = "" }), QueryPageUrl);
            DynamicJsResult dynamicJsResult = await GetDynamicJsAction(client, content, OrderPageUrl);
            InitDcResult result= new InitDcResult() {DynamicJsResult = dynamicJsResult};

            Regex regex = new Regex(@"[A-z0-9]{32,}");
            var matches = regex.Matches(content);
            string token = matches[0].Groups[1].ToString();
            string key_check_isChange = matches[1].Groups[1].ToString();
            string leftTicketStr = matches[2].Groups[1].ToString();
            result.RepeatSubmitToken = token;
            result.KeyCheckIsChange = key_check_isChange;
            result.LeftTicketStr = leftTicketStr;
            
            
          
            return result;
        }

        /// <summary>
        /// ��鶩����Ϣ
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task<Response<CheckOrderInfoResponse>> CheckOrderInfoAsync(this Client client, IEnumerable<Passenger> passengers, SeatType seatType, string randCode, InitDcResult initDcResult)
        {
            /*
             * cancel_flag	2
 bed_level_order_num	000000000000000000000000000000
 passengerTicketStr	3,0,1,��,1,36220,1591,N_3,0,1,����,1,36220,186,N
 oldPassengerStr	��,1,3622,1_��,1,362,1_
 tour_flag	dc
 randCode	nsph
 NDAxNDA5	NjA1NjRjNzRlOGZkMmQwZg==
 _json_att	
 REPEAT_SUBMIT_TOKEN	c92104171aee0b7323c8e2466a9d3f8c
             */
            string passengerTicketStr = "";
            string oldPassengerStr = "";
            foreach (Passenger passenger in passengers)
            {
                /*
                 * passengerTickets=3,0,1,����,1,362201198...,1591,Y
                 * &oldPassengers=����,1,362201198...&passenger_1_seat=3&passenger_1_seat_detail_select=0&passenger_1_seat_detail=0&passenger_1_ticket=1&passenger_1_name=����&passenger_1_cardtype=1&passenger_1_cardno=362201198&passenger_1_mobileno=15910675179&checkbox9=Y
                 * */
                //if (passenger.Checked)
                //{
                    passengerTicketStr += string.Format("{0},{1},{2},{3},{4},{5},{6},N_",
                        seatType.ToSeatTypeValue(),
                        (int)passenger.SeatDetailType,
                        (int)passenger.TicketType, passenger.Name,
                        passenger.CardType.ToCardTypeValue(),
                        passenger.CardNo, passenger.MobileNo);
                    oldPassengerStr += string.Format("{0},{1},{2},{3}_", passenger.Name,
                        passenger.CardType.ToCardTypeValue(),
                        passenger.CardNo, (int)passenger.TicketType);
                //}

            }
            passengerTicketStr = passengerTicketStr.TrimEnd('_');
            Dictionary<string, string> form = new Dictionary<string, string>()
            {
                {"cancel_flag","2"},
                {"bed_level_order_num","000000000000000000000000000000"},
                {"passengerTicketStr",passengerTicketStr},
                {"oldPassengerStr",oldPassengerStr},
                {"tour_flag","dc"},
                {"randCode",randCode},
                {initDcResult.DynamicJsResult.Key,initDcResult.DynamicJsResult.Value},
                {"_json_att",""},
                {"REPEAT_SUBMIT_TOKEN",initDcResult.RepeatSubmitToken},
            };
            return await AjaxPostToJsonObjectAsync<Response<CheckOrderInfoResponse>>(client,
                "https://kyfw.12306.cn/otn/confirmPassenger/checkOrderInfo", new FormUrlEncodedContent(form),
                OrderPageUrl);
        }


        public static async Task<Response<GetQueueCountResponse>> GetQueueCount(this Client client,DateTime trainDate, TrainItemInfo trainItemInfo, string leftTicket, string submitToken,SeatType seatType)
        {

            /* train_date	Thu Feb 19 2015 00:00:00 GMT+0800 (�й���׼ʱ��)
 train_no	2400000Z670C
 stationTrainCode	Z67
 seatType	3
 fromStationTelecode	BXP
 toStationTelecode	NCG
 leftTicket	10173531834048450007101735010860893500043030650014
 purpose_codes	00
 _json_att	
 REPEAT_SUBMIT_TOKEN	c92104171aee0b7323c8e2466a9d3f8c
             * */
           
            Dictionary<string, string> form = new Dictionary<string, string>()
            {
                {"train_date", trainDate.ToString("ddd MMM dd yyyy HH:mm:ss",System.Globalization.CultureInfo.InvariantCulture)+" GMT+0800 (�й���׼ʱ��)"},
                {"train_no",trainItemInfo.TrainNo4},
                {"stationTrainCode",trainItemInfo.TrainNo},
                {"seatType",seatType.ToSeatTypeValue()},
                {"fromStationTelecode",trainItemInfo.FromStationTelecode},
                {"toStationTelecode",trainItemInfo.ToStationTelecode},
                {"leftTicket",leftTicket},
                {"purpose_codes","00"},
                {"_json_att",""},
                {"REPEAT_SUBMIT_TOKEN",submitToken},


            };
            return await AjaxPostToJsonObjectAsync<Response<GetQueueCountResponse>>(client,
                "https://kyfw.12306.cn/otn/confirmPassenger/getQueueCount", new FormUrlEncodedContent(form),
                OrderPageUrl);
        }

        /// <summary>
        /// �Զ��ύ��ѯ������
        /// </summary>
        /// <param name="client"></param>
        /// <param name="trainDate"></param>
        /// <param name="trainItemInfo"></param>
        /// <param name="leftTicket"></param>
        /// <param name="seatType"></param>
        /// <returns></returns>
        public static async Task<Response<GetQueueCountResponse>> GetQueueCountAsync(this Client client, DateTime trainDate, TrainItemInfo trainItemInfo, string leftTicket, SeatType seatType)
        {

            Dictionary<string, string> form = new Dictionary<string, string>()
            {
                {"train_date", trainDate.ToString("ddd MMM dd yyyy HH:mm:ss",System.Globalization.CultureInfo.InvariantCulture)+" GMT+0800 (�й���׼ʱ��)"},
                {"train_no",trainItemInfo.TrainNo4},
                {"stationTrainCode",trainItemInfo.TrainNo},
                {"seatType",seatType.ToSeatTypeValue()},
                {"fromStationTelecode",trainItemInfo.FromStationTelecode},
                {"toStationTelecode",trainItemInfo.ToStationTelecode},
                {"leftTicket",leftTicket},
                {"purpose_codes","ADULT"},
                {"_json_att",""},
            };
            return await AjaxPostToJsonObjectAsync<Response<GetQueueCountResponse>>(client,
                "https://kyfw.12306.cn/otn/confirmPassenger/getQueueCountAsync", new FormUrlEncodedContent(form),
                QueryPageUrl);
        }

        public static async Task<Response<ConfirmSingleForQueueResponse>> ConfirmSingleForQueue(this Client client, IEnumerable<Passenger> passengers, SeatType seatType, string randCode, InitDcResult initDcResult, string trainLocation)
        {
            string passengerTicketStr = "";
            string oldPassengerStr = "";
            foreach (Passenger passenger in passengers)
            {
               
                if (passenger.Checked)
                {
                    passengerTicketStr += string.Format("{0},{1},{2},{3},{4},{5},{6},N_",
                        seatType.ToSeatTypeValue(),
                        (int)passenger.SeatDetailType,
                        (int)passenger.TicketType, passenger.Name,
                        passenger.CardType.ToCardTypeValue(),
                        passenger.CardNo, passenger.MobileNo);
                    oldPassengerStr += string.Format("{0},{1},{2},{3}_", passenger.Name,
                        passenger.CardType.ToCardTypeValue(),
                        passenger.CardNo, (int)passenger.TicketType);
                }

            }
            passengerTicketStr = passengerTicketStr.TrimEnd('_');
            Dictionary<string, string> form = new Dictionary<string, string>()
            {
              
                {"passengerTicketStr",passengerTicketStr},
                {"oldPassengerStr",oldPassengerStr},
                {"randCode",randCode},
                {"purpose_codes","00"},
                {"key_check_isChange",initDcResult.KeyCheckIsChange},
                {"leftTicket",initDcResult.LeftTicketStr},
                {"train_location",trainLocation},
                {"dwAll","N"},
                {"_json_att",""},
                {"REPEAT_SUBMIT_TOKEN",initDcResult.RepeatSubmitToken},
            };
            return await AjaxPostToJsonObjectAsync<Response<ConfirmSingleForQueueResponse>>(client,
                "https://kyfw.12306.cn/otn/confirmPassenger/confirmSingleForQueue", new FormUrlEncodedContent(form),
                OrderPageUrl);
        }


        public static async Task<Response<ConfirmSingleForQueueResponse>> ConfirmSingleForQueueAsys(this Client client, IEnumerable<Passenger> passengers, SeatType seatType, string randCode, InitDcResult initDcResult, string trainLocation)
        {
            string passengerTicketStr = "";
            string oldPassengerStr = "";
            foreach (Passenger passenger in passengers)
            {

                if (passenger.Checked)
                {
                    passengerTicketStr += string.Format("{0},{1},{2},{3},{4},{5},{6},N_",
                        seatType.ToSeatTypeValue(),
                        (int)passenger.SeatDetailType,
                        (int)passenger.TicketType, passenger.Name,
                        passenger.CardType.ToCardTypeValue(),
                        passenger.CardNo, passenger.MobileNo);
                    oldPassengerStr += string.Format("{0},{1},{2},{3}_", passenger.Name,
                        passenger.CardType.ToCardTypeValue(),
                        passenger.CardNo, (int)passenger.TicketType);
                }

            }
            passengerTicketStr = passengerTicketStr.TrimEnd('_');
            Dictionary<string, string> form = new Dictionary<string, string>()
            {
              
                {"passengerTicketStr",passengerTicketStr},
                {"oldPassengerStr",oldPassengerStr},
                {"randCode",randCode},
                {"purpose_codes","ADULT"},
                {"key_check_isChange",initDcResult.KeyCheckIsChange},
                {"leftTicketStr",initDcResult.LeftTicketStr},
                {"train_location",trainLocation},
                {"_json_att",""},
            };
            return await AjaxPostToJsonObjectAsync<Response<ConfirmSingleForQueueResponse>>(client,
                "https://kyfw.12306.cn/otn/confirmPassenger/confirmSingleForQueueAsys", new FormUrlEncodedContent(form),
                QueryPageUrl);
        }


        /// <summary>
        /// https://kyfw.12306.cn/otn/confirmPassenger/queryOrderWaitTime?random=1419994586921&tourFlag=dc&_json_att=&REPEAT_SUBMIT_TOKEN=c92104171aee0b7323c8e2466a9d3f8c
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static async Task<Response<QueryOrderWaitTimeResponse>> QueryOrderWaitTime(this Client client, string submitToken)
        {
            object obj = null;
            if (string.IsNullOrEmpty(submitToken))
            {
                obj = new
                {
                    random = new Random().Next(10000000, 99999999) +
                             new Random().Next(10000000, 99999999),
                    tourFlag = "dc",
                    _json_att = "",
                };
            }
            else
            {
                obj = new
                {
                    random = new Random().Next(10000000, 99999999) +
                             new Random().Next(10000000, 99999999),
                    tourFlag = "dc",
                    _json_att = "",
                    REPEAT_SUBMIT_TOKEN = submitToken,
                };
            }

            return await AjaxGetToJsonObjectAsync<Response<QueryOrderWaitTimeResponse>>(client,
                "https://kyfw.12306.cn/otn/confirmPassenger/queryOrderWaitTime?"+new UrlEncodedContent(obj).ReadAsStringAsync().Result,
                OrderPageUrl);
        }

        /// <summary>
        /// https://kyfw.12306.cn/otn/confirmPassenger/resultOrderForDcQueue HTTP/1.1
        /// </summary>
        /// <typeparam name="ResultOrderForDcQueueResponse"></typeparam>
        /// <param name="?"></param>
        /// <returns></returns>
        public static async Task<Response<ResultOrderForDcQueueResponse>> ResultOrderForDcQueue(this Client client, string orderId, string submitToken)
        {
            Dictionary<string, string> form = new Dictionary<string, string>()
            {    
                {"orderSequence_no",orderId},
                {"_json_att",""},
                {"REPEAT_SUBMIT_TOKEN",submitToken},
            };


            return await AjaxPostToJsonObjectAsync<Response<ResultOrderForDcQueueResponse>>(client,
                "https://kyfw.12306.cn/otn/confirmPassenger/confirmSingleForQueue", new FormUrlEncodedContent(form),
                OrderPageUrl);
        }

        #endregion

        #region Helpers
        /// <summary>
        /// ��ȡһ����ַ������
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <param name="refererUrl"></param>
        /// <returns></returns>
        public static async Task<string> GetStringAsync(this Client client, string url, string refererUrl = "")
        {
            return await GetHttpClient(client, refererUrl).GetStringAsync(url);
        }
        /// <summary>
        /// ��ȡһ����ַ������
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <param name="refererUrl"></param>
        /// <returns></returns>
        public static async Task<T> GetToJsonObjectAsync<T>(this Client client, string url, string refererUrl = "")
        {
            string str = await GetStringAsync(client, url, refererUrl);
            return str.ToJsonObject<T>();
        }
        /// <summary>
        /// ͨ��post����string
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <param name="refererUrl"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<string> GetStringPostAsync(this Client client, string url, HttpContent content = null, string refererUrl = "")
        {
            HttpResponseMessage response = await GetHttpClient(client, refererUrl).PostAsync(url, content);
            return await response.Content.ReadAsStringAsync();
        }
        /// <summary>
        /// ��ȡһ����ַ������
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <param name="refererUrl"></param>
        /// <returns></returns>
        public static async Task<string> AjaxGetStringAsync(this Client client, string url, string refererUrl = "")
        {
            return await GetHttpClient(client, refererUrl, true).GetStringAsync(url);
        }
        /// <summary>
        /// ��ȡһ����ַ������
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <param name="refererUrl"></param>
        /// <returns></returns>
        public static async Task<T> AjaxGetToJsonObjectAsync<T>(this Client client, string url, string refererUrl = "")
        {
            string str = await GetHttpClient(client, refererUrl, true).GetStringAsync(url);
            return str.ToJsonObject<T>();
        }
        /// <summary>
        /// ͨ��post����string
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <param name="refererUrl"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> AjaxPostAsync(this Client client, string url, HttpContent content = null, string refererUrl = "")
        {
            return await GetHttpClient(client, refererUrl, true).PostAsync(url, content);

        }
        /// <summary>
        /// ͨ��post����string
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <param name="refererUrl"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<string> AjaxPostToStringAsync(this Client client, string url, HttpContent content = null, string refererUrl = "")
        {
            HttpResponseMessage response = await AjaxPostAsync(client, url, content, refererUrl);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<T> AjaxPostToJsonObjectAsync<T>(this Client client, string url, HttpContent content = null, string refererUrl = "")
        {
            string str = await AjaxPostToStringAsync(client, url, content, refererUrl);
            return str.ToJsonObject<T>();
        }

        public static async Task<T> PostToJsonObjectAsync<T>(this Client client, string url, HttpContent content = null, string refererUrl = "")
        {
            string str = await PostToStringAsync(client, url, content, refererUrl);
            return str.ToJsonObject<T>();
        }

        private static async Task<string> PostToStringAsync(this Client client, string url, HttpContent content, string refererUrl)
        {
            HttpResponseMessage response = await PostAsync(client, url, content, refererUrl);
            return await response.Content.ReadAsStringAsync();
        }

        private static async Task<HttpResponseMessage> PostAsync(this Client client, string url, HttpContent content, string refererUrl)
        {
            return await GetHttpClient(client, refererUrl).PostAsync(url, content);
        }

        public const string UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";// "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
        private static LFNet.Net.Http.JHttpClient GetHttpClient(this Client client, string referrer, bool isAjax = false)
        {
            Net.Http.JHttpClient httpClient = HttpClientFactory.Create(referrer, UserAgent, client.Cookie, true);
            if (isAjax)
            {
                httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                httpClient.DefaultRequestHeaders.Add("Origin", " https://kyfw.12306.cn");
            }
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.8");
            httpClient.DefaultRequestHeaders.Add("DNT", "1");
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            return httpClient;
        }



        #endregion
    }
}