using Dapper;
using MeterReadings.Core.Entities;
using MeterReadings.Core.Repositories.Interfaces;
using MeterReadings.Shared;
using System.Data;
using System.Linq.Expressions;

namespace MeterReadings.DataAccess.Repositories
{
    /// <summary>
    /// Default implementation of <see cref="IMeterReadingsRepository"/>.
    /// </summary>
    public class MeterReadingsRepository : IMeterReadingsRepository
    {
        /// <summary>
        /// Initialise a new instance of <see cref="MeterReadingsRepository"/>.
        /// </summary>
        public MeterReadingsRepository(DatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection ?? throw new ArgumentNullException(nameof(databaseConnection));
        }

        private readonly DatabaseConnection _databaseConnection;

        public async Task<List<MeterReading>> ListAllAsync()
        {
            var result = await _databaseConnection._underlyingDatabaseConnection.QueryAsync<MeterReading>(@"
                SELECT * from meter_readings;");

            return result.ToList();
        }

        public async Task<MeterReading> AddAsync(MeterReading entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = await _databaseConnection.Database.QuerySingleAsync<MeterReading>(@"
                INSERT INTO meter_readings (
                    account_id,
                    submitted_at,
                    value)
                VALUES (
                    :accountid,
                    :submittedat,
                    :value)
                RETURNING *;", new
            {
                accountid = entity.AccountId,
                submittedat = entity.SubmittedAt,
                value = entity.Value
            });

            return result;
        }

        public async Task<ListResult<MeterReading>> ListAsync(PageRequest pageRequest, params Expression<Func<MeterReading, bool>>[] where)
        {
            if (pageRequest is null)
            {
                throw new ArgumentNullException(nameof(pageRequest));
            }

            return await _databaseConnection.Database.QueryListResultAsync("SELECT * FROM meter_readings", pageRequest, null, where);
        }

        public IDbTransaction BeginTransaction()
        {
            return _databaseConnection.Database.BeginTransaction();
        }
    }
}