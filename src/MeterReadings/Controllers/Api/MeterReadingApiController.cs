using MeterReadings.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;

namespace MeterReadings.Controllers.Api
{

    public class MeterReadingApiController : ApiController
    {
        public MeterReadingApiController(MeterReadingsService meterReadingsService, TemporaryFileUploadHandler temporaryFileUploadHandler)
        {
            _meterReadingsService = meterReadingsService ?? throw new ArgumentNullException(nameof(meterReadingsService));
            _temporaryFileUploadHandler = temporaryFileUploadHandler ?? throw new ArgumentNullException(nameof(temporaryFileUploadHandler));
        }

        private readonly MeterReadingsService _meterReadingsService;
        private readonly TemporaryFileUploadHandler _temporaryFileUploadHandler;

        [HttpGet("meter-readings/{id:long}")]
        public async Task<ActionResult<ApiResponse>> RetrieveAsync(long id)
        {
            MeterReadingDto dto = await _meterReadingsService.FindByIdAsync(id);

            return Success(dto);
        }

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

    public class MeterReadingUploadResult
    {
        [DataMember(Name = "accepted_readings_count")]
        public int AcceptedReadingsCount { get; set; }

        [DataMember(Name = "rejected_readings_count")]
        public int RejectedReadingsCount { get; set; }


    }
}