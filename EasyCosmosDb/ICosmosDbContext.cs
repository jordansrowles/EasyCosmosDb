using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCosmosDb
{
    /// <summary>
    ///     Interface for CosmosDbContext
    ///     Represents CRUD operations for the database.
    /// </summary>
    public interface ICosmosDbContext
    {
        /// <summary>
        ///     Get all items from a container
        /// </summary>
        /// <typeparam name="T">Business model T</typeparam>
        /// <param name="container">The container object</param>
        /// <returns>List of type T</returns>
        Task<List<T>> ItemFeedAsync<T>(Container container);

        /// <summary>
        ///     Gets all items from a container with a query
        /// </summary>
        /// <typeparam name="T">Business model T</typeparam>
        /// <param name="container">The container object</param>
        /// <param name="query">The query to perform</param>
        /// <returns>List of type T</returns>
        Task<List<T>> GetDocumentsAsync<T>(Container container, string query);

        /// <summary>
        ///     Get all items from a container
        /// </summary>
        /// <typeparam name="T">Business model T</typeparam>
        /// <param name="container">The container object</param>
        /// <returns>List of type T</returns>
        Task<List<T>> GetDocumentsAsync<T>(Container container);

        /// <summary>
        ///     Get a specific item from a container
        /// </summary>
        /// <typeparam name="T">Business model T</typeparam>
        /// <param name="id">Id of the item</param>
        /// <param name="partitionKey">Partition Key of the item</param>
        /// <param name="container">The container object</param>
        /// <returns>Type T</returns>
        Task<T> ReadItemAsync<T>(string id, PartitionKey partitionKey, Container container);

        /// <summary>
        ///     Inserts object of type T into container
        /// </summary>
        /// <typeparam name="T">Business model T</typeparam>
        /// <param name="item">Object to insert</param>
        /// <param name="partitionKey">Partition Key to use</param>
        /// <param name="container">Container object to insert into</param>
        /// <returns>The inserted object</returns>
        Task<T> InsertAsync<T>(T item, PartitionKey partitionKey, Container container);

        /// <summary>
        ///     Either inserts or updates an item in the container
        /// </summary>
        /// <typeparam name="T">Business model T</typeparam>
        /// <param name="item">Object to insert</param>
        /// <param name="partitionKey">Partition Key to use</param>
        /// <param name="container">Container object to upsert into</param>
        /// <returns>The upserted item</returns>
        Task<T> UpsertAsync<T>(T item, PartitionKey partitionKey, Container container);

        /// <summary>
        ///     Deletes an item in the container
        /// </summary>
        /// <typeparam name="T">Business model T</typeparam>
        /// <param name="id">Id of the item</param>
        /// <param name="partitionKey">Partition Key of the item</param>
        /// <param name="container">Container to delete item from</param>
        /// <returns>Deleted item</returns>
        Task<T> DeleteItemAsync<T>(string id, PartitionKey partitionKey, Container container);
    }
}
