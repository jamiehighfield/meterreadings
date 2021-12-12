using MeterReadings.Core.Services;
using MeterReadings.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;

namespace MeterReadings.Controllers.Api
{
    /// <summary>
    /// Controller for meter readings.
    /// </summary>
    public class MeterReadingApiController : ApiController
    {
        public MeterReadingApiController(
            MeterReadingsService meterReadingsService,
            TemporaryFileUploadHandler temporaryFileUploadHandler)
        {
            _meterReadingsService = meterReadingsService ?? throw new ArgumentNullException(nameof(meterReadingsService));
            _temporaryFileUploadHandler = temporaryFileUploadHandler ?? throw new ArgumentNullException(nameof(temporaryFileUploadHandler));
        }

        private readonly MeterReadingsService _meterReadingsService;
        private readonly TemporaryFileUploadHandler _temporaryFileUploadHandler;

        /// <summary>
        /// This route retrieves a single meter reading that has been previously submitted by its identifier.
        /// </summary>
        [HttpGet("meter-readings/{id:long}")]
        public async Task<ActionResult<ApiResponse>> RetrieveAsync(long id)
        {
            MeterReadingDto dto = await _meterReadingsService.FindByIdAsync(id);

            return Success(dto);
        }

        /// <summary>
        /// This route retrieves a collection of meter reading that has been previously submitted by their identifiers.
        /// </summary>
        [HttpGet("meter-readings")]
        public async Task<ActionResult<ApiResponse>> QueryAsync()
        {
            PageRequest pageRequest = GetPageRequest();

            ListResult<MeterReadingDto> results = await _meterReadingsService.ListAsync(pageRequest);

            return Success(results);
        }

        /// <summary>
        /// This route processes an uploaded CSV file containing meter readings.
        /// </summary>
        [HttpPost("meter-reading-uploads")]
        public async Task<ActionResult<ApiResponse>> UploadMeterReadingsAsync([FromForm][MaximumFileSize(1024 * 1024)] IFormFile meterReadingsFile)
        {
            if (meterReadingsFile is null)
            {
                throw new ArgumentNullException(nameof(meterReadingsFile));
            }

            using (UploadedFile uploadedMeterReadingsFile = await _temporaryFileUploadHandler.HandleUploadAsync(meterReadingsFile))
            {
                return Success(await _meterReadingsService.PersistMeterReadings(uploadedMeterReadingsFile.Open()));
            }
        }
    }
}