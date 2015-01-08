
using System;
using System.Collections.Generic;

namespace LFNet.TrainTicket
{

    public class GetQueueCountResponse
    {
        /// <summary>
        /// 余票？
        /// </summary>
        public string count { get; set; }
        /// <summary>
        /// 余票串
        /// </summary>
        public string ticket { get; set; }

        /// <summary>
        /// 未知
        /// </summary>
        public string op_2 { get; set; }
        /// <summary>
        /// 前面排队人数
        /// </summary>
        public int countT { get; set; }

        /// <summary>
        /// 可下单？
        /// </summary>
        public string op_1 { get; set; }
    }

}
