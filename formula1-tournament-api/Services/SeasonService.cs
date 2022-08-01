using formula1_tournament_api.Data;
using formula1_tournament_api.Interfaces;
using formula1_tournament_api.Models;

namespace formula1_tournament_api.Services
{
    public class SeasonService : ISeason
    {
        private readonly FormulaDbContext _formulaDbContext;

        public SeasonService(FormulaDbContext formulaDbContext)
        {
            _formulaDbContext = formulaDbContext;
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> AddSeason(Season season)
        {
            if (season != null)
            {
                season.Id = Guid.NewGuid();
                _formulaDbContext.Add(season);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Please provide the season data");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteSeason(Guid id)
        {
            var season = _formulaDbContext.Season.Where(e => e.Id == id).FirstOrDefault();
            if (season != null)
            {
                _formulaDbContext.Season.Remove(season);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Season not found");
        }

        public async Task<(bool IsSuccess, List<Season> Season, string ErrorMessage)> GetAllSeasons()
        {
            var seasons = _formulaDbContext.Season.ToList();
            if (seasons != null)
            {
                return (true, seasons, null);
            }
            return (false, null, "No seasons found");
        }

        public async Task<(bool IsSuccess, Season Season, string ErrorMessage)> GetSeasonById(Guid id)
        {
            var season = _formulaDbContext.Season.Where(e => e.Id == id).FirstOrDefault();
            if (season != null)
            {
                return (true, season, null);
            }
            return (false, null, "Season not found");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateSeason(Guid id, Season season)
        {
            var seasonObj = _formulaDbContext.Season.Where(e => e.Id == id).FirstOrDefault();
            if (seasonObj != null)
            {
                seasonObj.Name = season.Name;
                _formulaDbContext.Season.Update(seasonObj);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Season not found");
        }
    }
}
