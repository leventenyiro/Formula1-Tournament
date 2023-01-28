using AutoMapper;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.Profiles
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Driver, DriverDto>().ReverseMap();
            CreateMap<Race, RaceDto>().ReverseMap();
            CreateMap<Result, ResultDto>().ReverseMap();
            CreateMap<Season, SeasonDto>().ReverseMap();
        }
    }
}
