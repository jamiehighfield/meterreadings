using MeterReadings.Core.Entities;

namespace MeterReadings.Core.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for interfacing with the meter readings table.
    /// </summary>
    public interface IMeterReadingsRepository : IRepository, IReadRepository<MeterReading>, IWriteRepository<MeterReading>
    {
        Task<List<MeterReading>> ListAllAsync();
    }
}