using Autofac;
using MeterReadings.Core.Services;
using MeterReadings.Shared;
using System.Threading.Tasks;
using Xunit;

namespace MeterReadings.Tests.IntegrationTests
{
    [Collection("MeterReadingTests")]
    public class PaginationTests : IntegrationTest
    {
        /// <summary>
        /// This test determines whether paginated queries works correctly.
        /// </summary>
        [Fact]
        public async Task CanReturnPaginatedResults()
        {
            using (LifetimeScope scope = WithoutUser())
            {
                MeterReadingsService meterReadingsService = scope.Services.Resolve<MeterReadingsService>();

                var results = await meterReadingsService.ListAsync(new PageRequest(1, 200));

                Assert.Equal(2, results.TotalCount);
                Assert.Equal(2, results.Count);

                results = await meterReadingsService.ListAsync(new PageRequest(2, 200));

                Assert.Equal(2, results.TotalCount);
                Assert.Equal(0, results.Count);

                results = await meterReadingsService.ListAsync(new PageRequest(2, 1));

                Assert.Equal(2, results.TotalCount);
                Assert.Equal(1, results.Count);

                results = await meterReadingsService.ListAsync(new PageRequest(5, 200));

                Assert.Equal(2, results.TotalCount);
                Assert.Equal(0, results.Count);
            }
        }
    }
}