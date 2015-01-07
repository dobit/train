using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LFNet.Configuration;
using LFNet.Net.Http;
using LFNet.TrainTicket;
using LFNet.TrainTicket.Config;
using LFNet.TrainTicket.RqEntity;

namespace LFNet.TrainTicket
{
    /// <summary>
    /// 数据接口
    /// </summary>
    public static class InterfaceProvider
    {
        #region  常量
        /// <summary>
        /// 获取站点信息的URL
        /// </summary>
        public const string StationsUrl =
            "https://kyfw.12306.cn/otn/resources/js/framework/station_name.js?station_version=1.8253";

        /// <summary>
        /// 登陆提交地址
        /// </summary>
        public const string LoginAysnSuggestUrl = "https://kyfw.12306.cn/otn/login/loginAysnSuggest";

        /// <summary>
        /// 获取随机码
        /// </summary>
        public const string CheckRandCodeAnsynUrl = "https://kyfw.12306.cn/otn/passcodeNew/checkRandCodeAnsyn";

        /// <summary>
        /// 查询页地址
        /// </summary>
        public const string QueryPageUrl = "https://kyfw.12306.cn/otn/leftTicket/init";

        /// <summary>
        /// 登陆页地址
        /// </summary>
        public const string LoginPageUrl = "https://kyfw.12306.cn/otn/login/init";

        /// <summary>
        /// 订单提交页面
        /// </summary>
        public const string OrderPageUrl = "https://kyfw.12306.cn/otn/confirmPassenger/initDc";

        /// <summary>
        /// 首页
        /// </summary>
        public const string HomePage = "http://www.12306.cn";

        /// <summary>
        /// 订票首页
        /// </summary>
        public const string TicketHomePage = "https://kyfw.12306.cn/otn/";

        /// <summary>
        /// 我的12306页面
        /// </summary>
        public const string InitMy12306PageUrl = "https://kyfw.12306.cn/otn/index/initMy12306";

        /// <summary>
        /// 余票查询页面
        /// </summary>
        public const string LeftTicketUrl = "https://kyfw.12306.cn/otn/leftTicket/init";
        #endregion

        #region 数据获取

        /// <summary>
        /// 获取登录页面内容
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static async Task<string> GetLoginPageResult(this Account account)
        {
          string content=   await GetStringAsync(account, LoginPageUrl, TicketHomePage);
          
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="type">0=登录，1=下单</param>
        /// <returns></returns>
        public static async Task<Image> GetRandCode(this Account account, int type = 0)
        {
            string url;
            if (type == 0)
            {
                url = "https://kyfw.12306.cn/otn/passcodeNew/getPassCodeNew?module=login&rand=sjrand&0." +
                      new Random().Next(10000000, 99999999) +
                      new Random().Next(10000000, 99999999);
            }
            else
            {
                url = "https://kyfw.12306.cn/otn/passcodeNew/getPassCodeNew?module=passenger&rand=randp&0." +
                      new Random().Next(10000000, 99999999) +
                      new Random().Next(10000000, 99999999);
            }
            Stream stream = await GetHttpClient(account, LoginPageUrl).GetStreamAsync(url);
            return Image.FromStream(stream);
        }

        /// <summary>
        /// 检查验证码
        /// </summary>
        /// <param name="randCode">验证码</param>
        /// <param name="randType">0=登陆，1下单</param>
        /// <param name="token">档randtype=2必须有</param>
        /// <returns></returns>
        public static async Task<Response<CheckRandCodeAnsynResponse>> CheckRandCodeAnsyn(this Account account, string randCode, int randType, string token)
        {
            //randCode=6eed&rand=sjrand&randCode_validate=
            //randCode=nsph&rand=randp&_json_att=&REPEAT_SUBMIT_TOKEN=c92104171aee0b7323c8e2466a9d3f8c
            if (randType == 0)
            {

                return await AjaxPostToJsonObjectAsync<Response<CheckRandCodeAnsynResponse>>(account, CheckRandCodeAnsynUrl, new
                {
                    randCode,
                    rand = "sjrand",
                    randCode_validate = ""
                }.ToUrlEncodedContent(), LoginPageUrl);

            }
            else
            {
                return await AjaxPostToJsonObjectAsync<Response<CheckRandCodeAnsynResponse>>(account, CheckRandCodeAnsynUrl, new
                {
                    randCode,
                    rand = "randp",
                    _json_att = "",
                    REPEAT_SUBMIT_TOKEN = token
                }.ToUrlEncodedContent(), OrderPageUrl);
            }


        }
        /// <summary>
        /// 检查服务器状态
        /// </summary>
        /// <returns></returns>
        public static async Task<State> CheckState(this Account account)
        {
            string url = "https://dynamic.12306.cn/otsweb/order/querySingleAction.do?method=init";
            string content = await GetHttpClient(account, QueryPageUrl).GetStringAsync(InitMy12306PageUrl);
            if (content.Contains("系统维护"))
            {
                return State.Maintenance;
            }
            if (content.Contains("loginForm") && content.Contains("loginUserDTO.user_name"))
            {
                //IsLogin = false;
                return State.UnLogin;
            }
            else
            {
                //IsLogin = true;
                return State.Login;
            }
        }




        #region 动态js
        /// <summary>
        /// 页面动态js检查
        /// </summary>
        /// <returns></returns>
        public static async Task<DynamicJsResult> GetDynamicJsAction(this Account account, string content, string pageUrl)
        {
            Regex jsUrlRegex = new Regex(@"<script src=""/otn/dynamicJs/(.*?)""", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string jsurl = new Uri(new Uri(pageUrl), "/otn/dynamicJs/" + jsUrlRegex.Match(content).Groups[1]).ToString();//jsUrlRegex.Replace(content, "$1")).ToString();
            string jsContent = await GetHttpClient(account, pageUrl).GetStringAsync(jsurl);
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

            //Dictionary<string, string> result = new Dictionary<string, string>()
            // {
            //     {key,value},
            //     {"myversion","undefined"}
            // };
            Regex postUrlRegex = new Regex(@"/otn/dynamicJs/(.*?)'", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            string postUrl = new Uri(new Uri(pageUrl), "/otn/dynamicJs/" + postUrlRegex.Match(jsContent).Groups[1]).ToString();
            result.PostDynamicJsUrl = postUrl;

            //HttpResponseMessage httpResponseMessage = await GetHttpClient(account,pageUrl, true).PostAsync(postUrl, new StringContent(""));
            return result;

        }

        public static async Task<string> GetDynamicJsStringAsync(this Account account, string referrer, string dynamicJsUrl)
        {
            return await GetHttpClient(account, referrer).GetStringAsync(dynamicJsUrl);
        }
        public static async Task<string> PostDynamicJsStringAsync(this Account account, string referrer, string dynamicJsUrl)
        {
            HttpResponseMessage httpResponseMessage = await GetHttpClient(account, referrer).PostAsync(dynamicJsUrl, new StringContent(""));
            return await httpResponseMessage.Content.ReadAsStringAsync();
        }
        #endregion


        /// <summary>
        /// 获取乘客信息
        /// </summary>
        /// <returns></returns>
        public static async Task<Response<GetPassengerDTOs>> GetPassengers(this Account account, string submitToken = "")
        {
            HttpContent obj = new StringContent("");
            if (submitToken != "")
                obj = new
                {
                    _json_att = "",
                    REPEAT_SUBMIT_TOKEN = submitToken
                }.ToUrlEncodedContent();
            return await AjaxPostToJsonObjectAsync<Response<GetPassengerDTOs>>(account, "https://kyfw.12306.cn/otn/confirmPassenger/getPassengerDTOs", obj, QueryPageUrl);

        }

        /// <summary>
        /// 检查用户状态
        /// </summary>
        /// <returns></returns>
        public static async Task<Response<CheckUserResponse>> CheckUser(this Account account)
        {
            return
                await
                    AjaxPostToJsonObjectAsync<Response<CheckUserResponse>>(account,
                        "https://kyfw.12306.cn/otn/login/checkUser", new StringContent(""), QueryPageUrl);

        }

        /// <summary>
        /// 查询列车信息
        /// </summary>
        /// <param name="account"></param>
        /// <param name="trainDate"></param>
        /// <param name="fromStationTeleCode"></param>
        /// <param name="toStationTeleCode"></param>
        /// <param name="type">0=成人,1=学生</param>
        /// <returns></returns>
        public static async Task<Response<QueryResponse>> QueryTrainInfos(this Account account, DateTime trainDate, string fromStationTeleCode, string toStationTeleCode, int type = 0)
        {
            List<TrainItemInfo> list = new List<TrainItemInfo>();

            //GET https://kyfw.12306.cn/otn/leftTicket/queryT?leftTicketDTO.train_date=2015-02-19&leftTicketDTO.from_station=BJP&leftTicketDTO.to_station=NCG&purpose_codes=ADULT HTTP/1.1
            string purpose_codes = type == 0 ? "ADULT" : "STUDENT";
            string url = string.Format("https://kyfw.12306.cn/otn/leftTicket/queryT?leftTicketDTO.train_date={0}&leftTicketDTO.from_station={1}&leftTicketDTO.to_station={2}&purpose_codes={3}"
                , trainDate.ToString("yyyy-MM-dd"), fromStationTeleCode, toStationTeleCode, purpose_codes);

            return await AjaxGetToJsonObjectAsync<Response<QueryResponse>>(account, url, QueryPageUrl);

        }

        /// <summary>
        /// https://dynamic.12306.cn/otsweb/order/querySingleAction.do?method=submutOrderRequest
        /// </summary>
        /// <param name="secretStr">加密串</param>
        /// <param name="trainDate">去程日期</param>
        /// <param name="backTrainDate">返程日期</param>
        /// <param name="fromStationTeleCode"></param>
        /// <param name="toStationTeleCode"></param>
        /// <param name="type">0=成人,1=学生</param>
        /// <returns>得到页面的表单信息</returns>
        public static async Task<Response<string>> SubmitOrderRequest(this Account account, string secretStr, string dynamicJsKey, string dynamicJsValue, DateTime trainDate, DateTime backTrainDate, string fromStationTeleCode, string toStationTeleCode, int type = 0)
        {

            var config = ConfigFileManager.GetConfig<BuyTicketConfig>();
            ;
            Dictionary<string, string> newforms = new Dictionary<string, string>()
            {
                {dynamicJsKey,dynamicJsValue	},   
                {"myversion","undefined"	},  
                {"secretStr",secretStr	},
                {"train_date",	trainDate.ToString("yyyy-MM-dd")},
                {"back_train_date",backTrainDate.ToString("yyyy-MM-dd")},
                {"tour_flag","dc"},
                {"purpose_codes","ADULT"},
                {"query_from_station_name",config.OrderRequest.FromStationTelecodeName},
                {"query_to_station_name",config.OrderRequest.ToStationTelecodeName},
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
            var accountCollection = new CookieCollection()
            {
                //new account("_jc_save_czxxcx_toStation",""),
                //new account("_jc_save_czxxcx_fromDate","2014-12-25"),
                new Cookie("_jc_save_fromStation",Common.HtmlUtil.UrlEncode(config.OrderRequest.FromStationTelecodeName+","+config.OrderRequest.FromStationTelecode)){Expires = expires },
                new Cookie("_jc_save_toStation",Common.HtmlUtil.UrlEncode(config.OrderRequest.ToStationTelecodeName+","+config.OrderRequest.ToStationTelecode)){Expires = expires },
                new Cookie("_jc_save_fromDate",config.OrderRequest.TrainDate.ToString("yyyy-MM-dd")){Expires = expires },
                new Cookie("_jc_save_toDate",config.OrderRequest.TrainDate.ToString("yyyy-MM-dd")){Expires = expires },
                new Cookie("_jc_save_wfdc_flag","dc"){Expires = expires },
                // new account("current_captcha_type","C"){Expires = expires },
            };
            account.Cookie.Add(new Uri(ActionUrls.TicketHomePage), accountCollection);
            return await AjaxPostToJsonObjectAsync<Response<string>>(account, "https://kyfw.12306.cn/otn/leftTicket/submitOrderRequest", new FormUrlEncodedContent(newforms), QueryPageUrl);

            ////转到订单页面
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
            ////调用获取用户信息
            //GetPassengers(token);
            //return dynamicJsAction;
        }

        /// <summary>
        /// 获取订单提交页面内容
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static async Task<string> GetInitDc(this Account account)
        {
            return
                await
                    AjaxPostToStringAsync(account, "https://kyfw.12306.cn/otn/confirmPassenger/initDc",
                        new UrlEncodedContent(new { _json_att = "" }), QueryPageUrl);
        }

        /// <summary>
        /// 检查订单信息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static async Task<Response<CheckOrderInfoResponse>> CheckOrderInfoAsync(this Account account, IEnumerable<Passenger> passengers, SeatType seatType, string randCode, string dynamicJsKey, string dynamicJsValue, string submitToken)
        {
            /*
             * cancel_flag	2
 bed_level_order_num	000000000000000000000000000000
 passengerTicketStr	3,0,1,林,1,36220,1591,N_3,0,1,林艳,1,36220,186,N
 oldPassengerStr	林,1,3622,1_林,1,362,1_
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
                 * passengerTickets=3,0,1,林利,1,362201198...,1591,Y
                 * &oldPassengers=林利,1,362201198...&passenger_1_seat=3&passenger_1_seat_detail_select=0&passenger_1_seat_detail=0&passenger_1_ticket=1&passenger_1_name=林利&passenger_1_cardtype=1&passenger_1_cardno=362201198&passenger_1_mobileno=15910675179&checkbox9=Y
                 * */
                if (passenger.Checked)
                {
                    passengerTicketStr += string.Format("{0},{1},{2},{3},{4},{5},{6},N_",
                        seatType.ToSeatTypeValue(),
                        (int)passenger.SeatDetailType,
                        (int)passenger.TicketType, passenger.Name,
                        passenger.CardType.ToCardTypeValue(),
                        passenger.CardNo, passenger.MobileNo);
                    oldPassengerStr += string.Format("{0},{1},{2}_", passenger.Name,
                        passenger.CardType.ToCardTypeValue(),
                        passenger.CardNo);
                }

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
                {dynamicJsKey,dynamicJsValue},
                {"_json_att",""},
                {"REPEAT_SUBMIT_TOKEN",submitToken},
            };
            return await AjaxPostToJsonObjectAsync<Response<CheckOrderInfoResponse>>(account,
                "https://kyfw.12306.cn/otn/confirmPassenger/checkOrderInfo", new FormUrlEncodedContent(form),
                OrderPageUrl);
        }


        public static async Task<Response<GetQueueCountResponse>> GetQueueCount(this Account account,DateTime trainDate, TrainItemInfo trainItemInfo, string leftTicket, string submitToken,SeatType seatType)
        {

            /* train_date	Thu Feb 19 2015 00:00:00 GMT+0800 (中国标准时间)
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
                {"train_date", trainDate.ToString("ddd MMM dd yyyy HH:mm:ss",System.Globalization.CultureInfo.InvariantCulture)+" GMT+0800 (中国标准时间)"},
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
            return await AjaxPostToJsonObjectAsync<Response<GetQueueCountResponse>>(account,
                "https://kyfw.12306.cn/otn/confirmPassenger/getQueueCount", new FormUrlEncodedContent(form),
                OrderPageUrl);
        }

        public static async Task<Response<ConfirmSingleForQueueResponse>> ConfirmSingleForQueue(this Account account, IEnumerable<Passenger> passengers, SeatType seatType, string randCode, string dynamicJsKey, string dynamicJsValue, string submitToken, string keyCheckIsChange, string leftTicket, string trainLocation)
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
                    oldPassengerStr += string.Format("{0},{1},{2}_", passenger.Name,
                        passenger.CardType.ToCardTypeValue(),
                        passenger.CardNo);
                }

            }
            passengerTicketStr = passengerTicketStr.TrimEnd('_');
            Dictionary<string, string> form = new Dictionary<string, string>()
            {
              
                {"passengerTicketStr",passengerTicketStr},
                {"oldPassengerStr",oldPassengerStr},
                {"randCode",randCode},
                {"purpose_codes","00"},
                {"key_check_isChange",keyCheckIsChange},
                {"leftTicket",leftTicket},
                {"train_location",trainLocation},
                {"dwAll","N"},
                {"_json_att",""},
                {"REPEAT_SUBMIT_TOKEN",submitToken},
            };
            return await AjaxPostToJsonObjectAsync<Response<ConfirmSingleForQueueResponse>>(account,
                "https://kyfw.12306.cn/otn/confirmPassenger/confirmSingleForQueue", new FormUrlEncodedContent(form),
                OrderPageUrl);
        }

        /// <summary>
        /// https://kyfw.12306.cn/otn/confirmPassenger/queryOrderWaitTime?random=1419994586921&tourFlag=dc&_json_att=&REPEAT_SUBMIT_TOKEN=c92104171aee0b7323c8e2466a9d3f8c
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static async Task<Response<QueryOrderWaitTimeResponse>> QueryOrderWaitTime(this Account account, string submitToken)
        {
            
            return await AjaxGetToJsonObjectAsync<Response<QueryOrderWaitTimeResponse>>(account,
                "https://kyfw.12306.cn/otn/confirmPassenger/queryOrderWaitTime?"+new UrlEncodedContent(new
                {
                    random=new Random().Next(10000000, 99999999) +
                           new Random().Next(10000000, 99999999),
                    tourFlag="dc",
                    _json_att="",
                    REPEAT_SUBMIT_TOKEN = submitToken,
                }),
                OrderPageUrl);
        }

        /// <summary>
        /// https://kyfw.12306.cn/otn/confirmPassenger/resultOrderForDcQueue HTTP/1.1
        /// </summary>
        /// <typeparam name="ResultOrderForDcQueueResponse"></typeparam>
        /// <param name="?"></param>
        /// <returns></returns>
        public static async Task<Response<ResultOrderForDcQueueResponse>> ResultOrderForDcQueue(this Account account, string orderId, string submitToken)
        {
            Dictionary<string, string> form = new Dictionary<string, string>()
            {    
                {"orderSequence_no",orderId},
                {"_json_att",""},
                {"REPEAT_SUBMIT_TOKEN",submitToken},
            };


            return await AjaxPostToJsonObjectAsync<Response<ResultOrderForDcQueueResponse>>(account,
                "https://kyfw.12306.cn/otn/confirmPassenger/confirmSingleForQueue", new FormUrlEncodedContent(form),
                OrderPageUrl);
        }

        #endregion

        #region Helpers
        /// <summary>
        /// 获取一个地址的内容
        /// </summary>
        /// <param name="account"></param>
        /// <param name="url"></param>
        /// <param name="refererUrl"></param>
        /// <returns></returns>
        public static async Task<string> GetStringAsync(this Account account, string url, string refererUrl = "")
        {
            return await GetHttpClient(account, refererUrl).GetStringAsync(url);
        }
        /// <summary>
        /// 获取一个地址的内容
        /// </summary>
        /// <param name="account"></param>
        /// <param name="url"></param>
        /// <param name="refererUrl"></param>
        /// <returns></returns>
        public static async Task<T> GetToJsonObjectAsync<T>(this Account account, string url, string refererUrl = "")
        {
            string str = await GetStringAsync(account, url, refererUrl);
            return str.ToJsonObject<T>();
        }
        /// <summary>
        /// 通过post返回string
        /// </summary>
        /// <param name="account"></param>
        /// <param name="url"></param>
        /// <param name="refererUrl"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<string> GetStringPostAsync(this Account account, string url, HttpContent content = null, string refererUrl = "")
        {
            HttpResponseMessage response = await GetHttpClient(account, refererUrl).PostAsync(url, content);
            return await response.Content.ReadAsStringAsync();
        }
        /// <summary>
        /// 获取一个地址的内容
        /// </summary>
        /// <param name="account"></param>
        /// <param name="url"></param>
        /// <param name="refererUrl"></param>
        /// <returns></returns>
        public static async Task<string> AjaxGetStringAsync(this Account account, string url, string refererUrl = "")
        {
            return await GetHttpClient(account, refererUrl, true).GetStringAsync(url);
        }
        /// <summary>
        /// 获取一个地址的内容
        /// </summary>
        /// <param name="account"></param>
        /// <param name="url"></param>
        /// <param name="refererUrl"></param>
        /// <returns></returns>
        public static async Task<T> AjaxGetToJsonObjectAsync<T>(this Account account, string url, string refererUrl = "")
        {
            string str = await GetHttpClient(account, refererUrl, true).GetStringAsync(url);
            return str.ToJsonObject<T>();
        }
        /// <summary>
        /// 通过post返回string
        /// </summary>
        /// <param name="account"></param>
        /// <param name="url"></param>
        /// <param name="refererUrl"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> AjaxPostAsync(this Account account, string url, HttpContent content = null, string refererUrl = "")
        {
            return await GetHttpClient(account, refererUrl, true).PostAsync(url, content);

        }
        /// <summary>
        /// 通过post返回string
        /// </summary>
        /// <param name="account"></param>
        /// <param name="url"></param>
        /// <param name="refererUrl"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<string> AjaxPostToStringAsync(this Account account, string url, HttpContent content = null, string refererUrl = "")
        {
            HttpResponseMessage response = await AjaxPostAsync(account, url, content, refererUrl);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<T> AjaxPostToJsonObjectAsync<T>(this Account account, string url, HttpContent content = null, string refererUrl = "")
        {
            string str = await AjaxPostToStringAsync(account, url, content, refererUrl);
            return str.ToJsonObject<T>();
        }

        public static async Task<T> PostToJsonObjectAsync<T>(this Account account, string url, HttpContent content = null, string refererUrl = "")
        {
            string str = await PostToStringAsync(account, url, content, refererUrl);
            return str.ToJsonObject<T>();
        }

        private static async Task<string> PostToStringAsync(this Account account, string url, HttpContent content, string refererUrl)
        {
            HttpResponseMessage response = await PostAsync(account, url, content, refererUrl);
            return await response.Content.ReadAsStringAsync();
        }

        private static async Task<HttpResponseMessage> PostAsync(this Account account, string url, HttpContent content, string refererUrl)
        {
            return await GetHttpClient(account, refererUrl).PostAsync(url, content);
        }

        public const string UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
        private static LFNet.Net.Http.JHttpClient GetHttpClient(this Account account, string referrer, bool isAjax = false)
        {
            Net.Http.JHttpClient httpClient = HttpClientFactory.Create(referrer, UserAgent, account.Cookie, true);
            httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.8");
            httpClient.DefaultRequestHeaders.Add("Origin", " https://kyfw.12306.cn");
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            return httpClient;
        }



        #endregion
    }



}