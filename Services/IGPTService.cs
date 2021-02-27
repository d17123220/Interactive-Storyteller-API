namespace Interactive_Storyteller_API.Services
{

    using System.Threading.Tasks;

    public interface IGPTService
    {
        Task<string> GenerateText(string text);
        Task<string> GenerateText(string sessionID, string sessionPassword, string baseURL);
    }
}