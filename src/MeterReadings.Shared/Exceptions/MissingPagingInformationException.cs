namespace MeterReadings.Shared.Exceptions
{
    /// <summary>
    /// This exception is thrown if paging information is required, but is missing.
    /// </summary>
    public class MissingPagingInformationException : MeterReadingsException
    {
        /// <summary>
        /// Initialise a new instance of <see cref="MissingPagingInformationException"/>.
        /// </summary>
        public MissingPagingInformationException()
            : base("Missing paging information")
        { }
    }
}