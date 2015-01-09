namespace LFNet.TrainTicket.BLL
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
    /// <summary>
    /// �ͻ����¼�
    /// </summary>
    public class ClientEventArgs<T> : System.EventArgs
    {


        public T State { get; private set; }

        public ClientEventArgs(T state)
        {
            this.State = state;
        }
    }
    public enum EventType
    {
        Info,
        warn,
        Error,
        
    }
}