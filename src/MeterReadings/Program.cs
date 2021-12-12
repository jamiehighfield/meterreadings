using Autofac;
using Autofac.Extensions.DependencyInjection;
using MeterReadings.Configuration;
using MeterReadings.DataAccess;

namespace MeterReadings
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>((containerBuilder) =>
            {
                containerBuilder.RegisterMeterReadingsServices();
            });

            var app = builder.Build();

            using (IServiceScope scope = app.Services.CreateScope())
            {
                Console.WriteLine("Dropping database");

                DatabaseSetup dbSetup = scope.ServiceProvider.GetRequiredService<DatabaseSetup>();

                dbSetup.InitialiseDatabase();
            }

            // Configure the HTTP request pipeline.
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}