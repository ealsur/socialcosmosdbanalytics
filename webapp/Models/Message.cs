namespace cosmoschat.Models
{
    /// <summary>
    /// User's message
    /// </summary>
    public class MessagePayload
    {
        public string message { get; set; }
    }

    /// <summary>
    /// Text Analytics API result
    /// </summary>
    public class Result
    {
        public string id { get; set; }
        public double? score { get; set; }
    }
}
