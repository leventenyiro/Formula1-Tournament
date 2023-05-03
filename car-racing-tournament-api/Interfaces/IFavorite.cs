using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.Interfaces
{
    public interface IFavorite
    {
        Task<(bool IsSuccess, Favorite? Favorite, string? ErrorMessage)> GetFavoriteById(Guid id);
        Task<(bool IsSuccess, string? ErrorMessage)> AddFavorite(Guid userId, Season season);
        Task<(bool IsSuccess, string? ErrorMessage)> RemoveFavorite(Favorite favorite);
    }
}
