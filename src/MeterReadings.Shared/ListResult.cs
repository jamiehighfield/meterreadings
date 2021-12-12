using System.Collections;

namespace MeterReadings.Shared
{
    /// <summary>
    /// Used to support pagination in the data access layer.
    /// </summary>
    public class ListResult<TItemType> : IEnumerable<TItemType>
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
        public long TotalCount { get; }

        /// <summary>
        /// Gets the number of items in the list result.
        /// </summary>
        public long Count => List.Count;

        /// <summary>
        /// Gets the page size.
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Gets the page number.
        /// </summary>
        public int Page { get; }

        /// <summary>
        /// Gets the list of items in the result.
        /// </summary>
        public List<TItemType> List { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public IEnumerator<TItemType> GetEnumerator() => List.GetEnumerator();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)List).GetEnumerator();
    }
}