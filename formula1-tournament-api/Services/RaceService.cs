using formula1_tournament_api.Data;
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

        public async Task<(bool IsSuccess, string ErrorMessage)> AddRace(Race race)
        {
            if (race != null)
            {
                race.Id = Guid.NewGuid();
                _formulaDbContext.Add(race);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Please provide the race data");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteRace(Guid id)
        {
            var race = _formulaDbContext.Race.Where(e => e.Id == id).FirstOrDefault();
            if (race != null)
            {
                _formulaDbContext.Race.Remove(race);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Race not found");
        }

        public async Task<(bool IsSuccess, List<Race> Race, string ErrorMessage)> GetAllRaces()
        {
            var races = _formulaDbContext.Race.ToList();
            if (races != null)
            {
                return (true, races, null);
            }
            return (false, null, "No races found");
        }

        public async Task<(bool IsSuccess, Race Race, string ErrorMessage)> GetRaceById(Guid id)
        {
            var race = _formulaDbContext.Race.Where(e => e.Id == id).FirstOrDefault();
            if (race != null)
            {
                return (true, race, null);
            }
            return (false, null, "Race not found");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateRace(Guid id, Race race)
        {
            var raceObj = _formulaDbContext.Race.Where(e => e.Id == id).FirstOrDefault();
            if (raceObj != null)
            {
                raceObj.Name = race.Name;
                _formulaDbContext.Race.Update(raceObj);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Race not found");
        }
    }
}
