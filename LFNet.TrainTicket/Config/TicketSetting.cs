using System.Collections.Generic;
using LFNet.TrainTicket.Config;

namespace LFNet.TrainTicket
{
    /// <summary>
    /// ��Ʊ����
    /// </summary>
    public class TicketSetting
    {
        private List<Passenger> _passengers=new List<Passenger>();

        /// <summary>
        /// �û���
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// ���� ip:�˿�
        /// </summary>
        public string Proxy { get; set; }
        /// <summary>
        /// �˿���Ϣ
        /// </summary>
        public List<Passenger> Passengers
        {
            get { return _passengers; }
            set { _passengers = value; }
        }

        private OrderRequest _orderRequest = new OrderRequest();
        private int _querySpan = 3;

        /// <summary>
        /// ��ѯ����
        /// </summary>
        public OrderRequest OrderRequest
        {
            get { return _orderRequest; }
            set { _orderRequest = value; }
        }

       

        /// <summary>
        /// ǿ���µ�
        /// </summary>
        public bool ForceTicket { get; set; }

        /// <summary>
        /// ��ʾ��ʵ����Ʊ��Ϣ
        /// </summary>
        public bool ShowRealLeftTicketNum { get; set; }

        /// <summary>
        /// �µ�ʱ�Ƿ���������Ʊ��Ϣ
        /// </summary>
        public bool ContinueCheckLeftTicketNum { get; set; }

        /// <summary>
        /// ��֤��ʶ��ʽ
        /// </summary>
        public AutoVCodeType AutoVCodeType { get; set; }

        /// <summary>
        /// ֧����ʽ
        /// </summary>
        public PayType PayType { get; set; }

        /// <summary>
        /// ϯλ���ȼ�˳��
        /// </summary>
        public string SeatOrder { get; set; }

        /// <summary>
        /// �г����ȼ�˳��
        /// </summary>
        public string TrainOrder { get; set; }

        /// <summary>
        /// ��ѯ��� s
        /// </summary>
        public int QuerySpan
        {
            get { return _querySpan; }
            set { _querySpan = value; }
        }
    }
}