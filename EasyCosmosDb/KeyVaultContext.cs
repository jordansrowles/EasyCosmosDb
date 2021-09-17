using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCosmosDb
{
    public interface IKeyVaultContext
    {
        Task<string> CosmosConnectionStringAsync();
    }

    /// <summary>
    ///     Provides access to the Azure Key Vault
    /// </summary>
    public class KeyVaultContext : IKeyVaultContext
    {
        SecretClient _client;

        /// <summary>
        ///     Constructor, get the evnironment variable 'RC_KEYVAULT_URL'
        /// </summary>
        public KeyVaultContext()
        {
            string uri = Environment.GetEnvironmentVariable("ECDB_COSMOS_CXNSTR");
            _client = new SecretClient(new Uri(uri), credential: new DefaultAzureCredential());
        }

        /// <summary>
        ///     Gets the secret from the vault
        /// </summary>
        /// <param name="name">Name of the secret string</param>
        /// <returns>Secret string</returns>
        async Task<string> GetSecret(string name)
        {
            var result = await _client.GetSecretAsync(name);
            return result.Value.Value;
        }

        /// <summary>
        ///     Gets the Cosmos Db account connection string
        /// </summary>
        /// <returns></returns>
        public async Task<string> CosmosConnectionStringAsync() => await GetSecret("CosmosConnectionString");
    }
}
