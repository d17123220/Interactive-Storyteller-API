

namespace Interactive_Storyteller_API.Controllers
{
    
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    using Interactive_Storyteller_API.Models;
    using Interactive_Storyteller_API.Services;


    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ICosmosDBService _cosmosDBService;

        public SessionController(ICosmosDBService cosmosDBService)
        {
            _cosmosDBService = cosmosDBService;
        }

        // api methods: GET (list), GET/{sessionID, userName} (read), POST (create), DELETE (delete), PATCH (modify), PUT (update)

        // GET: api/Session
        [HttpGet]
        public async Task<IEnumerable<Session>> GetSessionAsync()
        {
            return (IEnumerable<Session>) await _cosmosDBService.GetItemsAsync("SELECT * FROM c", "Sessions");
        } 

        // GET api/Session?sessionID=202&userName=user@email.address
        [HttpGet("{sessionID, userName}")]
        public async Task<ActionResult<Session>> GetSessionAsync(long sessionID, string userName)
        {
            return null;
        }

        [HttpPost]
        public async Task<ActionResult<Session>> PostSessionAsync(Session session)
        {
            return null;
        }

        [HttpDelete("{sessionID}")]
        public async Task<IActionResult> DeleteSessionAsync(long sessionID)
        {
            return NoContent();
        }

        [HttpPut("{sessionID}")]
        public async Task<IActionResult> PutSessionAsync(long sessionID, Session session)
        {
            return NoContent(); 
        }

        [HttpPatch("{sessionID")]
        public async Task<IActionResult> PatchSessionAsync(long sessionID, Session session)
        {
            // patch and put are interchangable here
            return await PutSessionAsync(sessionID, session);
        }
    }
}