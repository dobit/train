using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using LFNet.TrainTicket.Config;
using LFNet.TrainTicket.RqEntity;

namespace LFNet.TrainTicket
{
    /// <summary>
    /// 购票者
    /// </summary>
    public class TicketBuyer
    {
        public TicketSetting TicketSetting { get; set; }
        private Account account;
        private bool Stop;


        public TicketBuyer(TicketSetting ticketSetting)
        {
            TicketSetting = ticketSetting;
            account = new Account(ticketSetting.Username, ticketSetting.Password, ticketSetting.Proxy);
        }

        public List<TrainItemInfo> TrainItemInfos { get; set; }

        public void RefreshPassengers()
        {
            //检查状态
            CheckState();
            //登录
            Login();
            //查询有效的票信息

            //先获取乘客
            try
            {
                GetPassengerDTOs passengerJsonInfo = account.GetPassengers();
                foreach (NormalPassenger jsonInfo in passengerJsonInfo.normal_passengers)
                {
                    var find = TicketSetting.Passengers.Find(p => p.Name == jsonInfo.passenger_name);
                    if (find == null)
                    {
                        TicketSetting.Passengers.Add(new Passenger()
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
                    }
                }
                //passengersSetCtrl3.Invoke(new Action(()=>
                //    {
                //        passengersSetCtrl3.Reset();
                //        passengersSetCtrl3.Save();
                //    }
                //                              ));


            }
            catch (Exception ex)
            {
                Log(account, ex);
            }
        }

        /// <summary>
        /// 查询余票信息
        /// </summary>
        private List<TrainItemInfo> QueryTrainInfos()
        {
            TrainItemInfos = account.QueryTrainInfos(TrainItemInfos);
            return TrainItemInfos;
        }

        private void BuyTicketOne()
        {
            Log(account, "查询余票信息");
            var list = QueryTrainInfos(); //查询余票信息
            int cnt = list.Count;
            Log(account, "找到" + cnt + "趟列车");
            if(cnt==0) return; //没有车

            DisplayTrainInfos(list, account);// 打印余票信息

            foreach (string seatTypeStr in TicketSetting.SeatOrder.Split(',')) //座位顺序
            {
                SeatType seatType = (SeatType)System.Enum.Parse(typeof(SeatType), seatTypeStr);
                var validTrains = GetValidTrains(seatType);
                if(validTrains.Count==0) continue;
                Log(account, string.Format("{0}，找到 {1} 趟列车最优顺序为：{2}", seatTypeStr,validTrains.Count, string.Join(",", validTrains.Select(p => p.TrainNo))));
                foreach (TrainItemInfo trainItemInfo in validTrains)
                {
                    OrderTrain(trainItemInfo,seatType);
                }

            }

        }

        private void OrderTrain(TrainItemInfo optimizeTrain,SeatType seatType)
        {
            Log(account, string.Format("{0}，选定：{1}下单", seatType, optimizeTrain.TrainNo));
            Passenger[] passengers =
                TicketSetting.Passengers.Where(p => p.Checked).ToArray();
            SubmitOrderRequest:
            NameValueCollection htmlForm = account.SubmitOrderRequest(optimizeTrain, passengers
                                                                      , seatType == SeatType.无座
                                                                            ? SeatType.硬座
                                                                            : seatType);

            optimizeTrain.YpInfoDetailReal = htmlForm["leftTicketStr"]; //用于实时查询余票信息
            if (string.IsNullOrEmpty(optimizeTrain.YpInfoDetailReal))
            {
                Log(account, "下单失败:未能获取真实的余票串信息");
                return;
            }
            if (Stop) return;

            GetPassengerDTOs resPassengerJsonInfo = account.GetPassengers(); //获取乘客信息
            if (Stop) return;

            int vcodeCnt = 1;
            GETVCode:
            //获取验证码

            string vcode = GetVerifyCode(VCodeType.SubmitOrder, vcodeCnt);
            vcodeCnt = 0;
            if (vcode == "BREAK") return;

            // Thread.Sleep(4000);//验证码输入耗时 4s
            if (Stop) return;
            string postStr;
            var forms = BuildOrderPostStr(seatType, htmlForm, vcode, passengers, out postStr);


            CheckOrderInfo:
            var checkOrderInfoContent = account.CheckOrderInfo(vcode, postStr);
            if (checkOrderInfoContent.Contains("验证码"))
            {
                Log(account, "验证码不正确:" + checkOrderInfoContent);
                // vcode = "";
                goto GETVCode;
            }
            if (Stop) return;
           
            //查询队列 获取余票信息
            ResYpInfo resYpInfo = account.GetQueueCount(forms, seatType == SeatType.无座 ? SeatType.硬座 : seatType);
            Thread.Sleep(1000);//查看剩余余票 耗时1s
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
                    if (TicketSetting.ContinueCheckLeftTicketNum) //查询余票开启
                    {
                        Log(account, optimizeTrain.TrainNo + " ,持续检查余票开启");
                        Thread.Sleep(1000);
                        goto CheckOrderInfo;
                    }
                    return;
                }

            }
            else
            {
                if (seatNum < passengers.Length) //小于座位数
                {
                    Log(account, optimizeTrain.TrainNo + " ,余票不足！");
                    if (!TicketSetting.ForceTicket) //强制下单
                    {
                        if (TicketSetting.ContinueCheckLeftTicketNum) //持续检查余票
                        {
                            Log(account, optimizeTrain.TrainNo + " ,持续检查余票开启");
                            Thread.Sleep(1000);
                            goto CheckOrderInfo;
                        }
                        return;
                    }
                    Log(account, optimizeTrain.TrainNo + " ,强制下单开启！");
                }
            }


            if (Stop) return;
           
            //下订单
            string resStateContent = account.ConfirmSingleForQueue(postStr);
            if (resStateContent.Contains("验证码"))
            {
                Log(account, "下单失败，验证码不正确," + resStateContent);
                goto GETVCode;
               
            }
            ResState resState = resStateContent.ToJsonObject<ResState>();
            if (resState == null)
            {
                Common.LogUtil.Log(resStateContent);
                Log(account, optimizeTrain.TrainNo + ",下单失败:确认订单时，系统返回的内容不正确," + resStateContent);

            }
            else
            {
                if (resState.ErrMsg.Equals("Y"))
                {
                    Log(account, optimizeTrain.TrainNo + "下单成功");
                    WaitOrder(optimizeTrain,seatType);

                }
                else
                {
                    Log(account, "下单失败：" + resState.ErrMsg);
                    goto GETVCode;
                }
            }


        }

        public event EventHandler OrderSuccessed;

        private void WaitOrder(TrainItemInfo optimizeTrain,SeatType seatType )
        {
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
                            if (OrderSuccessed != null)
                            {
                                OrderSuccessed(this, null); //触发成功事件
                            }
                            return;
                        case -2:
                            Log(account, "占座失败，原因:" + waitResponse.Msg);
                            break;
                        case -3:
                            Log(account, "订单已撤销");
                            break;
                        default:
                            Log(account, "请到未完成订单页查看");
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

                        int flashWaitTime = (int) (waitResponse.WaitTime/1.5);
                        flashWaitTime = flashWaitTime > 60 ? 60 : flashWaitTime;
                        var nextTime = waitResponse.WaitTime - flashWaitTime;

                        nextRequestTime = nextTime <= 0 ? 1 : nextTime;
                    }
                    else
                    {
                        if(errCnt<3)
                        {
                            errCnt++;
                            continue;
                        }
                        else
                        {
                            dispTime = 1;
                        }
                        
                    }
                }

                TimeSpan timeSpan = TimeSpan.FromSeconds(dispTime);

               
                Log(account, "您的订单已经提交，最新预估等待时间" + timeSpan.ToString("hh小时mm分ss秒") + "。");
              
                dispTime--;
                Thread.Sleep(1000);
            } while (true);
        }

        /// <summary>
        /// 生成下单需要post的串
        /// </summary>
        /// <param name="seatType"></param>
        /// <param name="htmlForm"></param>
        /// <param name="vcode"></param>
        /// <param name="passengers"></param>
        /// <param name="postStr"></param>
        /// <returns></returns>
        private NameValueCollection BuildOrderPostStr(SeatType seatType, NameValueCollection htmlForm, string vcode,
                                                      Passenger[] passengers, out string postStr)
        {
            NameValueCollection forms = new NameValueCollection();
            forms["org.apache.struts.taglib.html.TOKEN"] = htmlForm["org.apache.struts.taglib.html.TOKEN"];
            forms["leftTicketStr"] = htmlForm["leftTicketStr"];
            foreach (string key in htmlForm.Keys)
            {
                if (key.StartsWith("orderRequest"))
                {
                    forms[key] = htmlForm[key];
                }
            }
            forms["randCode"] = vcode;
            forms["orderRequest.reserve_flag"] = TicketSetting.PayType == PayType.Online ? "A" : "B"; //A=在线支付,B=取票现场支付
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
            postStr = forms.ToQueryString();
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
            return forms;
        }


        public void BuyTicket()
        {
            //刷新乘客信息
            RefreshPassengers();
            while (!Stop)
            {
                BuyTicketOne();
                Thread.Sleep(TicketSetting.QuerySpan* 1000); //休息3秒
            }
        }

        /// <summary>
        /// 获取某个席位有效的列车，并排序
        /// </summary>
        /// <param name="seatType"></param>
        /// <returns></returns>
        private List<TrainItemInfo> GetValidTrains(SeatType seatType)
        {
            int needSeatNumber = TicketSetting.Passengers.Count(p => p.Checked);
            List<TrainItemInfo> validTrainItemInfos = new List<TrainItemInfo>();
            foreach (TrainItemInfo trainItemInfo in TrainItemInfos)
            {
                if (trainItemInfo.GetSeatNumber(seatType) >= needSeatNumber)
                {
                    validTrainItemInfos.Add(trainItemInfo);
                }
                else if (TicketSetting.ForceTicket && !string.IsNullOrEmpty(trainItemInfo.MmStr))
                {     
                    validTrainItemInfos.Add(trainItemInfo);
                }
            }
            List<TrainItemInfo> optimizeTrains = validTrainItemInfos.OrderBy(p => (int)p.TripTime.TotalHours).ThenByDescending(p => p.GetSeatNumber(seatType)).ToList(); //这么排序不是会买到临客？

            if (!string.IsNullOrEmpty(TicketSetting.TrainOrder.Trim()))
            {
                List<TrainItemInfo> orderTrains=new List<TrainItemInfo>();
                foreach (string trainNo in TicketSetting.TrainOrder.Split(new char[]{',','，','|'},StringSplitOptions.RemoveEmptyEntries))
                {
                    var train = optimizeTrains.Find(p => p.TrainNo.Trim().Equals(trainNo.Trim(),StringComparison.OrdinalIgnoreCase));
                    if(train!=null)
                    {
                        orderTrains.Add(train);
                    }
                }
                return orderTrains;
            }



            return optimizeTrains;
        }


        //public void Stop()
        //{
            
        //}

        private void CheckState()
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
        private bool Login()
        {
            if (account.IsLogin)
            {
                Log(account, "已经登陆，不需要登录");
                return true;
            }

            Log(account, "开始登陆");
            var i = 0;
            while (!Stop && i < 3)
            {
                try
                {
                    
                    account.Login();
                    LoginAysnSuggestInfo loginAysnSuggestInfo = account.LoginAysnSuggest();
                    if (loginAysnSuggestInfo != null && loginAysnSuggestInfo.RandError == "Y")
                    {
                        string vcode = GetVerifyCode(VCodeType.Login);
                        if (vcode == "BREAK")
                        {
                            Stop = true;
                            return false;
                        }
                        if (account.Login(loginAysnSuggestInfo, vcode)) //登陆成功
                        {
                            Log(account, "登陆成功");
                            return true;
                        }

                    }
                    else
                    {
                        Log(account, "登陆出错");
                    }

                }
                catch (Exception exception)
                {
                    Log(account, exception);
                }
                Thread.Sleep(100); //休息0.1秒
                i++;
            }
            Log(account, "登陆失败");
            return false;
        }


        private void Log(Account account, Exception ex)
        {
            Log(account, ex.Message + "\r\n\t" + ex.StackTrace);

        }

        private void Log(Account account, string message)
        {
            Trace.WriteLine(account + "," + System.DateTime.Now.ToString("hh:mm:ss:") + message);
        }


        private void DisplayTrainInfos(List<TrainItemInfo> list, Account account)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("======查询到列车信息====");
            sb.Append("车次\t历时\t商务座\t特等座\t一等座\t二等座\t高级软\t软卧\t硬卧\t软座\t硬座\t无座\t其他\r\n");


            foreach (TrainItemInfo trainItemInfo in list)
            {
                if (TicketSetting.ShowRealLeftTicketNum)
                {
                    trainItemInfo.ParseYpDetail();
                }
                sb.Append(trainItemInfo.ToStringWithNoStation());
                sb.AppendLine();
            }
            Log(account, sb.ToString());
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="vCodeType"></param>
        /// <param name="queryCnt">刷新几次后取验证码</param>
        /// <returns></returns>
        public string GetVerifyCode(VCodeType vCodeType,int queryCnt=0)
        {
            string vcode = "";
            for (int i = 0; i < queryCnt; i++)
            {
                account.GetVerifyCode(vCodeType);
            }
            while (!Stop)
            {
                try
                {
                    Image image = account.GetVerifyCode(vCodeType);
                    
                    if (TicketSetting.AutoVCodeType == AutoVCodeType.半自动 ||
                        TicketSetting.AutoVCodeType == AutoVCodeType.自动)
                    {
                        vcode = new Cracker().Read(new Bitmap(image)); //识别验证码
                        if (vcode.Length >= 4)
                        {
                            break;
                        }
                        Log(account,"验证码识别错误,重试");
                    }
                    if (TicketSetting.AutoVCodeType == AutoVCodeType.人工 ||
                        TicketSetting.AutoVCodeType == AutoVCodeType.半自动)
                    {
                        vcode = Account.GetVCodeByForm(image);
                    }
                }catch(Exception ex)
                {
                    Log(account,ex);
                }
            }
            return vcode;
        }
    }
}