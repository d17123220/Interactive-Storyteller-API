namespace Interactive_Storyteller_API.Models
{
    using Newtonsoft.Json;

    public class Context : Item
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "sessionID")]
        public string SessionID { get; set; }

        [JsonProperty(PropertyName = "sessionText")]
        public string SessionText { get; set; }

        [JsonProperty(PropertyName = "contextCreator")]
        public string ContextCreator { get; set; }

        [JsonProperty(PropertyName = "sequenceNumber")]
        public long SequenceNumber { get; set; }
    }
}