using System;
using System.Xml.Serialization;
namespace LFNet.TrainTicket.Entity
{
    /// <summary>
    /// �û��˻���Ϣ
    /// </summary>
    [Serializable]
    public class AccountInfo
    {
        private DateTime _backTrainDate;
        private string _startTimeStr = "00:00--23:59";
        private DateTime _trainDate=DateTime.Now.Date;
        public string Username { get;  set; }
        public string Password { get;  set; }

        /// <summary>
        /// �ϳ�����
        /// </summary>
        public DateTime TrainDate
        {
            get { return _trainDate; }
            set { _trainDate = value; }
        }

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
        /// ��ʼվ��Ϣ
        /// </summary>
        [XmlIgnore]
        public StationInfo ToStationInfo
        {
            get { return new StationInfo() { Name = ToStation, Code = ToStationTeleCode }; }
            set
            {
                ToStation = value.Name;
                ToStationTeleCode = value.Code;
            }
        }
        /// <summary>
        /// ��ʼվ��Ϣ
        /// </summary>
         [XmlIgnore]
        public StationInfo FromStationInfo
        {
            get { return new StationInfo() {Name = FromStation, Code = FromStationTeleCode}; }
            set
            {
                FromStation = value.Name;
                FromStationTeleCode = value.Code;
            }
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
        public string StartTimeStr
        {
            get { return _startTimeStr; }
            set { _startTimeStr = value; }
        }

        /// <summary>
        /// ��ѡ�ĳ˿ͣ����ŷָ�����
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