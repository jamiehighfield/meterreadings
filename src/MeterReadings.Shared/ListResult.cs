using System.Text.Json.Serialization;

namespace MeterReadings.Shared
{
    /// <summary>
    /// Used to support pagination in the data access layer.
    /// </summary>
    public class ListResult<TItemType>
    {
        /// <summary>
        /// Initialise a new instance of <see cref="ListResult{TItemType}"/>.
        /// </summary>
        /// <param name="totalCount">The total number of items in the repository.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="page">The page number.</param>
        /// <param name="list">The list of items in the result.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ListResult(long totalCount, int pageSize, int page, List<TItemType> list)
        {
            TotalCount = totalCount;
            PageSize = pageSize;
            Page = page;
            List = list ?? throw new ArgumentNullException(nameof(list));
        }

        /// <summary>
        /// Gets the total number of items in the repository.
        /// </summary>
        [JsonPropertyName("total_count")]
        public long TotalCount { get; }

        /// <summary>
        /// Gets the number of items in the list result.
        /// </summary>
        [JsonPropertyName("count")]
        public long Count => List.Count;

        /// <summary>
        /// Gets the page size.
        /// </summary>
        [JsonPropertyName("page_size")]
        public int PageSize { get; }

        /// <summary>
        /// Gets the page number.
        /// </summary>
        [JsonPropertyName("page")]
        public int Page { get; }

        /// <summary>
        /// Gets the list of items in the result.
        /// </summary>
        [JsonPropertyName("data")]
        public List<TItemType> List { get; }
    }
}