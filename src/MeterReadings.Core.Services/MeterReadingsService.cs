using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation;
using FluentValidation.Results;
using MeterReadings.Core.Entities;
using MeterReadings.Core.Repositories.Interfaces;
using MeterReadings.DataAccess;
using MeterReadings.Shared;
using MeterReadings.Shared.Exceptions;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MeterReadings.Core.Services
{
    /// <summary>
    /// Service for meter readings.
    /// </summary>
    public class MeterReadingsService
    {
        /// <summary>
        /// Initialise a new instance of <see cref="MeterReadingsService"/>.
        /// </summary>
        public MeterReadingsService(
            IMapper mapper,
            IMeterReadingsRepository meterReadingsRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _meterReadingsRepository = meterReadingsRepository ?? throw new ArgumentNullException(nameof(meterReadingsRepository));
        }

        private readonly IMapper _mapper;
        private readonly IMeterReadingsRepository _meterReadingsRepository;

        /// <summary>
        /// Persists meter readings from a CSV file.
        /// </summary>
        /// <param name="fileStream">A stream pointing to the start of a CSV file containing meter readings.</param>
        public async Task<MeterReadingPersistResultDto> PersistMeterReadings(Stream fileStream)
        {
            if (fileStream is null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }

            var readings = await _meterReadingsRepository.FindByIdAsync(123);

            var validator = new MeterReadingValidator();

            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture);

            using (var textReader = new StreamReader(fileStream))
            using (var csvReader = new CsvReader(textReader, configuration))
            {
                IEnumerable<MeterReadingRecord> records = csvReader.GetRecords<MeterReadingRecord>();
                List<MeterReading> meterReadings = new List<MeterReading>();

                // Validate the general structure of the records. Checking for an existing account will be done in the
                // data access layer.
                foreach (var meterReading in records)
                {
                    {
                        meterReadings.Add(new MeterReading()
                        {
                            AccountId = Convert.ToInt64(meterReading.AccountId),
                            SubmittedAt = DateTime.Parse(meterReading.MeterReadingDateTime),
                            Value = meterReading.MeterReadValue
                        });
                    }
                }

                return await PersistMeterReadings(_mapper.Map<List<MeterReadingDto>>(meterReadings));
            }
        }

        /// <summary>
        /// Persists a collection of meter readings.
        /// </summary>
        /// <param name="meterReadings">A collection of <see cref="MeterReadingDto"/>.</param>
        public async Task<MeterReadingPersistResultDto> PersistMeterReadings(List<MeterReadingDto> meterReadings)
        {
            if (meterReadings is null)
            {
                throw new ArgumentNullException(nameof(meterReadings));
            }

            if (meterReadings.Count == 0) return new MeterReadingPersistResultDto()
            {
                AcceptedReadingsCount = 0,
                RejectedReadingsCount = 0,
                AcceptedReadings = new List<MeterReadingDto>()
            };

            // You may wish to perform authentication/authorisation here.
            // Not included for this task.

            // Validate the general structure of the records. Checking for an existing account will be done in the
            // data access layer.
            var validator = new MeterReadingDtoValidator();

            List<MeterReadingDto> validatedMeterReadings = meterReadings.Where((dto) => validator.Validate(dto).IsValid).ToList();

            List<MeterReading> entities = _mapper.Map<List<MeterReading>>(validatedMeterReadings);

            List<MeterReading> results = await _meterReadingsRepository.AddManyAsync(entities);

            return new MeterReadingPersistResultDto()
            {
                AcceptedReadingsCount = results.Count,
                RejectedReadingsCount = meterReadings.Count - results.Count,
                AcceptedReadings = _mapper.Map<List<MeterReadingDto>>(results)
            };
        }

        /// <summary>
        /// Finds a meter reading by its corresponding identifier.
        /// </summary>
        /// <param name="id">The identifier of the meter reading.</param>
        /// <returns>The meter reading as an instance of <see cref="MeterReadingDto"/>.</returns>
        public async Task<MeterReadingDto> FindByIdAsync(long id)
        {
            // You may wish to perform authentication/authorisation here.
            // Not included for this task.

            var entity = await _meterReadingsRepository.FindByIdAsync(id);

            if (entity is null)
                throw new EntityNotFoundException<MeterReading>();

            var dto = _mapper.Map<MeterReadingDto>(entity);

            return dto;
        }

        /// <summary>
        /// Lists all meter readings for the specified page request.
        /// </summary>
        public async Task<ListResult<MeterReadingDto>> ListAsync(PageRequest pageRequest)
        {
            if (pageRequest is null)
            {
                throw new ArgumentNullException(nameof(pageRequest));
            }

            // You may wish to perform authentication/authorisation here.
            // Not included for this task.

            var entities = await _meterReadingsRepository.ToListAsync(pageRequest);

            var dtos = _mapper.Map<List<MeterReadingDto>>(entities.List);

            return new ListResult<MeterReadingDto>(entities.TotalCount, entities.PageSize, entities.Page, dtos);
        }

        /// <summary>
        /// Lists all meter readings for the specified page request.
        /// </summary>
        public async Task<ListResult<MeterReadingDto>> ListAsync(PageRequest pageRequest, long accountId)
        {
            if (pageRequest is null)
            {
                throw new ArgumentNullException(nameof(pageRequest));
            }

            // You may wish to perform authentication/authorisation here.
            // Not included for this task.

            var entities = await _meterReadingsRepository.ToListAsync(pageRequest, (entity) => entity.AccountId == accountId);

            var dtos = _mapper.Map<List<MeterReadingDto>>(entities.List);

            return new ListResult<MeterReadingDto>(entities.TotalCount, entities.PageSize, entities.Page, dtos);
        }
    }

    public class MeterReadingDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("account_id")]
        public long AccountId { get; set; }

        [JsonPropertyName("submitted_at")]
        public DateTime SubmittedAt { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class MeterReadingPersistResultDto
    {
        [JsonPropertyName("accepted_readings_count")]
        public int AcceptedReadingsCount { get; set; }

        [JsonPropertyName("rejected_readings_count")]
        public int RejectedReadingsCount { get; set; }

        [JsonPropertyName("accepted_readings")]
        public List<MeterReadingDto> AcceptedReadings { get; set; }
    }
        
    public class MeterReadingRecord
    {
        public string AccountId { get; set; }

        public string MeterReadingDateTime { get; set; }

        public string MeterReadValue { get; set; }
    }

    public class MeterReadingValidator : AbstractValidator<MeterReadingRecord>
    {
        public MeterReadingValidator()
        {
            RuleFor((item) => item.MeterReadValue)
                .Must((meterReadValue) => meterReadValue.Length == 5)
                .Must((meterReadValue) =>
                {
                    return int.TryParse(meterReadValue, out int number) && number > 0 && number < 99999;
                })
                .WithMessage("Invalid meter read for record");
            RuleFor((item) => item.MeterReadingDateTime)
                .Must((meterReadingDateTime) =>
                {
                    return DateTime.TryParse(meterReadingDateTime, out DateTime parsedDateTime);
                })
                .WithMessage("Invalid date time for record");
        }
    }

    
}