﻿using AutoMapper;
using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace car_racing_tournament_api.Services
{
    public class SeasonService : ISeason
    {
        private readonly CarRacingTournamentDbContext _carRacingTournamentDbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public SeasonService(CarRacingTournamentDbContext carRacingTournamentDbContext, IMapper mapper)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
            _mapper = mapper;
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();;
        }

        public async Task<(bool IsSuccess, List<SeasonOutputDto>? Seasons, string? ErrorMessage)> GetSeasons()
        {
            List<SeasonOutputDto> seasons = await _carRacingTournamentDbContext.Seasons
                .Include(x => x.Permissions)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new SeasonOutputDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsArchived = x.IsArchived,
                    CreatedAt = x.CreatedAt,
                    Favorite = x.Favorites.Count,
                    Permissions = x.Permissions.Select(x => new PermissionOutputDto
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        Username = x.User.Username,
                        Type = x.Type
                    }).ToList()
                }).ToListAsync();

            if (seasons == null)
                return (false, null, _configuration["ErrorMessages:SeasonNotFound"]);
            
            return (true, seasons, null);
        }

        public async Task<(bool IsSuccess, Season? Season, string? ErrorMessage)> GetSeasonById(Guid id)
        {
            Season? season = await _carRacingTournamentDbContext.Seasons.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (season == null)
                return (false, null, _configuration["ErrorMessages:SeasonNotFound"]);

            return (true, season, null);
        }

        public async Task<(bool IsSuccess, SeasonOutputDto? Season, string? ErrorMessage)> GetSeasonByIdWithDetails(Guid id)
        {
            SeasonOutputDto? season = await _carRacingTournamentDbContext.Seasons
                .Where(x => x.Id == id)
                .Include(x => x.Permissions)
                .Include(x => x.Races!).ThenInclude(x => x.Results!)
                .Include(x => x.Races!).ThenInclude(x => x.Results!)
                .Select(x => new SeasonOutputDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsArchived = x.IsArchived,
                    CreatedAt = x.CreatedAt,
                    Favorite = x.Favorites.Count,
                    Permissions = x.Permissions.Select(x => new PermissionOutputDto
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        Username = x.User.Username,
                        Type = x.Type
                    }).ToList(),
                    Drivers = x.Drivers!.Select(x => new Driver
                    {
                        Id = x.Id,
                        Name = x.Name,
                        RealName = x.RealName,
                        Number = x.Number,
                        Nationality = x.NationalityAlpha2 != null ? DriverService.GetNationalityByAlpha2(x.NationalityAlpha2) : null,
                        ActualTeamId = x.ActualTeamId
                    }).ToList(),
                    Teams = x.Teams!.Select(x => new Team
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Color = x.Color
                    }).ToList(),
                    Races = x.Races!.OrderBy(x => x.DateTime).Select(x => new Race
                    {
                        Id = x.Id,
                        Name = x.Name,
                        DateTime = x.DateTime,
                        Results = x.Results!.OrderBy(x => x.Race.DateTime).Select(x => new Result
                        {
                            Id = x.Id,
                            Type = x.Type,
                            Position = x.Position,
                            Point = x.Point,
                            DriverId = x.DriverId,
                            TeamId = x.TeamId
                        }).ToList(),
                    }).ToList()
                }).FirstOrDefaultAsync();

            if (season == null)
                return (false, null, _configuration["ErrorMessages:SeasonNotFound"]);
            
            return (true, season, null);
        }

        public async Task<(bool IsSuccess, List<SeasonOutputDto>? Seasons, string? ErrorMessage)> GetSeasonsByUserId(Guid userId)
        {
            if (_carRacingTournamentDbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync().Result == null)
                return (false, null, _configuration["ErrorMessages:UserNotFound"]);

            var permissions = await _carRacingTournamentDbContext.Permissions
                .Where(x => x.UserId == userId)
                .Select(x => x.SeasonId)
                .ToListAsync();

            List<SeasonOutputDto> seasons = await _carRacingTournamentDbContext.Seasons
                .Where(x => permissions.Contains(x.Id))
                .Include(x => x.Permissions)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new SeasonOutputDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsArchived = x.IsArchived,
                    CreatedAt = x.CreatedAt,
                    Favorite = x.Favorites.Count,
                    Permissions = x.Permissions.Select(x => new PermissionOutputDto
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        Username = x.User.Username,
                        Type = x.Type
                    }).ToList()
                }).ToListAsync();

            return (true, seasons, null);
        }

        public async Task<(bool IsSuccess, Season? Season, string? ErrorMessage)> AddSeason(SeasonCreateDto seasonDto, Guid userId)
        {
            seasonDto.Name = seasonDto.Name.Trim();

            if (string.IsNullOrEmpty(seasonDto.Name))
                return (false, null, _configuration["ErrorMessages:SeasonName"]);

            var permissions = await _carRacingTournamentDbContext.Permissions
                .Where(x => x.UserId == userId && x.Type == PermissionType.Admin)
                .Select(x => x.SeasonId)
                .ToListAsync();

            seasonDto.Name = seasonDto.Name;
            var season = _mapper.Map<Season>(seasonDto);
            season.Id = Guid.NewGuid();
            season.CreatedAt = DateTime.Now;
            season.IsArchived = false;

            await _carRacingTournamentDbContext.Seasons.AddAsync(season);
            await _carRacingTournamentDbContext.SaveChangesAsync();
            
            return (true, season, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateSeason(Season season, SeasonUpdateDto seasonDto)
        {
            seasonDto.Name = seasonDto.Name.Trim();
            if (string.IsNullOrEmpty(seasonDto.Name))
                return (false, _configuration["ErrorMessages:SeasonName"]);

            var adminId = await _carRacingTournamentDbContext.Permissions
                .Where(x => x.SeasonId == season.Id).FirstOrDefaultAsync();

            var permissions = await _carRacingTournamentDbContext.Permissions
                .Where(x => x.UserId == adminId!.UserId && x.Type == PermissionType.Admin)
                .Select(x => x.SeasonId)
                .ToListAsync();

            season.Name = seasonDto.Name;
            season.Description = seasonDto.Description;
            season.IsArchived = seasonDto.IsArchived;
            _carRacingTournamentDbContext.Seasons.Update(season);
            await _carRacingTournamentDbContext.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> ArchiveSeason(Season season)
        {
            season.IsArchived = !season.IsArchived;
            _carRacingTournamentDbContext.Seasons.Update(season);
            await _carRacingTournamentDbContext.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteSeason(Season season)
        {
            _carRacingTournamentDbContext.Seasons.Remove(season);

            await _carRacingTournamentDbContext.SaveChangesAsync();

            return (true, null);
        }
    }
}
