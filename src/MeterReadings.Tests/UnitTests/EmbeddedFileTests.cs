using MeterReadings.DataAccess;
using Xunit;

namespace MeterReadings.Tests.UnitTests
{
    public class EmbeddedFileTests
    {
        /// <summary>
        /// This test determines whether the embedded file reader in the data access layer works as intended.
        /// </summary>
        [Fact]
        public void CanReadEmbeddedFile()
        {
            string fileContents = EmbeddedFile.Read("Seeds.developmentseed.sql");

            Assert.NotNull(fileContents);
            Assert.NotEqual("", fileContents);
        }
    }
}