using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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

        public string Username { get; private set; }
        public string Password { get; private  set; }
        //public AccountInfo AccountInfo { get { return Config.BuyTicketConfig.Instance.AccountInfo; } }
        public WebProxy Proxy { get; set; }
        private CookieContainer _cookie=new CookieContainer();

       // public JHttpClient JHttpClient { get; set; }
        public CookieContainer Cookie
        {
            get { return _cookie; }
            set { _cookie = value; }
        }

        public bool IsLogin { get; private set; }

        public Account(string userName,string password):this(userName,password,null)
        {
        }
        public Account(string userName, string password, string proxyIp)
        {
            this.Username = userName;
            this.Password = password;
            if(!string.IsNullOrEmpty(proxyIp))
            Proxy = new WebProxy(proxyIp,443); //https代理
          //  JHttpClient = new JHttpClient(Proxy) { Cookie=Cookie};
        }
        /// <summary>
        /// 获取登录时的验证码,自动重试当错误出现3次以上抛异常
        /// </summary>
        /// <returns></returns>
        public string GetLoginVCode()
        {
            string vcode = "";
            do
            {

                Stream stream = this.GetHttpClient(ActionUrls.LoginPageUrl).GetStreamAsync("https://kyfw.12306.cn/otn/passcodeNew/getPassCodeNew?module=login&rand=sjrand&0." + new Random().Next(100000000, 99999999) + new Random().Next(100000000, 99999999)).Result;
                    Image image = Image.FromStream(stream);
                    vcode = GetVCodeByForm(image);
               
            } while (vcode == "");
            return vcode;
        }

        /// <summary>
        /// 检查验证码有效性
        /// </summary>
        /// <param name="randCode"></param>
        /// <returns></returns>
        public CheckRandCodeAnsynResponse CheckRandCodeAnsyn(string randCode)
        {
          return   this.GetHttpClient(ActionUrls.LoginPageUrl).PostAsync("https://kyfw.12306.cn/otn/passcodeNew/checkRandCodeAnsyn",
                    new { randCode, rand = "sjrand", randCode_validate = "" }).Result.Content.ReadAsStringAsync().Result.ToJsonObject<CheckRandCodeAnsynResponse>();
        }
        ///// <summary>
        ///// 获取登录随机数
        ///// </summary>
        ///// <returns></returns>
        //public string GetLoginRand()
        //{
            
        //    LoginAysnSuggestInfo loginAysnSuggestInfo = HttpRequest.Create("https://dynamic.12306.cn/otsweb/loginAction.do?method=loginAysnSuggest", "https://dynamic.12306.cn/otsweb/loginAction.do?method=init", Cookie, HttpMethod.GET, "").GetJsonObject<LoginAysnSuggestInfo>();
        //    if (loginAysnSuggestInfo != null && loginAysnSuggestInfo.RandError == "Y")
        //    {
        //        return loginAysnSuggestInfo.LoginRand;
        //    }
        //    return "";
        //}

        /// <summary>
        /// 检查服务器状态
        /// </summary>
        /// <returns></returns>
        public State CheckState()
        {
            string url = "https://dynamic.12306.cn/otsweb/order/querySingleAction.do?method=init";
            string content = HttpRequest.Create(url,"https://dynamic.12306.cn/otsweb/loginAction.do?method=init", Cookie,HttpMethod.GET, "", this.Proxy).GetString();
            return GetState(content);
        }
        /// <summary>
        /// 根据内容扑捉状态
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private State GetState(string content)
        {
            if (content.Contains("系统维护"))
            {
                return State.Maintenance;
            }
            if (content.Contains("loginForm") && content.Contains("loginUser.user_name"))
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
        /// <param name="loginRand"></param>
        /// <param name="vcode"></param>
        /// <returns></returns>
        public bool Login(string loginRand,string vcode)
        {
           // loginUserDTO.user_name=mydobit&userDTO.password=03265791&randCode=6eed&randCode_validate=&OTU2MzI1=YzRhNGM5ZTdmODI2MjczZg%3D%3D&myversion=undefined


            System.Collections.Specialized.NameValueCollection forms = new NameValueCollection();
            forms["loginRand"] = loginRand;
            forms["refundLogin"] = "N";
            forms["refundFlag"] = "Y";
            forms["loginUser.user_name"] = Username;
            forms["nameErrorFocus"] = "";
            forms["user.password"] = Password;
            forms["passwordErrorFocus"] = "";
            forms["randCode"] = vcode;
            forms["randErrorFocus"] = "";
            string content =HttpRequest.Create("https://dynamic.12306.cn/otsweb/loginAction.do?method=login",
                                   "https://dynamic.12306.cn/otsweb/loginAction.do?method=init", Cookie, forms,this.Proxy).
                    GetString();
            return IsLogin = !content.Contains("<title>登录</title>");

        }

        public void Login(ref bool auto)
        {
            //string url= "https://dynamic.12306.cn/otsweb/loginAction.do?method=init"
            //var res = HttpRequest.Create(url, referer: "https://dynamic.12306.cn/otsweb/", cookie: Cookie, method: HttpMethod.GET, postStr: "").GetResponse();
            //https://dynamic.12306.cn/otsweb/loginAction.do?method=loginAysnSuggest
            //NameValueCollection keyValues = DynamicJsAction();
            LoginAysnSuggestInfo loginAysnSuggestInfo = HttpRequest.Create("https://dynamic.12306.cn/otsweb/loginAction.do?method=loginAysnSuggest", "https://dynamic.12306.cn/otsweb/loginAction.do?method=init", Cookie, HttpMethod.GET, "").GetJsonObject<LoginAysnSuggestInfo>();
            if (loginAysnSuggestInfo != null && loginAysnSuggestInfo.RandError=="Y")
            {
                //loginRand=366&refundLogin=N&refundFlag=Y&loginUser.user_name=mydobit&nameErrorFocus=&user.password=&passwordErrorFocus=&randCode=ysqv&randErrorFocus=
                string vcode = GetVerifyCode(VCodeType.Login, ref auto);
                System.Collections.Specialized.NameValueCollection forms=new NameValueCollection();
                forms["loginRand"] = loginAysnSuggestInfo.LoginRand;
                forms["refundLogin"] = "N";
                forms["refundFlag"] = "Y";
                forms["isClick"] = "";
                forms["form_tk"] = "null";
                forms["loginUser.user_name"] = Username;
                forms["nameErrorFocus"] = "";
                forms["user.password"] = Password;
                forms["passwordErrorFocus"] = "";
                forms["randCode"] = vcode;
                forms["randErrorFocus"] = "";
                //foreach (string key in keyValues.AllKeys)
                //{
                //    forms["key"] = keyValues[key];
                //}
                string content = HttpRequest.Create("https://dynamic.12306.cn/otsweb/loginAction.do?method=login", "https://dynamic.12306.cn/otsweb/loginAction.do?method=init", Cookie, forms).GetString();
                IsLogin = !content.Contains("<title>登录</title>");
                return;
            }
            IsLogin = false;
        }

        //public NameValueCollection  DynamicJsAction()
        //{
        //    string jsContent = JHttpClient.GetString(
        //        "https://dynamic.12306.cn/otsweb/dynamicJsAction.do?jsversion=5520&method=loginJs", null,
        //        "https://dynamic.12306.cn/otsweb/loginAction.do?method=init");
        //    System.Text.RegularExpressions.Regex jsregex=new Regex(@"(function\sbin216(.*?))function\saj()");
        //    string js = jsregex.Replace(jsContent, "$1");
        //    System.Text.RegularExpressions.Regex keyregex = new Regex(@"var\s*?key\s*?=[""']([A-Za-z0-9+/=]*?)[""'];");
        //    string key = keyregex.Replace(jsContent, "$1");
        //    int cnt = new Regex(@"value\+='0';").Matches(jsContent).Count;
        //    string value = "";
        //    for (int i = 0; i < cnt; i++)
        //    {
        //        value += "0";
        //    }

        //    value = Utils.ExcuteJScript(js + ";" + "encode32(bin216(Base32.encrypt('" + value + "', '" + key + "')));");

        //    NameValueCollection nv=new NameValueCollection();
        //    nv[key] = value;
        //    nv["myversion"] = "undefined";
        //    return nv;
        //}

        /// <summary>
        /// https://dynamic.12306.cn/otsweb/loginAction.do?method=loginAysnSuggest
        /// </summary>
        /// <returns></returns>
        public LoginAysnSuggestInfo LoginAysnSuggest()
        {
            //string url= "https://dynamic.12306.cn/otsweb/loginAction.do?method=init"
            LoginAysnSuggestInfo loginAysnSuggestInfo = HttpRequest.Create("https://dynamic.12306.cn/otsweb/loginAction.do?method=loginAysnSuggest", "https://dynamic.12306.cn/otsweb/loginAction.do?method=init", Cookie, HttpMethod.GET, "").GetJsonObject<LoginAysnSuggestInfo>();
           
            return loginAysnSuggestInfo;
        }

        /// <summary>
        /// https://dynamic.12306.cn/otsweb/loginAction.do?method=login
        /// </summary>
        /// <param name="loginAysnSuggestInfo"></param>
        /// <param name="loginVCode">登陆验证码</param>
        /// <returns></returns>
        public bool Login(LoginAysnSuggestInfo loginAysnSuggestInfo,  string loginVCode)
        {
            NameValueCollection forms = new NameValueCollection();
            forms["loginRand"] = loginAysnSuggestInfo.LoginRand;
            forms["refundLogin"] = "N";
            forms["refundFlag"] = "Y";
            forms["loginUser.user_name"] = Username;
            forms["nameErrorFocus"] = "";
            forms["user.password"] = Password;
            forms["passwordErrorFocus"] = "";
            forms["randCode"] = loginVCode;
            forms["randErrorFocus"] = "";
            string content = HttpRequest.Create("https://dynamic.12306.cn/otsweb/loginAction.do?method=login", "https://dynamic.12306.cn/otsweb/loginAction.do?method=init", Cookie, forms).GetString();
            IsLogin = !content.Contains("<title>登录</title>");
            return IsLogin;
        }

        /// <summary>
        /// 获取乘客信息
        /// </summary>
        /// <returns></returns>
        public ResPassengerJsonInfo GetPassengers()
        {
            //https://dynamic.12306.cn/otsweb/passengerAction.do?method=queryPagePassenger
            //https://dynamic.12306.cn/otsweb/passengerAction.do?method=initUsualPassenger
            //pageIndex=0&pageSize=7&passenger_name=%E8%AF%B7%E8%BE%93%E5%85%A5%E6%B1%89%E5%AD%97%E6%88%96%E6%8B%BC%E9%9F%B3%E9%A6%96%E5%AD%97%E6%AF%8D
            //pageIndex=0&pageSize=7&passenger_name=请输入汉字或拼音首字母
            //{"recordCount":2,"rows":[{"address":"","born_date":{"date":10,"day":1,"hours":0,"minutes":0,"month":8,"seconds":0,"time":463593600000,"timezoneOffset":-480,"year":84},"code":"1","country_code":"CN","email":"dobit@163.com","first_letter":"MYDOBIT","isUserSelf":"Y","mobile_no":"15910675179","old_passenger_id_no":"","old_passenger_id_type_code":"","old_passenger_name":"","passenger_flag":"0","passenger_id_no":"362201198409101614","passenger_id_type_code":"1","passenger_id_type_name":"二代身份证","passenger_name":"林利","passenger_type":"1","passenger_type_name":"成人","phone_no":"","postalcode":"","recordCount":"2","sex_code":"M","sex_name":"男","studentInfo":null},{"address":"","born_date":{"date":13,"day":5,"hours":0,"minutes":0,"month":4,"seconds":0,"time":421603200000,"timezoneOffset":-480,"year":83},"code":"2","country_code":"CN","email":"","first_letter":"LY","isUserSelf":"N","mobile_no":"18610037900","old_passenger_id_no":"","old_passenger_id_type_code":"","old_passenger_name":"","passenger_flag":"0","passenger_id_no":"362201198305131667","passenger_id_type_code":"1","passenger_id_type_name":"二代身份证","passenger_name":"林艳","passenger_type":"1","passenger_type_name":"成人","phone_no":"","postalcode":"","recordCount":"2","sex_code":"F","sex_name":"女","studentInfo":null}]}
            string url = "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=getpassengerJson";
            string reffer = "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=init";

            //{"passengerJson":[{"first_letter":"XIAOYUAN800208","isUserSelf":"","mobile_no":"13556718373","old_passenger_id_no":"","old_passenger_id_type_code":"","old_passenger_name":"","passenger_flag":"0","passenger_id_no":"430523198910104211","passenger_id_type_code":"1","passenger_id_type_name":"","passenger_name":"袁凌云","passenger_type":"1","passenger_type_name":"","recordCount":"11"},{"first_letter":"RB","isUserSelf":"","mobile_no":"13510549626","old_passenger_id_no":"","old_passenger_id_type_code":"","old_passenger_name":"","passenger_flag":"0","passenger_id_no":"500242198801275573","passenger_id_type_code":"1","passenger_id_type_name":"","passenger_name":"冉波","passenger_type":"1","passenger_type_name":"","recordCount":"11"}]}
            return HttpRequest.Create(url, reffer, Cookie, HttpMethod.GET, "").GetJsonObject<ResPassengerJsonInfo>();
           
        }
        /// <summary>
        /// 查询列车信息
        /// </summary>
        /// <returns></returns>
        public List<TrainItemInfo> QueryTrainInfos(List<TrainItemInfo> oldList)
        {
            List<TrainItemInfo> list=new List<TrainItemInfo>();
            if(IsLogin)
            {
                //var  res =
                //    HttpRequest.Create("https://dynamic.12306.cn/otsweb/order/querySingleAction.do?method=init",
                //                       "https://dynamic.12306.cn/otsweb/loginAction.do?method=login", Cookie,
                //                       HttpMethod.GET,
                //                       "").GetResponse();
                //Todo://
                //&orderRequest.train_date=2012-10-10&orderRequest.from_station_telecode=BJP
                //&orderRequest.to_station_telecode=NCG&orderRequest.train_no=
                //&trainPassType=QB&trainClass=QB%23D%23Z%23T%23K%23QT%23&includeStudent=00&seatTypeAndNum=&orderRequest.start_time_str=00%3A00--24%3A00
                StringBuilder sb = new StringBuilder("https://dynamic.12306.cn/otsweb/order/querySingleAction.do?method=queryLeftTicket");
                sb.AppendFormat("&orderRequest.train_date={0}",BuyTicketConfig.Instance.OrderRequest.TrainDate.ToString("yyyy-MM-dd"));
                sb.AppendFormat("&orderRequest.from_station_telecode={0}", BuyTicketConfig.Instance.OrderRequest.FromStationTelecode);
                sb.AppendFormat("&orderRequest.to_station_telecode={0}", BuyTicketConfig.Instance.OrderRequest.ToStationTelecode);
                sb.AppendFormat("&orderRequest.train_no={0}", BuyTicketConfig.Instance.OrderRequest.TrainNo);
                sb.AppendFormat("&trainPassType={0}",Common.HtmlUtil.UrlEncode(BuyTicketConfig.Instance.OrderRequest.TrainPassType));
                sb.AppendFormat("&trainClass={0}", Common.HtmlUtil.UrlEncode(BuyTicketConfig.Instance.OrderRequest.TrainClass));
                sb.AppendFormat("&includeStudent=00{0}", BuyTicketConfig.Instance.OrderRequest.IncludeStudent);
                sb.AppendFormat("&seatTypeAndNum=&orderRequest.start_time_str={0}", Common.HtmlUtil.UrlEncode(BuyTicketConfig.Instance.OrderRequest.StartTimeStr));
                string url = sb.ToString();
#if !DEBUG
                HttpRequest httpRequest = HttpRequest.Create(url,
                                                             "https://dynamic.12306.cn/otsweb/order/querySingleAction.do?method=init",
                                                             Cookie, HttpMethod.GET,
                                                             new Random().NextDouble().ToString());
#else
                 HttpRequest httpRequest = HttpRequest.Create(url,
                                                             "https://dynamic.12306.cn/otsweb/order/querySingleAction.do?method=init",
                                                             Cookie, HttpMethod.POST,
                                                             new Random().NextDouble().ToString());
#endif
                httpRequest.ContentType = "";//清空请求
                string content = httpRequest.GetString();

                /*content===>
                 * 0,
                 * <span id='id_24000014530R' class='base_txtdiv' onmouseover=javascript:onStopHover('24000014530R#BXP#NCG') onmouseout='onStopOut()'>1453</span>,
                 * <img src='/otsweb/images/tips/first.gif'>&nbsp;&nbsp;&nbsp;&nbsp;北京西&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;12:09,
                 * <img src='/otsweb/images/tips/last.gif'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;南昌&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;06:09,
                 * 18:00,4#
                 * --,--,--,--,--,<font color='#008800'>有</font>,<font color='#008800'>有</font>,--,<font color='#008800'>有</font>,<font color='#008800'>有</font>,--,
                 * <input type='button' class='yuding_u' onmousemove=this.className='yuding_u_over' onmousedown=this.className='yuding_u_down' onmouseout=this.className='yuding_u' onclick=javascript:getSelected('1453#18:00#12:09#24000014530R#BXP#NCG#06:09#北京西#南昌#1*****32464*****00221*****03383*****0189#901E8F9F74DD1531E4E4F263D7454E2332EB2A012A1034011F851E79') value='预订'></input>\n1,<span id='id_240000T14500' class='base_txtdiv' onmouseover=javascript:onStopHover('240000T14500#BJP#NCG') onmouseout='onStopOut()'>T145</span>,<img src='/otsweb/images/tips/first.gif'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;北京&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;12:09,<img src='/otsweb/images/tips/last.gif'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;南昌&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;10:37,22:28,--,--,--,--,--,11,9,--,<font color='darkgray'>无</font>,<font color='#008800'>有</font>,--,<input type='button' class='yuding_u' onmousemove=this.className='yuding_u_over' onmousedown=this.className='yuding_u_down' onmouseout=this.className='yuding_u' onclick=javascript:getSelected('T145#22:28#12:09#240000T14500#BJP#NCG#10:37#北京#南昌#1*****31624*****00111*****00003*****0009#10D427205AA48B5464D234D8C5397FB6806783F4ED6AD1C939C5424C') value='预订'></input>\n2,<span id='id_240000T1670I' class='base_txtdiv' onmouseover=javascript:onStopHover('240000T1670I#BXP#NCG') onmouseout='onStopOut()'>T167</span>,<img src='/otsweb/images/tips/first.gif'>&nbsp;&nbsp;&nbsp;&nbsp;北京西&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;14:54,<img src='/otsweb/images/tips/last.gif'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;南昌&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;08:20,17:26,--,--,--,--,--,9,5,--,<font color='darkgray'>无</font>,<font color='#008800'>有</font>,--,<input type='button' class='yuding_u' onmousemove=this.className='yuding_u_over' onmousedown=this.className='yuding_u_down' onmouseout=this.className='yuding_u' onclick=javascript:getSelected('T167#17:26#14:54#240000T1670I#BXP#NCG#08:20#北京西#南昌#1*****30624*****00091*****00003*****0005#8EC58665C1266269D823F20328FBB46578FE22BE9C12846B31502520') value='预订'></input>\n3,<span id='id_240000K5710A' class='base_txtdiv' onmouseover=javascript:onStopHover('240000K5710A#BXP#NCG') onmouseout='onStopOut()'>K571</span>,<img src='/otsweb/images/tips/first.gif'>&nbsp;&nbsp;&nbsp;&nbsp;北京西&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;16:55,&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;南昌&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;09:44,16:49,--,--,--,--,--,4,<font color='#008800'>有</font>,--,<font color='#008800'>有</font>,<font color='#008800'>有</font>,--,<input type='button' class='yuding_u' onmousemove=this.className='yuding_u_over' onmousedown=this.className='yuding_u_down' onmouseout=this.className='yuding_u' onclick=javascript:getSelected('K571#16:49#16:55#240000K5710A#BXP#NCG#09:44#北京西#南昌#1*****33464*****00041*****05323*****0041#13EDB01BC9D0162C4CE81DC3595ECDF7C01C0DB57A61413C39419712') value='预订'></input>\n4,<span id='id_240000Z13305' class='base_txtdiv' onmouseover=javascript:onStopHover('240000Z13305#BXP#NCG') onmouseout='onStopOut()'>Z133</span>,<img src='/otsweb/images/tips/first.gif'>&nbsp;&nbsp;&nbsp;&nbsp;北京西&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;19:45,&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;南昌&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;07:14,11:29,--,--,--,--,14,<font color='#008800'>有</font>,<font color='#008800'>有</font>,--,3,<font color='#008800'>有</font>,--,<input type='button' class='yuding_u' onmousemove=this.className='yuding_u_over' onmousedown=this.className='yuding_u_down' onmouseout=this.className='yuding_u' onclick=javascript:getSelected('Z133#11:29#19:45#240000Z13305#BXP#NCG#07:14#北京西#南昌#1*****31214*****00551*****00036*****00143*****0086#E3AA59C14B426728B9865034582D45FCD0F7B7DA2D5B5ABC115CEF74') value='预订'></input>\n5,<span id='id_2400000Z6506' class='base_txtdiv' onmouseover=javascript:onStopHover('2400000Z6506#BXP#NCG') onmouseout='onStopOut()'>Z65</span>,<img src='/otsweb/images/tips/first.gif'>&nbsp;&nbsp;&nbsp;&nbsp;北京西&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;20:00,<img src='/otsweb/images/tips/last.gif'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;南昌&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;07:22,11:22,--,--,--,--,14,<font color='#008800'>有</font>,<font color='#008800'>有</font>,--,<font color='darkgray'>无</font>,<font color='darkgray'>无</font>,--,<input type='button' class='yuding_u' onmousemove=this.className='yuding_u_over' onmousedown=this.className='yuding_u_down' onmouseout=this.className='yuding_u' onclick=javascript:getSelected('Z65#11:22#20:00#2400000Z6506#BXP#NCG#07:22#北京西#南昌#1*****30004*****01261*****00006*****00143*****0040#147C867FC1A296D487C9C628E6890170A504A53410C2688FA5958DF0') value='预订'></input>\n6,<span id='id_2400000Z6706' class='base_txtdiv' onmouseover=javascript:onStopHover('2400000Z6706#BXP#NCG') onmouseout='onStopOut()'>Z67</span>,<img src='/otsweb/images/tips/first.gif'>&nbsp;&nbsp;&nbsp;&nbsp;北京西&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;20:06,<img src='/otsweb/images/tips/last.gif'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;南昌&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;07:32,11:26,--,--,--,--,8,<font color='#008800'>有</font>,<font color='#008800'>有</font>,--,1,<font color='#008800'>有</font>,--,<input type='button' class='yuding_u' onmousemove=this.className='yuding_u_over' onmousedown=this.className='yuding_u_down' onmouseout=this.className='yuding_u' onclick=javascript:getSelected('Z67#11:26#20:06#2400000Z6706#BXP#NCG#07:32#北京西#南昌#1*****31594*****00361*****00016*****00083*****0100#0D4F337C3D339CA418DBBED1368663DE011CB59EA828E193D7B43707') value='预订'></input>\n7,<span id='id_240000T1070D' class='base_txtdiv' onmouseover=javascript:onStopHover('240000T1070D#BXP#NCG') onmouseout='onStopOut()'>T107</span>,<img src='/otsweb/images/tips/first.gif'>&nbsp;&nbsp;&nbsp;&nbsp;北京西&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;20:12,&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;南昌&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;09:31,13:19,--,--,--,--,--,<font color='darkgray'>无</font>,<font color='darkgray'>无</font>,--,<font color='darkgray'>无</font>,<font color='#008800'>有</font>,--,<input type='button' class='yuding_u' onmousemove=this.className='yuding_u_over' onmousedown=this.className='yuding_u_down' onmouseout=this.className='yuding_u' onclick=javascript:getSelected('T107#13:19#20:12#240000T1070D#BXP#NCG#09:31#北京西#南昌#1*****31264*****00001*****00003*****0000#5296112E65BC5E9064A23C5CFE49C14912D395D7A03947C27FA34E90') value='预订'></input>\n8,<span id='id_240000K1050Q' class='base_txtdiv' onmouseover=javascript:onStopHover('240000K1050Q#BXP#NCG') onmouseout='onStopOut()'>K105</span>,<img src='/otsweb/images/tips/first.gif'>&nbsp;&nbsp;&nbsp;&nbsp;北京西&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;23:45,&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;南昌&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;15:48,16:03,--,--,--,--,--,8,1,--,5,<font color='#008800'>有</font>,--,<input type='button' class='yuding_u' onmousemove=this.className='yuding_u_over' onmousedown=this.className='yuding_u_down' onmouseout=this.className='yuding_u' onclick=javascript:getSelected('K105#16:03#23:45#240000K1050Q#BXP#NCG#15:48#北京西#南昌#1*****31734*****00081*****00053*****0001#18B07C0C004B7BEF04F979E063EFDAF3401764D0968D81C7092E56B7') value='预订'></input>
                 */
                if (content.Contains("系统维护中")) throw new Exception("系统维护");
                return ToTrainItemInfos(content, oldList);
            }
            return list;
        }

        /// <summary>
        /// 下订单，但不返回订单编号
        /// </summary>
        /// <returns>无错误时返回空，其它返回错误</returns>
        public string OrderTicket(TrainItemInfo trainItemInfo,Passenger[] passengers,SeatType seatType,ref bool stop, bool force=false,RichTextBox rtbLog=null)
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
            NameValueCollection forms=new NameValueCollection();
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
            forms["train_start_time"] =trainItemInfo.TrainStartTime;
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
                if(key.StartsWith("orderRequest"))
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
                 * passengerTickets=3,0,1,林利,1,362201198...,15910675179,Y
                 * &oldPassengers=林利,1,362201198...&passenger_1_seat=3&passenger_1_seat_detail_select=0&passenger_1_seat_detail=0&passenger_1_ticket=1&passenger_1_name=林利&passenger_1_cardtype=1&passenger_1_cardno=362201198&passenger_1_mobileno=15910675179&checkbox9=Y
                 * */
                if (passenger.Checked)
                {
                    postStr += "&passengerTickets=" +
                               Common.HtmlUtil.UrlEncode(string.Format("{0},{1},{2},{3},{4},{5},{6},Y",
                                                                       //passenger.SeatType.ToSeatTypeValue(),
                                                                       seatType.ToSeatTypeValue(),
                                                                       (int) passenger.SeatDetailType,
                                                                       (int) passenger.TicketType, passenger.Name,
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
            if(stop)
            {
                return "用户终止执行";
            }
            string resCheckContent = HttpRequest.Create(checkOrderInfoUrl, "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=init", Cookie, HttpMethod.POST, postStr + "&tFlag=dc").GetString();

            if (resCheckContent.Contains("验证码"))
            {
                goto ConfirmRequest;
            }
            //https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=getQueueCount&train_date=2013-02-04&train_no=24000000T50E&station=T5&seat=1&from=BXP&to=NNZ&ticket=1027353027407675000010273500003048050000
            string  getQueueCountUrl=@"https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=getQueueCount&train_date="
            +forms["orderRequest.train_date"]
            +"&train_no="+forms["orderRequest.train_no"]
            +"&station="+forms["orderRequest.station_train_code"]+
            "&seat="+seatType.ToSeatTypeValue()+
            "&from="+forms["orderRequest.from_station_telecode"]+
            "&to=" + forms["orderRequest.to_station_telecode"] +
            "&ticket="+ forms["leftTicketStr"];
          ResYpInfo resYpInfo =  HttpRequest.Create(getQueueCountUrl, "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=init", Cookie, HttpMethod.GET, "").GetJsonObject<ResYpInfo>();;
           // {"countT":0,"count":355,"ticket":"1*****30504*****00001*****00003*****0000","op_1":true,"op_2":false}

            int seatNum = Utils.GetRealSeatNumber(resYpInfo.Ticket, seatType);
            int  wuzuo =0;
            if(seatType==SeatType.硬座)
             wuzuo=Utils.GetRealSeatNumber(resYpInfo.Ticket, SeatType.无座);
             if (rtbLog!=null)
              {
                  if (rtbLog.InvokeRequired)
                  {
                      rtbLog.Invoke(new Action(()=>
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
          if (force &&  seatNum< passengers.Length)
          {
              if(wuzuo==0)
              {
                  System.Threading.Thread.Sleep(1000);
                  goto CheckOrderInfo;
              }else
              {
                  if(wuzuo <passengers.Length)
                  {
                      System.Threading.Thread.Sleep(1000);
                      goto CheckOrderInfo;
                  }
              }
              
          }
            string confirmSingleForQueueOrderUrl = "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=confirmSingleForQueueOrder ";
           string resStateContent=  HttpRequest.Create(confirmSingleForQueueOrderUrl, "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=init", Cookie, HttpMethod.POST, postStr).GetString();
           
            if(resStateContent.Contains("验证码"))
            {
                goto ConfirmRequest;
            }
            ResState resState = resStateContent.ToJsonObject<ResState>();
            
            if(resState==null)
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
        public NameValueCollection SubmitOrderRequest(TrainItemInfo trainItemInfo, Passenger[] passengers, SeatType seatType)
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
            //trainItemInfo.YpInfoDetailReal = htmlForm["leftTicketStr"]; //用于实时查询余票信息
            //if (string.IsNullOrEmpty(trainItemInfo.YpInfoDetailReal))
            //{
            //    Common.LogUtil.Log(content);
            //    return "下单失败:未能获取真实的余票串信息";
            //}
            return htmlForm;
        }


        /// <summary>
        /// 获取登录时的验证码,自动重试当错误出现3次以上抛异常
        /// </summary>
        /// <returns>返回BREAK 表示用户终止执行 否则为验证码值</returns>
        public string GetVerifyCode(VCodeType vCodeType,ref bool auto)
        {
            //0.9789911571440171
            Random random = new Random(DateTime.Now.Millisecond);

            string url = "https://dynamic.12306.cn/otsweb/passCodeNewAction.do?module=login&rand=sjrand&" + random.NextDouble(); ;
            string referUrl = "https://dynamic.12306.cn/otsweb/loginAction.do?method=init";
            if(vCodeType==VCodeType.SubmitOrder)
            {
                url = "https://dynamic.12306.cn/otsweb/passCodeNewAction.do?module=passenger&rand=randp&" + random.NextDouble(); ;
                referUrl = "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=init";
            }
            string vcode = "";
            do
            {

                Stream stream =HttpRequest.Create(url,referUrl, Cookie,HttpMethod.GET, "", Proxy).GetStream();
                Image image = Image.FromStream(stream);
               
                    vcode = new Cracker().Read(new Bitmap(image));
                    if (vcode.Length < 4)
                    {
                        if (auto)
                            vcode = "";
                        else
                            vcode = GetVCodeByForm(image);
                    }
               
                //vcode = GetVCodeByForm(image);
                if (vcode == "BREAK")
                    return "用户终止";
            } while (vcode == "");
            return vcode;
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
        public ResYpInfo GetQueueCount(NameValueCollection forms,SeatType seatType)
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
            ResYpInfo resYpInfo = HttpRequest.Create(getQueueCountUrl+"&t="+DateTime.Now.Ticks, "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=init", Cookie, HttpMethod.GET, "").GetJsonObject<ResYpInfo>(); ;
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
            string waitUrl ="https://dynamic.12306.cn/otsweb/order/myOrderAction.do?method=getOrderWaitTime&tourFlag=dc";
            string  waitResponseContent = HttpRequest.Create(waitUrl, "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=init", Cookie, HttpMethod.GET, "").GetString();
            rspContent = waitResponseContent;
            return waitResponseContent.ToJsonObject<WaitResponse>();
           

        }



        /// <summary>
        /// 查询余票,确保trainItemInfo.YpInfoDetailReal已经获取
        /// </summary>
        /// <param name="trainItemInfo"></param>
        /// <param name="seatType"></param>
        /// <returns></returns>
        public ResYpInfo Query(TrainItemInfo trainItemInfo,string seatType )
        {
            if(string.IsNullOrEmpty(trainItemInfo.YpInfoDetailReal)) return null;

            //https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=getQueueCount&train_date=2012-10-12&station=Z133&seat=3&from=BXP&to=NCG&ticket=10175031254048600024101750000060895000113030800111
           
             string queryUrl =string.Format("https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=getQueueCount&train_date={0}&station={1}&seat={2}&from={3}&to={4}&ticket={5}",
                BuyTicketConfig.Instance.OrderRequest.TrainDate.ToString("yyyy-MM-dd"),trainItemInfo.TrainNo,seatType,trainItemInfo.FromStationTelecode,trainItemInfo.ToStationTelecode,trainItemInfo.YpInfoDetailReal);
            ResYpInfo resYpInfo = HttpRequest.Create(queryUrl, "https://dynamic.12306.cn/otsweb/order/confirmPassengerAction.do?method=init", Cookie, HttpMethod.GET, "").GetJsonObject<ResYpInfo>();
            return resYpInfo;
        }
        /// <summary>
        /// 通过窗体获取验证码
        /// </summary>
        /// <returns></returns>
        public static string GetVCodeByForm(Image image)
        {
            string ret;
           VCodeForm vCodeForm=new VCodeForm(image);
           if (vCodeForm.ShowDialog() == DialogResult.OK)
           {
               ret= vCodeForm.Value.Trim();
           }
           else
               ret= "BREAK";
            vCodeForm.Dispose();
            System.GC.ReRegisterForFinalize(vCodeForm);
            return ret;
        }

        public static List<TrainItemInfo> ToTrainItemInfos(string content, List<TrainItemInfo> oldList)
        {
            string[] records = content.Split(new string[]{"\\n"},StringSplitOptions.RemoveEmptyEntries);
            //int step = 16;
            List<TrainItemInfo> list=new List<TrainItemInfo>();
            for (int i = 0; i < records.Length; i ++)
            {
                string[] contents = records[i].Split(',');

                string trainNo = Common.HtmlUtil.RemoveHtml(contents[1]).Trim();
                TrainItemInfo itemInfo=oldList.Find(p=>p.TrainNo==trainNo);
                if (itemInfo == null)
                    itemInfo = new TrainItemInfo();


                itemInfo.No = contents[0];
                itemInfo.TrainNo = Common.HtmlUtil.RemoveHtml(contents[1]);
                itemInfo.StartStation = Common.HtmlUtil.HtmlDecode(Common.HtmlUtil.RemoveHtml(contents[2])).Trim();
                itemInfo.EndStation = Common.HtmlUtil.HtmlDecode(Common.HtmlUtil.RemoveHtml(contents[3])).Trim();
                itemInfo.lishi = Common.HtmlUtil.RemoveHtml(contents[4]);
                itemInfo.ShangwuSeat = Common.HtmlUtil.RemoveHtml(contents[5]);
                itemInfo.TedengSeat = Common.HtmlUtil.RemoveHtml(contents[6]);
                itemInfo.YidengSeat = Common.HtmlUtil.RemoveHtml(contents[7]);
                itemInfo.ErdengSeat = Common.HtmlUtil.RemoveHtml(contents[8]);
                itemInfo.GaojiRuanwoSeat = Common.HtmlUtil.RemoveHtml(contents[9]);
                itemInfo.RuanwoSeat = Common.HtmlUtil.RemoveHtml(contents[10]);
                itemInfo.YingwoSeat = Common.HtmlUtil.RemoveHtml(contents[11]);
                itemInfo.RuanzuoSeat = Common.HtmlUtil.RemoveHtml(contents[12]);
                itemInfo.YingzuoSeat = Common.HtmlUtil.RemoveHtml(contents[13]);
                itemInfo.WuzuoSeat = Common.HtmlUtil.RemoveHtml(contents[14]);
                itemInfo.OtherSeat = Common.HtmlUtil.RemoveHtml(contents[15]);


                itemInfo.Tag = Common.HtmlUtil.RemoveHtml(contents[16]);
                                             ;
                string tag = contents[16];//<input type='button' class='yuding_u' onmousemove=this.className='yuding_u_over' onmousedown=this.className='yuding_u_down' onmouseout=this.className='yuding_u' onclick=javascript:getSelected('K105#16:03#23:45#240000K1050Q#BXP#NCG#15:48#北京西#南昌#1*****31714*****00041*****00063*****0000#6CFF0AEBE197C7375772402464FD0A02D0FBFA53D12A0190DE083F2A') value='预订'></input>
                if (!string.IsNullOrEmpty(tag) && tag.Contains("getSelected('"))
                {

                    tag = tag.Split(new string[] {"getSelected('"},StringSplitOptions.RemoveEmptyEntries)[1].Split('\'')[0]; 
                    //Z133#11:29#19:45#240000Z13307#BXP#NCG#07:14#北京西#南昌#01#05#1*****30004*****00011*****00006*****00013*****0000#F3ED9FF7BC7D88A97D9AAD036FAABE99B4223BD3FB94FF50044F66A2#P2
                    //K105#16:03#23:45#240000K1050Q#BXP#NCG#15:48#北京西#南昌#1*****31714*****00041*****00063*****0000#6CFF0AEBE197C7375772402464FD0A02D0FBFA53D12A0190DE083F2A
                   // K301#02:41#02:05#480000K30423#NCG#YCG#04:46#南昌#宜春#14#16#1*****31034*****00081*****00003*****0001#MzM1QThFRTVDODlBNjlCODVERUNDMUVEODc4NjdENjlCODNDN0I0OUQyRTJENkUwQzZCMjk4QkE6Ojo6MTM4MDM4MDY2NDk2Nw==#H2
                    string[] tags = tag.Split('#');
                    itemInfo.TrainNo = tags[0];
                    itemInfo.lishi = tags[1];
                    itemInfo.TrainStartTime = tags[2];
                    itemInfo.TrainNo4 = tags[3];
                    itemInfo.FromStationTelecode = tags[4];
                    itemInfo.ToStationTelecode = tags[5];
                    itemInfo.ArriveTime = tags[6];
                    itemInfo.FromStationName = tags[7];
                    itemInfo.ToStationName = tags[8];
                    itemInfo.FromStationNo = tags[9];
                    itemInfo.ToStationNo = tags[10];
                    itemInfo.YpInfoDetail = tags[tags.Length - 3];
                    itemInfo.MmStr = tags[tags.Length-2];
                    itemInfo.LocationCode = tags[tags.Length - 1];
                }
                list.Add(itemInfo);
            }
            return list;
        }

        public override string ToString()
        {
            if (Proxy!=null)
            {return this.Username + "[" + Proxy.Address.Host + "]";
               
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
       static Dictionary<string,Account> accounts=new Dictionary<string, Account>(); 
        /// <summary>
        /// 当账号密码代理ip变化时会创建新的账号，否则沿用原来的账号
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="proxyIp"></param>
        /// <returns></returns>
        public static Account GetAccount(string userName,string password,string proxyIp)
        {
            string key = string.Format("{0},{1},{2}", userName, password, proxyIp).ToLower();
            if(accounts.ContainsKey(key))
            {
                return accounts[key];
            }
            else
            {
                Account account=new Account(userName,password,proxyIp);
                accounts.Add(key,account);
                return account;
            }
        }

       
    }
}