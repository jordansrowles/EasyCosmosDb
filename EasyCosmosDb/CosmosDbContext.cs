using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EasyCosmosDb
{
    /// <summary>
    ///     Represents the context of a Cosmos database service
    /// </summary>
    public class CosmosDbContext :  ICosmosDbContext
    {
        /// <summary>
        ///     CosmosClient for interacting with Azure Cosmos services
        /// </summary>
        public CosmosClient Client { get; set; }
        Database _database;
        /// <summary>
        ///     The database in context
        /// </summary>
        public Database Database { get => _database; set => _database = value; }
        readonly IKeyVaultContext _vault;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="vault">Azure Key Vault context</param>
        /// <param name="database">Database name</param>
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

        /// <summary>
        ///     Get all items from a container using a feed iterator
        /// </summary>
        /// <typeparam name="T">Business model T</typeparam>
        /// <param name="container">The container object</param>
        /// <returns>List of type T</returns>
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

        /// <summary>
        ///     Gets all items from a container with a query using query definition
        /// </summary>
        /// <typeparam name="T">Business model T</typeparam>
        /// <param name="container">The container object</param>
        /// <param name="query">The query to perform</param>
        /// <returns>List of type T</returns>
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

        /// <summary>
        ///     Get all items from a container
        /// </summary>
        /// <typeparam name="T">Business model T</typeparam>
        /// <param name="container">The container object</param>
        /// <returns>List of type T</returns>
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

        /// <summary>
        ///     Get a specific item from a container
        /// </summary>
        /// <typeparam name="T">Business model T</typeparam>
        /// <param name="id">Id of the item</param>
        /// <param name="partitionKey">Partition Key of the item</param>
        /// <param name="container">The
        public async Task<T> ReadItemAsync<T>(string id, PartitionKey partitionKey, Container container)
        {
            ItemResponse<T> response = await container.ReadItemAsync<T>(partitionKey: partitionKey, id: id);
            return response.Resource;
        }

        /// <summary>
        ///     Inserts object of type T into container
        /// </summary>
        /// <typeparam name="T">Business model T</typeparam>
        /// <param name="item">Object to insert</param>
        /// <param name="partitionKey">Partition Key to use</param>
        /// <param name="container">Container object to insert into</param>
        /// <returns>The inserted object</returns>
        public async Task<T> InsertAsync<T>(T item, PartitionKey partitionKey, Container container)
        {
            ItemResponse<T> response = await container.CreateItemAsync(item, partitionKey);
            T resitem = response;
            return resitem;
        }

        /// <summary>
        ///     Either inserts or updates an item in the container
        /// </summary>
        /// <typeparam name="T">Business model T</typeparam>
        /// <param name="item">Object to insert</param>
        /// <param name="partitionKey">Partition Key to use</param>
        /// <param name="container">Container object to upsert into</param>
        /// <returns>The upserted item</returns>
        public async Task<T> UpsertAsync<T>(T item, PartitionKey partitionKey, Container container)
        {
            ItemResponse<T> response = await container.UpsertItemAsync(
                partitionKey: partitionKey,
                item: item);
            T resitem = response.Resource;
            return resitem;
        }

        /// <summary>
        ///     Deletes an item in the container
        /// </summary>
        /// <typeparam name="T">Business model T</typeparam>
        /// <param name="id">Id of the item</param>
        /// <param name="partitionKey">Partition Key of the item</param>
        /// <param name="container">Container to delete item from</param>
        /// <returns>Deleted item</returns>
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
