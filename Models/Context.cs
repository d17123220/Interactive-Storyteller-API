namespace Interactive_Storyteller_API.Models
{
    using Newtonsoft.Json;

    public class Context : Item
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "sessionID")]
        public long SessionID { get; set; }

        [JsonProperty(PropertyName = "context")]
        public string SessionText { get; set; }

        [JsonProperty(PropertyName = "contextCreator")]
        public string Creator { get; set; }

        [JsonProperty(PropertyName = "contextSequence")]
        public long SequenceNumber { get; set; }

        [JsonProperty(PropertyName = "screenedContext")]
        public ScreenedContext UserInput { get; set; }

    }
}