using formula1_tournament_api.Models;

namespace formula1_tournament_api.Interfaces
{
    public interface IUserSeason
    {
        public bool HasPermission(Guid userId, Season season);
        public bool IsAdmin(Guid userId, Season season);
        public bool IsModerator(Guid userId, Season season);
        public Task<(bool IsSuccess, string ErrorMessage)> AddPermission(User admin, User moderator, Season season);
        public Task<(bool IsSuccess, string ErrorMessage)> RemovePermission(User admin, User moderator, Season season);
    }
}
