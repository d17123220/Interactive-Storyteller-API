namespace Interactive_Storyteller_API.Services
{

    using System.Threading.Tasks;
    using Interactive_Storyteller_API.Models;

    public interface IContentModeratorService
    {
        Task<ScreenedContext> ModerateText(string text);
    }    
}