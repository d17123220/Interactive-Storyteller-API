namespace Interactive_Storyteller_API.Services
{

    using Microsoft.Azure.CognitiveServices.ContentModerator;
    using Microsoft.Azure.CognitiveServices.ContentModerator.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Interactive_Storyteller_API.Models;

    public class ContentModeratorService : IContentModeratorService
    {
        
        private ContentModeratorClient _client;

        public ContentModeratorService(string account, string key)
        {
            _client = new ContentModeratorClient(new ApiKeyServiceClientCredentials(key));
            _client.Endpoint = account;
        }
    }
}