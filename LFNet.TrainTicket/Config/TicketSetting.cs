using System.Collections.Generic;
using LFNet.TrainTicket.Config;

namespace LFNet.TrainTicket
{
    /// <summary>
    /// 购票设置
    /// </summary>
    public class TicketSetting
    {
        private List<Passenger> _passengers=new List<Passenger>();

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 代理 ip:端口
        /// </summary>
        public string Proxy { get; set; }
        /// <summary>
        /// 乘客信息
        /// </summary>
        public List<Passenger> Passengers
        {
            get { return _passengers; }
            set { _passengers = value; }
        }

        private OrderRequest _orderRequest = new OrderRequest();
        private int _querySpan = 3;

        /// <summary>
        /// 查询请求
        /// </summary>
        public OrderRequest OrderRequest
        {
            get { return _orderRequest; }
            set { _orderRequest = value; }
        }

       

        /// <summary>
        /// 强制下单
        /// </summary>
        public bool ForceTicket { get; set; }

        /// <summary>
        /// 显示真实的余票信息
        /// </summary>
        public bool ShowRealLeftTicketNum { get; set; }

        /// <summary>
        /// 下单时是否持续检查余票信息
        /// </summary>
        public bool ContinueCheckLeftTicketNum { get; set; }

        /// <summary>
        /// 验证码识别方式
        /// </summary>
        public AutoVCodeType AutoVCodeType { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public PayType PayType { get; set; }

        /// <summary>
        /// 席位优先级顺序
        /// </summary>
        public string SeatOrder { get; set; }

        /// <summary>
        /// 列车优先级顺序
        /// </summary>
        public string TrainOrder { get; set; }

        /// <summary>
        /// 查询间隔 s
        /// </summary>
        public int QuerySpan
        {
            get { return _querySpan; }
            set { _querySpan = value; }
        }
    }
}