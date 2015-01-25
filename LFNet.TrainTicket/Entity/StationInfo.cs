namespace LFNet.TrainTicket.Entity
{
    public class StationInfo
    {
        public StationInfo()
        {
            
        }
        public StationInfo(string stationStr)
        {
            string[] str = stationStr.Split('|');
            PY = str[0];
            Name = str[1];
            Code = str[2];
            Id = str[3];

        }
        public string Id { get; set; }
        public string PY { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}