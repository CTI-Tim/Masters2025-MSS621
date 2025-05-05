namespace Masters_2024_MSS_521.MessageSystem
{
    /// <summary>
    /// Class which defines what a "Message" is within the context of the "MessageBroker"
    /// </summary>
    public class Message
    {
        public bool Digital { get; set; }
        public ushort Analog { get; set; }
        public string Serial { get; set; }
    }
}