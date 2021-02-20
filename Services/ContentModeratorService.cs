namespace Interactive_Storyteller_API.Services
{

    using Microsoft.Azure.CognitiveServices.ContentModerator;
    using Microsoft.Azure.CognitiveServices.ContentModerator.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.IO;
    using System.Text;
    using Interactive_Storyteller_API.Models;

    public class ContentModeratorService : IContentModeratorService
    {
        
        private ContentModeratorClient _client;

        public ContentModeratorService(string account, string key)
        {
            // create a new ContentModeratorClient, which will be used for Text moderation/screening
            // Details: https://docs.microsoft.com/en-us/azure/cognitive-services/content-moderator/client-libraries?pivots=programming-language-csharp&tabs=cli
            _client = new ContentModeratorClient(new ApiKeyServiceClientCredentials(key));
            _client.Endpoint = account;
        }

        public async Task<ScreenedContext> ModerateText(string text)
        {
            // create in-memory stream, as ContentModerator SDK accepts text only as a stream
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

            // use ContentModeratorClient.TextModeration to check provided text
            var answer = await _client.TextModeration.ScreenTextAsync("text/plain", stream, "eng", true, false, null, false);
            // Content-Type: text/plain
            // Text: (variable)
            // Language: eng
            // Autocorrect: true
            // Check for PII (US only): false
            // ListID (custom list of words): null
            // Classify as adult (class1), racist (class2) or offensive (class3): false
            //
            // Up to 1024 characters long stream can be used per one request. 
            // Free version of Azure ContentModerator allows only 1 request per second, and up to 5000 requests per month
            // Details: https://docs.microsoft.com/en-us/rest/api/cognitiveservices/contentmoderator/textmoderation/screentext
            // and https://docs.microsoft.com/en-us/azure/cognitive-services/content-moderator/client-libraries?pivots=programming-language-csharp&tabs=cli

            // create a new screened text object and add some values. Can be done through model itself
            var screen = new ScreenedContext()
            {
                OriginalText = answer.OriginalText,
                CorrectedText = answer.AutoCorrectedText,
                OffensiveTerms = new HashSet<string>(),
                IsBounced = false
            };

            // Check if ContentModerator found any offensive terms
            if (null != answer.Terms)
            {
                // Add all of them to the HashSet (so it keeps unique terms only)
                foreach (var term in answer.Terms)
                {
                    screen.OffensiveTerms.Add(term.Term);
                }
                screen.IsBounced = true;
            }

            return screen;
        }
    }
}