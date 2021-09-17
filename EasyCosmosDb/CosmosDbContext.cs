using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EasyCosmosDb
{
    public class CosmosDbContext :  ICosmosDbContext
    {
        public CosmosClient Client { get; set; }
        Database _database;
        public Database Database { get => _database; set => _database = value; }
        readonly IKeyVaultContext _vault;

        public CosmosDbContext(IKeyVaultContext vault, string database)
        {
            _vault = vault;
            CosmosClientOptions options = new CosmosClientOptions()
            {
                SerializerOptions = new CosmosSerializationOptions()
                {
                    IgnoreNullValues = true,
                    Indented = false
                }
            };

            string connectionstring = _vault.CosmosConnectionStringAsync().Result;
            Client = new CosmosClient(
                connectionstring,
                options);
            _database = Client.GetDatabase(database);
        }
        public async Task<List<T>> ItemFeedAsync<T>(Container container)
        {
            List<T> values = new List<T>();

            using (FeedIterator<T> iterator = container.GetItemQueryIterator<T>())
            {
                while (iterator.HasMoreResults)
                {
                    foreach (T item in await iterator.ReadNextAsync())
                    {
                        values.Add(item);
                    }
                }
            }

            return values;
        }

        public async Task<List<T>> GetDocumentsAsync<T>(Container container, string query)
        {
            var q = container.GetItemQueryIterator<T>(new QueryDefinition(query));
            List<T> results = new List<T>();
            while (q.HasMoreResults)
            {
                var response = await q.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task<List<T>> GetDocumentsAsync<T>(Container container)
        {
            var q = container.GetItemQueryIterator<T>();
            List<T> results = new List<T>();
            while (q.HasMoreResults)
            {
                var response = await q.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }
        public async Task<T> ReadItemAsync<T>(string id, PartitionKey partitionKey, Container container)
        {
            ItemResponse<T> response = await container.ReadItemAsync<T>(partitionKey: partitionKey, id: id);
            return response.Resource;
        }

        public async Task<T> InsertAsync<T>(T item, PartitionKey partitionKey, Container container)
        {
            ItemResponse<T> response = await container.CreateItemAsync(item, partitionKey);
            T resitem = response;
            return resitem;
        }

        public async Task<T> UpsertAsync<T>(T item, PartitionKey partitionKey, Container container)
        {
            ItemResponse<T> response = await container.UpsertItemAsync(
                partitionKey: partitionKey,
                item: item);
            T resitem = response.Resource;
            return resitem;
        }

        public async Task<T> DeleteItemAsync<T>(string id, PartitionKey partitionKey, Container container)
        {
            try
            {
                ItemResponse<T> response = await container.DeleteItemAsync<T>(id, partitionKey);
                return response.Resource;
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Debug.WriteLine("Couldn't find the document to delete. Make sure the parameters are correct.");
                    return default;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
