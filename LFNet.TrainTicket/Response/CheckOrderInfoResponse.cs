
using System;
using System.Collections.Generic;

namespace LFNet.TrainTicket
{

    public class CheckOrderInfoResponse
    {
        public bool submitStatus { get; set; }
    }

    public class ValidateMessages
    {
        
    }

    public class Response<T>
    {
        public string validateMessagesShowId { get; set; }
        public bool status { get; set; }
        public int httpstatus { get; set; }
        public T data { get; set; }
        public IList<object> messages { get; set; }
        public ValidateMessages validateMessages { get; set; }
    }

    /// <summary>
    /// 检查用户返回消息
    /// </summary>
    public class CheckUserResponse
    {
        public  bool flag { get; set; }
    }
}
