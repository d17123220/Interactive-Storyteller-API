namespace Interactive_Storyteller_API.Models
{
    using System.Text.Json.Serialization;

    public class Context : Item
    {
        [JsonPropertyName("sessionID")]
        public long SessionID { get; set; }

        [JsonPropertyName("context")]
        public string SessionText { get; set; }

        [JsonPropertyName("contextCreator")]
        public string Creator { get; set; }

        [JsonPropertyName("contextSequence")]
        public long SequenceNumber { get; set; }

    }
}