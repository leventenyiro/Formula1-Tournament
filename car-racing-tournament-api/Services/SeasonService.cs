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
        private IMapper _mapper;
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

        public async Task<(bool IsSuccess, Season? Season, string? ErrorMessage)> AddSeason(SeasonCreateDto seasonDto, Guid userId)
        {
            if (seasonDto == null)
                return (false, null, _configuration["ErrorMessages:MissingSeason"]);

            seasonDto.Name = seasonDto.Name.Trim();

            if (string.IsNullOrEmpty(seasonDto.Name))
                return (false, null, _configuration["ErrorMessages:SeasonName"]);

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
            if (seasonDto == null)
                return (false, _configuration["ErrorMessages:MissingSeason"]);

            seasonDto.Name = seasonDto.Name.Trim();
            if (string.IsNullOrEmpty(seasonDto.Name))
                return (false, _configuration["ErrorMessages:SeasonName"]);

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

        public async Task<(bool IsSuccess, List<Driver>? Drivers, string? ErrorMessage)> GetDriversBySeasonId(Season season)
        {
            var drivers = await _carRacingTournamentDbContext.Drivers
                .Where(x => x.SeasonId == season.Id)
                .Include(x => x.ActualTeam)
                .Include(x => x.Results!).ThenInclude(x => x.Race)
                .Include(x => x.Results!).ThenInclude(x => x.Team)
                .Select(x => new Driver
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
                        Points = x.Points,
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
                }).ToListAsync();
            
            return (true, drivers, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddDriver(Season season, DriverDto driverDto, Team team)
        {
            if (driverDto == null)
                return (false, _configuration["ErrorMessages:MissingDriver"]);

            driverDto.Name = driverDto.Name.Trim();
            if (string.IsNullOrEmpty(driverDto.Name))
                return (false, _configuration["ErrorMessages:DriverName"]);

            if (driverDto.Number <= 0 || driverDto.Number >= 100)
                return (false, _configuration["ErrorMessages:DriverNumber"]);

            if (driverDto.ActualTeamId != null && season.Id != team.SeasonId)
                return (false, _configuration["ErrorMessages:DriverTeamNotSameSeason"]);

            driverDto.RealName = driverDto.RealName?.Trim();
            var driver = _mapper.Map<Driver>(driverDto);
            driver.Id = Guid.NewGuid();
            driver.SeasonId = season.Id;
            await _carRacingTournamentDbContext.Drivers.AddAsync(driver);
            _carRacingTournamentDbContext.SaveChanges();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, List<Team>? Teams, string? ErrorMessage)> GetTeamsBySeasonId(Season season)
        {
            var teams = await _carRacingTournamentDbContext.Teams
                .Where(x => x.SeasonId == season.Id)
                .Include(x => x.Drivers)
                .Include(x => x.Results!).ThenInclude(x => x.Race)
                .Include(x => x.Results!).ThenInclude(x => x.Driver)
                .Select(x => new Team
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
                        Points = x.Points,
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
                }).ToListAsync();
            
            return (true, teams, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddTeam(Season season, TeamDto teamDto)
        {
            if (teamDto == null)
                return (false, _configuration["ErrorMessages:MissingTeam"]);

            teamDto.Name = teamDto.Name.Trim();
            if (string.IsNullOrEmpty(teamDto.Name))
                return (false, _configuration["ErrorMessages:TeamName"]);


            Team teamObj = new Team
            {
                Id = Guid.NewGuid(),
                Name = teamDto.Name,
                Season = await _carRacingTournamentDbContext.Seasons.Where(e => e.Id == season.Id).FirstAsync(),
            };
            try
            {
                teamDto.Color = teamDto.Color.Trim();
                if (teamDto.Color.Substring(0, 1) != "#")
                    teamDto.Color = "#" + teamDto.Color;
                var color = ColorTranslator.FromHtml(teamDto.Color);

                teamObj.Color = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            }
            catch (Exception)
            {
                return (false, _configuration["ErrorMessages:TeamColor"]);
            }
            await _carRacingTournamentDbContext.Teams.AddAsync(teamObj);
            _carRacingTournamentDbContext.SaveChanges();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, List<Race>? Races, string? ErrorMessage)> GetRacesBySeasonId(Season season)
        {
            var races = await _carRacingTournamentDbContext.Races
                .Where(x => x.SeasonId == season.Id)
                .Include(x => x.Results!).ThenInclude(x => x.Driver)
                .Include(x => x.Results!).ThenInclude(x => x.Team)
                .Select(x => new Race
                {
                    Id = x.Id,
                    Name = x.Name,
                    DateTime = x.DateTime,
                    Results = x.Results!.Select(x => new Result
                    {
                        Id = x.Id,
                        Position = x.Position,
                        Points = x.Points,
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
                }).ToListAsync();
            
            return (true, races, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddRace(Season season, RaceDto raceDto)
        {
            if (raceDto == null)
                return (false, _configuration["ErrorMessages:MissingRace"]);

            raceDto.Name = raceDto.Name.Trim();
            if (string.IsNullOrEmpty(raceDto.Name))
                return (false, _configuration["ErrorMessages:RaceNameCannotBeEmpty"]);

            raceDto.Name = raceDto.Name;
            var race = _mapper.Map<Race>(raceDto);
            race.Id = Guid.NewGuid();
            race.SeasonId = season.Id;
            await _carRacingTournamentDbContext.Races.AddAsync(race);
            _carRacingTournamentDbContext.SaveChanges();
            
            return (true, null);
        }
    }
}
