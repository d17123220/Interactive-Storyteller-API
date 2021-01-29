namespace Interactive_Storyteller_API.Models
{
    using System.Text.Json.Serialization;

    public class Session : Item
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("sessionID")]
        public long SessionID { get; set; }

        [JsonPropertyName("sessionPassword")]
        public string Password { get; set; }
    }
}