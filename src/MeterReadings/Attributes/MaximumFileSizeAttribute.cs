using System.ComponentModel.DataAnnotations;

namespace MeterReadings
{
    /// <summary>
    /// Annotate a declaration of <see cref="IFormFile"/> to indicate the maximum size that should be accepted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class MaximumFileSizeAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initialise a new instance of <see cref="MaximumFileSizeAttribute"/>.
        /// </summary>
        /// <param name="maximumSize">The maximum size that should be accepted in bytes..</param>
        public MaximumFileSizeAttribute(int maximumSize)
        {
            if (maximumSize < 0) throw new ArgumentOutOfRangeException("Maximum size is an invalid value");

            _maximumSize = maximumSize;
        }

        private readonly int _maximumSize;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var file = (IFormFile)value;

                if (file.Length > _maximumSize)
                {
                    return new ValidationResult($"File size exceeds the maximum allowed size");
                }
            }

            return null;
        }
    }
}