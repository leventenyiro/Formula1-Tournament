using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using Microsoft.EntityFrameworkCore;

namespace car_racing_tournament_api.Services
{
    public class FavoriteService : IFavorite
    {
        private readonly CarRacingTournamentDbContext _carRacingTournamentDbContext;
        private readonly IConfiguration _configuration;

        public FavoriteService(CarRacingTournamentDbContext carRacingTournamentDbContext)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public async Task<(bool IsSuccess, Favorite? Favorite, string? ErrorMessage)> GetFavoriteById(Guid id)
        {
            var favorite = await _carRacingTournamentDbContext.Favorites.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (favorite == null)
                return (false, null, _configuration["ErrorMessages:FavoriteNotFound"]);

            return (true, favorite, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddFavorite(Guid userId, Season season)
        {
            if (await _carRacingTournamentDbContext.Favorites.CountAsync(x => x.UserId == userId && x.SeasonId == season.Id) != 0)
                return (false, _configuration["ErrorMessages:FavoriteExists"]);

            await _carRacingTournamentDbContext.Favorites.AddAsync(new Favorite
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SeasonId = season.Id
            });
            await _carRacingTournamentDbContext.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> RemoveFavorite(Favorite favorite)
        {
            _carRacingTournamentDbContext.Favorites.Remove(favorite);
            await _carRacingTournamentDbContext.SaveChangesAsync();
            
            return (true, null);
        }
    }
}
