using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LFNet.TrainTicket
{
   public class InitDcResult:PageResult
    {
       public string RepeatSubmitToken { get; set; }
       public string KeyCheckIsChange { get; set; }
       public string LeftTicketStr { get; set; }
    }
}
