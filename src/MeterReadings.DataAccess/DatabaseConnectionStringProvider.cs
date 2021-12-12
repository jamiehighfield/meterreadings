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
        public string GetConnectionString()
        {
            return "Server=meterreadings-do-user-2137260-0.b.db.ondigitalocean.com;Port=25060;Database=meterreadings;User Id=doadmin;Password=HQSMQk8EORrwqliN;";
        }
    }
}