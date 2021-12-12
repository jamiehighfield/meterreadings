namespace MeterReadings.Shared
{
    /// <summary>
    /// Used to determine the page for paginated queries.
    /// </summary>
    public class PageRequest
    {
        /// <summary>
        /// Initialise a new instance of <see cref="PageRequest"/>.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        public PageRequest(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;

            OrderBy = "id";
        }

        /// <summary>
        /// Initialise a new instance of <see cref="PageRequest"/>.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="orderBy">The column to order by.</param>
        public PageRequest(int page, int pageSize, string orderBy)
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                throw new ArgumentException($"'{nameof(orderBy)}' cannot be null or empty.", nameof(orderBy));
            }

            Page = page;
            PageSize = pageSize;
            OrderBy = orderBy;
        }

        /// <summary>
        /// Gets the page number.
        /// </summary>
        public int Page { get; }

        /// <summary>
        /// Gets the page size.
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Gets the column to order by.
        /// </summary>
        public string OrderBy { get; }
    }
}