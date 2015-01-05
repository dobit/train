﻿
using System;
using System.Collections.Generic;

namespace LFNet.TrainTicket
{

    public class QueryOrderWaitTimeResponse
    {
        public bool queryOrderWaitTimeStatus { get; set; }
        public int count { get; set; }
        public int waitTime { get; set; }
        public long requestId { get; set; }
        public int waitCount { get; set; }
        public string tourFlag { get; set; }
        public object orderId { get; set; }
    }


}