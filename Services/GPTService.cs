namespace Interactive_Storyteller_API.Services
{

    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Interactive_Storyteller_API.Models;

    public class GPTService : IGPTService
    {
        private HttpClient _httpClient;

        public GPTService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GenerateText(string text)
        {
            return await DeepAITextGenerator(text, false);
        }

        public async Task<string> GenerateText(string sessionID, string sessionPassword, string baseURL)
        {
            string callBack = $"{baseURL}/api/Callback/{sessionID}/{sessionPassword}";
                        
            return await DeepAITextGenerator(callBack, true);
        }

        private async Task<string> DeepAITextGenerator(string text, bool isUrl = false)
        {
            // create a custom form-encoded body using text: key
            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("text", text),
            });

            // Use POST async call with httpCleint send a body to DeepAI
            var response = await _httpClient.PostAsync("", requestContent);
            // ensure recieved succsessfull answer
            response.EnsureSuccessStatusCode();

            // decode answer into stream and de-serialize stream into DeepAI object
            var responseString = await response.Content.ReadAsStringAsync();
            var sessionResponse = JsonConvert.DeserializeObject<DeepAI>(responseString);
            
            // Check what was the last - newline or dot
            int lastDot = sessionResponse.CombinedText.LastIndexOf('.');
            int lastNewLine = sessionResponse.CombinedText.LastIndexOf('\n');
            int lastPosition = lastDot > lastNewLine ? lastDot : lastNewLine;
            string trimmedText = sessionResponse.CombinedText.Substring(0,lastPosition+1).Trim();

            // remove input text from output text
            if (isUrl)
            {
                var responseContext = await _httpClient.GetAsync(text);
                // ensure recieved succsessfull answer
                responseContext.EnsureSuccessStatusCode();
                var sessionContext = await responseContext.Content.ReadAsStringAsync();
                return trimmedText.Substring(sessionContext.Trim('\"').Length);
            }
            else
                return trimmedText.Substring(text.Length);

        }

    }
}