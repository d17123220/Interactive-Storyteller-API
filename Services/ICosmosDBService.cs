namespace Interactive_Storyteller_API.Services
{

    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Interactive_Storyteller_API.Models;

    public interface ICosmosDBService
    {
        Task<IEnumerable<Item>> GetItemsAsync(string queryString, string container);
        Task<Item> GetItemAsync(string id, string container);
        Task<bool> AddItemAsync(Item item, string container);
        Task<bool> UpdateItemAsync(string id, Item item, string container);
        Task<bool> DeleteItemAsync(string id, string container);    
    }
}