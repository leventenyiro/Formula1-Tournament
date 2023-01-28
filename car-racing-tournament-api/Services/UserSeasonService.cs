﻿using car_racing_tournament_api.Data;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using Microsoft.EntityFrameworkCore;

namespace car_racing_tournament_api.Services
{
    public class UserSeasonService : IUserSeason
    {
        private readonly CarRacingTournamentDbContext _carRacingTournamentDbContext;

        public UserSeasonService(CarRacingTournamentDbContext carRacingTournamentDbContext)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
        }

        public bool HasPermission(Guid userId, Guid seasonId)
        {
            UserSeasonPermission permission = _carRacingTournamentDbContext.UserSeasons.Where(x => x.UserId == userId && x.SeasonId == seasonId).First().Permission;
            return permission == UserSeasonPermission.Moderator || permission == UserSeasonPermission.Admin;
        }

        public bool IsAdmin(Guid userId, Guid seasonId)
        {
            UserSeasonPermission permission = _carRacingTournamentDbContext.UserSeasons.Where(x => x.UserId == userId && x.SeasonId == seasonId).First().Permission;
            return permission == UserSeasonPermission.Admin;
        }

        public bool IsModerator(Guid userId, Guid seasonId)
        {
            UserSeasonPermission permission = _carRacingTournamentDbContext.UserSeasons.Where(x => x.UserId == userId && x.SeasonId == seasonId).First().Permission;
            return permission == UserSeasonPermission.Moderator;
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddAdmin(Guid userId, Guid seasonId)
        {
            await _carRacingTournamentDbContext.UserSeasons.AddAsync(new UserSeason { Id = new Guid(), UserId = userId, SeasonId = seasonId, Permission = UserSeasonPermission.Admin });
            _carRacingTournamentDbContext.SaveChanges();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddModerator(Guid adminId, Guid moderatorId, Guid seasonId)
        {
            if (!IsAdmin(adminId, seasonId))
                return (false, "You don't have permission for it");
            
            await _carRacingTournamentDbContext.UserSeasons.AddAsync(new UserSeason { Id = new Guid(), UserId = moderatorId, SeasonId = seasonId, Permission = UserSeasonPermission.Moderator });
            _carRacingTournamentDbContext.SaveChanges();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> RemovePermission(Guid adminId, Guid moderatorId, Guid seasonId)
        {
            if (!IsAdmin(adminId, seasonId))
                return (false, "You don't have permission for it");

            if (IsAdmin(moderatorId, seasonId))
                return (false, "You can't remove permission of an admin");

            UserSeason moderatorObj = await _carRacingTournamentDbContext.UserSeasons.Where(x => x.UserId == moderatorId && x.SeasonId == seasonId).FirstAsync();
            if (moderatorObj == null)
                return (false, "This moderator doesn't exists");

            _carRacingTournamentDbContext.UserSeasons.Remove(moderatorObj);
            _carRacingTournamentDbContext.SaveChanges();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, List<UserSeason>? UserSeasons, string? ErrorMessage)> GetSeasonsByUserId(Guid userId)
        {
            List<UserSeason> userSeasons = await _carRacingTournamentDbContext.UserSeasons.Where(x => x.UserId == userId).ToListAsync();

            if (userSeasons == null)
                return (false, null, "Seasons not found");

            return (true, userSeasons, null);
        }
    }
}