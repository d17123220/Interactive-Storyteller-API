namespace Interactive_Storyteller_API.Models
{
    using Newtonsoft.Json;
    using System;

    public class Session : Item
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "sessionID")]
        public string SessionID { get; set; }

        [JsonProperty(PropertyName = "sessionPassword")]
        public string Password { get; set; }
    }
}