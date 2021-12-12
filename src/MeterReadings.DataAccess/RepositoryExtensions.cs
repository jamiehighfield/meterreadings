using Dapper;
using ExpressionExtensionSQL;
using ExpressionExtensionSQL.Extensions;
using MeterReadings.Core.Entities;
using MeterReadings.Core.Repositories.Interfaces;
using MeterReadings.Shared;
using System.Data;
using System.Linq.Expressions;

namespace MeterReadings.DataAccess
{
    /* READ THIS
     * This is a very rudimentary implementation of dynamic SQL/where expression generation.
     * 
     * This uses an external library that isn't really mature enough to be used properly; I have
     * my own library that achieves a similar result in a more robust way, but would have needed
     * refactoring to be used here.
     * 
     * I have included this more to illustrate the concept. I appreciate it's not the best... */


    /// <summary>
    /// Extension methods for repositories.
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Gets the first item from the repository or an exception will be thrown.
        /// </summary>
        /// <typeparam name="TEntityType">The type of entity.</typeparam>
        /// <param name="repository">The repository as an instance of <see cref="IReadRepository{TEntityType}"/>.</param>
        /// <param name="where">Where conditions for the query.</param>
        /// <returns>An instance of <typeparamref name="TEntityType"/>.</returns>
        public static async Task<TEntityType> SingleAsync<TEntityType>(this IReadRepository<TEntityType> repository, params Expression<Func<TEntityType, bool>>[] @where)
            where TEntityType : class
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var result = await repository.ListAsync(new PageRequest(1, 1), where);

            return result.List.Single();
        }

        /// <summary>
        /// Gets the first item from the repository or null.
        /// </summary>
        /// <typeparam name="TEntityType">The type of entity.</typeparam>
        /// <param name="repository">The repository as an instance of <see cref="IReadRepository{TEntityType}"/>.</param>
        /// <param name="where">Where conditions for the query.</param>
        /// <returns>An instance of <typeparamref name="TEntityType"/>.</returns>
        public static async Task<TEntityType> SingleOrDefaultAsync<TEntityType>(this IReadRepository<TEntityType> repository, params Expression<Func<TEntityType, bool>>[] @where)
            where TEntityType : class
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var result = await repository.ListAsync(new PageRequest(1, 1), where);

            return result.List.SingleOrDefault();
        }

        /// <summary>
        /// Gets all items from the repository.
        /// </summary>
        /// <typeparam name="TEntityType">The type of entity.</typeparam>
        /// <param name="repository">The repository as an instance of <see cref="IReadRepository{TEntityType}"/>/</param>
        /// <param name="pageRequest">The page information for the query.</param>
        /// <param name="where">Where conditions for the query.</param>
        /// <returns>A paginated collection of results.</returns>
        public static async Task<ListResult<TEntityType>> ToListAsync<TEntityType>(this IReadRepository<TEntityType> repository, PageRequest pageRequest, params Expression<Func<TEntityType, bool>>[] @where)
            where TEntityType : class
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (pageRequest is null)
            {
                throw new ArgumentNullException(nameof(pageRequest));
            }

            var result = await repository.ListAsync(pageRequest, where);

            return result;
        }

        /// <summary>
        /// Adds multiple items to the repository as instances <typeparamref name="TEntityType"/>.
        /// </summary>
        /// <typeparam name="TEntityType">The type of entity.</typeparam>
        /// <param name="repository">The repository as an instance of <see cref="IWriteRepository{TEntityType}"/>/</param>
        /// <param name="entities">The entities to add as instances <typeparamref name="TEntityType"/>.</param>
        /// <returns>The added items as instance of <typeparamref name="TEntityType"/>.</returns>
        public static async Task<List<TEntityType>> AddManyAsync<TEntityType>(this IWriteRepository<TEntityType> repository, IEnumerable<TEntityType> entities)
            where TEntityType : class
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (entities is null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            List<TEntityType> results = new List<TEntityType>();

            using (var transaction = repository.BeginTransaction())
            {
                foreach (TEntityType entity in entities)
                {
                    TEntityType result = await repository.AddAsync(entity);

                    if (result != null)
                    {
                        results.Add(result);
                    }
                }

                transaction.Commit();
            }

            return results;
        }

        /// <summary>
        /// Finds an item by the corresponding identifier.
        /// </summary>
        /// <typeparam name="TEntityType">The type of entity.</typeparam>
        /// <param name="repository">The repository as an instance of <see cref="IReadRepository{TEntityType}"/>.</param>
        /// <param name="id">The identifier to find by.</param>
        /// <returns>An instance of <typeparamref name="TEntityType"/>.</returns>
        public static async Task<TEntityType> FindByIdAsync<TEntityType>(this IReadRepository<TEntityType> repository, long id)
            where TEntityType : class, IEntity
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var result = await repository.SingleOrDefaultAsync((entity) => entity.Id == id);

            return result;
        }

        /// <summary>
        /// Finds an item by the corresponding account identifier.
        /// </summary>
        /// <typeparam name="TEntityType">The type of entity.</typeparam>
        /// <param name="repository">The repository as an instance of <see cref="IReadRepository{TEntityType}"/>.</param>
        /// <param name="accountId">The account identifier to find by.</param>
        /// <returns>An instance of <typeparamref name="TEntityType"/>.</returns>
        public static async Task<TEntityType> FindByAccountId<TEntityType>(this IReadRepository<TEntityType> repository, long accountId)
            where TEntityType : class, IAccountOwned
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var result = await repository.SingleOrDefaultAsync((entity) => entity.AccountId == accountId);

            return result;
        }

        /// <summary>
        /// Queries the repository with the specified SQL, page request and parameters and returns a paginated list of items that match any where conditions.
        /// </summary>
        /// <typeparam name="TEntityType">The type of entity.</typeparam>
        /// <param name="databaseConnection">The database connection as an instance of <see cref="IDbConnection"/>.</param>
        /// <param name="sql">The SQL to query.</param>
        /// <param name="pageRequest">The page information for the query.</param>
        /// <param name="parameters">Parameters for the query.</param>
        /// <param name="where">Where conditions for the query.</param>
        /// <returns>A paginated collection of results.</returns>
        public static async Task<ListResult<TEntityType>> QueryListResultAsync<TEntityType>(this IDbConnection databaseConnection, string sql, PageRequest pageRequest, params Expression<Func<TEntityType, bool>>[] @where)
            where TEntityType : class
            => await databaseConnection.QueryListResultAsync(sql, pageRequest, null, where);

        /// <summary>
        /// Queries the repository with the specified SQL, page request and parameters and returns a paginated list of items that match any where conditions.
        /// </summary>
        /// <typeparam name="TEntityType">The type of entity.</typeparam>
        /// <param name="databaseConnection">The database connection as an instance of <see cref="IDbConnection"/>.</param>
        /// <param name="sql">The SQL to query.</param>
        /// <param name="pageRequest">The page information for the query.</param>
        /// <param name="parameters">Parameters for the query.</param>
        /// <param name="where">Where conditions for the query.</param>
        /// <returns>A paginated collection of results.</returns>
        public static async Task<ListResult<TEntityType>> QueryListResultAsync<TEntityType>(this IDbConnection databaseConnection, string sql, PageRequest pageRequest, object parameters, params Expression<Func<TEntityType, bool>>[] @where)
            where TEntityType : class
        {
            if (databaseConnection is null)
            {
                throw new ArgumentNullException(nameof(databaseConnection));
            }

            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentException($"'{nameof(sql)}' cannot be null or empty.", nameof(sql));
            }

            if (pageRequest is null)
            {
                throw new ArgumentNullException(nameof(pageRequest));
            }

            var whereSql = string.Empty;
            var dynamicParameters = new DynamicParameters(parameters ?? new { });
            dynamicParameters.Add("pageOffset", (pageRequest.Page - 1) * pageRequest.PageSize);
            dynamicParameters.Add("pageSize", pageRequest.PageSize);
            dynamicParameters.Add("orderBy", pageRequest.OrderBy);

            string tableName = Configuration.GetInstance().Entity<TEntityType>().GetTableName();
            int count = 0;

            foreach (var whereExpression in where)
            {
                WherePart wherePart = whereExpression.ToSql();

                if (!string.IsNullOrEmpty(whereSql)) whereSql += " AND ";

                whereSql += wherePart.Sql;

                foreach (var dynamicParameter in wherePart.Parameters)
                {
                    string parameterName = ":whereExp" + count++;

                    whereSql = whereSql.Replace("@" + dynamicParameter.Key, parameterName);

                    dynamicParameters.Add(parameterName, dynamicParameter.Value);
                }
            }

            whereSql = whereSql.Replace("[" + tableName + "].", "[inner_query].").Replace("[", "\"").Replace("]", "\"");

            if (string.IsNullOrEmpty(whereSql))
            {
                whereSql = "(TRUE = TRUE)";
            }

            // Construct the rest of the query.
            string query = "SELECT * FROM (" + sql + ") inner_query WHERE " + whereSql;
            string mainquery = query + " ORDER BY " + pageRequest.OrderBy + " ASC OFFSET :pageOffset LIMIT :pageSize;";
            string countQuery = "SELECT COUNT(*) FROM (" + query + ") main_query";

            var gridReader = await databaseConnection.QueryMultipleAsync(mainquery + "; " + countQuery + ";", dynamicParameters);

            var result = await gridReader.ReadAsync<TEntityType>();
            int totalCount = await gridReader.ReadSingleAsync<int>();

            return new ListResult<TEntityType>(totalCount, pageRequest.PageSize, pageRequest.Page, result.ToList());
        }
    }
}