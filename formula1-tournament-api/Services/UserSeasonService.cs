using formula1_tournament_api.Data;
using formula1_tournament_api.Models;

namespace formula1_tournament_api.Services
{
    public class UserSeasonService
    {
        private readonly FormulaDbContext _formulaDbContext;

        public UserSeasonService(FormulaDbContext formulaDbContext)
        {
            _formulaDbContext = formulaDbContext;
        }

        public UserSeasonPermission GetPermission(User user, Season season)
        {
            return _formulaDbContext.UserSeasons.Where(x => x.User == user && x.Season == season).First().Permission;
        }

        // addPermission, removePermission
        public void AddPermission(User admin, User moderator, Season season)
        {
            if (GetPermission(admin, season) == UserSeasonPermission.Admin)
            {
                _formulaDbContext.UserSeasons.Add(new UserSeason { Id = new Guid(), User = moderator, Season = season, Permission = UserSeasonPermission.Moderator });
                _formulaDbContext.SaveChanges();
            }
        }

        public void RemovePermission(User admin, User moderator, Season season)
        {
            if (GetPermission(admin, season) == UserSeasonPermission.Admin)
            {
                _formulaDbContext.UserSeasons.Add(new UserSeason { Id = new Guid(), User = moderator, Season = season, Permission = UserSeasonPermission.Moderator });
                _formulaDbContext.SaveChanges();
            }
        }
    }
}
