using FluentValidation;

namespace MeterReadings.Core.Services
{
    /// <summary>
    /// Validator for <see cref="MeterReadingDto"/>.
    /// </summary>
    public class MeterReadingDtoValidator : AbstractValidator<MeterReadingDto>
    {
        /// <summary>
        /// Initialise a new instance of <see cref="MeterReadingDtoValidator"/>.
        /// </summary>
        public MeterReadingDtoValidator()
        {
            RuleFor((dto) => dto.Value)
                .Must((value) => value.Length == 5)
                .Must((value) =>
                {
                    return int.TryParse(value, out int number) && number > 0 && number < 99999;
                });
        }
    }
}