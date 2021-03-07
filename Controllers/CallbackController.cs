namespace Interactive_Storyteller_API.Controllers
{

    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using System.Linq;
    using Interactive_Storyteller_API.Services;
    using Interactive_Storyteller_API.Models;

    [Route("api/[controller]")]
    [ApiController]
    public class CallbackController : ControllerBase
    {
        private ICosmosDBService _cosmosDBService;

        public CallbackController(ICosmosDBService cosmosDBService)
        {
            _cosmosDBService = cosmosDBService;
        }

        // GET: api/Callback/202/123456qwerty
        // GET: api/Callback/sessionID=202&password=123456qwerty
        // Get whole context of the session as a plaintext
        [HttpGet("{sessionID}/{password}")]
        public async Task<ActionResult<string>> GetContextAsync(string sessionID, string password)
        {
            string query;
            
            // check if session and password are correct
            query = $"SELECT * FROM c WHERE c.sessionID = '{sessionID}' AND c.password = '{password}'";
            var sessions = await _cosmosDBService.GetItemsAsync<Session>(query, "Sessions");
            if (null != sessions && sessions.Any())
            {
                query = $"SELECT * FROM c WHERE c.sessionID = '{sessionID}' and c.contextCreator = 'User'";
                var contexts = await _cosmosDBService.GetItemsAsync<Context>(query, "Context");
                if (null != contexts && contexts.Any())
                {
                    // Combine all user inputs into one big text
                    var longString = string.Join(" ", (contexts.OrderBy(s => s.SequenceNumber).Select(s => s.SessionText)));
                    // Split it into separate words (sub-tokens)
                    var sessionList = longString.Split(' ').ToList();
                    // Take only last tokens (up to tokensToTake) and combine them back into big text
                    int wordsToGenerate = 300; // how many tokens GPT will be able to generate
                    int tokensToTake = 1024 - wordsToGenerate; // up to 1024 
                    var sessionText = string.Join(" ", sessionList.Skip(sessionList.Count < tokensToTake ? 0 : sessionList.Count - tokensToTake).ToList());
                    return sessionText;
                }
                else
                    return NoContent();
            }
            else
                return BadRequest();


        }

    }

}