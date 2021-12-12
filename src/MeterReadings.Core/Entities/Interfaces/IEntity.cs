using ExpressionExtensionSQL;

namespace MeterReadings.Core.Entities
{
    public interface IEntity
    {
        [ColumnName("id")]
        public long Id { get; set; }
    }
}