using ExpressionExtensionSQL;

namespace MeterReadings.Core.Entities
{
    public class Account : IAccountOwned
    {
        public long Id { get; set; }

        [ColumnName("account_id")]
        public long AccountId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}