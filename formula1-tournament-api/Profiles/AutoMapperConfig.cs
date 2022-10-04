using AutoMapper;
using formula1_tournament_api.DTO;
using formula1_tournament_api.Models;

namespace formula1_tournament_api.Profiles
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
