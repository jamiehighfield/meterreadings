using AutoMapper;
using MeterReadings.Core.Entities;
using MeterReadings.Core.Services;

namespace MeterReadings.Configuration.MappingProfiles
{
    public class MeterReadingMappingProfile : Profile
    {
        public MeterReadingMappingProfile()
        {
            CreateMap<MeterReading, MeterReadingDto>()
                .ReverseMap();
        }
    }
}