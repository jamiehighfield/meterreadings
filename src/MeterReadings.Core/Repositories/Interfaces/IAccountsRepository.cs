using MeterReadings.Core.Entities;

namespace MeterReadings.Core.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for interfacing with the accounts table.
    /// </summary>
    public interface IAccountsRepository : IRepository
    {
        /// <summary>
        /// Finds an account by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the account.</param>
        Task<Account> FindByIdAsync(long id);

        /// <summary>
        /// Finds accounts by their unique account identifiers.
        /// </summary>
        /// <param name="accountIds">The unique account identifiers of the account.</param>
        Task<List<Account>> FindByAccountIdsAsync(params long[] accountIds);
    }
}