using Npgsql;
using System.Data;

namespace MeterReadings.DataAccess
{
    public class DatabaseConnection
    {
        /// <summary>
        /// Initialise a new instance of <see cref="DatabaseConnection"/>.
        /// </summary>
        public DatabaseConnection(IDatabaseConnectionStringProvider databaseConnectionStringProvider)
        {
            _underlyingDatabaseConnection = new NpgsqlConnection(databaseConnectionStringProvider.GetConnectionString());
        }

        public NpgsqlConnection _underlyingDatabaseConnection;

        /// <summary>
        /// Gets the underlying database as an instance of <see cref="NpgsqlConnection"/>.
        /// </summary>
        public NpgsqlConnection Database
        {
            get
            {
                if (_underlyingDatabaseConnection != null && _underlyingDatabaseConnection.State == ConnectionState.Closed)
                    _underlyingDatabaseConnection.Open();

                return _underlyingDatabaseConnection;
            }
        }
    }
}