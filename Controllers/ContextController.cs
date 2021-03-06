namespace Interactive_Storyteller_API.Controllers
{

    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;
    using Interactive_Storyteller_API.Services;
    using Interactive_Storyteller_API.Models;
    using System;

    [Route("api/[controller]")]
    [ApiController]
    public class ContextController : ControllerBase
    {
        private IContentModeratorService _contentModerator;
        private ICosmosDBService _cosmosDBService;
        private IGPTService _gptService;

        public ContextController(IContentModeratorService contentModerator, ICosmosDBService cosmosDBService, IGPTService gptService)
        {
            _contentModerator = contentModerator;
            _cosmosDBService = cosmosDBService;
            _gptService = gptService;
        }


        // GET: api/Context/user@email.address/202
        // GET: api/Context/userName=user@email.address&sessionID=202
        // Get whole context of the session
        [HttpGet("{userName}/{sessionID}")]
        public async Task<ActionResult<IEnumerable<Context>>> GetContextAsync(string userName, string sessionID)
        {
            // check if such userName and sessionID pair exist in database
            var query = $"SELECT * FROM s WHERE s.userName = '{userName}' and s.sessionID = '{sessionID}'";
            var sessions = await _cosmosDBService.GetItemsAsync<Session>(query, "Sessions");

            if (null != sessions && sessions.Any())
            {
                query = $"SELECT * FROM c WHERE c.sessionID = '{sessionID}'";
                var contexts = await _cosmosDBService.GetItemsAsync<Context>(query, "Context");
                
                if (null != contexts && contexts.Any())
                {
                    contexts = contexts.OrderBy(c => c.SequenceNumber).ToList();
                    return Ok(contexts);
                }
                else
                    return NoContent();
            }
            else
                return NoContent();
        }


        // POST: api/Context
        // manage user input:
        //   Assume input was already screend with api/Context/Check endpoint
        //   add to context database
        //   send a new context to GPT API
        //   add generated text to context database
        //   send generated context to user (with Screened Context of input)
        [HttpPost]
        public async Task<ActionResult<Context>> PostContextAsync(UserInput userInput)
        {
            string query, generatedText;
            long nextSequence = 0;
            bool result;
            Context context;
            
            if (string.IsNullOrEmpty(userInput.Text) || string.IsNullOrEmpty(userInput.SessionID))
                return BadRequest();
            
            query = $"SELECT * FROM s WHERE s.sessionID = '{userInput.SessionID}'";
            var sessions = await _cosmosDBService.GetItemsAsync<Session>(query, "Sessions");
            if (null == sessions || !sessions.Any())
                return BadRequest();

            // prepare userInput into context
            context = new Context()
            {
                Id = Guid.NewGuid().ToString(),
                ContextCreator = "User",
                SessionID = userInput.SessionID,
                SessionText = userInput.Text
            };

            query = $"SELECT * FROM c WHERE c.sessionID = '{userInput.SessionID}'";
            var wholeContext = await _cosmosDBService.GetItemsAsync<Context>(query, "Context");
            if (null != wholeContext && wholeContext.Any())
                nextSequence = wholeContext.Max(s => s.SequenceNumber) + 1;

            context.SequenceNumber = nextSequence;
            nextSequence++;

            // add userInput to context database
            result = await _cosmosDBService.AddItemAsync<Context>(context, "Context");

            // send whole prepared context to API
            var session = sessions.First();
            var baseURL = $"{Url.ActionContext.HttpContext.Request.Scheme}://{Url.ActionContext.HttpContext.Request.Host.Value}";
            if (!baseURL.Contains("localhost"))
            {    
                generatedText = await _gptService.GenerateText(session.SessionID, session.Password, baseURL);
            }
            else
            {
                // use text instead of callback for debug, as https://localhost is not available for callback
                var tmpContexts = await GetContextAsync(session.UserName, session.SessionID);
                var tmpObj = (ObjectResult) tmpContexts.Result;
                var tmpList = (List<Context>) tmpObj.Value;
                var tmpText = string.Join(" ",(tmpList.OrderBy(s => s.SequenceNumber).Select(s => s.SessionText)));
                generatedText = await _gptService.GenerateText(tmpText);
            }
            // prepare generated text into context
            context = new Context()
            {
                Id = Guid.NewGuid().ToString(),
                ContextCreator = "GPT",
                SessionID = userInput.SessionID,
                SessionText = generatedText.Replace("\n\n","\n"),
                SequenceNumber = nextSequence
            };

            // add generated text to context database
            result = await _cosmosDBService.AddItemAsync<Context>(context, "Context");

            // return generated context 
            return context;
        } 

        // GET: api/Context/check/Quick brown fox jumps over...
        // check string passed as get (unsecure!)
        [HttpGet("Check/{phrase}")]
        public async Task<ActionResult<ScreenedContext>> GetContextAsync(string phrase)
        {
            if (string.IsNullOrEmpty(phrase))
                return BadRequest();

            // check with content moderator and return result
            return await _contentModerator.ModerateText(phrase);
        }

        // POST: api/Context/check
        // check string passed as body in post (secure)
        [HttpPost("Check/{verifier}")]
        public async Task<ActionResult<ScreenedContext>> PostContextAsync(UserInput userInput, string verifier) 
        {
            if (string.IsNullOrEmpty(userInput.Text) || string.IsNullOrEmpty(verifier))
                return BadRequest();
    
            // check with content moderator and return result
            return await _contentModerator.ModerateText(userInput.Text);
        }




    }
}