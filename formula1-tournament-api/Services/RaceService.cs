using formula1_tournament_api.Data;
using formula1_tournament_api.DTO;
using formula1_tournament_api.Interfaces;
using formula1_tournament_api.Models;

namespace formula1_tournament_api.Services
{
    public class RaceService : IRace
    {
        private readonly FormulaDbContext _formulaDbContext;

        public RaceService(FormulaDbContext formulaDbContext)
        {
            _formulaDbContext = formulaDbContext;
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> AddRace(RaceDto raceDto)
        {
            if (raceDto != null)
            {
                _formulaDbContext.Add(new Race
                {
                    Id = Guid.NewGuid(),
                    Name = raceDto.Name,
                    DateTime = raceDto.DateTime,
                    SeasonId = raceDto.SeasonId
                });
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
