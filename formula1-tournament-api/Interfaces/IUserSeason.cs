using formula1_tournament_api.Models;

namespace formula1_tournament_api.Interfaces
{
    public interface IUserSeason
    {
        public bool HasPermission(Guid userId, Guid seasonId);
        public bool IsAdmin(User user, Season season);
        public bool IsModerator(User user, Season season);
        public Task<(bool IsSuccess, string ErrorMessage)> AddPermission(User admin, User moderator, Season season);
        public Task<(bool IsSuccess, string ErrorMessage)> RemovePermission(User admin, User moderator, Season season);
        public Task<(bool IsSuccess, List<UserSeason> UserSeasons, string ErrorMessage)> GetAllOwnedSeasonId(Guid userId);
    }
}
