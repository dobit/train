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
        /// ��ѯ����
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
        /// �û��˻�
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
        /// �Զ�����
        /// </summary>
        public bool AutoBuy { get; set; }

        /// <summary>
        /// �Զ�ʶ����֤��
        /// </summary>
        public bool AutoVcode { get; set; }

        /// <summary>
        /// ����ʽ
        /// </summary>
        public BuyMode BuyMode { get; set; }

        /// <summary>
        /// ֧����ʽ
        /// </summary>
        public PayType PayType { get; set; }
        /// <summary>
        /// ʹ�ô���
        /// </summary>
        public bool UseProxy { get; set; }

        /// <summary>
        /// ��Ʊ˳��
        /// </summary>
        public string BuyTicketSeatOrder { get; set; }

        //private AccountInfo _accountInfo=new AccountInfo();
        //public AccountInfo AccountInfo
        //{
        //    get { return _accountInfo; }
        //    set { _accountInfo = value; }
        //}

        /// <summary>
        /// ��֤��ʶ�����
        /// </summary>
        public string VCodeAssemblyType { get; set; }
    }
    /// <summary>
    /// ����һ���û��˻�
    /// </summary>
    public class  UserAccount
    {
        public string Username { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public bool Enabled { get; set; }

    }
}