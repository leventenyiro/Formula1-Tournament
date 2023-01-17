using AutoMapper;
using api.DTO;
using api.Models;

namespace api.Profiles
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Driver, DriverDto>().ReverseMap();
            CreateMap<Race, RaceDto>().ReverseMap();
            CreateMap<Result, ResultDto>().ReverseMap();
        }
    }
}
