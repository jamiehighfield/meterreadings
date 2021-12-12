namespace MeterReadings.DataAccess
{
    /// <summary>
    /// Implement this interface to indicate that the implementing class is a connection string provider. This can be used when initiating a new instance of <see cref="DatabaseConnection"/>.
    /// </summary>
    public interface IDatabaseConnectionStringProvider
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <returns>The connection string.</returns>
        string GetConnectionString();
    }
}