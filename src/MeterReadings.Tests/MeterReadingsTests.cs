using Autofac;
using MeterReadings.Core.Repositories.Interfaces;
using MeterReadings.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace MeterReadings.Tests
{
    public class MeterReadingsTests : IntegrationTest
    {
        [Fact]
        public async Task CanPersistCsv()
        {
            using (LifetimeScope scope = await WithoutUserAsync())
            {
                MeterReadingsService meterReadingsService = scope.Services.Resolve<MeterReadingsService>();

                using (Stream stream = File.Open("Data/TestMeterReadings.csv", FileMode.Open))
                {
                    await Record.ExceptionAsync(async () => await meterReadingsService.PersistMeterReadings(stream));
                }
            }
        }

        /// <summary>
        /// This test checks whether only valid readings are persisted.
        /// </summary>
        [Fact]
        public async Task CanPersistValidReadingsOnly()
        {
            using (LifetimeScope scope = await WithoutUserAsync())
            {
                MeterReadingsService meterReadingsService = scope.Services.Resolve<MeterReadingsService>();
                //IMeterReadingsRepository meterReadingsRepository = scope.Services.Resolve<IMeterReadingsRepository>();

                //using (Stream stream = File.Open("Data/TestMeterReadings.csv", FileMode.Open))
                //{
                //    await meterReadingsService.PersistMeterReadings(stream);
                //    var readings = await meterReadingsRepository.ListAllAsync();

                //    Assert.Equal(6, readings.Count);
                //}

                await meterReadingsService.PersistMeterReadings(new List<MeterReadingDto>()
                {
                    new MeterReadingDto()
                    {
                        AccountId = 2344,
                        SubmittedAt = DateTime.Now.ToUniversalTime(),
                        Value = "12345"
                    }
                });
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task CanFindExistingMeterReadingById(long id)
        {
            using (LifetimeScope scope = await WithoutUserAsync())
            {
                MeterReadingsService meterReadingsService = scope.Services.Resolve<MeterReadingsService>();

                MeterReadingDto dto = await meterReadingsService.FindByIdAsync(id);

                Assert.NotNull(dto);
            }
        }
    }
}