using System;
using System.ComponentModel;
using LFNet.Common;
using LFNet.TrainTicket.Entity;

namespace LFNet.TrainTicket.Config
{
    public class TrainItemInfo
    {
        private string _ypInfoDetail;

        [DisplayName("编号")]
        public string No { get; set; }
        [DisplayName("车次")]
        public string TrainNo { get; set; }

        [DisplayName("发站")]
        public string StartStation { get; set; }

        [DisplayName("到站")]
        public string EndStation { get; set; }

        [DisplayName("历时")]
        public string lishi { get; set; }

        /// <summary>
        /// 商务座
        /// </summary>
        [DisplayName("商务座")]
        public string swz_num { get; set; }

        [DisplayName("特等座")]
        public string tz_num { get; set; }

        [DisplayName("一等座")]
        public string zy_num { get; set; }

        [DisplayName("二等座")]
        public string ze_num { get; set; }

        [DisplayName("高级软卧")]
        public string gr_num { get; set; }

        [DisplayName("软卧")]
        public string rw_num { get; set; }

        [DisplayName("硬卧")]
        public string yw_num { get; set; }

        [DisplayName("软座")]
        public string rz_num { get; set; }

        [DisplayName("硬座")]
        public string yz_num { get; set; }

        [DisplayName("无座")]
        public string wz_num { get; set; }

        [DisplayName("其他")]
        public string qt_num { get; set; }

       
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
        /// 是否可以购买
        /// </summary>
        public bool CanBuy { get { return !string.IsNullOrEmpty(MmStr); } }

        /// <summary>
        /// 真实的余票信息
        /// </summary>
        public string YpInfoDetailReal { get; set; }

        public string ToStringWithNoStation()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}", TrainNo, lishi, swz_num, tz_num, zy_num,
                                 ze_num, gr_num, rw_num, yw_num, rz_num, yz_num,
                                 wz_num, qt_num);
        }
      
        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}", TrainNo, StartStation, EndStation, lishi, swz_num, tz_num, zy_num,
                                 ze_num, gr_num, rw_num, yw_num, rz_num, yz_num,
                                 wz_num, qt_num);
        }

        /// <summary>
        /// 根据坐位类型获取票数
        /// </summary>
        /// <param name="seatTypeStr"></param>
        /// <returns></returns>
        public int GetSeatNumber(SeatType seatType)
        {
            
            string numStr = "";
            switch (seatType)
            {
                case SeatType.硬座:
                    numStr = yz_num;
                    break;
                case SeatType.软座:
                    numStr = rz_num;
                    break;
                case SeatType.硬卧:
                    numStr = yw_num;
                    break;
                case SeatType.软卧:
                    numStr = rw_num;
                    break;
                case SeatType.高级软卧:
                    numStr = gr_num;
                    break;
                case SeatType.商务座:
                    numStr = swz_num;
                    break;
                case SeatType.一等座:
                    numStr = zy_num;
                    break;
                case SeatType.二等座:
                    numStr = ze_num;
                    break;
                case SeatType.特等座:
                    numStr = tz_num;
                    break;
                case SeatType.观光座:
                    return 0;
                    break;
                case SeatType.一等包座:
                    return 0;
                    break;
                default:
                    numStr = wz_num;
                    //return 0;
                    break;
            }

            if(numStr=="有")
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
        /// 历时
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
                        LogUtil.Log(ex);
                        LogUtil.Log(lishi);
                        return new TimeSpan(1,0,1);
                    }
                    
                }
            }
        }

        public string LocationCode { get; set; }

        public string FromStationNo { get; set; }

        public string ToStationNo { get; set; }

        /// <summary>
        /// 将余票信息解析成具体的座位数
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
                        case SeatType.硬座:
                            yz_num = numStr;
                            break;
                        case SeatType.软座:
                            rz_num = numStr;
                            break;
                        case SeatType.硬卧:
                            yw_num = numStr;
                            break;
                        case SeatType.软卧:
                            rw_num = numStr;
                            break;
                        case SeatType.高级软卧:
                            gr_num = numStr;
                            break;
                        case SeatType.商务座:
                            swz_num = numStr;
                            break;
                        case SeatType.一等座:
                            zy_num = numStr;
                            break;
                        case SeatType.二等座:
                            ze_num = numStr;
                            break;
                        case SeatType.特等座:
                            tz_num = numStr;
                            break;
                        case SeatType.观光座:

                            break;
                        case SeatType.一等包座:
                            break;

                        default:
                            break;
                    }
                }
                else if(count>3000)
                {
                    count -= 3000;
                    wz_num = count.ToString();
                }
                i += 10;
            }
        }

        /// <summary>
        /// 获取座位的余票信息
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
            if(seatType==SeatType.无座) return wz;
            else
            {
                return 0;
            }
           
        }
    }
}