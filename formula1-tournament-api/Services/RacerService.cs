using formula1_tournament_api.Data;
using formula1_tournament_api.Interfaces;
using formula1_tournament_api.Models;

namespace formula1_tournament_api.Services
{
    public class RacerService : IRacer
    {
        private readonly FormulaDbContext _formulaDbContext;

        public RacerService(FormulaDbContext formulaDbContext)
        {
            _formulaDbContext = formulaDbContext;
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> AddRacer(Racer racer, Guid adminId)
        {
            // is the adminId the real admin? racer.Season.UserId
            bool isAdmin = _formulaDbContext.Season.Where(x => x.Id == racer.SeasonId).FirstOrDefault().UserId.Equals(adminId);
            if (racer != null || isAdmin)
            {
                racer.Id = Guid.NewGuid();
                _formulaDbContext.Add(racer);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Please provide the racer data");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteRacer(Guid id)
        {
            var racer = _formulaDbContext.Racer.Where(e => e.Id == id).FirstOrDefault();
            if (racer != null)
            {
                _formulaDbContext.Racer.Remove(racer);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Racer not found");
        }

        public async Task<(bool IsSuccess, List<Racer> Racer, string ErrorMessage)> GetAllRacers()
        {
            var racers = _formulaDbContext.Racer.ToList();
            if (racers != null)
            {
                return (true, racers, null);
            }
            return (false, null, "No racers found");
        }

        public async Task<(bool IsSuccess, Racer Racer, string ErrorMessage)> GetRacerById(Guid id)
        {
            var racer = _formulaDbContext.Racer.Where(e => e.Id == id).FirstOrDefault();
            if (racer != null)
            {
                return (true, racer, null);
            }
            return (false, null, "Racer not found");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateRacer(Guid id, Racer racer)
        {
            var racerObj = _formulaDbContext.Racer.Where(e => e.Id == id).FirstOrDefault();
            if (racerObj != null)
            {
                racerObj.Name = racer.Name;
                //racerObj.TeamId = racer.TeamId; just when point is null, every race
                _formulaDbContext.Racer.Update(racerObj);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Racer not found");
        }
    }
}
