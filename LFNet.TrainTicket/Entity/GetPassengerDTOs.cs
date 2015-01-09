using System.Collections.Generic;

namespace LFNet.TrainTicket.Entity
{
    public class GetPassengerDTOs
    {
        public bool isExist { get; set; }
        public string exMsg { get; set; }
        public IList<string> two_isOpenClick { get; set; }
        public IList<string> other_isOpenClick { get; set; }
        public IList<PassengerInfo> normal_passengers { get; set; }
        public IList<object> dj_passengers { get; set; }
    }
}