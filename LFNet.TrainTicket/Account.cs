using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LFNet.Configuration;
using LFNet.TrainTicket.Config;
using LFNet.TrainTicket.RqEntity;
using LFNet.Net.Http;
using Newtonsoft.Json;

namespace LFNet.TrainTicket
{


    public enum State
    {
        /// <summary>
        /// 维护
        /// </summary>
        Maintenance,

        /// <summary>
        /// 未登录
        /// </summary>
        UnLogin,

        /// <summary>
        /// 登录
        /// </summary>
        Login
    }

    /// <summary>
    /// 代表一个账号
    /// </summary>
    public class Account
    {

        #region Events

        public event EventHandler<ClientEventArgs> ClientChanged;

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="message"></param>
        protected virtual void OnClientChanged(string message)
        {
            EventHandler<ClientEventArgs> handler = ClientChanged;
            if (handler != null) handler(this, new ClientEventArgs(message));
        }

        #endregion

        #region Fields
        /// <summary>
        /// 停止状态
        /// </summary>
        private bool _stop = false;

        /// <summary>
        /// 查询页面访问时间
        /// </summary>
        private DateTime _queryPageTime = DateTime.MinValue;
        /// <summary>
        /// 查询页面的动态js检测结果
        /// </summary>
        private DynamicJsResult _queryPageDynamicJsResult = null;
        #endregion


        public string Username { get; private set; }
        public string Password { get; private set; }
      
        public WebProxy Proxy { get; set; }

        private CookieContainer _cookie = new CookieContainer();

        // public JHttpClient JHttpClient { get; set; }
        public CookieContainer Cookie
        {
            get { return _cookie; }
            set { _cookie = value; }
        }

        public bool IsLogin { get; private set; }

        public Account(string userName, string password)
            : this(userName, password, null)
        {
        }
        public Account(string userName, string password, string proxyIp)
        {
            this.Username = userName;
            this.Password = password;
            if (!string.IsNullOrEmpty(proxyIp))
                Proxy = new WebProxy(proxyIp, 443); //https代理
        }

        /// <summary>
        /// 停止执行
        /// </summary>
        public void Stop()
        {
            _stop = true;
        }
        
        

        /// <summary>
        /// 检查服务器状态
        /// </summary>
        /// <returns></returns>
        public State CheckState()
        {
            string url = "https://dynamic.12306.cn/otsweb/order/querySingleAction.do?method=init";
            string content =
                this.GetHttpClient(ActionUrls.QueryPageUrl).GetStringAsync(ActionUrls.InitMy12306PageUrl).Result;// HttpRequest.Create(url, "https://dynamic.12306.cn/otsweb/loginAction.do?method=init", Cookie, HttpMethod.GET, "", this.Proxy).GetString();
            if (content.Contains("系统维护"))
            {
                return State.Maintenance;
            }
            if (content.Contains("loginForm") && content.Contains("loginUserDTO.user_name"))
            {
                IsLogin = false;
                return State.UnLogin;
            }
            else
            {
                IsLogin = true;
                return State.Login;
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns>登录时否成功</returns>
        public async Task<bool> Login()
        {
            //打开登陆页面
            var loginPageResult = await this.GetLoginPageResult();

            var vcode = await GetRandCode();

            Thread.Sleep(5000); //单击等待
            Response<LoginAysnSuggestResponse> response =
                await 
                    this.LoginAsynSuggest(this.Username, this.Password, vcode, loginPageResult.DynamicJsResult.Key,loginPageResult.DynamicJsResult.Value);
            if (response.data != null && response.data.loginCheck == "Y")
            {
                IsLogin = true;
                Info("登录成功");
                return true;
            }
            else
            {
                Info(response.messages[0].ToString());
                return false;
            }
        }

       

        /// <summary>
        /// 查询
        /// </summary>
        public async void QueryLeftTicket()
        {
            OpenQueryPage();
            //查询联系人

            //查询
            
        }



      

        /// <summary>
        /// 获取一个有效的验证码
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetRandCode(int type=0)
        {
            string vcode = "";
            do
            {
                Image image = await InterfaceProvider.GetRandCode(this,type);
                var codeByForm = image.GetVCodeByForm();
                Thread.Sleep(5000); //等待5s 输入时间
                vcode = await codeByForm;
                if (_stop) return vcode;
                var checkRandCodeAnsynResponse = await this.CheckRandCodeAnsyn(vcode, type, "");

                if (checkRandCodeAnsynResponse.data != null && checkRandCodeAnsynResponse.data.result == "1") break;
                else
                {
                    Info(checkRandCodeAnsynResponse.messages[0].ToString());
                }
            } while (true);
            return vcode;
        }

        private async void Info(string  message)
        {
            OnClientChanged(message);
        }


        /// <summary>
        /// 获取乘客信息
        /// </summary>
        /// <returns></returns>
        public GetPassengerDTOs GetPassengers()
        {
            OpenQueryPage();
        }


        public bool CheckUser()
        {
          var response= this.GetHttpClient(ActionUrls.QueryPageUrl)
                .PostAsync("https://kyfw.12306.cn/otn/login/checkUser", new StringContent(""))
                .Result.Content.ReadAsStringAsync()
                .Result.ToJsonObject<Response<CheckUserResponse>>();
            return response.data.flag;
        }

        /// <summary>
        /// 查询列车信息
        /// </summary>
        /// <returns></returns>
        public List<TrainItemInfo> QueryTrainInfos(List<TrainItemInfo> oldList,int type=0 )
        {
            List<TrainItemInfo> list = new List<TrainItemInfo>();
           
               //GET https://kyfw.12306.cn/otn/leftTicket/queryT?leftTicketDTO.train_date=2015-02-19&leftTicketDTO.from_station=BJP&leftTicketDTO.to_station=NCG&purpose_codes=ADULT HTTP/1.1
            string purpose_codes = type == 0 ? "ADULT" : "STUDENT";
            string url =string.Format(
                    "https://kyfw.12306.cn/otn/leftTicket/queryT?leftTicketDTO.train_date={0}&leftTicketDTO.from_station={1}&leftTicketDTO.to_station={2}&purpose_codes={3}"
                    , BuyTicketConfig.Instance.OrderRequest.TrainDate.ToString("yyyy-MM-dd"), BuyTicketConfig.Instance.OrderRequest.FromStationTelecode,
                    BuyTicketConfig.Instance.OrderRequest.ToStationTelecode, purpose_codes);


            this.GetHttpClient(ActionUrls.QueryPageUrl).GetStringAsync(url.Replace("queryT", "log"));
            var response =this.GetHttpClient(ActionUrls.QueryPageUrl).GetStringAsync(url).Result.ToJsonObject<Response<QueryResponse>>();

               if(response.status)
                return ToTrainItemInfos(response.data, oldList);
          
            return list;
        }

        /// <summary>
        /// 下订单，但不返回订单编号
        /// </summary>
        /// <returns>无错误时返回空，其它返回错误</returns>
        public string OrderTicket(TrainItemInfo trainItemInfo, Passenger[] passengers, SeatType seatType, ref bool stop, bool force = false, RichTextBox rtbLog = null)
        {
            /*POST https://dynamic.12306.cn/otsweb/order/querySingleAction.do?method=submutOrderRequest HTTP/1.1
             * Referer: https://dynamic.12306.cn/otsweb/order/querySingleAction.do?method=init
             * 
             * station_train_code=Z133&train_date=2012-10-12&seattype_num=&from_station_telecode=BXP&to_station_telecode=NCG&include_student=00
             * &from_station_telecode_name=%E5%8C%97%E4%BA%AC&to_station_telecode_name=%E5%8D%97%E6%98%8C
             * &round_train_date=2012-10-11&round_start_time_str=00%3A00--24%3A00&single_round_type=1&train_pass_type=QB&train_class_arr=QB%23D%23Z%23T%23K%23QT%23
             * &start_time_str=00%3A00--24%3A00&lishi=11%3A29&train_start_time=19%3A45&trainno4=240000Z13305
             * &arrive_time=07%3A14&from_station_name=%E5%8C%97%E4%BA%AC%E8%A5%BF&to_station_name=%E5%8D%97%E6%98%8C&ypInfoDetail=1*****31254*****00241*****00006*****00113*****0111&mmStr=7D1B712CD355990896422EECCC4C11205C7DFD31C26962626B630FEE
             */
            NameValueCollection forms = new NameValueCollection();
            forms["station_train_code"] = trainItemInfo.TrainNo;
            forms["train_date"] = BuyTicketConfig.Instance.OrderRequest.TrainDate.ToString("yyyy-MM-dd");
            forms["seattype_num"] = "";
            forms["from_station_telecode"] = trainItemInfo.FromStationTelecode;
            forms["to_station_telecode"] = trainItemInfo.ToStationTelecode;
            forms["include_student"] = "00";
            forms["from_station_telecode_name"] = BuyTicketConfig.Instance.OrderRequest.FromStationTelecodeName;
            forms["to_station_telecode_name"] = BuyTicketConfig.Instance.OrderRequest.ToStationTelecodeName;
            forms["round_train_date"] = System.DateTime.Today.ToString("yyyy-MM-dd");
            forms["round_start_time_str"] = BuyTicketConfig.Instance.OrderRequest.StartTimeStr;
            forms["single_round_type"] = "1";
            forms["train_pass_type"] = BuyTicketConfig.Instance.OrderRequest.TrainPassType;
            forms["train_class_arr"] = BuyTicketConfig.Instance.OrderRequest.TrainClass;
            forms["start_time_str"] = BuyTicketConfig.Instance.OrderRequest.StartTimeStr;
            forms["lishi"] = trainItemInfo.lishi;
            forms["train_start_time"] = trainItemInfo.TrainStartTime;
            forms["trainno4"] = trainItemInfo.TrainNo4;
            forms["arrive_time"] = trainItemInfo.ArriveTime;
            forms["from_station_name"] = trainItemInfo.FromStationName;
            forms["to_station_name"] = trainItemInfo.ToStationName;
            forms["from_station_no"] = trainItemInfo.FromStationNo;
            forms["to_station_no"] = trainItemInfo.ToStationNo;
            forms["ypInfoDetail"] = trainItemInfo.YpInfoDetail;
            forms["mmStr"] = trainItemInfo.MmStr;
            forms["locationCode"] = trainItemInfo.LocationCode;
            string content = HttpRequest.Create("https://dynamic.12306.cn/otsweb/order/querySingleAction.do?method=submutOrderRequest", "https://dynamic.12306.cn/otsweb/order/querySingleAction.do?method=init", Cookie, forms).GetString();
            NameValueCollection htmlForm = Utils.GetForms(content);
            trainItemInfo.YpInfoDetailReal = htmlForm["leftTicketStr"]; //用于实时查询余票信息
            if (string.IsNullOrEmpty(trainItemInfo.YpInfoDetailReal))
            {
                Common.LogUtil.Log(content);
                return "下单失败:未能获取真实的余票串信息";
            }

        ConfirmRequest:

            forms = new NameValueCollection();
            forms["org.apache.struts.taglib.html.TOKEN"] = htmlForm["org.apache.struts.taglib.html.TOKEN"];
            forms["leftTicketStr"] = htmlForm["leftTicketStr"];
            foreach (string key in htmlForm.Keys)
            {
                if (key.StartsWith("orderRequest"))
                {
                    forms[key] = htmlForm[key];
                }
            }
            //&randCode=5xpy&orderRequest.reserve_flag=A

            string vcode = "";
            do
            {
                Stream stream =
                    HttpRequest.Create("https://dynamic.12306.cn/otsweb/passCodeAction.do?rand=randp",
                                       "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=init", Cookie,
                                       HttpMethod.GET, "").GetStream();
                Image image = Image.FromStream(stream);
                vcode = GetVCodeByForm(image);
                if (vcode == "BREAK")
                    return "用户终止";
            } while (vcode == "");
            forms["randCode"] = vcode;
            forms["orderRequest.reserve_flag"] = BuyTicketConfig.Instance.SystemSetting.PayType == PayType.Online ? "A" : "B";//A=在线支付,B=取票现场支付
            //if (force && trainItemInfo.GetRealSeatNumber(seatType) < passengers.Length)
            //{
            //    int p = trainItemInfo.YpInfoDetail.IndexOf(((char)seatType) + "*");
            //    if (p > -1)
            //    {
            //        string newLeftTickets = forms["leftTicketStr"].Remove(p + 6, 1);
            //        newLeftTickets = newLeftTickets.Insert(p + 6, "1");
            //        forms["leftTicketStr"] = newLeftTickets;
            //    }
            //}
            string postStr = forms.ToQueryString();
            foreach (Passenger passenger in passengers)
            {
                /*
                 * passengerTickets=3,0,1,林利,1,362201198...,1591,Y
                 * &oldPassengers=林利,1,362201198...&passenger_1_seat=3&passenger_1_seat_detail_select=0&passenger_1_seat_detail=0&passenger_1_ticket=1&passenger_1_name=林利&passenger_1_cardtype=1&passenger_1_cardno=362201198&passenger_1_mobileno=15910675179&checkbox9=Y
                 * */
                if (passenger.Checked)
                {
                    postStr += "&passengerTickets=" +
                               Common.HtmlUtil.UrlEncode(string.Format("{0},{1},{2},{3},{4},{5},{6},Y",
                        //passenger.SeatType.ToSeatTypeValue(),
                                                                       seatType.ToSeatTypeValue(),
                                                                       (int)passenger.SeatDetailType,
                                                                       (int)passenger.TicketType, passenger.Name,
                                                                       passenger.CardType.ToCardTypeValue(),
                                                                       passenger.CardNo, passenger.MobileNo));
                    postStr += "&oldPassengers=" +
                               Common.HtmlUtil.UrlEncode(string.Format("{0},{1},{2}", passenger.Name,
                                                                       passenger.CardType.ToCardTypeValue(),
                                                                       passenger.CardNo));
                }
            }

            string checkOrderInfoUrl = "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=checkOrderInfo&rand=" + vcode;

        CheckOrderInfo:
            if (stop)
            {
                return "用户终止执行";
            }
            string resCheckContent = HttpRequest.Create(checkOrderInfoUrl, "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=init", Cookie, HttpMethod.POST, postStr + "&tFlag=dc").GetString();

            if (resCheckContent.Contains("验证码"))
            {
                goto ConfirmRequest;
            }
            //https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=getQueueCount&train_date=2013-02-04&train_no=24000000T50E&station=T5&seat=1&from=BXP&to=NNZ&ticket=1027353027407675000010273500003048050000
            string getQueueCountUrl = @"https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=getQueueCount&train_date="
            + forms["orderRequest.train_date"]
            + "&train_no=" + forms["orderRequest.train_no"]
            + "&station=" + forms["orderRequest.station_train_code"] +
            "&seat=" + seatType.ToSeatTypeValue() +
            "&from=" + forms["orderRequest.from_station_telecode"] +
            "&to=" + forms["orderRequest.to_station_telecode"] +
            "&ticket=" + forms["leftTicketStr"];
            ResYpInfo resYpInfo = HttpRequest.Create(getQueueCountUrl, "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=init", Cookie, HttpMethod.GET, "").GetJsonObject<ResYpInfo>(); ;
            // {"countT":0,"count":355,"ticket":"1*****30504*****00001*****00003*****0000","op_1":true,"op_2":false}

            int seatNum = Utils.GetRealSeatNumber(resYpInfo.Ticket, seatType);
            int wuzuo = 0;
            if (seatType == SeatType.硬座)
                wuzuo = Utils.GetRealSeatNumber(resYpInfo.Ticket, SeatType.无座);
            if (rtbLog != null)
            {
                if (rtbLog.InvokeRequired)
                {
                    rtbLog.Invoke(new Action(() =>
                        {
                            rtbLog.Text += string.Format("===>{0},{1}人排队,余票 {2} 张 {3}\r\n", seatType, resYpInfo.CountT, seatNum, wuzuo == 0 ? "" : ",无座 " + wuzuo + " 张");
                            rtbLog.SelectionStart = rtbLog.TextLength;
                            rtbLog.ScrollToCaret();
                        }));

                }
                else
                {
                    rtbLog.Text += string.Format("===>{0},{1}人排队,余票 {2} 张 {3}\r\n", seatType, resYpInfo.CountT, seatNum, wuzuo == 0 ? "" : ",无座 " + wuzuo + " 张");
                    rtbLog.SelectionStart = rtbLog.TextLength;
                    rtbLog.ScrollToCaret();
                }
            }
            if (force && seatNum < passengers.Length)
            {
                if (wuzuo == 0)
                {
                    System.Threading.Thread.Sleep(1000);
                    goto CheckOrderInfo;
                }
                else
                {
                    if (wuzuo < passengers.Length)
                    {
                        System.Threading.Thread.Sleep(1000);
                        goto CheckOrderInfo;
                    }
                }

            }
            string confirmSingleForQueueOrderUrl = "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=confirmSingleForQueueOrder ";
            string resStateContent = HttpRequest.Create(confirmSingleForQueueOrderUrl, "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=init", Cookie, HttpMethod.POST, postStr).GetString();

            if (resStateContent.Contains("验证码"))
            {
                goto ConfirmRequest;
            }
            ResState resState = resStateContent.ToJsonObject<ResState>();

            if (resState == null)
            {
                Common.LogUtil.Log(resStateContent);
                return "下单失败:确认订单时，系统返回的内容不正确," + resStateContent;
            }
            else
            {
                if (resState.ErrMsg.Equals("Y"))
                {
                    return "";
                }
                else
                {
                    Common.LogUtil.Log(resStateContent);
                    return "请求异常,响应状态为：" + resState.ErrMsg;
                }
            }


        }

        /// <summary>
        /// https://dynamic.12306.cn/otsweb/order/querySingleAction.do?method=submutOrderRequest
        /// </summary>
        /// <param name="trainItemInfo"></param>
        /// <param name="passengers"></param>
        /// <param name="seatType"></param>
        /// <returns>得到页面的表单信息</returns>
        public Dictionary<string, string> SubmitOrderRequest(TrainItemInfo trainItemInfo, Passenger[] passengers, SeatType seatType)
        {
            /*POST https://dynamic.12306.cn/otsweb/order/querySingleAction.do?method=submutOrderRequest HTTP/1.1
            * Referer: https://dynamic.12306.cn/otsweb/order/querySingleAction.do?method=init
            * 
            */

            var config=ConfigFileManager.GetConfig<BuyTicketConfig>();
            string backTrainDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1).ToString("yyyy-MM-dd");
            Dictionary<string,string> newforms=new Dictionary<string, string>()
            {
                
{"secretStr",trainItemInfo.MmStr	},
{"train_date",	config.OrderRequest.TrainDate.ToString("yyyy-MM-dd")},
{"back_train_date",backTrainDate},
{"tour_flag","dc"},
{"purpose_codes","ADULT"},
{"query_from_station_name",config.OrderRequest.FromStationTelecodeName},
{"query_to_station_name",config.OrderRequest.ToStationTelecodeName},
{"undefined",""},
            };
            foreach (var keyValue in this.QueryDynamicJsActionResult)
            {
                newforms.Add(keyValue.Key, keyValue.Value);
            }
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
            CookieCollection cookieCollection=new CookieCollection()
            {
                //new Cookie("_jc_save_czxxcx_toStation",""),
                //new Cookie("_jc_save_czxxcx_fromDate","2014-12-25"),
             new Cookie("_jc_save_fromStation",Common.HtmlUtil.UrlEncode(config.OrderRequest.FromStationTelecodeName+","+config.OrderRequest.FromStationTelecode)){Expires = expires },
             new Cookie("_jc_save_toStation",Common.HtmlUtil.UrlEncode(config.OrderRequest.ToStationTelecodeName+","+config.OrderRequest.ToStationTelecode)){Expires = expires },
             new Cookie("_jc_save_fromDate",config.OrderRequest.TrainDate.ToString("yyyy-MM-dd")){Expires = expires },
             new Cookie("_jc_save_toDate",config.OrderRequest.TrainDate.ToString("yyyy-MM-dd")){Expires = expires },
             new Cookie("_jc_save_wfdc_flag","dc"){Expires = expires },
            // new Cookie("current_captcha_type","C"){Expires = expires },
            };
            this.Cookie.Add(new Uri(ActionUrls.TicketHomePage), cookieCollection);
            var response = this.GetHttpClient(ActionUrls.QueryPageUrl, true)
                .PostAsync("https://kyfw.12306.cn/otn/leftTicket/submitOrderRequest",
                    new FormUrlEncodedContent(newforms))
                .Result.Content.ReadAsStringAsync()
                .Result.ToJsonObject<Response<string>>();
            if (!response.status)//说明提交成功
            {
                throw  new Exception(response.messages[0].ToString()); //抛出异常
            }

            //转到订单页面
            string content = this.GetHttpClient(ActionUrls.QueryPageUrl).PostAsync("https://kyfw.12306.cn/otn/confirmPassenger/initDc",new UrlEncodedContent(new{_json_att=""})).Result.Content.ReadAsStringAsync().Result;

            Dictionary<string, string> dynamicJsAction = GetDynamicJsAction(content, "https://kyfw.12306.cn/otn/confirmPassenger/initDc");
            Regex regex = new Regex(@"[A-z0-9]{32,}");
            var matches = regex.Matches(content);
            string token = matches[0].Groups[1].ToString();
            string key_check_isChange = matches[1].Groups[1].ToString();
            string leftTicketStr = matches[2].Groups[1].ToString();
          dynamicJsAction.Add("REPEAT_SUBMIT_TOKEN",token);
          dynamicJsAction.Add("key_check_isChange", key_check_isChange);
          dynamicJsAction.Add("leftTicketStr", leftTicketStr);
          //调用获取用户信息
            GetPassengers(token); 
          return dynamicJsAction;
        }


        
        /// <summary>
        /// 验证码
        /// </summary>
        /// <param name="vCodeType"></param>
        /// <returns></returns>
        public Image GetVerifyCode(VCodeType vCodeType)
        {
            //0.9789911571440171
            Random random = new Random(DateTime.Now.Millisecond);

            string url = "https://dynamic.12306.cn/otsweb/passCodeNewAction.do?module=login&rand=sjrand&" + random.NextDouble(); ;
            string referUrl = "https://dynamic.12306.cn/otsweb/loginAction.do?method=init";
            if (vCodeType == VCodeType.SubmitOrder)
            {
                url = "https://dynamic.12306.cn/otsweb/passCodeNewAction.do?module=passenger&rand=randp&" + random.NextDouble(); ;
                referUrl = "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=init";
            }
            Stream stream = HttpRequest.Create(url, referUrl, Cookie, HttpMethod.GET, "", Proxy).GetStream();
            return Image.FromStream(stream);
        }
        /// <summary>
        /// 检查验证码及表单信息
        /// https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=checkOrderInfo&rand={0}
        /// </summary>
        /// <returns>会返回验证码不正确等信息</returns>
        public string CheckOrderInfo(string vcode, string postStr)
        {
            string checkOrderInfoUrl = "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=checkOrderInfo&rand=" + vcode;

            string resCheckContent = HttpRequest.Create(checkOrderInfoUrl, "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=init", Cookie, HttpMethod.POST, postStr + "&tFlag=dc").GetString();

            //if (resCheckContent.Contains("验证码"))
            //{
            //    goto ConfirmRequest;
            //}
            return resCheckContent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="seatType"></param>
        /// <returns></returns>
        public ResYpInfo GetQueueCount(NameValueCollection forms, SeatType seatType)
        {
            //https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=getQueueCount&train_date=2013-02-04&train_no=24000000T50E&station=T5&seat=1&from=BXP&to=NNZ&ticket=1027353027407675000010273500003048050000
            string getQueueCountUrl = @"https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=getQueueCount&train_date="
            + forms["orderRequest.train_date"]
            + "&train_no=" + forms["orderRequest.train_no"]
            + "&station=" + forms["orderRequest.station_train_code"] +
            "&seat=" + seatType.ToSeatTypeValue() +
            "&from=" + forms["orderRequest.from_station_telecode"] +
            "&to=" + forms["orderRequest.to_station_telecode"] +
            "&ticket=" + forms["leftTicketStr"];
            ResYpInfo resYpInfo = HttpRequest.Create(getQueueCountUrl + "&t=" + DateTime.Now.Ticks, "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=init", Cookie, HttpMethod.GET, "").GetJsonObject<ResYpInfo>(); ;
            // {"countT":0,"count":355,"ticket":"1*****30504*****00001*****00003*****0000","op_1":true,"op_2":false}
            return resYpInfo;
            //int seatNum = Utils.GetRealSeatNumber(resYpInfo.Ticket, seatType);
            //int wuzuo = 0;
            //if (seatType == SeatType.硬座)
            //    wuzuo = Utils.GetRealSeatNumber(resYpInfo.Ticket, SeatType.无座);
            //if (rtbLog != null)
            //{
            //    if (rtbLog.InvokeRequired)
            //    {
            //        rtbLog.Invoke(new Action(() =>
            //        {
            //            rtbLog.Text += string.Format("===>{0},{1}人排队,余票 {2} 张 {3}\r\n", seatType, resYpInfo.CountT, seatNum, wuzuo == 0 ? "" : ",无座 " + wuzuo + " 张");
            //            rtbLog.SelectionStart = rtbLog.TextLength;
            //            rtbLog.ScrollToCaret();
            //        }));

            //    }
            //    else
            //    {
            //        rtbLog.Text += string.Format("===>{0},{1}人排队,余票 {2} 张 {3}\r\n", seatType, resYpInfo.CountT, seatNum, wuzuo == 0 ? "" : ",无座 " + wuzuo + " 张");
            //        rtbLog.SelectionStart = rtbLog.TextLength;
            //        rtbLog.ScrollToCaret();
            //    }
            //}
            //if (force && seatNum < passengers.Length)
            //{
            //    if (wuzuo == 0)
            //    {
            //        System.Threading.Thread.Sleep(1000);
            //        goto CheckOrderInfo;
            //    }
            //    else
            //    {
            //        if (wuzuo < passengers.Length)
            //        {
            //            System.Threading.Thread.Sleep(1000);
            //            goto CheckOrderInfo;
            //        }
            //    }

            //}
        }
        public string ConfirmSingleForQueue(string postStr)
        {
            string confirmSingleForQueueOrderUrl = "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=confirmSingleForQueue";
            string resStateContent = HttpRequest.Create(confirmSingleForQueueOrderUrl, "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=init", Cookie, HttpMethod.POST, postStr).GetString();
            return resStateContent;
            //if (resStateContent.Contains("验证码"))
            //{
            //    goto ConfirmRequest;
            //}
            //ResState resState = resStateContent.ToJsonObject<ResState>();

            //if (resState == null)
            //{
            //    Common.LogUtil.Log(resStateContent);
            //    return "下单失败:确认订单时，系统返回的内容不正确," + resStateContent;
            //}
            //else
            //{
            //    if (resState.ErrMsg.Equals("Y"))
            //    {
            //        return "";
            //    }
            //    else
            //    {
            //        Common.LogUtil.Log(resStateContent);
            //        return "请求异常,响应状态为：" + resState.ErrMsg;
            //    }
            //}
        }

        /// <summary>
        /// 等待订单完成
        /// </summary>
        /// <returns></returns>
        public WaitResponse GetWaitResponse(out string rspContent)
        {
            string waitUrl = "https://dynamic.12306.cn/otsweb/order/myOrderAction.do?method=getOrderWaitTime&tourFlag=dc";
            string waitResponseContent = HttpRequest.Create(waitUrl, "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=init", Cookie, HttpMethod.GET, "").GetString();
            rspContent = waitResponseContent;
            return waitResponseContent.ToJsonObject<WaitResponse>();


        }



        /// <summary>
        /// 查询余票,确保trainItemInfo.YpInfoDetailReal已经获取
        /// </summary>
        /// <param name="trainItemInfo"></param>
        /// <param name="seatType"></param>
        /// <returns></returns>
        public ResYpInfo Query(TrainItemInfo trainItemInfo, string seatType)
        {
            if (string.IsNullOrEmpty(trainItemInfo.YpInfoDetailReal)) return null;

            //https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=getQueueCount&train_date=2012-10-12&station=Z133&seat=3&from=BXP&to=NCG&ticket=10175031254048600024101750000060895000113030800111

            string queryUrl = string.Format("https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=getQueueCount&train_date={0}&station={1}&seat={2}&from={3}&to={4}&ticket={5}",
               BuyTicketConfig.Instance.OrderRequest.TrainDate.ToString("yyyy-MM-dd"), trainItemInfo.TrainNo, seatType, trainItemInfo.FromStationTelecode, trainItemInfo.ToStationTelecode, trainItemInfo.YpInfoDetailReal);
            ResYpInfo resYpInfo = HttpRequest.Create(queryUrl, "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=init", Cookie, HttpMethod.GET, "").GetJsonObject<ResYpInfo>();
            return resYpInfo;
        }
        

        public static List<TrainItemInfo> ToTrainItemInfos(QueryResponse response, List<TrainItemInfo> oldList)
        {
           
            List<TrainItemInfo> list = new List<TrainItemInfo>();

            foreach (var item in response)
            {
                var trainItem = item.queryLeftNewDTO;
                string trainNo = trainItem.station_train_code;
                TrainItemInfo itemInfo = oldList.Find(p => p.TrainNo == trainNo);
                if (itemInfo == null)
                    itemInfo = new TrainItemInfo();
               // itemInfo.No =item.queryLeftNewDTO.
                itemInfo.TrainNo = trainItem.station_train_code;
                itemInfo.StartStation = trainItem.start_station_name;
                itemInfo.EndStation = trainItem.end_station_name;
                itemInfo.lishi = trainItem.lishi;
                itemInfo.swz_num = trainItem.swz_num;
                itemInfo.tz_num = trainItem.tz_num;
                itemInfo.zy_num = trainItem.zy_num;// Common.HtmlUtil.RemoveHtml(contents[7]);
                itemInfo.ze_num = trainItem.ze_num;//Common.HtmlUtil.RemoveHtml(contents[8]);
                itemInfo.gr_num = trainItem.gr_num;//Common.HtmlUtil.RemoveHtml(contents[9]);
                itemInfo.rw_num = trainItem.rw_num; //Common.HtmlUtil.RemoveHtml(contents[10]);
                itemInfo.yw_num = trainItem.yw_num;//Common.HtmlUtil.RemoveHtml(contents[11]);
                itemInfo.rz_num = trainItem.rz_num;// Common.HtmlUtil.RemoveHtml(contents[12]);
                itemInfo.yz_num = trainItem.yz_num;// Common.HtmlUtil.RemoveHtml(contents[13]);
                itemInfo.wz_num = trainItem.wz_num;//Common.HtmlUtil.RemoveHtml(contents[14]);
                itemInfo.qt_num = trainItem.qt_num;//Common.HtmlUtil.RemoveHtml(contents[15]);

                itemInfo.TrainNo4 = trainItem.train_no;
                //itemInfo.Tag =trainItem. Common.HtmlUtil.RemoveHtml(contents[16]);
                
                   // itemInfo.TrainNo = tags[0];
                   // itemInfo.lishi = tags[1];
                itemInfo.TrainStartTime = trainItem.start_time;
                   // itemInfo.TrainNo4 = tags[3];
                itemInfo.FromStationTelecode = trainItem.from_station_telecode;
                itemInfo.ToStationTelecode = trainItem.to_station_telecode;
                itemInfo.ArriveTime = trainItem.arrive_time;
                itemInfo.FromStationName = trainItem.from_station_name;
                itemInfo.ToStationName = trainItem.to_station_name;
                itemInfo.FromStationNo = trainItem.from_station_no;
                itemInfo.ToStationNo = trainItem.to_station_no;
                itemInfo.YpInfoDetail = trainItem.yp_info;
                itemInfo.MmStr = item.secretStr;
                itemInfo.LocationCode = trainItem.location_code;
               
                list.Add(itemInfo);
            }
            return list;
        }


        
        public override string ToString()
        {
            if (Proxy != null)
            {
                return this.Username + "[" + Proxy.Address.Host + "]";

            }
            else
            {
                return this.Username;
            }
            //    return base.ToString();
        }



    }

    /// <summary>
    /// 验证码类型
    /// </summary>
    public enum VCodeType
    {
        Login,
        SubmitOrder,
    }


    /// <summary>
    /// 账户管理器
    /// </summary>
    public class AccountManager
    {
        static Dictionary<string, Account> accounts = new Dictionary<string, Account>();
        /// <summary>
        /// 当账号密码代理ip变化时会创建新的账号，否则沿用原来的账号
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="proxyIp"></param>
        /// <returns></returns>
        public static Account GetAccount(string userName, string password, string proxyIp)
        {
            string key = string.Format("{0},{1},{2}", userName, password, proxyIp).ToLower();
            if (accounts.ContainsKey(key))
            {
                return accounts[key];
            }
            else
            {
                Account account = new Account(userName, password, proxyIp);
                accounts.Add(key, account);
                return account;
            }
        }


    }
}