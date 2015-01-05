using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LFNet.TrainTicket.Request
{
    /// <summary>
    /// randCode=6eed&rand=sjrand&randCode_validate=
    /// </summary>
    public  class CheckRandCodeAnsynRequest
    {
        public string randCode { get; set; }
        public string rand { get; set; }

        public string randCode_validate { get; set; }
    }
}
