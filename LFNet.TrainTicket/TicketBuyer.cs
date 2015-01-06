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
    /// ��Ʊ��
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
            //���״̬
            CheckState();
            //��¼
            Login();
            //��ѯ��Ч��Ʊ��Ϣ

            //�Ȼ�ȡ�˿�
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
                                SeatDetailType = SeatDetailType.���,
                                Checked = false,
                                SeatType = SeatType.Ӳ��,
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
        /// ��ѯ��Ʊ��Ϣ
        /// </summary>
        private List<TrainItemInfo> QueryTrainInfos()
        {
            TrainItemInfos = account.QueryTrainInfos(TrainItemInfos);
            return TrainItemInfos;
        }

        private void BuyTicketOne()
        {
            Log(account, "��ѯ��Ʊ��Ϣ");
            var list = QueryTrainInfos(); //��ѯ��Ʊ��Ϣ
            int cnt = list.Count;
            Log(account, "�ҵ�" + cnt + "���г�");
            if(cnt==0) return; //û�г�

            DisplayTrainInfos(list, account);// ��ӡ��Ʊ��Ϣ

            foreach (string seatTypeStr in TicketSetting.SeatOrder.Split(',')) //��λ˳��
            {
                SeatType seatType = (SeatType)System.Enum.Parse(typeof(SeatType), seatTypeStr);
                var validTrains = GetValidTrains(seatType);
                if(validTrains.Count==0) continue;
                Log(account, string.Format("{0}���ҵ� {1} ���г�����˳��Ϊ��{2}", seatTypeStr,validTrains.Count, string.Join(",", validTrains.Select(p => p.TrainNo))));
                foreach (TrainItemInfo trainItemInfo in validTrains)
                {
                    OrderTrain(trainItemInfo,seatType);
                }

            }

        }

        private void OrderTrain(TrainItemInfo optimizeTrain,SeatType seatType)
        {
            Log(account, string.Format("{0}��ѡ����{1}�µ�", seatType, optimizeTrain.TrainNo));
            Passenger[] passengers =
                TicketSetting.Passengers.Where(p => p.Checked).ToArray();
            SubmitOrderRequest:
            NameValueCollection htmlForm = account.SubmitOrderRequest(optimizeTrain, passengers
                                                                      , seatType == SeatType.����
                                                                            ? SeatType.Ӳ��
                                                                            : seatType);

            optimizeTrain.YpInfoDetailReal = htmlForm["leftTicketStr"]; //����ʵʱ��ѯ��Ʊ��Ϣ
            if (string.IsNullOrEmpty(optimizeTrain.YpInfoDetailReal))
            {
                Log(account, "�µ�ʧ��:δ�ܻ�ȡ��ʵ����Ʊ����Ϣ");
                return;
            }
            if (Stop) return;

            GetPassengerDTOs resPassengerJsonInfo = account.GetPassengers(); //��ȡ�˿���Ϣ
            if (Stop) return;

            int vcodeCnt = 1;
            GETVCode:
            //��ȡ��֤��

            string vcode = GetVerifyCode(VCodeType.SubmitOrder, vcodeCnt);
            vcodeCnt = 0;
            if (vcode == "BREAK") return;

            // Thread.Sleep(4000);//��֤�������ʱ 4s
            if (Stop) return;
            string postStr;
            var forms = BuildOrderPostStr(seatType, htmlForm, vcode, passengers, out postStr);


            CheckOrderInfo:
            var checkOrderInfoContent = account.CheckOrderInfo(vcode, postStr);
            if (checkOrderInfoContent.Contains("��֤��"))
            {
                Log(account, "��֤�벻��ȷ:" + checkOrderInfoContent);
                // vcode = "";
                goto GETVCode;
            }
            if (Stop) return;
           
            //��ѯ���� ��ȡ��Ʊ��Ϣ
            ResYpInfo resYpInfo = account.GetQueueCount(forms, seatType == SeatType.���� ? SeatType.Ӳ�� : seatType);
            Thread.Sleep(1000);//�鿴ʣ����Ʊ ��ʱ1s
            int seatNum = Utils.GetRealSeatNumber(resYpInfo.Ticket, seatType);
            int wuzuo = 0;
            if (seatType == SeatType.Ӳ��)
                wuzuo = Utils.GetRealSeatNumber(resYpInfo.Ticket, SeatType.����);

            Log(account, string.Format("{0},�Ŷ�{1}��,{2} {3} �� {4}", optimizeTrain.TrainNo, resYpInfo.CountT, seatType, seatNum, wuzuo == 0 ? "" : ",���� " + wuzuo + " ��"));

            if (seatType == SeatType.Ӳ��)//Ӳ��Ҫ��ֹ��������Ʊ
            {
                if (seatNum - resYpInfo.CountT < passengers.Length) //��Ʊ����
                {
                    Log(account, optimizeTrain.TrainNo + " ,Ӳ�� ��Ʊ���㣡");
                    if (TicketSetting.ContinueCheckLeftTicketNum) //��ѯ��Ʊ����
                    {
                        Log(account, optimizeTrain.TrainNo + " ,���������Ʊ����");
                        Thread.Sleep(1000);
                        goto CheckOrderInfo;
                    }
                    return;
                }

            }
            else
            {
                if (seatNum < passengers.Length) //С����λ��
                {
                    Log(account, optimizeTrain.TrainNo + " ,��Ʊ���㣡");
                    if (!TicketSetting.ForceTicket) //ǿ���µ�
                    {
                        if (TicketSetting.ContinueCheckLeftTicketNum) //���������Ʊ
                        {
                            Log(account, optimizeTrain.TrainNo + " ,���������Ʊ����");
                            Thread.Sleep(1000);
                            goto CheckOrderInfo;
                        }
                        return;
                    }
                    Log(account, optimizeTrain.TrainNo + " ,ǿ���µ�������");
                }
            }


            if (Stop) return;
           
            //�¶���
            string resStateContent = account.ConfirmSingleForQueue(postStr);
            if (resStateContent.Contains("��֤��"))
            {
                Log(account, "�µ�ʧ�ܣ���֤�벻��ȷ," + resStateContent);
                goto GETVCode;
               
            }
            ResState resState = resStateContent.ToJsonObject<ResState>();
            if (resState == null)
            {
                Common.LogUtil.Log(resStateContent);
                Log(account, optimizeTrain.TrainNo + ",�µ�ʧ��:ȷ�϶���ʱ��ϵͳ���ص����ݲ���ȷ," + resStateContent);

            }
            else
            {
                if (resState.ErrMsg.Equals("Y"))
                {
                    Log(account, optimizeTrain.TrainNo + "�µ��ɹ�");
                    WaitOrder(optimizeTrain,seatType);

                }
                else
                {
                    Log(account, "�µ�ʧ�ܣ�" + resState.ErrMsg);
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
                            string msg = string.Format("����ɹ��������ţ�{0}", waitResponse.OrderId);
                            Log(account, optimizeTrain.TrainNo + msg);
                            if (OrderSuccessed != null)
                            {
                                OrderSuccessed(this, null); //�����ɹ��¼�
                            }
                            return;
                        case -2:
                            Log(account, "ռ��ʧ�ܣ�ԭ��:" + waitResponse.Msg);
                            break;
                        case -3:
                            Log(account, "�����ѳ���");
                            break;
                        default:
                            Log(account, "�뵽δ��ɶ���ҳ�鿴");
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

               
                Log(account, "���Ķ����Ѿ��ύ������Ԥ���ȴ�ʱ��" + timeSpan.ToString("hhСʱmm��ss��") + "��");
              
                dispTime--;
                Thread.Sleep(1000);
            } while (true);
        }

        /// <summary>
        /// �����µ���Ҫpost�Ĵ�
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
            forms["orderRequest.reserve_flag"] = TicketSetting.PayType == PayType.Online ? "A" : "B"; //A=����֧��,B=ȡƱ�ֳ�֧��
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
                 * passengerTickets=3,0,1,����,1,362201198...,15910675179,Y
                 * &oldPassengers=����,1,362201198...&passenger_1_seat=3&passenger_1_seat_detail_select=0&passenger_1_seat_detail=0&passenger_1_ticket=1&passenger_1_name=����&passenger_1_cardtype=1&passenger_1_cardno=362201198&passenger_1_mobileno=15910675179&checkbox9=Y
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
            //ˢ�³˿���Ϣ
            RefreshPassengers();
            while (!Stop)
            {
                BuyTicketOne();
                Thread.Sleep(TicketSetting.QuerySpan* 1000); //��Ϣ3��
            }
        }

        /// <summary>
        /// ��ȡĳ��ϯλ��Ч���г���������
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
            List<TrainItemInfo> optimizeTrains = validTrainItemInfos.OrderBy(p => (int)p.TripTime.TotalHours).ThenByDescending(p => p.GetSeatNumber(seatType)).ToList(); //��ô�����ǻ����ٿͣ�

            if (!string.IsNullOrEmpty(TicketSetting.TrainOrder.Trim()))
            {
                List<TrainItemInfo> orderTrains=new List<TrainItemInfo>();
                foreach (string trainNo in TicketSetting.TrainOrder.Split(new char[]{',','��','|'},StringSplitOptions.RemoveEmptyEntries))
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
                Log(account, "������ά����,10s���¼��");
                Thread.Sleep(10000);
            }
        }

        /// <summary>
        /// ��¼
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private bool Login()
        {
            if (account.IsLogin)
            {
                Log(account, "�Ѿ���½������Ҫ��¼");
                return true;
            }

            Log(account, "��ʼ��½");
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
                        if (account.Login(loginAysnSuggestInfo, vcode)) //��½�ɹ�
                        {
                            Log(account, "��½�ɹ�");
                            return true;
                        }

                    }
                    else
                    {
                        Log(account, "��½����");
                    }

                }
                catch (Exception exception)
                {
                    Log(account, exception);
                }
                Thread.Sleep(100); //��Ϣ0.1��
                i++;
            }
            Log(account, "��½ʧ��");
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
            sb.AppendLine("======��ѯ���г���Ϣ====");
            sb.Append("����\t��ʱ\t������\t�ص���\tһ����\t������\t�߼���\t����\tӲ��\t����\tӲ��\t����\t����\r\n");


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
        /// ��ȡ��֤��
        /// </summary>
        /// <param name="vCodeType"></param>
        /// <param name="queryCnt">ˢ�¼��κ�ȡ��֤��</param>
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
                    
                    if (TicketSetting.AutoVCodeType == AutoVCodeType.���Զ� ||
                        TicketSetting.AutoVCodeType == AutoVCodeType.�Զ�)
                    {
                        vcode = new Cracker().Read(new Bitmap(image)); //ʶ����֤��
                        if (vcode.Length >= 4)
                        {
                            break;
                        }
                        Log(account,"��֤��ʶ�����,����");
                    }
                    if (TicketSetting.AutoVCodeType == AutoVCodeType.�˹� ||
                        TicketSetting.AutoVCodeType == AutoVCodeType.���Զ�)
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