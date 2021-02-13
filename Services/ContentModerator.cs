namespace Interactive_Storyteller_API.Services
{

    using Microsoft.Azure.CognitiveServices.ContentModerator;
    using Microsoft.Azure.CognitiveServices.ContentModerator.Models;

    public class ContentModerator : IContentModerator
    {
        
        private ContentModeratorClient _client;

        public ContentModerator(string account, string key)
        {
            _client = new ContentModeratorClient(new ApiKeyServiceClientCredentials(key));
            _client.Endpoint = account;
        }
    }
}