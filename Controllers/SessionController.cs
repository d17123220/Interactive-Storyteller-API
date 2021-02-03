

namespace Interactive_Storyteller_API.Controllers
{
    
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;

    using Interactive_Storyteller_API.Models;
    using Interactive_Storyteller_API.Services;


    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ICosmosDBService _cosmosDBService;
        private readonly string containerName;

        public SessionController(ICosmosDBService cosmosDBService)
        {
            _cosmosDBService = cosmosDBService;
            containerName = "Sessions";
        }

        // api methods: GET (list), GET/{sessionID, userName} (read), POST (create), DELETE (delete), PATCH (modify), PUT (update)

        // GET: api/Session
        // Lists all session in database (debug only)
        [HttpGet]
        public async Task<IEnumerable<Session>> GetSessionAsync()
        {
            // debug:
            var sessions = await _cosmosDBService.GetItemsAsync<Session>("SELECT * FROM c", containerName);
            if (null != sessions)
                return sessions;
            else
                return new List<Session>();

            /* 
            // non debug:
            await Task.Run(() => {});
            return null;
            */
        } 

        // GET api/Session?sessionID=202&userName=user@email.address
        // GET api/Sessiom/user@email.address/202
        // Checks if such session for such user exists
        [HttpGet("{userName}/{sessionID:long}")]
        public async Task<bool> GetSessionAsync(string userName, long sessionID)
        {
            var sessions = await _cosmosDBService.GetItemsAsync<Session>($"SELECT * FROM s WHERE s.userName = \"{userName}\" and s.sessionID = {sessionID}", containerName);

            if ( null == sessions || !sessions.Any() )
                return false;
            else
                return true;

        }

        // POST api/Session
        // body of request should contain Session
        // creates a new session for userName, generates new random session ID and password, obfuscates password and returns to UI
        [HttpPost]
        public async Task<ActionResult<Session>> PostSessionAsync(Session session)
        {
            return null;
        }

        [HttpDelete("{sessionID:long}")]
        public async Task<IActionResult> DeleteSessionAsync(long sessionID)
        {
            return NoContent();
        }

        [HttpPut("{sessionID:long}")]
        public async Task<IActionResult> PutSessionAsync(long sessionID, Session session)
        {
            return NoContent(); 
        }

        [HttpPatch("{sessionID:long}")]
        public async Task<IActionResult> PatchSessionAsync(long sessionID, Session session)
        {
            // patch and put are interchangable here
            return await PutSessionAsync(sessionID, session);
        }
    }
}