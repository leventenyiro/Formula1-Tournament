using formula1_tournament_api.Data;
using formula1_tournament_api.DTO;
using formula1_tournament_api.Interfaces;
using formula1_tournament_api.Models;
using Microsoft.EntityFrameworkCore;

namespace formula1_tournament_api.Services
{
    public class SeasonService : ISeason
    {
        private readonly FormulaDbContext _formulaDbContext;

        public SeasonService(FormulaDbContext formulaDbContext)
        {
            _formulaDbContext = formulaDbContext;
        }

        public async Task<(bool IsSuccess, Guid SeasonId, string ErrorMessage)> AddSeason(string name, Guid userId)
        {
            if (!string.IsNullOrEmpty(name))
            {
                UserSeason userSeason = new UserSeason { Id = new Guid(), Permission = UserSeasonPermission.Admin, UserId = userId };
                List<UserSeason> userSeasons = new List<UserSeason> { userSeason }; 

                var seasonObj = new Season { Id = Guid.NewGuid(), Name = name, UserSeasons = userSeasons };
                _formulaDbContext.Seasons.Add(seasonObj);
                _formulaDbContext.SaveChanges();
                return (true, seasonObj.Id, null);
            }
            return (false, Guid.Empty, "Please provide the season data");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteSeason(Guid id)
        {
            var season = _formulaDbContext.Seasons.Where(e => e.Id == id).FirstOrDefault();
            if (season != null)
            {
                _formulaDbContext.Seasons.Remove(season);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Season not found");
        }

        public async Task<(bool IsSuccess, List<SeasonDto> Seasons, string ErrorMessage)> GetAllSeasons()
        {
            List<SeasonDto> seasons = _formulaDbContext.Seasons.Include(x => x.UserSeasons).Select(x => new SeasonDto
            {
                Id = x.Id,
                Name = x.Name,
                UserSeasons = x.UserSeasons.Select(x => new UserSeasonDto
                {
                    Username = x.User.Username,
                    Permission = x.Permission
                }).ToList()
            }).ToList();

            if (seasons != null)
            {
                return (true, seasons, null);
            }
            return (false, null, "No seasons found");
        }

        public async Task<(bool IsSuccess, SeasonDto Season, string ErrorMessage)> GetSeasonById(Guid id)
        {
            SeasonDto season = _formulaDbContext.Seasons
                .Include(x => x.UserSeasons)
                .Where(x => x.Id == id)
                .Select(x => new SeasonDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UserSeasons = x.UserSeasons.Select(x => new UserSeasonDto
                    {
                        Username = x.User.Username,
                        Permission = x.Permission
                    }).ToList()
                }).First();
            if (season != null)
            {
                return (true, season, null);
            }
            return (false, null, "Season not found");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateSeason(Guid id, string name)
        {
            var seasonObj = _formulaDbContext.Seasons.Where(e => e.Id == id).FirstOrDefault();
            if (seasonObj != null)
            {
                seasonObj.Name = name;
                _formulaDbContext.Seasons.Update(seasonObj);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Season not found");
        }

        public async Task<(bool IsSuccess, List<SeasonDto> Seasons, string ErrorMessage)> GetAllSeasonsByUserSeasonList(List<Guid> userSeasons)
        {
            List<SeasonDto> seasons = _formulaDbContext.Seasons
                .Include(x => x.UserSeasons)
                .Where(x => userSeasons.Contains(x.Id))
                .Select(x => new SeasonDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UserSeasons = x.UserSeasons.Select(x => new UserSeasonDto
                    {
                        Username = x.User.Username,
                        Permission = x.Permission
                    }).ToList()
                }).ToList();
            if (seasons != null)
            {
                return (true, seasons, null);
            }
            return (false, null, "No seasons found");
        }
    }
}
