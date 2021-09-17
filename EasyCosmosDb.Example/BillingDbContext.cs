using Microsoft.Azure.Cosmos;
using System;

namespace EasyCosmosDb.Example
{
    public interface IBillingDbContext
    {
        public Container GeneratedInvoices { get; }
        public Container BillableUsers { get; }
        public Container Transactions { get; }
    }

    public class BillingDbContext : CosmosDbContext, IBillingDbContext
    {
        public Container GeneratedInvoices => Database.GetContainer("generated-invoices");
        public Container BillableUsers => Database.GetContainer("billable-users");
        public Container Transactions => Database.GetContainer("transactions");

        public BillingDbContext(IKeyVaultContext vault) : base(vault, "userdata")
        {
        }

        public async void InsertTranslationRequest(Transaction transaction) =>
            await InsertAsync<Transaction>(transaction, new PartitionKey() { }, Transactions);

        // extend from here
    }
}
