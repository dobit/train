using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LFNet.TrainTicket.Config;
using LFNet.TrainTicket.DAL;
using LFNet.TrainTicket.Entity;
using LFNet.TrainTicket.Response;

namespace LFNet.TrainTicket.BLL
{
    /// <summary>
    /// 代表一个模拟的客户端
    /// </summary>
    public class Client
    {
        #region Events
        /// <summary>
        /// 运行状态发生变化
        /// </summary>
        public event EventHandler<ClientEventArgs<ExcuteState>> ExcuteStateChanged;

        protected virtual void OnExcuteStateChanged(ClientEventArgs<ExcuteState> e)
        {
            EventHandler<ClientEventArgs<ExcuteState>> handler = ExcuteStateChanged;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        /// 客户端状态发生改变
        /// </summary>
        public event EventHandler<ClientEventArgs<LoginState>> LoginStateChanged;

        protected virtual void OnLoginStateChanged(ClientEventArgs<LoginState> e)
        {
            EventHandler<ClientEventArgs<LoginState>> handler = LoginStateChanged;
            if (handler != null) handler(this, e);
        }


        public event EventHandler<ClientEventArgs<string>> ClientChanged;

        /// <summary>
        /// 乘客信息发生变化
        /// </summary>
        public event EventHandler<ClientEventArgs<List<Passenger>>>  PassengersChanged;

        protected virtual void OnPassengersChanged(ClientEventArgs<List<Passenger>> e)
        {
            EventHandler<ClientEventArgs<List<Passenger>>> handler = PassengersChanged;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnClientChanged(ClientEventArgs<string> e)
        {
            EventHandler<ClientEventArgs<string>> handler = ClientChanged;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        /// 当发生错误时
        /// </summary>
        public event EventHandler<ClientEventArgs<Exception>> Error;

        protected virtual void OnError(ClientEventArgs<Exception> e)
        {
            EventHandler<ClientEventArgs<Exception>> handler = Error;
            if (handler != null) handler(this, e);
        }
        /// <summary>
        /// 当出现可订的票时警告
        /// </summary>
        public event EventHandler<ClientEventArgs> Alarm;

        protected virtual void OnAlarm(ClientEventArgs e)
        {
            EventHandler<ClientEventArgs> handler = Alarm;
            if (handler != null) handler(this, e);
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

        private LoginState _loginState;
        private ExcuteState _excuteState;

        #endregion

        #region  Properties
        /// <summary>
        /// 用户账户信息
        /// </summary>
        public AccountInfo Account { get; set; }

        ///乘客信息
        public List<Passenger> Passengers { get; set; }

        /// <summary>
        /// 获取的列车信息
        /// </summary>
        public List<TrainItemInfo> TrainInfos { get; set; }

        /// <summary>
        /// 登录状态发生变化
        /// </summary>
        public LoginState LoginState
        {
            get { return _loginState; }
            private set
            {
                if (_loginState != value)
                {
                    _loginState = value;
                    OnLoginStateChanged(new ClientEventArgs<LoginState>(value));
                }
            }
        }

        /// <summary>
        /// 执行状态发生变化
        /// </summary>
        public ExcuteState ExcuteState
        {
            get { return _excuteState; }
            private set
            {
                if (_excuteState != value)
                {
                    _excuteState = value;
                   OnExcuteStateChanged(new ClientEventArgs<ExcuteState>(value));
                }
            }
        }

        /// <summary>
        /// Cookie信息
        /// </summary>
        public CookieContainer Cookie { get; private set; }
       

        #endregion

        #region ctor

        public Client() : this(new AccountInfo())
        {
            
        }

        public Client(AccountInfo account)
        {
            Account = account;
            Cookie = new CookieContainer();
            TrainInfos=new List<TrainItemInfo>();
            Passengers=new List<Passenger>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 执行
        /// </summary>
        public async void Run()
        {
            this._stop = false;
            this.ExcuteState = ExcuteState.Running;
            //登陆
            var login = await Login();

            
            //查询余票
        QueryTickets:
           await QueryLeftTicket();
           var list = Filter(TrainInfos);
            if (list.Count > 0)
            {
                //display infomation
                Info(list.ToDisplayString());

                int needSeatNumber = this.Passengers.Count(p => p.Checked);// Account.Passengers.Split(new []{','},StringSplitOptions.RemoveEmptyEntries).Length;//需要的票数

                foreach (string seatTypeStr in Account.SeatOrder.Split(','))
                {
                    //有效的列车
                    List<TrainItemInfo> validTrainItemInfos = new List<TrainItemInfo>();
                    SeatType seatType = (SeatType)System.Enum.Parse(typeof(SeatType), seatTypeStr);
                    foreach (TrainItemInfo trainItemInfo in list)
                    {

                        if (trainItemInfo.GetSeatNumber(seatType) >= needSeatNumber)
                        {
                            validTrainItemInfos.Add(trainItemInfo);
                        }
                        else if (!string.IsNullOrEmpty(trainItemInfo.MmStr)) //cbForce.Checked &&是否强制下单
                        {
                            // Log(account, string.Format("{0}，对" + trainItemInfo.TrainNo + " 强制下单", seatTypeStr));
                            validTrainItemInfos.Add(trainItemInfo);
                        }

                    }
                    if (validTrainItemInfos.Count == 0) continue;//无该座位的车次，换其它座位

                    //todo:事件 播放音乐
                    OnAlarm(new ClientEventArgs("有可订的车次"));
                    Info(string.Format("{0}，找到" + validTrainItemInfos.Count + "趟列车", seatTypeStr));
                      List<TrainItemInfo> optimizeTrains = validTrainItemInfos.OrderBy(p => (int)p.TripTime.TotalHours).ThenByDescending(p => p.GetSeatNumber(seatType)).ToList(); //这么排序不是会买到临客？
                                Info(string.Format("{0}，最优顺序为：{1}", seatTypeStr, string.Join(",", optimizeTrains.Select(p => p.TrainNo))));
                    foreach (TrainItemInfo optimizeTrain in optimizeTrains)
                    {
                        Info(string.Format("{0}，选定：{1}下单", seatTypeStr, optimizeTrain.TrainNo));
                       
                        Passenger[] passengers =this.Passengers.Where(p=>p.Checked).ToArray();//.Where(p=>Account.Passengers.Contains(p.code)).ToArray();

                        Thread.Sleep(10000);//鼠标单击等待

                        SubmitOrderRequest:
                        //检查用户状态
                        Response<CheckUserResponse> response = await this.CheckUser();
                        if (!response.data.flag) //失败则再登录
                        {
                            Info( "需要重新登录");
                            LoginState = LoginState.UnLogin;
                            await Login();
                        }
                        //提交查询
                        var submitOrder =await this.SubmitOrderRequest(optimizeTrain.MmStr,_queryPageDynamicJsResult,Account);
                        if (!submitOrder.status)//提交不成功
                        {
                            Info(submitOrder.messages[0].ToString());
                            continue;
                        }

                        //跳转到订单页面
                        InitDcResult initDcResult = await this.GetInitDc();
                        //调用获取用户信息
                        Task<bool> refreshPassengers = this.RefreshPassengers(initDcResult.RepeatSubmitToken);

                        //获取提交的验证码
                        string randCode = await GetRandCode(1);

                       await refreshPassengers; //等待刷新乘客信息完成
                        if (!string.IsNullOrEmpty(initDcResult.LeftTicketStr)) //更新余票串
                        optimizeTrain.YpInfoDetailReal = initDcResult.LeftTicketStr;
                        if (string.IsNullOrEmpty(optimizeTrain.YpInfoDetailReal))
                        {
                            Info( "下单失败:未能获取真实的余票串信息");
                            continue;
                        }
                        //提交的座位
                       var seatTypev = seatType == SeatType.无座 ? SeatType.硬座 : seatType;
                       Task<Response<CheckOrderInfoResponse>> checkOrderInfoAsync = this.CheckOrderInfoAsync(passengers, seatTypev, randCode, initDcResult);
                        Response<GetQueueCountResponse> queueCount = await this.GetQueueCount(Account.TrainDate, optimizeTrain, initDcResult.LeftTicketStr,
                            initDcResult.RepeatSubmitToken, seatType);
                        var checkOrderInfo = await checkOrderInfoAsync;
                        if (!checkOrderInfo.status)
                        {
                            Info(checkOrderInfo.messages[0].ToString());

                        }
                        int seatNum = Utils.GetRealSeatNumber(queueCount.data.ticket, seatTypev);
                        int wuzuo = 0;
                        if (seatType == SeatType.硬座)
                            wuzuo = Utils.GetRealSeatNumber(queueCount.data.ticket, SeatType.无座);
                        Info(string.Format("{0},排队{1}人,{2} {3} 张 {4}", optimizeTrain.TrainNo, queueCount.data.countT, seatType, seatNum, wuzuo == 0 ? "" : ",无座 " + wuzuo + " 张"));

                        var resYpInfo = queueCount.data;
                        if (seatType == SeatType.硬座)//硬座要防止买到无座的票
                        {
                            if (seatNum - resYpInfo.countT < passengers.Length) //余票不足
                            {
                                Info(optimizeTrain.TrainNo + " ,硬座 余票不足！");
                                //if (cbShowRealYp.Checked) //查询余票开启
                                //{
                                //    Info(optimizeTrain.TrainNo + " ,持续检查余票开启");
                                //    Thread.Sleep(1000);
                                //    goto CheckOrderInfo;
                                //}
                                continue;
                            }

                        }
                        else
                        {
                            if (seatNum < passengers.Length) //小于座位数
                            {
                                Info(optimizeTrain.TrainNo + " ,余票不足！");

                                //todo:当余票不足是否下单？
                                continue;
                                //if (!cbForce.Checked) //强制下单
                                //{
                                //    if (cbShowRealYp.Checked) //持续检查余票
                                //    {
                                //        Log(account, optimizeTrain.TrainNo + " ,持续检查余票开启");
                                //        Thread.Sleep(1000);
                                //        goto CheckOrderInfo;
                                //    }
                                //    continue;
                                //}
                                //Log(account, optimizeTrain.TrainNo + " ,强制下单开启！");
                            }
                        }

                        //下单
                        var confirmSingleForQueue = await this.ConfirmSingleForQueue(passengers, seatTypev, randCode, initDcResult,
                            optimizeTrain.LocationCode);
                        if (!confirmSingleForQueue.status || ! confirmSingleForQueue.data.submitStatus) //下单成功
                        {
                            Info(confirmSingleForQueue.messages[0].ToString());
                            continue;
                        }
                        Info( optimizeTrain.TrainNo + "下单成功");

                        //检查等待时间
                        do
                        {
                            Response<QueryOrderWaitTimeResponse> waitTimeResponse =
                                await this.QueryOrderWaitTime(initDcResult.RepeatSubmitToken);

                            Thread.Sleep(waitTimeResponse.data.waitTime*1000);

                        } while (!_stop);

                    }

                }
            }

            if (!_stop)
            {
                goto QueryTickets;
            }



            this._stop = true;
            this.ExcuteState = ExcuteState.Stopped;
        }

        /// <summary>
        /// 停止执行
        /// </summary>
        public async void Stop()
        {
            this._stop = true;


        }

       

        #region login
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns>登录时否成功</returns>
        public async Task<bool> Login()
        {
            //todo:检查用户状态
            if (LoginState == LoginState.Login) return true;
            //打开登陆页面
            var loginPageResult = await this.GetLoginPageResult();

            var randCode = await GetRandCode();

            Thread.Sleep(5000); //单击等待
            Response<LoginAysnSuggestResponse> response =
                await
                    this.LoginAsynSuggest(this.Account.Username, this.Account.Password, randCode, loginPageResult.DynamicJsResult.Key, loginPageResult.DynamicJsResult.Value);
            if (response.data != null && response.data.loginCheck == "Y")
            {
                Info("登录成功");
                LoginState = LoginState.Login;
                //刷新乘客信息
                Task<bool> refreshPassengers = RefreshPassengers();
                return true;
            }
            else
            {
                Info(response.messages[0].ToString());
                LoginState = LoginState.UnLogin;
                return false;
            }
        }

        #endregion

        #region Passengers

        /// <summary>
        /// 刷新乘客信息
        /// </summary>
        /// <param name="submitToken"></param>
        /// <returns></returns>
        public async Task<bool> RefreshPassengers(string submitToken = "")
        {
            OpenQueryPage();
            Response<GetPassengerDTOs> response = await this.GetPassengers(submitToken);

            if (response.data != null)
            {
                bool findNewPassenger=false;
                foreach (PassengerInfo jsonInfo in response.data.normal_passengers)
                {
                    var find = Passengers.FirstOrDefault(p => p.CardNo == jsonInfo.passenger_id_no);
                    if (find == null)
                    {
                        Passengers.Add(new Passenger()
                        {
                            Name = jsonInfo.passenger_name,
                            CardNo = jsonInfo.passenger_id_no,
                            CardType = (CardType)jsonInfo.passenger_id_type_code[0],
                            MobileNo = jsonInfo.mobile_no,
                            SeatDetailType = SeatDetailType.随机,
                            Checked = false,
                            SeatType = SeatType.硬卧,
                            TicketType = (TicketType)int.Parse(jsonInfo.passenger_type)

                        });
                        findNewPassenger = true;
                    }
                }

                //this.Passengers = response.data.normal_passengers;
                //事件乘客发生变动
                if(findNewPassenger)
                OnPassengersChanged(new ClientEventArgs<List<Passenger>>(this.Passengers));
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion


        #region QueryTickets

        public async Task<bool> QueryLeftTicket()
        {
            OpenQueryPage();
            Response<QueryResponse> response = await this.QueryTrainInfos(Account.TrainDate, Account.FromStationTeleCode, Account.ToStationTeleCode,Account.TicketType);
            if (response.status)
            {
               this.TrainInfos=  ToTrainItemInfos(response.data, this.TrainInfos);
                //todo:事件
                return true;
            }
            return false;
        }

        /// <summary>
        /// 转换成列车信息
        /// </summary>
        /// <param name="response"></param>
        /// <param name="oldList"></param>
        /// <returns></returns>
        private static List<TrainItemInfo> ToTrainItemInfos(IEnumerable<Datum> response, List<TrainItemInfo> oldList)
        {

            List<TrainItemInfo> list = new List<TrainItemInfo>();

            foreach (var item in response)
            {
                var trainItem = item.queryLeftNewDTO;
                string trainNo = trainItem.station_train_code;
                TrainItemInfo itemInfo = oldList.Find(p => p.TrainNo == trainNo);
                if (itemInfo == null)
                    itemInfo = new TrainItemInfo();
                itemInfo.TrainNo = trainItem.station_train_code;
                itemInfo.StartStation = trainItem.start_station_name;
                itemInfo.EndStation = trainItem.end_station_name;
                itemInfo.lishi = trainItem.lishi;
                itemInfo.swz_num = trainItem.swz_num;
                itemInfo.tz_num = trainItem.tz_num;
                itemInfo.zy_num = trainItem.zy_num;
                itemInfo.ze_num = trainItem.ze_num;
                itemInfo.gr_num = trainItem.gr_num;
                itemInfo.rw_num = trainItem.rw_num; 
                itemInfo.yw_num = trainItem.yw_num;
                itemInfo.rz_num = trainItem.rz_num;
                itemInfo.yz_num = trainItem.yz_num;
                itemInfo.wz_num = trainItem.wz_num;
                itemInfo.qt_num = trainItem.qt_num;
                itemInfo.TrainNo4 = trainItem.train_no;
                itemInfo.TrainStartTime = trainItem.start_time;
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
        #endregion

        /// <summary>
        /// 打开查询页面
        /// </summary>
        private async void OpenQueryPage()
        {
            if (_queryPageDynamicJsResult == null || (DateTime.Now - _queryPageTime).TotalMinutes > 20)
            {
                var queryPageResult = await this.GetQueryPageResult();
                //查询准备 打开查询页 获取页面动态js结果
                _queryPageDynamicJsResult = queryPageResult.DynamicJsResult;
                _queryPageTime = DateTime.Now;
            }
        }


        #endregion


        #region Helpers

        /// <summary>
        /// 筛选符合条件的列车
        /// </summary>
        /// <param name="querylist"></param>
        /// <returns></returns>
        private List<TrainItemInfo> Filter(List<TrainItemInfo> querylist)
        {
            var result = new List<TrainItemInfo>();
           // var Account = this.Account;
            string trainClass = Account.TrainClass;
            string[] startTimeStrs = Account.StartTimeStr.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            TimeSpan startTime = TimeSpan.Parse(startTimeStrs[0]);
            TimeSpan endTime = TimeSpan.Parse(startTimeStrs[1]);

            foreach (var trainItemInfo in querylist)
            {
                string pc = trainItemInfo.TrainNo.Substring(0, 1);
                if (string.IsNullOrEmpty(trainClass) || trainClass.Contains(pc + ",") ||
                    (trainClass.Contains("QT,") && pc[0] >= '0' && pc[0] <= '9'))
                {
                    TimeSpan timeSpan = TimeSpan.Parse(trainItemInfo.TrainStartTime);
                    if (timeSpan >= startTime && timeSpan <= endTime)
                    {
                        result.Add(trainItemInfo);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取一个有效的验证码
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetRandCode(int type = 0)
        {
            string vcode = "";
            do
            {
                Image image = await InterfaceProvider.GetRandCode(this, type);
                var codeByForm = image.GetVCodeByForm();
                Thread.Sleep(5000); //等待5s 输入时间
                vcode = await codeByForm;
                if (_stop) return vcode;
                var checkRandCodeAnsynResponse = await this.CheckRandCodeAnsyn(vcode, type, "");

                if (checkRandCodeAnsynResponse.data != null && checkRandCodeAnsynResponse.data.result == "1") break;
                else
                {
                    if (checkRandCodeAnsynResponse.messages!=null&&checkRandCodeAnsynResponse.messages.Count>0)
                        Info(checkRandCodeAnsynResponse.messages[0].ToString());
                    Info("验证码不正确");
                }
            } while (true);
            return vcode;
        }

        private async void Info(string message)
        {
            OnClientChanged(new ClientEventArgs<string>(message));
        }



        #endregion
    }
}
