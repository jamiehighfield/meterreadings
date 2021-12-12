using ExpressionExtensionSQL;

namespace MeterReadings.Core.Entities
{
    [TableName("meter_readings")]
    public class MeterReading : IEntity, IAccountOwned
    {
        [ColumnName("id")]
        public long Id { get; set; }

        [ColumnName("account_id")]
        public long AccountId { get; set; }

        [ColumnName("submitted_at")]
        public DateTime SubmittedAt { get; set; }

        [ColumnName("value")]
        public string Value { get; set; }
    }
}