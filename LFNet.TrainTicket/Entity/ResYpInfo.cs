namespace LFNet.TrainTicket.Entity
{
    /// <summary>
    /// {"countT":0,"count":5,"ticket":111,"op_1":true,"op_2":false}
    /// </summary>
    public class ResYpInfo
    {
        /// <summary>
        /// 时间 排队人数
        /// </summary>
        public int CountT { get; set; }

        /// <summary>
        /// 计数 //今日已有多少人先于您提交相同的购票需求
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 票数
        /// </summary>
        public string Ticket { get; set; }
        /// <summary>
        /// 可以下单？
        /// </summary>
        public bool Op_1 { get; set; }

        /// <summary>
        /// 目前排队人数已经超过余票张数，请您选择其他席别或车次，特此提醒。";
        /// </summary>
        public bool Op_2 { get; set; }

    } 
}