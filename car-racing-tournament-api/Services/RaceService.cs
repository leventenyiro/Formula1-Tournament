using AutoMapper;
using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.Services
{
    public class RaceService : IRace
    {
        private readonly FormulaDbContext _formulaDbContext;
        private IMapper _mapper;

        public RaceService(FormulaDbContext formulaDbContext, IMapper mapper)
        {
            _formulaDbContext = formulaDbContext;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, Race Race, string ErrorMessage)> GetRaceById(Guid id)
        {
            var race = _formulaDbContext.Races.Where(e => e.Id == id).FirstOrDefault();
            if (race != null)
            {
                return (true, race, null);
            }
            return (false, null, "Race not found");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateRace(Guid id, RaceDto raceDto)
        {
            var raceObj = _formulaDbContext.Races.Where(e => e.Id == id).FirstOrDefault();
            if (raceObj != null)
            {
                raceObj.Name = raceDto.Name;
                raceObj.DateTime = raceDto.DateTime;
                _formulaDbContext.Races.Update(raceObj);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Race not found");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteRace(Guid id)
        {
            var race = _formulaDbContext.Races.Where(e => e.Id == id).FirstOrDefault();
            if (race != null)
            {
                _formulaDbContext.Races.Remove(race);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Race not found");
        }

        public async Task<(bool IsSuccess, List<Result> Results, string ErrorMessage)> GetResultsByRaceId(Guid raceId)
        {
            var results = _formulaDbContext.Results.Where(x => x.RaceId == raceId).ToList();
            if (results != null)
            {
                return (true, results, null);
            }
            return (false, null, "No results found");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> AddResult(Guid raceId, ResultDto resultDto)
        {
            if (resultDto != null)
            {
                var result = _mapper.Map<Result>(resultDto);
                result.Id = Guid.NewGuid();
                _formulaDbContext.Add(result);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Please provide the result data");
        }
    }
}
