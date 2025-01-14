﻿using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using Microsoft.EntityFrameworkCore;

namespace car_racing_tournament_api.Services
{
    public class PermissionService : IPermission
    {
        private readonly CarRacingTournamentDbContext _carRacingTournamentDbContext;
        private readonly IConfiguration _configuration;

        public PermissionService(CarRacingTournamentDbContext carRacingTournamentDbContext)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public async Task<bool> IsAdmin(Guid userId, Guid seasonId)
        {
            var userSeason = await _carRacingTournamentDbContext.Permissions.Where(x => x.UserId == userId && x.SeasonId == seasonId).FirstOrDefaultAsync();
            if (userSeason == null)
                return false;

            return userSeason.Type == PermissionType.Admin;
        }

        public async Task<bool> IsAdminModerator(Guid userId, Guid seasonId)
        {
            var userSeason = await _carRacingTournamentDbContext.Permissions.Where(x => x.UserId == userId && x.SeasonId == seasonId).FirstOrDefaultAsync();
            if (userSeason == null)
                return false;

            return userSeason.Type == PermissionType.Moderator || userSeason.Type == PermissionType.Admin;
        }

        public async Task<(bool IsSuccess, Permission? Permission, string? ErrorMessage)> GetPermissionBySeasonAndUser(Season season, Guid userId) {
            var permission = await _carRacingTournamentDbContext.Permissions
                .Where(x => x.SeasonId == season.Id && x.UserId == userId)
                .FirstOrDefaultAsync();
            if (permission == null)
                return (false, null, _configuration["ErrorMessages:PermissionNotFound"]);

            return (true, permission, null);
        }

        public async Task<(bool IsSuccess, List<PermissionOutputDto>? Permissions, string? ErrorMessage)> GetPermissionsBySeason(Season season)
        {
            var permissions = await _carRacingTournamentDbContext.Permissions
                .Where(x => x.SeasonId == season.Id)
                .OrderByDescending(x => x.Type)
                .Select(x => new PermissionOutputDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Username = x.User.Username,
                    Type = x.Type,
                })
                .ToListAsync();
            if (permissions == null)
                return (false, null, _configuration["ErrorMessages:PermissionNotFound"]);

            return (true, permissions, null);
        }

        public async Task<(bool IsSuccess, List<PermissionOutputDto>? Permissions, string? ErrorMessage)> GetPermissionsByUser(User user, PermissionType? permissionTypeFilter)
        {
            var permissions = await _carRacingTournamentDbContext.Permissions
                .Where(x => permissionTypeFilter == null ?
                    x.UserId == user.Id :
                    x.UserId == user.Id && x.Type == permissionTypeFilter)
                .OrderByDescending(x => x.Type)
                .Select(x => new PermissionOutputDto
                {
                    Id = x.Id,
                    SeasonId = x.SeasonId,
                    Username = x.User.Username,
                    Type = x.Type,
                })
                .ToListAsync();
            if (permissions == null)
                return (false, null, _configuration["ErrorMessages:PermissionNotFound"]);

            return (true, permissions, null);
        }

        public async Task<(bool IsSuccess, Permission? Permission, string? ErrorMessage)> GetPermissionById(Guid id)
        {
            var permission = await _carRacingTournamentDbContext.Permissions.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (permission == null)
                return (false, null, _configuration["ErrorMessages:PermissionNotFound"]);

            return (true, permission, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddPermission(User user, Season season, PermissionType permissionType)
        {
            if (await _carRacingTournamentDbContext.Permissions.CountAsync(x => x.UserId == user.Id && x.SeasonId == season.Id) != 0)
                return (false, _configuration["ErrorMessages:PermissionExists"]);

            await _carRacingTournamentDbContext.Permissions.AddAsync(new Permission
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                SeasonId = season.Id,
                Type = permissionType
            });
            await _carRacingTournamentDbContext.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdatePermissionType(Permission permission, PermissionType permissionType)
        {
            if (permissionType == PermissionType.Admin && await _carRacingTournamentDbContext.Permissions.CountAsync(x => x.SeasonId == permission.SeasonId && x.Type == PermissionType.Admin) > 0)
                return (false, _configuration["ErrorMessages:SeasonHasAdmin"]);

            permission.Type = permissionType;
            _carRacingTournamentDbContext.Permissions.Update(permission);
            await _carRacingTournamentDbContext.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> RemovePermission(Permission permission)
        {
            _carRacingTournamentDbContext.Permissions.Remove(permission);
            await _carRacingTournamentDbContext.SaveChangesAsync();
            
            return (true, null);
        }
    }
}
