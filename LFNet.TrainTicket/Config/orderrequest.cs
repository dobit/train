using System;
using System.Linq;
using LFNet.TrainTicket.Entity;

namespace LFNet.TrainTicket.Config
{
    /// <summary>
    /// 订单信息
    /// https://dynamic.12306.cn/otsweb/order/querySingleAction.do?method=queryLeftTicket&orderRequest.train_date=2012-10-10&orderRequest.from_station_telecode=BJP&orderRequest.to_station_telecode=NCG&orderRequest.train_no=&trainPassType=QB&trainClass=QB%23D%23Z%23T%23K%23QT%23&includeStudent=00&seatTypeAndNum=&orderRequest.start_time_str=00%3A00--24%3A00
    /// </summary>
    public class OrderRequest
    {
        private DateTime _backTrainDate;

        /// <summary>
        /// 日期
        /// </summary>
        public System.DateTime TrainDate { get; set; }

        public string FromStationTelecode { get; set; }
        public string FromStationTelecodeName
        {
            get
            {
                if (string.IsNullOrEmpty(FromStationTelecode)) return "";
                StationInfo stationInfo = Global.GetStations().FirstOrDefault(p => p.Code == FromStationTelecode);
                if (stationInfo != null)
                    return stationInfo.Name;
                return "";
            }
        }
        public string ToStationTelecode { get; set; }
        public string ToStationTelecodeName
        {
            get
            {
                if (string.IsNullOrEmpty(ToStationTelecode)) return "";
                StationInfo stationInfo = Global.GetStations().FirstOrDefault(p => p.Code == ToStationTelecode);
                if (stationInfo != null)
                    return stationInfo.Name;
                return "";
            }
        }
        public string TrainNo { get; set; }
        public string TrainPassType { get; set; }

        public string TrainClass { get; set; }


        public string IncludeStudent { get; set; }
        public string SeatTypeAndNum { get; set; }

        public string StartTimeStr { get; set; }

        //public DateTime BackTrainDate
        //{
        //    get
        //    {
        //        if(_backTrainDate<DateTime.Now)
        //        return _backTrainDate;
        //    }
        //    set { _backTrainDate = value; }
        //}
    }
}