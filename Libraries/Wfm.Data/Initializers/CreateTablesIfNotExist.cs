using System;
using System.Data.Entity;
using System.Transactions;

namespace Wfm.Data.Initializers
{
    public class CreateTablesIfNotExist<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
    {

        public void InitializeDatabase(TContext context)
        {
            bool dbExists;
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                dbExists = context.Database.Exists();
            }
            if (!dbExists)
            {
                throw new ApplicationException("No database instance");
            }
        }
    }
}
