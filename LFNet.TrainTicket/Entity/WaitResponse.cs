namespace LFNet.TrainTicket.Entity
{
    /// <summary>
    /// 等待信息
    /// {"tourFlag":"dc","waitTime":5,"waitCount":1,"requestId":5661971198205498224,"count":0}
    /// {"tourFlag":"dc","waitTime":-1,"waitCount":0,"orderId":"E245228688","requestId":5661971198205498224,"count":0}
    /// ,"errorcode":"0","msg":"证件号码输入有误!","
    /// </summary>
   public class WaitResponse
    {
       public string TourFlag { get; set; }

       public int WaitTime { get; set; }

       public int WaitCount { get; set; }

       public string RequestId { get; set; }

       public int Count { get; set; }

       /// <summary>
       /// 最后会返回订单信息
       /// </summary>
       public string OrderId { get; set; }
       /// <summary>
       /// 错误编码
       /// </summary>
       public string ErrorCode { get; set; }
       /// <summary>
       /// 错误消息
       /// </summary>
       public string Msg { get; set; }
    }
}
