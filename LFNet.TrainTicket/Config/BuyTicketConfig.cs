using System.Collections.Generic;
using LFNet.Configuration;
using LFNet.TrainTicket.Entity;

namespace LFNet.TrainTicket.Config
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public  class BuyTicketConfig:BaseConfig<BuyTicketConfig>
    {
        private OrderRequest _orderRequest = new OrderRequest();

        /// <summary>
        /// 查询请求
        /// </summary>
        public OrderRequest OrderRequest
        {
            get { return _orderRequest; }
            set { _orderRequest = value; }
        }

        public SystemSetting SystemSetting
        {
            get { return _systemSetting; }
            set { _systemSetting = value; }
        }


        private List<Passenger> _passengers=new List<Passenger>();
        private List<UserAccount> _userAccounts=new List<UserAccount>();
        private SystemSetting _systemSetting=new SystemSetting();

        public List<Passenger> Passengers
        {
            get { return _passengers; }
            set { _passengers = value; }
        }
        /// <summary>
        /// 用户账户
        /// </summary>
        public List<UserAccount> UserAccounts
        {
            get { return _userAccounts; }
            set { _userAccounts = value; }
        }
    }
    public class SystemSetting
    {
        /// <summary>
        /// 自动购买
        /// </summary>
        public bool AutoBuy { get; set; }

        /// <summary>
        /// 自动识别验证码
        /// </summary>
        public bool AutoVcode { get; set; }

        /// <summary>
        /// 购买方式
        /// </summary>
        public BuyMode BuyMode { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public PayType PayType { get; set; }
        /// <summary>
        /// 使用代理
        /// </summary>
        public bool UseProxy { get; set; }

        /// <summary>
        /// 购票顺序
        /// </summary>
        public string BuyTicketSeatOrder { get; set; }

        //private AccountInfo _accountInfo=new AccountInfo();
        //public AccountInfo AccountInfo
        //{
        //    get { return _accountInfo; }
        //    set { _accountInfo = value; }
        //}

        /// <summary>
        /// 验证码识别程序
        /// </summary>
        public string VCodeAssemblyType { get; set; }
    }
    /// <summary>
    /// 代表一个用户账户
    /// </summary>
    public class  UserAccount
    {
        public string Username { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// 启用
        /// </summary>
        public bool Enabled { get; set; }

    }
}