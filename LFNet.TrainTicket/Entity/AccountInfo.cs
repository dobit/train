using System;
using System.Xml.Serialization;
namespace LFNet.TrainTicket.Entity
{
    /// <summary>
    /// 用户账户信息
    /// </summary>
    [Serializable]
    public class AccountInfo
    {
        private DateTime _backTrainDate;
        public string Username { get;  set; }
        public string Password { get;  set; }
        /// <summary>
        /// 上车日期
        /// </summary>
        public DateTime TrainDate { get; set; }

        /// <summary>
        /// 返程日期
        /// </summary>
        public DateTime BackTrainDate
        {
            get
            {
                if (_backTrainDate <= DateTime.Now)
                {
                    return TrainDate;
                }
                return _backTrainDate;
            }
            set { _backTrainDate = value; }
        }

        /// <summary>
        /// 出发站编码
        /// </summary>
        public string FromStationTeleCode { get; set; }
        /// <summary>
        /// 到站编码
        /// </summary>
        public string ToStationTeleCode { get; set; }

        /// <summary>
        /// 车票类型
        /// </summary>
        public int TicketType { get; set; }

        /// <summary>
        /// 列车类型
        /// </summary>
        public string TrainClass { get; set; }

        /// <summary>
        /// 开车时范围
        /// </summary>
        public string StartTimeStr { get; set; }

        /// <summary>
        /// 勾选的乘客，逗号分隔身份证号码
        /// </summary>
        public string Passengers { get; set; }
        /// <summary>
        /// 座位优选顺序
        /// </summary>
        public string SeatOrder { get; set; }

        public string FromStation { get; set; }
        public string ToStation { get; set; }
    }
}