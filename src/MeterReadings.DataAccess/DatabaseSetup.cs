using SimpleMigrations;
using SimpleMigrations.DatabaseProvider;
using System.Data;

namespace MeterReadings.DataAccess
{
    /// <summary>
    /// Used to facilitate the setting up of the database for various usage scenarios.
    /// </summary>
    public abstract class DatabaseSetup
    {
        /// <summary>
        /// Initialise a new instance of <see cref="DatabaseSetup"/>.
        /// </summary>
        protected DatabaseSetup(DatabaseConnection databaseConnection)
        {
            DatabaseConnection = databaseConnection ?? throw new ArgumentNullException(nameof(databaseConnection));
        }

        /// <summary>
        /// 
        /// </summary>
        protected DatabaseConnection DatabaseConnection { get; }

        /// <summary>
        /// Executes SQL to the the database from an SQL file as an embedded resource.
        /// </summary>
        /// <param name="sqlFile">The SQL file to execute.</param>
        public void ExecuteSqlFile(string sqlFile)
        {
            if (string.IsNullOrEmpty(sqlFile))
            {
                throw new ArgumentException($"'{nameof(sqlFile)}' cannot be null or empty.", nameof(sqlFile));
            }

            string sqlFileContents = EmbeddedFile.Read(sqlFile);

            ExecuteSql(sqlFileContents);
        }

        /// <summary>
        /// Executes SQL to the the database.
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        public void ExecuteSql(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentException($"'{nameof(sql)}' cannot be null or empty.", nameof(sql));
            }

            using (IDbTransaction transaction = DatabaseConnection.Database.BeginTransaction())
            {
                IDbCommand command = DatabaseConnection.Database.CreateCommand();

                command.CommandText = sql;

                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }

        /// <summary>
        /// Initialises the database.
        /// </summary>
        public virtual void InitialiseDatabase()
        {
            ExecuteSqlFile("Sql.cleardatabase.sql");

            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            RunMigrations();
        }

        /// <summary>
        /// Runs the latest migrations.
        /// </summary>
        protected virtual void RunMigrations()
        {
            SimpleMigrator migrator = new SimpleMigrator(typeof(DatabaseConnection).Assembly, new PostgresqlDatabaseProvider(DatabaseConnection._underlyingDatabaseConnection));

            migrator.Load();
            migrator.MigrateToLatest();
        }
    }

    /// <summary>
    /// Used to facilitate the setting up of the database for the development usage scenario.
    /// </summary>
    public class DevelopmentDatabaseSetup : DatabaseSetup
    {
        /// <summary>
        /// Initialise a new instance of <see cref="DevelopmentDatabaseSetup"/>.
        /// </summary>
        public DevelopmentDatabaseSetup(DatabaseConnection database)
            : base(database)
        { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void InitialiseDatabase()
        {
            base.InitialiseDatabase();

            ExecuteSqlFile("Seeds.developmentseed.sql");
        }
    }

    /// <summary>
    /// Used to facilitate the setting up of the database for the testing usage scenario.
    /// </summary>
    public class TestingDatabaseSetup : DatabaseSetup
    {
        /// <summary>
        /// Initialise a new instance of <see cref="TestingDatabaseSetup"/>.
        /// </summary>
        public TestingDatabaseSetup(DatabaseConnection database)
            : base(database)
        { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void InitialiseDatabase()
        {
            base.InitialiseDatabase();

            ExecuteSqlFile("Seeds.developmentseed.sql");
        }
    }
}