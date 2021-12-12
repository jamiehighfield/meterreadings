namespace MeterReadings.DataAccess
{
    /// <summary>
    /// Default implementation of <see cref="IDatabaseConnectionStringProvider"/>.
    /// </summary>
    public class DatabaseConnectionStringProvider : IDatabaseConnectionStringProvider
    {
        /// <summary>
        /// Initialise a new instance of <see cref="DatabaseConnectionStringProvider"/>.
        /// </summary>
        public DatabaseConnectionStringProvider() { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public string GetConnectionString() => Environment.GetEnvironmentVariable("MeterReadingsConnectionString");
    }
}