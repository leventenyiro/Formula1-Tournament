using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using Microsoft.EntityFrameworkCore;

namespace car_racing_tournament_api.Services
{
    public class UserSeasonService : IUserSeason
    {
        private readonly CarRacingTournamentDbContext _carRacingTournamentDbContext;
        private readonly IConfiguration _configuration;

        public UserSeasonService(CarRacingTournamentDbContext carRacingTournamentDbContext, IConfiguration configuration)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
            _configuration = configuration;
        }

        public bool IsAdmin(UserSeason userSeason)
        {
            return userSeason.Permission == UserSeasonPermission.Admin;
        }

        public bool IsAdminModerator(UserSeason userSeason)
        {
            return userSeason.Permission == UserSeasonPermission.Moderator || userSeason.Permission == UserSeasonPermission.Admin;
        }

        public async Task<(bool IsSuccess, UserSeason? userSeason, string? ErrorMessage)> GetUserSeasonById(UserSeason userSeason)
        {
            
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddUserSeason(UserSeasonDto userSeasonDto)
        {
            await _carRacingTournamentDbContext.UserSeasons.AddAsync(new UserSeason
            {
                Id = Guid.NewGuid(),
                UserId = userSeasonDto.UserId,
                SeasonId = userSeasonDto.SeasonId,
                Permission = userSeasonDto.Permission
            });
            await _carRacingTournamentDbContext.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> RemovePermission(Guid adminId, Guid moderatorId, Guid seasonId)
        {
            UserSeason moderatorObj = await _carRacingTournamentDbContext.UserSeasons.Where(x => x.UserId == moderatorId && x.SeasonId == seasonId).FirstAsync();
            if (moderatorObj == null)
                return (false, _configuration["ErrorMessages:ModeratorNotFound"]);

            _carRacingTournamentDbContext.UserSeasons.Remove(moderatorObj);
            _carRacingTournamentDbContext.SaveChanges();
            
            return (true, null);
        }
    }
}
