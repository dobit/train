using System.Collections.Generic;

namespace LFNet.TrainTicket.RqEntity
{
    /// <summary>
    /// {"countT":0,"count":5,"ticket":111,"op_1":true,"op_2":false}
    /// </summary>
    public class ResYpInfo
    {
        /// <summary>
        /// ʱ�� �Ŷ�����
        /// </summary>
        public int CountT { get; set; }

        /// <summary>
        /// ���� //�������ж������������ύ��ͬ�Ĺ�Ʊ����
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Ʊ��
        /// </summary>
        public string Ticket { get; set; }
        /// <summary>
        /// �����µ���
        /// </summary>
        public bool Op_1 { get; set; }

        /// <summary>
        /// Ŀǰ�Ŷ������Ѿ�������Ʊ����������ѡ������ϯ��򳵴Σ��ش����ѡ�";
        /// </summary>
        public bool Op_2 { get; set; }

    }

    //{"passengerJson":[{"first_letter":"XIAOYUAN800208","isUserSelf":"","mobile_no":"13556718373","old_passenger_id_no":"","old_passenger_id_type_code":"","old_passenger_name":"","passenger_flag":"0","passenger_id_no":"430523198910104211","passenger_id_type_code":"1","passenger_id_type_name":"","passenger_name":"Ԭ����","passenger_type":"1","passenger_type_name":"","recordCount":"11"},{"first_letter":"RB","isUserSelf":"","mobile_no":"13510549626","old_passenger_id_no":"","old_passenger_id_type_code":"","old_passenger_name":"","passenger_flag":"0","passenger_id_no":"500242198801275573","passenger_id_type_code":"1","passenger_id_type_name":"","passenger_name":"Ƚ��","passenger_type":"1","passenger_type_name":"","recordCount":"11"}]}
    public class GetPassengerDTOs
    {
        public bool isExist { get; set; }
        public string exMsg { get; set; }
        public IList<string> two_isOpenClick { get; set; }
        public IList<string> other_isOpenClick { get; set; }
        public IList<NormalPassenger> normal_passengers { get; set; }
        public IList<object> dj_passengers { get; set; }
    }

    public class NormalPassenger
    {
        public string code { get; set; }
        public string passenger_name { get; set; }
        public string sex_code { get; set; }
        public string sex_name { get; set; }
        public string born_date { get; set; }
        public string country_code { get; set; }
        public string passenger_id_type_code { get; set; }
        public string passenger_id_type_name { get; set; }
        public string passenger_id_no { get; set; }
        public string passenger_type { get; set; }
        public string passenger_flag { get; set; }
        public string passenger_type_name { get; set; }
        public string mobile_no { get; set; }
        public string phone_no { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string postalcode { get; set; }
        public string first_letter { get; set; }
        public string recordCount { get; set; }
        public string total_times { get; set; }
        public string index_id { get; set; }
    }
}