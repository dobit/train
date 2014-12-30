using System;
using System.ComponentModel;

namespace LFNet.TrainTicket.Config
{
    public class TrainItemInfo
    {
        private string _ypInfoDetail;

        [DisplayName("���")]
        public string No { get; set; }
        [DisplayName("����")]
        public string TrainNo { get; set; }

        [DisplayName("��վ")]
        public string StartStation { get; set; }

        [DisplayName("��վ")]
        public string EndStation { get; set; }

        [DisplayName("��ʱ")]
        public string lishi { get; set; }

        [DisplayName("������")]
        public string ShangwuSeat { get; set; }

        [DisplayName("�ص���")]
        public string TedengSeat { get; set; }

        [DisplayName("һ����")]
        public string YidengSeat { get; set; }

        [DisplayName("������")]
        public string ErdengSeat { get; set; }

        [DisplayName("�߼�����")]
        public string GaojiRuanwoSeat { get; set; }

        [DisplayName("����")]
        public string RuanwoSeat { get; set; }

        [DisplayName("Ӳ��")]
        public string YingwoSeat { get; set; }

        [DisplayName("����")]
        public string RuanzuoSeat { get; set; }

        [DisplayName("Ӳ��")]
        public string YingzuoSeat { get; set; }

        [DisplayName("����")]
        public string WuzuoSeat { get; set; }

        [DisplayName("����")]
        public string OtherSeat { get; set; }

       
        public string Tag { get; set; }

        public string TrainStartTime { get; set; }

        public string TrainNo4 { get; set; }

        public string ArriveTime { get; set; }

        public string FromStationName { get; set; }

        public string ToStationName { get; set; }

        public string YpInfoDetail
        {
            get; set; //get { return _ypInfoDetail; }
            //set { _ypInfoDetail = value;
            //    try
            //    {
            //        ParseYpDetail();
            //    }catch(Exception ex)
            //    {
            //        Common.LogUtil.Log(ex);
            //    }
            //}
        }

        public string MmStr { get; set; }

        public string FromStationTelecode { get; set; }

        public string ToStationTelecode { get; set; }

        /// <summary>
        /// �Ƿ���Թ���
        /// </summary>
        public bool CanBuy { get { return !string.IsNullOrEmpty(MmStr); } }

        /// <summary>
        /// ��ʵ����Ʊ��Ϣ
        /// </summary>
        public string YpInfoDetailReal { get; set; }

        public string ToStringWithNoStation()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}", TrainNo, lishi, ShangwuSeat, TedengSeat, YidengSeat,
                                 ErdengSeat, GaojiRuanwoSeat, RuanwoSeat, YingwoSeat, RuanzuoSeat, YingzuoSeat,
                                 WuzuoSeat, OtherSeat);
        }
      
        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}", TrainNo, StartStation, EndStation, lishi, ShangwuSeat, TedengSeat, YidengSeat,
                                 ErdengSeat, GaojiRuanwoSeat, RuanwoSeat, YingwoSeat, RuanzuoSeat, YingzuoSeat,
                                 WuzuoSeat, OtherSeat);
        }

        /// <summary>
        /// ������λ���ͻ�ȡƱ��
        /// </summary>
        /// <param name="seatTypeStr"></param>
        /// <returns></returns>
        public int GetSeatNumber(SeatType seatType)
        {
            
            string numStr = "";
            switch (seatType)
            {
                case SeatType.Ӳ��:
                    numStr = YingzuoSeat;
                    break;
                case SeatType.����:
                    numStr = RuanzuoSeat;
                    break;
                case SeatType.Ӳ��:
                    numStr = YingwoSeat;
                    break;
                case SeatType.����:
                    numStr = RuanwoSeat;
                    break;
                case SeatType.�߼�����:
                    numStr = GaojiRuanwoSeat;
                    break;
                case SeatType.������:
                    numStr = ShangwuSeat;
                    break;
                case SeatType.һ����:
                    numStr = YidengSeat;
                    break;
                case SeatType.������:
                    numStr = ErdengSeat;
                    break;
                case SeatType.�ص���:
                    numStr = TedengSeat;
                    break;
                case SeatType.�۹���:
                    return 0;
                    break;
                case SeatType.һ�Ȱ���:
                    return 0;
                    break;
                default:
                    numStr = WuzuoSeat;
                    //return 0;
                    break;
            }

            if(numStr=="��")
            {
                return int.MaxValue;
            }
            //else if(numStr=="*")
            //    {
            //        return int.MaxValue;
            //    }
            else
            {
                int num = 0;
                int.TryParse(numStr, out num);
                return num;
            }
        }

        /// <summary>
        /// ��ʱ
        /// </summary>
        public TimeSpan TripTime
        {
            get
            {
                if(string.IsNullOrEmpty(lishi)) return TimeSpan.MaxValue;
                else
                {
                    try
                    {
                        string[] lishis = lishi.Split(':');
                        return new TimeSpan(int.Parse(lishis[0].Trim()),int.Parse(lishis[1].Trim()),0);
                        //return TimeSpan.Parse(lishi);
                    }
                    catch (Exception ex)
                    {
                        Common.LogUtil.Log(ex);
                        Common.LogUtil.Log(lishi);
                        return new TimeSpan(1,0,1);
                    }
                    
                }
            }
        }

        public string LocationCode { get; set; }

        public string FromStationNo { get; set; }

        public string ToStationNo { get; set; }

        /// <summary>
        /// ����Ʊ��Ϣ�����ɾ������λ��
        /// </summary>
        public void ParseYpDetail()
        {
            if(string.IsNullOrEmpty(YpInfoDetail)) return;
            int i = 0;
            while (i < YpInfoDetail.Length)
            {
                string s = YpInfoDetail.Substring(i, 10);

                string numStr = s.Substring(6, 4);
                int count = int.Parse(numStr);
                numStr = numStr.TrimStart('0');
                if (count < 3000&&count>0)
                {
                    SeatType c_seat = (SeatType) s[0]; //.Substring(0, 1);

                    switch (c_seat)
                    {
                        case SeatType.Ӳ��:
                            YingzuoSeat = numStr;
                            break;
                        case SeatType.����:
                            RuanzuoSeat = numStr;
                            break;
                        case SeatType.Ӳ��:
                            YingwoSeat = numStr;
                            break;
                        case SeatType.����:
                            RuanwoSeat = numStr;
                            break;
                        case SeatType.�߼�����:
                            GaojiRuanwoSeat = numStr;
                            break;
                        case SeatType.������:
                            ShangwuSeat = numStr;
                            break;
                        case SeatType.һ����:
                            YidengSeat = numStr;
                            break;
                        case SeatType.������:
                            ErdengSeat = numStr;
                            break;
                        case SeatType.�ص���:
                            TedengSeat = numStr;
                            break;
                        case SeatType.�۹���:

                            break;
                        case SeatType.һ�Ȱ���:
                            break;

                        default:
                            break;
                    }
                }
                else if(count>3000)
                {
                    count -= 3000;
                    WuzuoSeat = count.ToString();
                }
                i += 10;
            }
        }

        /// <summary>
        /// ��ȡ��λ����Ʊ��Ϣ
        /// </summary>
        /// <param name="seatType"></param>
        /// <returns></returns>
        public int GetRealSeatNumber(SeatType seatType)
        {
            int i = 0;
            int wz = 0;
            while (i < YpInfoDetail.Length)
            {
                string s = YpInfoDetail.Substring(i, 10);
                SeatType c_seat = (SeatType)s[0];
                string numStr = s.Substring(6, 4);
                int count = int.Parse(numStr);
                if (count > 3000)
                {
                    count -= 3000;
                    wz = count ;
                }
                else
                {
                    if(seatType==c_seat) return count;
                }
                i += 10; 
            }
            if(seatType==SeatType.����) return wz;
            else
            {
                return 0;
            }
           
        }
    }
}