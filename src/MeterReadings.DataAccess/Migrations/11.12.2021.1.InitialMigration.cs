using SimpleMigrations;

namespace MeterReadings.DataAccess.Migrations
{
    [Migration(1, "Initial migration")]
    public class InitialMigration : Migration
    {
        protected override void Up()
        {
            Execute(@"
                CREATE TABLE accounts
                (
                    id         BIGSERIAL CONSTRAINT accounts_pk PRIMARY KEY,
                    account_id BIGINT NOT NULL,
                    first_name VARCHAR(128),
                    last_name  VARCHAR(128) NOT NULL
                );

                CREATE UNIQUE INDEX accounts_id_uindex
                    ON accounts (id);

                CREATE TABLE meter_readings
                (
                    id          BIGSERIAL CONSTRAINT meter_readings_pk PRIMARY KEY,
                    account_id   BIGINT NOT NULL,
                    submitted_at TIMESTAMP NOT NULL,
                    value       VARCHAR(5) NOT NULL
                );

                CREATE UNIQUE INDEX meter_readings_id_uindex
                    ON meter_readings (id);");
        }

        protected override void Down() { }
    }
}