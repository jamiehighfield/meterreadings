using Dapper;
using MeterReadings.Core.Entities;
using MeterReadings.Core.Repositories.Interfaces;

namespace MeterReadings.DataAccess.Repositories
{
    /// <summary>
    /// Default implementation of <see cref="IAccountsRepository"/>.
    /// </summary>
    public class AccountsRepository : IAccountsRepository
    {
        /// <summary>
        /// Initialise a new instance of <see cref="AccountsRepository"/>.
        /// </summary>
        public AccountsRepository(DatabaseConnection database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        private readonly DatabaseConnection _database;

        public async Task<Account> FindByIdAsync(long id) => await _database._underlyingDatabaseConnection.QuerySingleOrDefaultAsync<Account>(@"
            SELECT * from accounts WHERE id = :id;", id);

        public async Task<List<Account>> FindByAccountIdsAsync(params long[] ids)
        {
            var result = await _database._underlyingDatabaseConnection.QueryAsync<Account>(@"
               SELECT * FROM accounts WHERE account_id = ANY(:ids);", new
            {
                ids = ids
            });

            return result.ToList();
        }
    }
}