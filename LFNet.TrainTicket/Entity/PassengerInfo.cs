namespace LFNet.TrainTicket.Entity
{
    public class PassengerInfo
    {
        public string code { get; set; }
        public string passenger_name { get; set; }
        /// <summary>
        /// 性别 M
        /// </summary>
        public string sex_code { get; set; }
        /// <summary>
        /// 性别男
        /// </summary>
        public string sex_name { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public string born_date { get; set; }
        /// <summary>
        /// 国家CN
        /// </summary>
        public string country_code { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public string passenger_id_type_code { get; set; }
        /// <summary>
        /// 类型名称
        /// </summary>
        public string passenger_id_type_name { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string passenger_id_no { get; set; }

        /// <summary>
        /// 乘客类型1
        /// </summary>
        public string passenger_type { get; set; }
        /// <summary>
        /// 标识0
        /// </summary>
        public string passenger_flag { get; set; }
        /// <summary>
        /// 成人
        /// </summary>
        public string passenger_type_name { get; set; }
        public string mobile_no { get; set; }
        public string phone_no { get; set; }
        /// <summary>
        /// Email地址
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string address { get; set; }
        public string postalcode { get; set; }
        /// <summary>
        /// 拼音首字母
        /// </summary>
        public string first_letter { get; set; }

        public string recordCount { get; set; }
        public string total_times { get; set; }
        public string index_id { get; set; }
    }
}