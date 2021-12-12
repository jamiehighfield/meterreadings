using Autofac;
using MeterReadings.Core.Services;
using MeterReadings.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MeterReadings.Tests.IntegrationTests
{
    [Collection("MeterReadingTests")]
    public class MeterReadingsTests : IntegrationTest
    {
        [Fact]
        public async Task CanPersistCsv()
        {
            using (LifetimeScope scope = await WithoutUserAsync())
            {
                MeterReadingsService meterReadingsService = scope.Services.Resolve<MeterReadingsService>();

                MeterReadingPersistResultDto result = null;

                using (Stream stream = File.Open("Data/TestMeterReadings.csv", FileMode.Open))
                {
                    result = await meterReadingsService.PersistMeterReadings(stream);
                }

                var firstMeterRead = await meterReadingsService.FindByIdAsync(result.AcceptedReadings.First().Id);
                var lastMeterRead = await meterReadingsService.FindByIdAsync(result.AcceptedReadings.Last().Id);

                Assert.Equal(result.AcceptedReadings.First().Value, firstMeterRead.Value);
                Assert.Equal(result.AcceptedReadings.First().SubmittedAt, firstMeterRead.SubmittedAt);
                Assert.Equal(result.AcceptedReadings.Last().Value, lastMeterRead.Value);
                Assert.Equal(result.AcceptedReadings.Last().SubmittedAt, lastMeterRead.SubmittedAt);
            }
        }

        /// <summary>
        /// This test checks whether a reading can be persisted more than once.
        /// </summary>
        [Fact]
        public async Task CanPersistOnce()
        {
            using (LifetimeScope scope = await WithoutUserAsync())
            {
                MeterReadingsService meterReadingsService = scope.Services.Resolve<MeterReadingsService>();

                var result = await meterReadingsService.PersistMeterReadings(new List<MeterReadingDto>()
                {
                    new MeterReadingDto()
                    {
                        AccountId = 2344,
                        SubmittedAt = DateTime.Now.ToUniversalTime(),
                        Value = "12345"
                    }
                });

                Assert.Single(result.AcceptedReadings);

                result = await meterReadingsService.PersistMeterReadings(new List<MeterReadingDto>()
                {
                    new MeterReadingDto()
                    {
                        AccountId = 2344,
                        SubmittedAt = DateTime.Now.ToUniversalTime(),
                        Value = "12345"
                    }
                });

                Assert.Empty(result.AcceptedReadings);
            }
        }

        /// <summary>
        /// This test checks whether a reading lower than the latest for that account can be persisted more than once.
        /// </summary>
        [Fact]
        public async Task CanPersistLowerReading()
        {
            using (LifetimeScope scope = await WithoutUserAsync())
            {
                long accountId = 2344;

                MeterReadingsService meterReadingsService = scope.Services.Resolve<MeterReadingsService>();

                var result = await meterReadingsService.PersistMeterReadings(new List<MeterReadingDto>()
                {
                    new MeterReadingDto()
                    {
                        AccountId = accountId,
                        SubmittedAt = DateTime.Now.ToUniversalTime(),
                        Value = "12345"
                    }
                });

                Assert.Single(result.AcceptedReadings);

                result = await meterReadingsService.PersistMeterReadings(new List<MeterReadingDto>()
                {
                    new MeterReadingDto()
                    {
                        AccountId = accountId,
                        SubmittedAt = DateTime.Now.ToUniversalTime(),
                        Value = "00587"
                    }
                });

                Assert.Empty(result.AcceptedReadings);

                result = await meterReadingsService.PersistMeterReadings(new List<MeterReadingDto>()
                {
                    new MeterReadingDto()
                    {
                        AccountId = accountId,
                        SubmittedAt = DateTime.Now.ToUniversalTime(),
                        Value = "22345"
                    }
                });

                Assert.Single(result.AcceptedReadings);

                var allReadings = await meterReadingsService.ListAsync(new PageRequest(1, 100), accountId);

                Assert.Equal(2, allReadings.TotalCount);
                Assert.Equal(2, allReadings.Count);
            }
        }

        /// <summary>
        /// This test determines whether meter readings from one customer aren't returned for another.
        /// </summary>
        [Fact]
        public async Task CanMaintainCrossAccountBoundaries()
        {
            using (LifetimeScope scope = await WithoutUserAsync())
            {
                long firstAccountId = 2344;
                long secondAccountId = 2353;

                MeterReadingsService meterReadingsService = scope.Services.Resolve<MeterReadingsService>();

                var result = await meterReadingsService.PersistMeterReadings(new List<MeterReadingDto>()
                {
                    new MeterReadingDto()
                    {
                        AccountId = firstAccountId,
                        SubmittedAt = DateTime.Now.ToUniversalTime(),
                        Value = "12345"
                    }
                });

                var allReadings = await meterReadingsService.ListAsync(new PageRequest(1, 100), firstAccountId);

                Assert.Equal(1, allReadings.TotalCount);
                Assert.Equal(1, allReadings.Count);

                allReadings = await meterReadingsService.ListAsync(new PageRequest(1, 100), secondAccountId);

                Assert.Equal(0, allReadings.TotalCount);
                Assert.Equal(0, allReadings.Count);
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