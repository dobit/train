namespace LFNet.TrainTicket.Entity
{
    public class StationInfo
    {
        public StationInfo()
        {
            
        }
        /// <summary>
        /// @bjb|北京北|VAP|beijingbei|bjb|0
        /// </summary>
        /// <param name="stationStr"></param>
        public StationInfo(string stationStr)
        {
            string[] str = stationStr.Split('|');
            PY = str[0];
            Name = str[1];
            Code = str[2];
            PinYin = str[3];


            Id = str[5];

        }
        public string Id { get; set; }
        /// <summary>
        /// 简拼
        /// </summary>
        public string PY { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        /// <summary>
        /// 拼音
        /// </summary>
        public string PinYin { get; set; }
    }
}