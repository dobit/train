namespace LFNet.TrainTicket
{
    /// <summary>
    /// �ͻ����¼�
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