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

            var inputTextLength = text.Length;

            // remove input text from output text
            if (isUrl)
            {
                var responseContext = await _httpClient.GetAsync(text);
                // ensure recieved succsessfull answer
                responseContext.EnsureSuccessStatusCode();
                var sessionContext = await responseContext.Content.ReadAsStringAsync();
                inputTextLength = sessionContext.Trim('\"').Length;
            }

            // Check what was earlier - first newline or last dot
            bool isValidText = false;
            int firstNewLine;
            int offset = 0;
            do
            {
                offset++;
                firstNewLine = sessionResponse.CombinedText.IndexOf('\n', inputTextLength + offset);
                // if newline is not found in the text or difference between input text and first new line more than 10 characters
                if (firstNewLine == -1 || firstNewLine - inputTextLength > 10)
                    isValidText = true;
            }
            while (!isValidText);
            
            int lastDot = sessionResponse.CombinedText.LastIndexOf('.');
            int lastPosition = firstNewLine == -1 ? lastDot + 1 : firstNewLine;
            string trimmedText = sessionResponse.CombinedText.Substring(0,lastPosition).Trim();

            return trimmedText.Substring(inputTextLength).Trim();
        }
    }
}