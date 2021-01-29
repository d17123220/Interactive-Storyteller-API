namespace Interactive_Storyteller_API.Models
{
    using System.Text.Json.Serialization;

    public class Item
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}