using System;

namespace LFNet.TrainTicket
{
    /// <summary>
    /// �û��˻���Ϣ
    /// </summary>
    public class AccountInfo
    {
        private DateTime _backTrainDate;
        public string Username { get; private set; }
        public string Password { get; private set; }
        /// <summary>
        /// �ϳ�����
        /// </summary>
        public DateTime TrainDate { get; set; }

        /// <summary>
        /// ��������
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
        /// ����վ����
        /// </summary>
        public string FromStationTeleCode { get; set; }
        /// <summary>
        /// ��վ����
        /// </summary>
        public string ToStationTeleCode { get; set; }

        /// <summary>
        /// ��Ʊ����
        /// </summary>
        public int TicketType { get; set; }

        /// <summary>
        /// �г�����
        /// </summary>
        public string TrainClass { get; set; }

        /// <summary>
        /// ����ʱ��Χ
        /// </summary>
        public string StartTimeStr { get; set; }

        /// <summary>
        /// ��ѡ�ĳ˿ͣ����ŷָ����֤����
        /// </summary>
        public string Passengers { get; set; }
        /// <summary>
        /// ��λ��ѡ˳��
        /// </summary>
        public string SeatOrder { get; set; }

        public string FromStation { get; set; }
        public string ToStation { get; set; }
    }
}