namespace Interactive_Storyteller_API.Services
{

    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Interactive_Storyteller_API.Models;

    public interface ICosmosDBService
    {
        Task<IEnumerable<T>> GetItemsAsync<T>(string queryString, string container);
        Task<T> GetItemAsync<T>(string id, string container);
        Task<bool> AddItemAsync<T>(T item, string container) where T : Item;
        Task<bool> UpdateItemAsync<T>(T item, string container) where T : Item;
        Task<bool> DeleteItemAsync(string id, string container);    
    }
}