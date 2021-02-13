namespace Interactive_Storyteller_API.Controllers
{
    
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;
    using System;
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
        public async Task<ActionResult<IEnumerable<Session>>> GetSessionAsync()
        {
            // debug:
            var sessions = await _cosmosDBService.GetItemsAsync<Session>("SELECT * FROM c", containerName);
            if (null != sessions)
                return Ok(sessions);
            else
                return Ok();

            /* 
            // non debug:
            await Task.Run(() => {});
            return null;
            */
        } 

        // GET api/Session?sessionID=202&userName=user@email.address
        // GET api/Session/user@email.address/202
        // Checks if such session for such user exists
        [HttpGet("{userName}/{sessionID}")]
        public async Task<bool> GetSessionAsync(string userName, string sessionID)
        {
            string query = "";
            if (userName.Equals("*"))
                query = $"SELECT * FROM s WHERE s.sessionID = '{sessionID}'";
            else
                query = $"SELECT * FROM s WHERE s.userName = '{userName}' and s.sessionID = '{sessionID}'";
            
            var sessions = await _cosmosDBService.GetItemsAsync<Session>(query, containerName);

            if ( null == sessions || !sessions.Any() )
                return false;
            else
                return true;

        }

        // POST api/Session
        // body of request should contain Session object
        // creates a new session for userName, generates new random session ID and password, obfuscates password and returns to UI
        [HttpPost]
        public async Task<ActionResult<Session>> PostSessionAsync(Session session)
        {
            // Define local variables
            bool result;
            // Initialize random generator
            Random random = new Random();

            // clean unneeded fields
            session.Id = Guid.NewGuid().ToString();
            session.SessionID = null;
            session.Password = null;
            
            // check if userName was passed in request body
            if (string.IsNullOrEmpty(session.UserName))
                return BadRequest();

            // generate new sessionID till unique is found
            do 
            {
                session.SessionID = Guid.NewGuid().ToString();
                result = await GetSessionAsync("*", session.SessionID);
                // if such session already exists
            } 
            while (result);

            // generate random password
            // https://jonathancrozier.com/blog/how-to-generate-a-random-string-with-c-sharp

            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string randomString = new string(
                Enumerable
                .Repeat(chars, 20)
                .Select(
                    s => s[random.Next(s.Length)]
                ).ToArray()
            );

            session.Password = randomString;

            result = await _cosmosDBService.AddItemAsync<Session>(session, containerName);
            if (!result)
                return BadRequest(session);
            else
            {
                session.Password = "<redacted>";
                return Ok(session);
            }
          
        }

        // DELETE api/Session?sessionID=202&userName=user@email.address
        // DELETE api/Session/user@email.address/202
        // deletes a session entry from database
        [HttpDelete("{userName}/{sessionID}")]
        public async Task<IActionResult> DeleteSessionAsync(string userName, string sessionID)
        {
            var result = await _cosmosDBService.GetItemsAsync<Session>($"SELECT * FROM s WHERE s.userName = '{userName}' and s.sessionID = '{sessionID}'", containerName);            
            if (result.Any())
            {    
                await _cosmosDBService.DeleteItemAsync(result.First().Id, containerName);
                return NoContent();
            }
            else 
                return NotFound();
        }

        [HttpPut("{sessionID}")]
        public async Task<IActionResult> PutSessionAsync(string sessionID, Session session)
        {
            // not implemented - nothing to do
            await Task.Run(() => {});
            return NotFound(); 
        }

        [HttpPatch("{sessionID}")]
        public async Task<IActionResult> PatchSessionAsync(string sessionID, Session session)
        {
            // patch and put are interchangable here
            return await PutSessionAsync(sessionID, session);
        }
    }
}