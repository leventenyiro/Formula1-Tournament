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
                    CreatedAt = x.CreatedAt,
                    Permissions = x.Permissions.Select(x => new PermissionOutputDto
                    {
                        Id = x.Id,
                        JoinedAt = x.JoinedAt,
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
                .Include(x => x.Drivers!).ThenInclude(x => x.ActualTeam)
                .Include(x => x.Drivers!).ThenInclude(x => x.Results!).ThenInclude(x => x.Race)
                .Include(x => x.Drivers!).ThenInclude(x => x.Results!).ThenInclude(x => x.Team)

                .Include(x => x.Teams!).ThenInclude(x => x.Drivers)
                .Include(x => x.Teams!).ThenInclude(x => x.Results!).ThenInclude(x => x.Race)
                .Include(x => x.Teams!).ThenInclude(x => x.Results!).ThenInclude(x => x.Driver)

                .Include(x => x.Races!).ThenInclude(x => x.Results!).ThenInclude(x => x.Driver)
                .Include(x => x.Races!).ThenInclude(x => x.Results!).ThenInclude(x => x.Team)
                .Select(x => new SeasonOutputDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsArchived = x.IsArchived,
                    CreatedAt = x.CreatedAt,
                    Permissions = x.Permissions.Select(x => new PermissionOutputDto
                    {
                        Id = x.Id,
                        JoinedAt = x.JoinedAt,
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
                        ActualTeam = x.ActualTeam != null ? new Team
                        {
                            Id = x.ActualTeam.Id,
                            Name = x.ActualTeam.Name,
                            Color = x.ActualTeam.Color
                        } : null,
                        Results = x.Results!.Select(x => new Result
                        {
                            Id = x.Id,
                            Position = x.Position,
                            Point = x.Point,
                            Race = new Race
                            {
                                Id = x.Race.Id,
                                Name = x.Race.Name,
                                DateTime = x.Race.DateTime
                            },
                            Team = new Team
                            {
                                Id = x.Team.Id,
                                Name = x.Team.Name,
                                Color = x.Team.Color
                            }
                        }).ToList()
                    }).ToList(),
                    Teams = x.Teams!.Select(x => new Team
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Color = x.Color,
                        Drivers = x.Drivers!.Select(x => new Driver
                        {
                            Id = x.Id,
                            Name = x.Name,
                            RealName = x.RealName,
                            Number = x.Number
                        }).ToList(),
                        Results = x.Results!.Select(x => new Result
                        {
                            Id = x.Id,
                            Position = x.Position,
                            Point = x.Point,
                            Race = new Race
                            {
                                Id = x.Race.Id,
                                Name = x.Race.Name,
                                DateTime = x.Race.DateTime
                            },
                            Driver = new Driver
                            {
                                Id = x.Driver.Id,
                                Name = x.Driver.Name,
                                RealName = x.Driver.RealName,
                                Number = x.Driver.Number
                            }
                        }).ToList()
                    }).ToList(),
                    Races = x.Races!.Select(x => new Race
                    {
                        Id = x.Id,
                        Name = x.Name,
                        DateTime = x.DateTime,
                        Results = x.Results!.Select(x => new Result
                        {
                            Id = x.Id,
                            Position = x.Position,
                            Point = x.Point,
                            Driver = new Driver
                            {
                                Id = x.Driver.Id,
                                Name = x.Driver.Name,
                                RealName = x.Driver.RealName,

                            },
                            Team = new Team
                            {
                                Id = x.Team.Id,
                                Name = x.Team.Name,
                                Color = x.Team.Color
                            }
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
                .Select(x => new SeasonOutputDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsArchived = x.IsArchived,
                    CreatedAt = x.CreatedAt,
                    Permissions = x.Permissions.Select(x => new PermissionOutputDto
                    {
                        Id = x.Id,
                        JoinedAt = x.JoinedAt,
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
