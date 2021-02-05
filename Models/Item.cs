namespace Interactive_Storyteller_API.Models
{
    using Newtonsoft.Json;

    public interface Item
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}