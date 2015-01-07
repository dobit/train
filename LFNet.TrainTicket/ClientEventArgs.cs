namespace LFNet.TrainTicket
{
    /// <summary>
    /// 客户端事件
    /// </summary>
    public class ClientEventArgs : System.EventArgs
    {
        public string Message { get; set; }

        public ClientEventArgs(string message)
        {
            Message = message;
        }
    }
}