namespace MeterReadings.Shared.Exceptions
{
    /// <summary>
    /// Extend from this class to create an exception. This will be handled differently to a standard exception by the exception filter.
    /// </summary>
    public abstract class MeterReadingsException : Exception
    {
        /// <summary>
        /// Initialise a new instance of <see cref="MeterReadingsException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MeterReadingsException(string message)
            : base(message)
        { }
    }
}