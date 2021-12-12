using ExpressionExtensionSQL;

namespace MeterReadings.Core.Entities
{
    /// <summary>
    /// Implement this interface to indicate that the implementing entity is linked to an account.
    /// </summary>
    public interface IAccountOwned
    {
        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        [ColumnName("account_id")]
        long AccountId { get; set; }
    }
}