namespace Interactive_Storyteller_API.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ScreenedContext
    {
        [JsonProperty(PropertyName = "originalText")]
        public string OriginalText { get; set; }

        [JsonProperty(PropertyName = "correctedText")]
        public string CorrectedText { get; set; }
        
        [JsonProperty(PropertyName = "offensiveTerms")]
        public ISet<string> OffensiveTerms { get; set; }

    }
}