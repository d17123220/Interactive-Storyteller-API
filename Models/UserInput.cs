namespace Interactive_Storyteller_API.Models
{
    using Newtonsoft.Json;

    public class UserInput
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "sessionID")]
        public string SessionID { get; set; }
    }
}