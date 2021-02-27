namespace Interactive_Storyteller_API.Models
{
    using Newtonsoft.Json;

    public class DeepAI
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "output")]
        public string CombinedText { get; set; }

    }
}