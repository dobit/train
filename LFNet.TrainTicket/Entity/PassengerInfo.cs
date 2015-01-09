namespace LFNet.TrainTicket.Entity
{
    public class PassengerInfo
    {
        public string code { get; set; }
        public string passenger_name { get; set; }
        /// <summary>
        /// �Ա� M
        /// </summary>
        public string sex_code { get; set; }
        /// <summary>
        /// �Ա���
        /// </summary>
        public string sex_name { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public string born_date { get; set; }
        /// <summary>
        /// ����CN
        /// </summary>
        public string country_code { get; set; }
        /// <summary>
        /// ֤������
        /// </summary>
        public string passenger_id_type_code { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public string passenger_id_type_name { get; set; }
        /// <summary>
        /// ���֤����
        /// </summary>
        public string passenger_id_no { get; set; }

        /// <summary>
        /// �˿�����1
        /// </summary>
        public string passenger_type { get; set; }
        /// <summary>
        /// ��ʶ0
        /// </summary>
        public string passenger_flag { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string passenger_type_name { get; set; }
        public string mobile_no { get; set; }
        public string phone_no { get; set; }
        /// <summary>
        /// Email��ַ
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// ��ַ
        /// </summary>
        public string address { get; set; }
        public string postalcode { get; set; }
        /// <summary>
        /// ƴ������ĸ
        /// </summary>
        public string first_letter { get; set; }

        public string recordCount { get; set; }
        public string total_times { get; set; }
        public string index_id { get; set; }
    }
}