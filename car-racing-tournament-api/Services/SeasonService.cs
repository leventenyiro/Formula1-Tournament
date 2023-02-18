using AutoMapper;
using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace car_racing_tournament_api.Services
{
    public class SeasonService : ISeason
    {
        private readonly CarRacingTournamentDbContext _carRacingTournamentDbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public SeasonService(CarRacingTournamentDbContext carRacingTournamentDbContext, IMapper mapper, IConfiguration configuration)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<(bool IsSuccess, List<SeasonOutputDto>? Seasons, string? ErrorMessage)> GetSeasons()
        {
            List<SeasonOutputDto> seasons = await _carRacingTournamentDbContext.Seasons
                .Include(x => x.Permissions)
                .Select(x => new SeasonOutputDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsArchived = x.IsArchived,
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
                .Select(x => new SeasonOutputDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsArchived = x.IsArchived,
                    Permissions = x.Permissions.Select(x => new PermissionOutputDto
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        Username = x.User.Username,
                        Type = x.Type
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
                .Select(x => new SeasonOutputDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsArchived = x.IsArchived,
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

            if (await _carRacingTournamentDbContext.Seasons
                .CountAsync(x => x.Name == seasonDto.Name && permissions.Contains(x.Id)) != 0)
                return (false, null, _configuration["ErrorMessages:SeasonExists"]);

            seasonDto.Name = seasonDto.Name;
            var season = _mapper.Map<Season>(seasonDto);
            season.Id = Guid.NewGuid();
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

            if (season.Name != seasonDto.Name && await _carRacingTournamentDbContext.Seasons
                .CountAsync(x => x.Name == seasonDto.Name && permissions.Contains(x.Id)) != 0)
                return (false, _configuration["ErrorMessages:SeasonExists"]);

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
