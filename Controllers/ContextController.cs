namespace Interactive_Storyteller_API.Controllers
{

    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Interactive_Storyteller_API.Services;
    using Interactive_Storyteller_API.Models;

    [Route("api/[controller]")]
    [ApiController]
    public class ContextController : ControllerBase
    {
        private IContentModeratorService _contentModerator;

        public ContextController(IContentModeratorService contentModerator)
        {
            _contentModerator = contentModerator;
        }





        // GET: api/Context/check/Quick brown fox jumps over...
        // check string passed as get (unsecure!)
        [HttpGet("Check/{phrase}")]
        public async Task<ActionResult<ScreenedContext>> GetContextAsync(string phrase)
        {
            if (string.IsNullOrEmpty(phrase))
                return NoContent();
            else
                // check with content moderator and return result
                return await _contentModerator.ModerateText(phrase);
        }

        // POST: api/Context/check
        // check string passed as body in post (secure)
       [HttpPost("Check")]
       public async Task<ActionResult<ScreenedContext>> PostContextAsync(string phrase) 
       {
            if (string.IsNullOrEmpty(phrase))
                return NoContent();
            else
                // check with content moderator and return result
                return await _contentModerator.ModerateText(phrase);
       }




    }
}