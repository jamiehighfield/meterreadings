using Autofac;
using AutoMapper;
using MeterReadings.Configuration.MappingProfiles;
using MeterReadings.Core.Services;
using MeterReadings.DataAccess;
using MeterReadings.DataAccess.Repositories;
using System.Reflection;

namespace MeterReadings.Configuration
{
    /// <summary>
    /// Helper class to initialise dependency injection.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers all unregistered services, such as repositories, to a container builder.
        /// </summary>
        /// <param name="containerBuilder">The container builder to register services to as an instance of <see cref="ContainerBuilder"/>.</param>
        public static void RegisterMeterReadingsServices(this ContainerBuilder containerBuilder)
        {
            if (containerBuilder is null)
            {
                throw new ArgumentNullException(nameof(containerBuilder));
            }

            // Adds AutoMapper
            containerBuilder.RegisterInstance(ConfigureMappings())
                .AsImplementedInterfaces()
                .SingleInstance();

            containerBuilder.RegisterType<DatabaseConnection>()
                .AsSelf()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<DevelopmentDatabaseSetup>()
                .As<DatabaseSetup>()
                .InstancePerLifetimeScope();
            containerBuilder.RegisterType<DatabaseConnectionStringProvider>()
                .AsImplementedInterfaces()
                .SingleInstance();

            containerBuilder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(MeterReadingsRepository)))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(MeterReadingsService)))
                .AsSelf()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<TemporaryFileUploadHandler>()
                .AsSelf()
                .SingleInstance();
        }

        public static IMapper ConfigureMappings()
        {
            var configuration = new MapperConfiguration((mapperConfiguration) =>
            {
                mapperConfiguration.AddProfile(new MeterReadingMappingProfile());
            });

            return configuration.CreateMapper();
        }
    }
}