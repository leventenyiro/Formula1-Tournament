using AutoMapper;
using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using Microsoft.EntityFrameworkCore;

namespace car_racing_tournament_api.Services
{
    public class RaceService : IRace
    {
        private readonly FormulaDbContext _formulaDbContext;
        private IMapper _mapper;

        private const string RACE_NOT_FOUND = "Race not found";

        public RaceService(FormulaDbContext formulaDbContext, IMapper mapper)
        {
            _formulaDbContext = formulaDbContext;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, Race? Race, string? ErrorMessage)> GetRaceById(Guid id)
        {
            var race = await _formulaDbContext.Races.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (race == null)
                return (false, null, RACE_NOT_FOUND);

            return (true, race, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateRace(Guid id, RaceDto raceDto)
        {
            var raceObj = await _formulaDbContext.Races.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (raceObj == null)
                return (false, RACE_NOT_FOUND);

            raceObj.Name = raceDto.Name;
            raceObj.DateTime = raceDto.DateTime;
            _formulaDbContext.Races.Update(raceObj);
            _formulaDbContext.SaveChanges();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteRace(Guid id)
        {
            var race = await _formulaDbContext.Races.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (race == null)
                return (false, RACE_NOT_FOUND);

            _formulaDbContext.Races.Remove(race);
            _formulaDbContext.SaveChanges();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, List<Result>? Results, string? ErrorMessage)> GetResultsByRaceId(Guid raceId)
        {
            var results = await _formulaDbContext.Results.Where(x => x.RaceId == raceId).ToListAsync();
            if (results == null)
                return (false, null, "No results found");

            return (true, results, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddResult(Guid raceId, ResultDto resultDto)
        {
            if (resultDto == null)
                return (false, "Please provide the result data");

            var result = _mapper.Map<Result>(resultDto);
            result.Id = Guid.NewGuid();
            await _formulaDbContext.AddAsync(result);
            _formulaDbContext.SaveChanges();
            
            return (true, null);
        }
    }
}
