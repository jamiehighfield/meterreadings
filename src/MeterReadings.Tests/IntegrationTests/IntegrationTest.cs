using Autofac;
using Autofac.Core.Lifetime;
using MeterReadings.Configuration;
using MeterReadings.DataAccess;
using System;
using System.Threading.Tasks;

namespace MeterReadings.Tests.IntegrationTests
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class IntegrationTest : IDisposable
    {
        /// <summary>
        /// Initialise a new instance of <see cref="IntegrationTest"/>.
        /// </summary>
        protected IntegrationTest()
        {
            InitialiseServiceProvider();
            InitialiseDatabase();
        }

        private IContainer _services;

        /// <summary>
        /// Gets the root service provider as an instance of <see cref="IContainer"/>.
        /// </summary>
        /// <remarks>
        /// Note: this is a non-scoped service provider and may not provide the correct service resolution. Instead, use <see cref="WithoutUserAsync"/> to provide a lifetime scope.
        /// </remarks>
        protected IContainer Services => _services;

        /// <summary>
        /// Initiates a new lifetime scope from the root service provider.
        /// </summary>
        /// <returns>A lifetime scope as an instance of <see cref="LifetimeScope"/>.</returns>
        protected virtual LifetimeScope InitialiseLifetimeScope()
        {
            return new LifetimeScope(Services.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag, (containerBuilder) =>
            {
                containerBuilder.RegisterType<TestingDatabaseSetup>()
                    .As<DatabaseSetup>()
                    .InstancePerLifetimeScope();
            }));
        }

        /// <summary>
        /// Initiates a new lifetime scope from the root service provider without a user context.
        /// </summary>
        /// <returns>A lifetime scope as an instance of <see cref="LifetimeScope"/>.</returns>
        protected LifetimeScope WithoutUser()
        {
            return new LifetimeScope(Services.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag, (containerBuilder) =>
            {
                containerBuilder.RegisterType<TestingDatabaseSetup>()
                    .As<DatabaseSetup>()
                    .InstancePerLifetimeScope();
            }));
        }

        /// <summary>
        /// Initiates a new lifetime scope from the root service provider without a user context.
        /// </summary>
        /// <returns>A lifetime scope as an instance of <see cref="LifetimeScope"/>.</returns>
        protected async Task<LifetimeScope> WithoutUserAsync()
        {
            await Task.CompletedTask;

            return new LifetimeScope(Services.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag, (containerBuilder) =>
            {
                containerBuilder.RegisterType<TestingDatabaseSetup>()
                    .As<DatabaseSetup>()
                    .InstancePerLifetimeScope();
            }));
        }

        /// <summary>
        /// Initialises the service provider for use in testing.
        /// </summary>
        private protected void InitialiseServiceProvider()
        {
            if (_services is null)
            {
                var containerBuilder = new ContainerBuilder();

                containerBuilder.RegisterMeterReadingsServices();

                _services = containerBuilder.Build();
            }
        }

        /// <summary>
        /// Initialises the database for use in testing.
        /// </summary>
        private protected virtual void InitialiseDatabase()
        {
            using (LifetimeScope scope = InitialiseLifetimeScope())
            {
                DatabaseSetup databaseSetup = scope.Services.Resolve<DatabaseSetup>();

                databaseSetup.InitialiseDatabase();
            }
        }

        /// <summary>
        /// Seeds the database from a SQL.
        /// </summary>
        /// <param name="sqlFile">The SQL to use as a seed.</param>
        protected void ExecuteSql(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentException($"'{nameof(sql)}' cannot be null or empty.", nameof(sql));
            }

            using (LifetimeScope scope = InitialiseLifetimeScope())
            {
                var databaseSetup = scope.Services.Resolve<DatabaseSetup>();

                databaseSetup.ExecuteSql(sql);
            }
        }

        /// <summary>
        /// Seeds the database from an SQL file as an embedded resource.
        /// </summary>
        /// <param name="sqlFile">The SQL file to use as a seed.</param>
        protected void ExecuteSqlFile(string sqlFile)
        {
            if (string.IsNullOrEmpty(sqlFile))
            {
                throw new ArgumentException($"'{nameof(sqlFile)}' cannot be null or empty.", nameof(sqlFile));
            }

            using (LifetimeScope scope = InitialiseLifetimeScope())
            {
                var databaseSetup = scope.Services.Resolve<DatabaseSetup>();

                databaseSetup.ExecuteSqlFile(sqlFile);
            }
        }

        /// <summary>
        /// Override this method to perform a task before testing this collection begins.
        /// </summary>
        /// <returns></returns>
        protected virtual void BeforeCollectionTest() { }

        /// <summary>
        /// Override this method to perform a task after testing this collection has completed.
        /// </summary>
        /// <returns></returns>
        protected virtual void AfterCollectionTest() { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void Dispose() => Services?.Dispose();
    }

    public class LifetimeScope : IDisposable
    {
        /// <summary>
        /// Initialise a new instance of <see cref="LifetimeScope"/>.
        /// </summary>
        /// <param name="services">The scoped service provider as an instance of <see cref="ILifetimeScope"/>.</param>
        public LifetimeScope(ILifetimeScope services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// Gets the scoped service provider as an instance of <see cref="ILifetimeScope"/>.
        /// </summary>
        public ILifetimeScope Services { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void Dispose() => Services?.Dispose();
    }

    public class UnauthenticatedLifetimeScope : LifetimeScope
    {
        /// <summary>
        /// Initialise a new instance of <see cref="UnauthenticatedLifetimeScope"/>.
        /// </summary>
        /// <param name="services">The scoped service provider as an instance of <see cref="ILifetimeScope"/>.</param>
        public UnauthenticatedLifetimeScope(ILifetimeScope services)
            : base(services)
        { }
    }
}