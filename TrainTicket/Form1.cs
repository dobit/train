using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AxWMPLib;
using LFNet.Configuration;
using LFNet.TrainTicket.Config;
using LFNet.TrainTicket.RqEntity;

namespace LFNet.TrainTicket
{
    public partial class Form1 : Form
    {
        private Thread t;
        public Form1()
        {
            InitializeComponent();
            //this.buyTicketConfigBindingSource.DataSource = Config.BuyTicketConfig.Instance;
            System.Net.ServicePointManager.Expect100Continue = false;
            if (Config.BuyTicketConfig.Instance.UserAccounts.Count == 0)
            {
                Config.BuyTicketConfig.Instance.UserAccounts.Add(new UserAccount());
            }
            this.userAccountBindingSource.DataSource = Config.BuyTicketConfig.Instance.UserAccounts[0];
            if (Config.BuyTicketConfig.Instance.OrderRequest.TrainDate < DateTime.Now)
            {
                Config.BuyTicketConfig.Instance.OrderRequest.TrainDate = DateTime.Now;
            }
            if (string.IsNullOrEmpty(Config.BuyTicketConfig.Instance.OrderRequest.TrainClass))
            {
                Config.BuyTicketConfig.Instance.OrderRequest.TrainClass = "QB#D#T#K#QT#";
            }
            if (string.IsNullOrEmpty(Config.BuyTicketConfig.Instance.OrderRequest.TrainPassType))
            {
                Config.BuyTicketConfig.Instance.OrderRequest.TrainPassType = "QB";
            }
            this.orderRequestBindingSource.DataSource = Config.BuyTicketConfig.Instance.OrderRequest;
            this.systemSettingBindingSource.DataSource = Config.BuyTicketConfig.Instance.SystemSetting;
            // passengersSetCtrl1.Text = "乘车人设置";
            //tbSeatTypes.Text

            axWindowsMediaPlayer1.PlayStateChange += axWindowsMediaPlayer1_PlayStateChange;
            cbAutoVCode.CheckedChanged += cbAutoVCode_CheckedChanged;
           System.IO.Stream  stream= this.GetType().Assembly.GetManifestResourceStream("LFNet.TrainTicket.music.PayMe.html");
            //System.Resources.ResourceManager resourceManager=new ResourceManager(this.GetType());
            //System.IO.Stream  stream=resourceManager.GetStream("LFNet.TrainTicket.music.PayMe.html");
            var fs = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory + "music/payme.html", FileMode.Create);
            stream.CopyTo(fs);
            stream.Close();
            fs.Close();
            webBrowser1.Navigate(System.AppDomain.CurrentDomain.BaseDirectory+"music/payme.html"); 
            
#if !DEBUG
            nudDelay.Enabled = false;
            nudDelay.Value = 5;
#endif
        }

        private bool autoVcode = false;
        void cbAutoVCode_CheckedChanged(object sender, EventArgs e)
        {
            autoVcode = cbAutoVCode.Checked;
        }

        private void PlayMusic()
        {
            try
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory + "music/";
                if (!System.IO.Directory.Exists(path))
                {
                    return;
                }
                string[] files = System.IO.Directory.GetFiles(path, "*.mp3");
                if (files.Length > 0)
                {
                    System.Random random = new Random();
                    int p = random.Next(0, files.Length - 1);

                    axWindowsMediaPlayer1.URL = files[p];
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                }
            }
            catch (Exception ex)
            {
                PlaySound();
            }
        }

        void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsMediaEnded)
            {
                PlayMusic();
            }
        }


        private void seatTypesCtrl1_ValueChanged(object sender, EventArgs e)
        {
            lbSeattypes.Text = seatTypesCtrl1.Value;
        }


        private void btnQuery_Click(object sender, EventArgs e)
        {

            Config.BuyTicketConfig.Instance.UserAccounts[0] = this.userAccountBindingSource.DataSource as UserAccount;
            Config.BuyTicketConfig.Instance.OrderRequest =
                this.orderRequestBindingSource.DataSource as OrderRequest;
            Config.BuyTicketConfig.Instance.SystemSetting = this.systemSettingBindingSource.DataSource as SystemSetting;
            Config.BuyTicketConfig.Save();
            if (string.IsNullOrEmpty(tbUsername.Text))
            {
                MessageBox.Show("用户名不能空");
                tbUsername.Focus();
                return;
            }
            if (string.IsNullOrEmpty(tbPassword.Text))
            {
                MessageBox.Show("密码不能空");
                tbPassword.Focus();
                return;
            }
            if (string.IsNullOrEmpty(Config.BuyTicketConfig.Instance.OrderRequest.FromStationTelecode) || string.IsNullOrEmpty(Config.BuyTicketConfig.Instance.OrderRequest.ToStationTelecode))
            {
                MessageBox.Show("出发站和目的站设置不正确");
                return;
            }
            if (string.IsNullOrEmpty(Config.BuyTicketConfig.Instance.OrderRequest.StartTimeStr))
            {
                MessageBox.Show("请选择出发时间");
                tbTime.Focus();
                return;
            }
            if (Config.BuyTicketConfig.Instance.Passengers.Count(p => p.Checked) == 0)
            {
                MessageBox.Show("请选择乘客");
                return;
            }
            if (string.IsNullOrEmpty(Config.BuyTicketConfig.Instance.SystemSetting.BuyTicketSeatOrder))
            {
                MessageBox.Show("请选择购买车票的席别顺序");
                return;
            }
            string passengerNames = string.Join(",",
                                                Config.BuyTicketConfig.Instance.Passengers.Where(p => p.Checked).Select(
                                                    p => p.Name).ToArray());
            string confirmText = string.Format("乘车人为：{0}\r\n车票优先顺序为:{1}", passengerNames, Config.BuyTicketConfig.Instance.SystemSetting.BuyTicketSeatOrder);
            if (MessageBox.Show(confirmText, "请确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Stop = false;
                btnQuery.Enabled = false;
                btnStop.Enabled = true;
                autoVcode = cbAutoVCode.Checked;
                t = new Thread(Run) { IsBackground = true };
                // t.ApartmentState = ApartmentState.STA;
                t.Start(AccountManager.GetAccount(tbUsername.Text, tbPassword.Text, ""));
            }
        }

        private bool Stop;
        public void Run(object accountObject)
        {

            Account account = accountObject as Account;
            // Account account = AccountManager.GetAccount("", "", ""); //new Account(Config.BuyTicketConfig.Instance.AccountInfo);
            
            //检查状态
            CheckState(account);
            //登录
            Login(account);
            //查询有效的票信息

            //先获取乘客
            try
            {
                GetPassengerDTOs passengerJsonInfo = account.GetPassengers();
                foreach (PassengerInfo jsonInfo in passengerJsonInfo.normal_passengers)
                {
                    var find = Config.BuyTicketConfig.Instance.Passengers.Find(p => p.Name == jsonInfo.passenger_name);
                    if (find == null)
                    {
                        Config.BuyTicketConfig.Instance.Passengers.Add(new Passenger()
                            {
                                Name = jsonInfo.passenger_name,
                                CardNo = jsonInfo.passenger_id_no,
                                CardType = (CardType) jsonInfo.passenger_id_type_code[0],
                                MobileNo = jsonInfo.mobile_no,
                                SeatDetailType = SeatDetailType.随机,
                                Checked = false,
                                SeatType = SeatType.硬卧,
                                TicketType = (TicketType) int.Parse(jsonInfo.passenger_type)

                            });
                    }
                }
                passengersSetCtrl3.Invoke(new Action(()=>
                    {
                        passengersSetCtrl3.Reset();
                        passengersSetCtrl3.Save();
                    }
                   ));
                
                
            }catch(Exception ex)
            {
                Log(account,ex);
            }

            List<TrainItemInfo> querylist = new List<TrainItemInfo>();
            while ( !Stop)
            {
                
                try
                {
                    ////获取验证码
                    //if (string.IsNullOrEmpty(vcode) || vcode == "BREAK")
                    //{
                    //    Log(account, "请输入下单时的验证码：");
                    //    vcode = account.GetVerifyCode(VCodeType.SubmitOrder, cbAutoVCode.Checked);
                    //    if (vcode != "BREAK")
                    //        Log(account, "下单验证码：" + vcode);
                    //}
                    Log(account, "查询余票信息");

                    querylist = account.QueryTrainInfos(querylist);
                    var list = Filter(querylist);
                    Log(account, "返回" + querylist.Count + "趟列车，筛选");
                    
                    if (list.Count > 0)
                    {

                        DisplayTrainInfos(list, account);// 打印余票信息

                        int needSeatNumber = Config.BuyTicketConfig.Instance.Passengers.Count(p => p.Checked);
                        foreach (string seatTypeStr in Config.BuyTicketConfig.Instance.SystemSetting.BuyTicketSeatOrder.Split(','))
                        {
                            if (Stop)
                            {
                                break;
                            }
                            List<TrainItemInfo> validTrainItemInfos = new List<TrainItemInfo>();
                            SeatType seatType = (SeatType)System.Enum.Parse(typeof(SeatType), seatTypeStr);
                            foreach (TrainItemInfo trainItemInfo in list)
                            {

                                if (trainItemInfo.GetSeatNumber(seatType) >= needSeatNumber)
                                {
                                    validTrainItemInfos.Add(trainItemInfo);
                                }
                                else if (cbForce.Checked && !string.IsNullOrEmpty(trainItemInfo.MmStr))
                                {
                                    // Log(account, string.Format("{0}，对" + trainItemInfo.TrainNo + " 强制下单", seatTypeStr));
                                    validTrainItemInfos.Add(trainItemInfo);
                                }
                            }
                            if (validTrainItemInfos.Count > 0)
                            {
                                //Thread.Sleep(2000);
                                if (axWindowsMediaPlayer1.playState != WMPLib.WMPPlayState.wmppsPlaying)
                                {
                                    if (axWindowsMediaPlayer1.InvokeRequired)
                                    {
                                        this.Invoke(new Action(PlayMusic));
                                    }
                                    else
                                    {
                                        PlayMusic();
                                    }
                                }
                                Log(account, string.Format("{0}，找到" + validTrainItemInfos.Count + "趟列车", seatTypeStr));

                                //  validTrainItemInfos.OrderByDescending(p => p.GetSeatNumber(seatType)).ThenBy(p => p.TripTime.TotalHours);
                                List<TrainItemInfo> optimizeTrains = validTrainItemInfos.OrderBy(p => (int)p.TripTime.TotalHours).ThenByDescending(p => p.GetSeatNumber(seatType)).ToList(); //这么排序不是会买到临客？
                                Log(account, string.Format("{0}，最优顺序为：{1}", seatTypeStr, string.Join(",", optimizeTrains.Select(p => p.TrainNo))));
                                foreach (TrainItemInfo optimizeTrain in optimizeTrains)
                                {
                                    if (Stop)
                                    {
                                        break;
                                    }
                                    Log(account, string.Format("{0}，选定：{1}下单", seatTypeStr, optimizeTrain.TrainNo));
                                    if (Stop)
                                    {
                                        break;
                                    }
                                    Passenger[] passengers =
                                        Config.BuyTicketConfig.Instance.Passengers.Where(p => p.Checked).ToArray();
                                    Thread.Sleep(10000);
                                    //检查用户状态
                                    if (!account.CheckUser()) //失败则再登录
                                    {
                                        Log(account,"需要重新登录");
                                        account.Login();
                                    }
                                    
                                SubmitOrderRequest:
                                    var htmlForm = account.SubmitOrderRequest(optimizeTrain, passengers
                                                                                              , seatType == SeatType.无座
                                                                                                  ? SeatType.硬座
                                                                                                  : seatType);

                                    optimizeTrain.YpInfoDetailReal = htmlForm["leftTicketStr"]; //用于实时查询余票信息
                                    if (string.IsNullOrEmpty(optimizeTrain.YpInfoDetailReal))
                                    {
                                        Log(account, "下单失败:未能获取真实的余票串信息");
                                        continue;
                                    }
                                    if (Stop)
                                    {
                                        break;
                                    }
                                    ////account.GetVerifyCode(VCodeType.SubmitOrder);
                                    //GetPassengerDTOs resPassengerJsonInfo = account.GetPassengers(); //获取乘客信息
                                    //if (Stop)
                                    //{
                                    //    break;
                                    //}

                                    int span = 10;// 10;
                                GETVCode:
                                    //获取验证码
                                    string vcode = "dobit";
                                    
                                        Stopwatch stopwatch = new Stopwatch();
                                        stopwatch.Start();
                                    do
                                    {
                                        vcode = account.GetOrderVCode();
                                        //.GetVerifyCode(VCodeType.SubmitOrder, ref autoVcode);
                                        if (vcode == "BREAK") break;
                                    } while (account.CheckRandCodeAnsyn(vcode, 1, htmlForm["REPEAT_SUBMIT_TOKEN"])); 

                                    if (vcode == "BREAK") continue;
                                    int st = (int) (span*1000 - stopwatch.ElapsedMilliseconds);
                                        if (st > 0)
                                            Thread.Sleep(st); //验证码输入耗时 4s
                                        span = 2;
                                        stopwatch.Stop();
                                  
                                    if (Stop)
                                    {
                                        break;
                                    }
                                    NameValueCollection forms = new NameValueCollection();
                                    forms["org.apache.struts.taglib.html.TOKEN"] = htmlForm["org.apache.struts.taglib.html.TOKEN"];
                                    forms["leftTicketStr"] = htmlForm["leftTicketStr"];
                                    forms["textfield"] = "中文或拼音首字母";
                                    forms["checkbox0"] = "0";
                                    forms["checkbox2"] = "2";
                                    foreach (string key in htmlForm.Keys)
                                    {
                                        if (key.StartsWith("orderRequest"))
                                        {
                                            forms[key] = htmlForm[key];
                                        }
                                    }
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
                                    int pIndex = 0;
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
                                                                                               (int)passenger.SeatDetailType,
                                                                                               (int)passenger.TicketType, passenger.Name,
                                                                                               passenger.CardType.ToCardTypeValue(),
                                                                                               passenger.CardNo, passenger.MobileNo));
                                            postStr += "&oldPassengers=" +
                                                       Common.HtmlUtil.UrlEncode(string.Format("{0},{1},{2}", passenger.Name,
                                                                                               passenger.CardType.ToCardTypeValue(),
                                                                                               passenger.CardNo));
                                            postStr +=
                                                string.Format(
                                                    "&passenger_{7}_seat={0}&passenger_{7}_ticket={1}&passenger_{7}_name={2}&passenger_{7}_cardtype={3}&passenger_{7}_cardno={4}&passenger_{7}_mobileno={5}&checkbox9={6}",
                                                    seatType.ToSeatTypeValue(), (int) passenger.TicketType,
                                                    Common.HtmlUtil.UrlEncode(passenger.Name),
                                                    passenger.CardType.ToCardTypeValue(), passenger.CardNo,
                                                    passenger.MobileNo, "Y",++pIndex);
                                        }
                                    }
                                    for(int i=pIndex;i<5;i++)
                                    {
                                        postStr += "&oldPassengers=&checkbox9=Y";
                                    }

                             
                                CheckOrderInfo:
                                    //
                                    var checkOrderInfoContent = account.CheckOrderInfo(vcode, postStr);
                                    if (checkOrderInfoContent.Contains("验证码"))
                                    {
                                        Log(account, "验证码不正确:" + checkOrderInfoContent);
                                       // vcode = "";
                                        goto GETVCode;
                                    }
                                    if (Stop)
                                    {
                                        break;
                                    }
                                    //查询队列 获取余票信息
                                    ResYpInfo resYpInfo = account.GetQueueCount(forms, seatType == SeatType.无座 ? SeatType.硬座 : seatType);
                                    
                                    int seatNum = Utils.GetRealSeatNumber(resYpInfo.Ticket, seatType);
                                    int wuzuo = 0;
                                    if (seatType == SeatType.硬座)
                                        wuzuo = Utils.GetRealSeatNumber(resYpInfo.Ticket, SeatType.无座);

                                    Log(account, string.Format("{0},排队{1}人,{2} {3} 张 {4}", optimizeTrain.TrainNo, resYpInfo.CountT, seatType, seatNum, wuzuo == 0 ? "" : ",无座 " + wuzuo + " 张"));

                                    if (seatType == SeatType.硬座)//硬座要防止买到无座的票
                                    {
                                        if (seatNum - resYpInfo.CountT < passengers.Length) //余票不足
                                        {
                                            Log(account, optimizeTrain.TrainNo + " ,硬座 余票不足！");
                                            if (cbShowRealYp.Checked) //查询余票开启
                                            {
                                                Log(account, optimizeTrain.TrainNo + " ,持续检查余票开启");
                                                Thread.Sleep(1000);
                                                goto CheckOrderInfo;
                                            }
                                            continue;
                                        }
                                        
                                    }
                                    else
                                    {
                                        if (seatNum < passengers.Length) //小于座位数
                                        {
                                            Log(account, optimizeTrain.TrainNo + " ,余票不足！");
                                            if (!cbForce.Checked) //强制下单
                                            {
                                                if (cbShowRealYp.Checked) //持续检查余票
                                                {
                                                    Log(account, optimizeTrain.TrainNo + " ,持续检查余票开启");
                                                    Thread.Sleep(1000);
                                                    goto CheckOrderInfo;
                                                }
                                                continue;
                                            }
                                            Log(account, optimizeTrain.TrainNo + " ,强制下单开启！");
                                        }
                                    }

                                    if (Stop)
                                    {
                                        break;
                                    }
                                    Thread.Sleep(1000);//查看剩余余票 耗时1s
                                    //下订单
                                    string resStateContent = account.ConfirmSingleForQueue(postStr);
                                    if (resStateContent.Contains("验证码"))
                                    {
                                        Log(account, "下单失败，验证码不正确," + resStateContent);
                                        goto GETVCode;
                                       // continue;
                                        //goto SubmitOrderRequest;
                                    }
                                    ResState resState = resStateContent.ToJsonObject<ResState>();

                                    if (resState == null)
                                    {
                                        Common.LogUtil.Log(resStateContent);
                                        Log(account, optimizeTrain.TrainNo + ",下单失败:确认订单时，系统返回的内容不正确," + resStateContent.Substring(0,10));

                                    }
                                    else
                                    {
                                        if (resState.ErrMsg.Equals("Y"))
                                        {
                                            Log(account, optimizeTrain.TrainNo + "下单成功");

                                            WaitResponse waitResponse = null;
                                            int dispTime = 1;
                                            int nextRequestTime = 1;
                                            int errCnt = 0;
                                            do
                                            {

                                                if (dispTime <= 0)
                                                {

                                                    switch (dispTime)
                                                    {
                                                        case -1:
                                                            string msg = string.Format("购买成功，订单号：{0}", waitResponse.OrderId);
                                                            Log(account, optimizeTrain.TrainNo + msg);
                                                        PlayMusic();
                                                        MessageBox.Show(msg);
                                                        Stop = true;
                                                        this.Invoke(
                                                            new Action(
                                                                () => btnOpenExplorer_Click(btnOpenExplorer, null)),
                                                            null);
                                                        goto End;
                                                        case -2:
                                                            Log(account, "占座失败，原因:" + waitResponse.Msg);
                                                            break;
                                                        case -3:
                                                            Log(account, "订单已撤销");
                                                            break;
                                                        default:
                                                            Log(account, "请到未完成订单页查看");
                                                            MessageBox.Show("请到未完成订单页查看");
                                                            this.Invoke(
                                                           new Action(
                                                               () => btnOpenExplorer_Click(btnOpenExplorer, null)),
                                                           null);
                                                            break;
                                                    }
                                                    return;
                                                }
                                                if (dispTime == nextRequestTime)
                                                {
                                                    string resContent;
                                                    waitResponse = account.GetWaitResponse(out resContent);
                                                    if (waitResponse != null)
                                                    {
                                                        dispTime = waitResponse.WaitTime;

                                                        int flashWaitTime = (int)(waitResponse.WaitTime / 1.5);
                                                        flashWaitTime = flashWaitTime > 60 ? 60 : flashWaitTime;
                                                        var nextTime = waitResponse.WaitTime - flashWaitTime;

                                                        nextRequestTime = nextTime <= 0 ? 1 : nextTime;
                                                        if (dispTime == 1) dispTime = 2;
                                                    }
                                                    else
                                                    {
                                                        if (errCnt < 3)
                                                        {
                                                            errCnt++;
                                                            continue;
                                                        }
                                                        else
                                                        {
                                                            Log(account, resContent);
                                                            dispTime = 2;
                                                        }

                                                    }
                                                }

                                                TimeSpan timeSpan = TimeSpan.FromSeconds(dispTime);


                                                Log(account, "您的订单已经提交，最新预估等待时间" + timeSpan.ToString("hh'小时'mm'分'ss'秒'") + "。");

                                                dispTime--;
                                                
                                                Thread.Sleep(1000);
                                            } while (true);

                                        }
                                        else
                                        {
                                            Common.LogUtil.Log(resStateContent);
                                            Log(account, "下单失败：" + resState.ErrMsg);
                                            //if (resStateContent.Contains("未能获取真实的余票串信息"))
                                            //{
                                            //    optimizeTrain.MmStr = "";
                                            //}
                                            //else if (resStateContent.Contains("验证码"))
                                            //{
                                            //    vcode = "";
                                            //    goto SubmitOrderRequest;
                                            //}
                                            //else
                                            //{
                                            //    if (cbForce.Checked)
                                            //    {
                                            //        Log(account, "由于用户强制下单，返回检查余票操作");
                                            //        goto SubmitOrderRequest;
                                            //    }

                                            //}

                                        }
                                    }


                                }

                            }
                        }
                        if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
                        {
                            if (this.InvokeRequired)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    axWindowsMediaPlayer1.Ctlcontrols.stop();
                                }));
                            }
                            else
                            {
                                axWindowsMediaPlayer1.Ctlcontrols.stop();
                            }
                        }
                    }

                }
                catch (Exception exception)
                {
                    Log(account, exception.Message);
                    Common.LogUtil.Log(exception);
                    // throw;
                    //检查状态
                    CheckState(account);
                    //登录
                    Login(account);

                }

                Thread.Sleep((int)nudDelay.Value * 1000); //休息3秒
            }


          
        End:
            //if (Stop)
            //{
            //    Log(account, "用户暂停操作");

            //}
           StopAll();

        }

        private List<TrainItemInfo> Filter(List<TrainItemInfo> querylist)
        {
            var result = new List<TrainItemInfo>();
            BuyTicketConfig buyTicketConfig = ConfigFileManager.GetConfig<BuyTicketConfig>();
            string trainClass =buyTicketConfig .OrderRequest.TrainClass;
            string[] startTimeStrs = buyTicketConfig.OrderRequest.StartTimeStr.Split(new []{'-'},StringSplitOptions.RemoveEmptyEntries);
            TimeSpan startTime = TimeSpan.Parse(startTimeStrs[0]);
            TimeSpan endTime = TimeSpan.Parse(startTimeStrs[1]);
           
            foreach (var trainItemInfo in querylist)
            {
                string pc = trainItemInfo.TrainNo.Substring(0, 1);
                if (string.IsNullOrEmpty(trainClass) || trainClass.Contains(pc + ",") ||
                    (trainClass.Contains("QT,") && pc[0] >= '0' && pc[0] <= '9'))
                {
                     TimeSpan timeSpan =TimeSpan.Parse(trainItemInfo.TrainStartTime);
                    if (timeSpan >= startTime && timeSpan <= endTime)
                    {
                        result.Add(trainItemInfo);
                    }
                }
            }
            return result;
        }
        

        private Mp3 soundPlayer;
        private void PlaySound()
        {

            string path = System.AppDomain.CurrentDomain.BaseDirectory + "music/";
            if (!System.IO.Directory.Exists(path))
            {
                return;
            }
            string[] files = System.IO.Directory.GetFiles(path, "*.mp3");
            if (files.Length > 0)
            {
                System.Random random = new Random();
                int p = random.Next(0, files.Length - 1);
                if (soundPlayer!=null)
                {
                    soundPlayer.StopT();
                   
                }
                soundPlayer = new Mp3();
                soundPlayer.FileName = files[p];
                soundPlayer.play();
                

            }


        }
        private void DisplayTrainInfos(List<TrainItemInfo> list, Account account)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("======查询到列车信息====");
            sb.Append("车次\t历时\t商务座\t特等座\t一等座\t二等座\t高级软\t软卧\t硬卧\t软座\t硬座\t无座\t其他\r\n");


            foreach (TrainItemInfo trainItemInfo in list)
            {
                if (cbShowRealYp.Checked)
                {
                    trainItemInfo.ParseYpDetail();
                }
                sb.Append(trainItemInfo.ToStringWithNoStation());
                sb.AppendLine();
            }
            Log(account, sb.ToString());
        }



        private void CheckState(Account account)
        {
            if (Stop) return;
            while (!Stop && account.CheckState() == State.Maintenance)
            {
                Log(account, "服务器维护中,10s重新检测");
                Thread.Sleep(10000);
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private void Login(Account account)
        {
            if (Stop) return;
            if (account.IsLogin)
            {
                Log(account, "已经登陆，不需要登录");
            }
            else
            {
                Log(account, "开始登陆");
                while (!Stop && !account.IsLogin)
                {
                    try
                    {
                        account.Login(autoVcode);
                    }
                    catch (Exception exception)
                    {
                        Log(account, exception);
                    }
                    Thread.Sleep(100); //休息0.1秒
                }
                if (account.IsLogin)
                    Log(account, "登陆成功");
            }
        }


        private void Log(Account account, Exception ex)
        {
            Log(account, ex.Message + "\r\n\t" + ex.StackTrace);

        }

        public void Log(Account account, string message)
        {
            if (lbPassengers.InvokeRequired)
            {
                lbPassengers.Invoke(new Action(() =>
                    {
                        var l = lbPassengers.Text.Length - 10000;
                        lbPassengers.Text = lbPassengers.Text.Substring(l > 0 ? l : 0) + account + "," + System.DateTime.Now.ToString("hh:mm:ss:") + message + "\r\n";
                        lbPassengers.SelectionStart = lbPassengers.TextLength;
                        lbPassengers.ScrollToCaret();

                    }));
            }
            else
            {
                var l = lbPassengers.Text.Length - 10000;
                lbPassengers.Text = lbPassengers.Text.Substring(l > 0 ? l : 0) + account + "," + System.DateTime.Now.ToString("hh:mm:ss:") + message + "\r\n";
                lbPassengers.SelectionStart = lbPassengers.TextLength;
                lbPassengers.ScrollToCaret();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            new Thread(StopAll) { IsBackground = true }.Start();

        }

        public void StopAll()
        {
            this.Stop = true;
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                        {
                            axWindowsMediaPlayer1.Ctlcontrols.stop();

                        }));
                }
                else
                {
                    axWindowsMediaPlayer1.Ctlcontrols.stop();

                }
            }
            if (soundPlayer != null)
            {
                soundPlayer.StopT();
                //soundPlayer.Dispose();
                soundPlayer = null;
            }

            if (t!=null&&System.Threading.Thread.CurrentThread != t)
            {
                int i = 0;
                while (i < 5000 && t.IsAlive)
                {
                    Application.DoEvents();
                    Thread.Sleep(i += 100);
                }
                if (t.IsAlive)
                {
                    t.Abort();
                }
            }
            this.Invoke(new Action(() =>
                {
                    btnQuery.Enabled = true;
                    btnStop.Enabled = false;
                }));
        }



        private void SetPassengers()
        {
            PassengersForm passengersForm = new PassengersForm();
            passengersForm.ShowDialog(this);
            //string passengers = string.Join(",", Config.BuyTicketConfig.Instance.Passengers.Where(p => p.Checked).Select(p => p.Name));
            //if (string.IsNullOrEmpty(passengers))
            //    lbPassengers.Text = "请设置购票人";
            //lbPassengers.Text = passengers;
            //passengersCtrl1.Reset();
        }

        private void btnOpenExplorer_Click(object sender, EventArgs e)
        {
            string url =
                "https://dynamic.12306.cn/otsweb/order/myOrderAction.do?method=queryMyOrderNotComplete&leftmenu=Y";
            string addtional = "Cookie: " + AccountManager.GetAccount(tbUsername.Text, tbPassword.Text, "").Cookie.GetCookieHeader(new Uri(url)) + Environment.NewLine;

            WebBrowser webBrowser = new WebBrowser();
            // webBrowser.Navigate("https://dynamic.12306.cn/otsweb/");

            //do
            //{
            //    Thread.Sleep(100);
            //} while (!(webBrowser.ReadyState == WebBrowserReadyState.Complete && webBrowser.IsBusy==false));

            //webBrowser.Document.Cookie =
            //    AccountManager.GetAccount(tbUsername.Text, tbPassword.Text, "").Cookie.GetCookieHeader(new Uri(url)) + ";path=/;";
            CookieCollection cookieCollection = AccountManager.GetAccount(tbUsername.Text, tbPassword.Text, "").Cookie.GetCookies(new Uri("https://dynamic.12306.cn/otsweb/"));
            foreach (Cookie cookie in cookieCollection)
            {
                InternetSetCookie("https://dynamic.12306.cn/otsweb/", cookie.Name, cookie.Value + ";expires=Sun,22-Feb-2099 00:00:00 GMT");
            }
            webBrowser.Navigate(url, true);//,  "_blank", null, addtional);
        }


        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);

    }
}
