using formula1_tournament_api.Data;
using formula1_tournament_api.Interfaces;
using formula1_tournament_api.Models;

namespace formula1_tournament_api.Services
{
    public class UserSeasonService : IUserSeason
    {
        private readonly FormulaDbContext _formulaDbContext;

        public UserSeasonService(FormulaDbContext formulaDbContext)
        {
            _formulaDbContext = formulaDbContext;
        }

        public bool HasPermission(Guid userId, Guid seasonId)
        {
            UserSeasonPermission permission = _formulaDbContext.UserSeasons.Where(x => x.UserId == userId && x.SeasonId == seasonId).First().Permission;
            return permission == UserSeasonPermission.Moderator || permission == UserSeasonPermission.Admin;
        }

        public bool IsAdmin(User user, Season season)
        {
            UserSeasonPermission permission = _formulaDbContext.UserSeasons.Where(x => x.User == user && x.Season == season).First().Permission;
            return permission == UserSeasonPermission.Admin;
        }

        public bool IsModerator(User user, Season season)
        {
            UserSeasonPermission permission = _formulaDbContext.UserSeasons.Where(x => x.User == user && x.Season == season).First().Permission;
            return permission == UserSeasonPermission.Moderator;
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> AddPermission(User admin, User moderator, Season season)
        {
            if (!IsAdmin(admin, season))
                return (false, "You don't have permission for it");
            _formulaDbContext.UserSeasons.Add(new UserSeason { Id = new Guid(), User = moderator, Season = season, Permission = UserSeasonPermission.Moderator });
            _formulaDbContext.SaveChanges();
            return (true, null);
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> RemovePermission(User admin, User moderator, Season season)
        {
            if (!IsAdmin(admin, season))
                return (false, "You don't have permission for it");
            UserSeason moderatorObj = _formulaDbContext.UserSeasons.Where(x => x.User == moderator && x.Season == season).First();
            if (moderatorObj == null)
                return (false, "This moderator doesn't exists");
            _formulaDbContext.UserSeasons.Remove(moderatorObj);
            _formulaDbContext.SaveChanges();
            return (true, null);
        }

        public async Task<(bool IsSuccess, List<UserSeason> UserSeasons, string ErrorMessage)> GetAllOwnedSeasonId(Guid userId)
        {
            List<UserSeason> userSeasons = _formulaDbContext.UserSeasons.Where(x => x.UserId == userId).ToList();
            return (true, userSeasons, null);
        }
    }
}
