namespace Interactive_Storyteller_API.Models
{
    using Newtonsoft.Json;

    public class UserInput
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }
}