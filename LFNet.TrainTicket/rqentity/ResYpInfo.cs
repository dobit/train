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
    public class ResPassengerJsonInfo
    {
        public PassengerJsonInfo[] PassengerJson { get; set; }
    }

    public class PassengerJsonInfo
    {
        public string first_letter { get; set; } // "XIAOYUAN800208";
        public string isUserSelf { get; set; } // "";
        public string mobile_no { get; set; } // "13556718373";
        public string old_passenger_id_no { get; set; } // "";
        public string old_passenger_id_type_code { get; set; } // "";
        public string old_passenger_name { get; set; } // "";
        public string passenger_flag { get; set; } // "0";
        public string passenger_id_no { get; set; } // "430523198910104211";
        public string passenger_id_type_code { get; set; } // "1";
        public string passenger_id_type_name { get; set; } // "";
        public string passenger_name { get; set; } // "Ԭ����";
        public string passenger_type { get; set; } // "1";
        public string passenger_type_name { get; set; } // "";
        public string recordCount { get; set; } // "11";
    }
}