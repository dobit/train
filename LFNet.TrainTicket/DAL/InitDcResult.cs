namespace LFNet.TrainTicket.DAL
{
   public class InitDcResult:PageResult
    {
       public string RepeatSubmitToken { get; set; }
       public string KeyCheckIsChange { get; set; }
       public string LeftTicketStr { get; set; }
    }
}
