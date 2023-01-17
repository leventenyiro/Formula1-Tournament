using AutoMapper;
using api.Data;
using api.DTO;
using api.Interfaces;
using api.Models;

namespace api.Services
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

        public async Task<(bool IsSuccess, string ErrorMessage)> AddRace(RaceDto raceDto)
        {
            if (raceDto != null)
            {
                var race = _mapper.Map<Race>(raceDto);
                race.Id = Guid.NewGuid();
                _formulaDbContext.Add(race);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Please provide the race data");
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

        public async Task<(bool IsSuccess, List<Race> Races, string ErrorMessage)> GetAllRacesBySeasonId(Guid seasonId)
        {
            var races = _formulaDbContext.Races.Where(x => x.SeasonId == seasonId).ToList();
            if (races != null)
            {
                return (true, races, null);
            }
            return (false, null, "No races found");
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
    }
}
