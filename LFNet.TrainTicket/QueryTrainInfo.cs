using System.Collections.Generic;
using LFNet.TrainTicket.Config;

namespace LFNet.TrainTicket
{
    /// <summary>
    /// ≤È—Ø∆±–≈œ¢
    /// </summary>
    public class QueryTrainInfo : IQuery
    {
        public QueryTrainInfo()
        {

        }

        public bool Excute()
        {
            return true;
        }

        private List<TrainItemInfo> _trains;

        public List<TrainItemInfo> Trains
        {
            get { return _trains; }
            set { _trains = value; }
        }
    }
}