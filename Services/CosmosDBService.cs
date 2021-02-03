namespace Interactive_Storyteller_API.Services
{

    using Interactive_Storyteller_API.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;
    using Microsoft.Azure.Cosmos;

    public class CosmosDBService : ICosmosDBService
    {

        private List<Container> _containers;
        private CosmosClient _dbClient;

        public CosmosDBService(CosmosClient dbClient)
        {
            _dbClient = dbClient;
            _containers = new List<Container>();
        }

        // Methods for the interface

        public async Task<IEnumerable<T>> GetItemsAsync<T>(string queryString, string container="Sessions")
        {
            var _container = GetContainerByName(container);
            if (null != _container)
            {
                var query = _container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
                List<T> results = new List<T>();
                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();
                    results.AddRange(response.ToList());
                }

                return results;
            }
            else
                return null;

        }

        public async Task<T> GetItemAsync<T>(string id, string container="Sessions")
        {
            var _container = GetContainerByName(container);
            if (null != _container)
            {
                try
                {
                    ItemResponse<T> response = await _container.ReadItemAsync<T>(id, new PartitionKey(id));
                    return response.Resource;
                }   
                catch(CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                { 
                    return default(T);
                }
            }
            else
                return default(T);

        }

        public async Task<bool> AddItemAsync<T>(T item, string container="Sessions") where T : Item
        {
            var _container = GetContainerByName(container);
            if (null != _container)
            {
                await _container.CreateItemAsync<T>(item, new PartitionKey(item.Id));
                return true;
            }
            else
                return false;

        }

        public async Task<bool> UpdateItemAsync<T>(T item, string container="Sessions") where T : Item
        {
            var _container = GetContainerByName(container);
            if (null != _container)
            {
                await _container.UpsertItemAsync<T>(item, new PartitionKey(item.Id));
                return true;
            }
            else
                return false;
        }

        public async Task<bool> DeleteItemAsync(string id, string container="Sessions")
        {
            var _container = GetContainerByName(container);
            if (null != _container)
            {
                await _container.DeleteItemAsync<Item>(id, new PartitionKey(id));
                return true;
            }
            else
                return false;
        }

        // Additional methods

        // Add new database/container to the list of available containers
        public async Task AddContainerDefinition(string databaseName, string containerName)
        {
            // check if container with such id already in the list
            if (null == _containers.FirstOrDefault(container => container.Id.Equals(containerName)))
            {
                await Task.Run(() => _containers.Add(_dbClient.GetContainer(databaseName, containerName)));
            }
        }

        // get container by name
        public Container GetContainerByName(string containerName)
        {
            // use LINQ query on List of container
            // returns either Container with matching id or null
            Container container = _containers.FirstOrDefault
            (
                container => container.Id.ToLower().Equals(containerName.ToLower())
            );

            return container;
        }
    }
}