namespace LFNet.TrainTicket
{
    public class TextValue
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public TextValue(string text,object value)
        {
            Text = text;
            Value = value;
        }
    }
}