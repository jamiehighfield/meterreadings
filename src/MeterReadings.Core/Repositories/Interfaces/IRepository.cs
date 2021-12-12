using MeterReadings.Shared;
using System.Data;
using System.Linq.Expressions;

namespace MeterReadings.Core.Repositories.Interfaces
{
    /// <summary>
    /// Implement this interface to indicate that the implementing class is a repository.
    /// </summary>
    public interface IRepository { }

    /// <summary>
    /// Implement this interface to indicate that the implementing class is a read repository.
    /// </summary>
    /// <typeparam name="TEntityType">The type of entity.</typeparam>
    public interface IReadRepository<TEntityType> : IRepository
        where TEntityType : class
    {
        /// <summary>
        /// Lists all items from the repository for the specified page that match any where conditions.
        /// </summary>
        /// <param name="pageRequest">The page information for the query.</param>
        /// <param name="where">Where conditions for the query.</param>
        /// <returns>A paginated collection of results.</returns>
        Task<ListResult<TEntityType>> ListAsync(PageRequest pageRequest, params Expression<Func<TEntityType, bool>>[] @where);
    }

    /// <summary>
    /// Implement this interface to indicate that the implementing class is a write repository.
    /// </summary>
    /// <typeparam name="TEntityType">The type of entity.</typeparam>
    public interface IWriteRepository<TEntityType> : IRepository
        where TEntityType : class
    {
        /// <summary>
        /// Adds an item to the repository as an instance <typeparamref name="TEntityType"/>.
        /// </summary>
        /// <param name="entity">The entity to add as an instance of <typeparamref name="TEntityType"/>.</param>
        /// <returns>The added entity as an instance of <typeparamref name="TEntityType"/>.</returns>
        Task<TEntityType> AddAsync(TEntityType entity);

        IDbTransaction BeginTransaction();
    }
}