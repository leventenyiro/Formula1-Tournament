using AutoMapper;
using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace car_racing_tournament_api.Services
{
    public class SeasonService : ISeason
    {
        private readonly CarRacingTournamentDbContext _carRacingTournamentDbContext;
        private IMapper _mapper;

        private const string SEASON_NOT_FOUND = "Season not found";
        private const int SEASON_NAME_MIN_LENGTH = 5;
        private const string INCORRECT_SEASON_NAME = "Season name must be at least 5 characters";

        public SeasonService(CarRacingTournamentDbContext carRacingTournamentDbContext, IMapper mapper)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, List<SeasonOutputDto>? Seasons, string? ErrorMessage)> GetSeasons()
        {
            List<SeasonOutputDto> seasons = await _carRacingTournamentDbContext.Seasons
                .Include(x => x.UserSeasons)
                .Select(x => new SeasonOutputDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsArchived = x.IsArchived,
                    UserSeasons = x.UserSeasons.Select(x => new UserSeasonOutputDto
                    {
                        UserId = x.UserId,
                        Username = x.User.Username,
                        Permission = x.Permission
                    }).ToList()
                }).ToListAsync();

            if (seasons == null)
                return (false, null, SEASON_NOT_FOUND);
            
            return (true, seasons, null);
        }

        public async Task<(bool IsSuccess, SeasonOutputDto? Season, string? ErrorMessage)> GetSeasonById(Guid id)
        {
            SeasonOutputDto? season = await _carRacingTournamentDbContext.Seasons
                .Where(x => x.Id == id)
                .Include(x => x.UserSeasons)
                .Select(x => new SeasonOutputDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsArchived = x.IsArchived,
                    UserSeasons = x.UserSeasons.Select(x => new UserSeasonOutputDto
                    {
                        UserId = x.UserId,
                        Username = x.User.Username,
                        Permission = x.Permission
                    }).ToList()
                }).FirstOrDefaultAsync();

            if (season == null)
                return (false, null, SEASON_NOT_FOUND);
            
            return (true, season, null);
        }

        public async Task<(bool IsSuccess, Guid SeasonId, string? ErrorMessage)> AddSeason(SeasonCreateDto seasonDto, Guid userId)
        {
            if (seasonDto == null)
                return (false, Guid.Empty, "Please provide the season data");

            if (seasonDto.Name.Length < SEASON_NAME_MIN_LENGTH)
                return (false, Guid.Empty, INCORRECT_SEASON_NAME);

            var season = _mapper.Map<Season>(seasonDto);
            season.Id = Guid.NewGuid();
            season.IsArchived = false;

            UserSeason userSeason = new UserSeason { Id = new Guid(), Permission = UserSeasonPermission.Admin, UserId = userId };
            season.UserSeasons = new List<UserSeason> { userSeason };

            await _carRacingTournamentDbContext.Seasons.AddAsync(season);
            _carRacingTournamentDbContext.SaveChanges();
            
            return (true, season.Id, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateSeason(Guid id, SeasonUpdateDto seasonDto)
        {
            var seasonObj = await _carRacingTournamentDbContext.Seasons.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (seasonObj == null)
                return (false, SEASON_NOT_FOUND);

            if (seasonDto.Name.Length < SEASON_NAME_MIN_LENGTH)
                return (false, INCORRECT_SEASON_NAME);

            seasonObj.Name = seasonDto.Name;
            seasonObj.Description = seasonDto.Description;
            seasonObj.IsArchived = seasonDto.IsArchived;
            _carRacingTournamentDbContext.Seasons.Update(seasonObj);
            _carRacingTournamentDbContext.SaveChanges();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> ArchiveSeason(Guid id)
        {
            var seasonObj = await _carRacingTournamentDbContext.Seasons.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (seasonObj == null)
                return (false, SEASON_NOT_FOUND);

            seasonObj.IsArchived = !seasonObj.IsArchived;
            _carRacingTournamentDbContext.Seasons.Update(seasonObj);
            _carRacingTournamentDbContext.SaveChanges();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteSeason(Guid id)
        {
            var season = await _carRacingTournamentDbContext.Seasons.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (season == null)
                return (false, SEASON_NOT_FOUND);
            
            _carRacingTournamentDbContext.Seasons.Remove(season);

            var userSeasons = await _carRacingTournamentDbContext.UserSeasons.Where(e => e.SeasonId == id).ToListAsync();
            _carRacingTournamentDbContext.UserSeasons.RemoveRange(userSeasons);

            _carRacingTournamentDbContext.SaveChanges();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, List<SeasonOutputDto>? Seasons, string? ErrorMessage)> GetSeasonsByUserId(Guid userId)
        {
            if (_carRacingTournamentDbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync().Result == null)
                return (false, null, "User not found!");

            var userSeasons = await _carRacingTournamentDbContext.UserSeasons
                .Where(x => x.UserId == userId)
                .Select(x => x.SeasonId)
                .ToListAsync();

            List<SeasonOutputDto> seasons = await _carRacingTournamentDbContext.Seasons
                .Where(x => userSeasons.Contains(x.Id))
                .Include(x => x.UserSeasons)
                .Select(x => new SeasonOutputDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsArchived = x.IsArchived,
                    UserSeasons = x.UserSeasons.Select(x => new UserSeasonOutputDto
                    {
                        UserId = x.UserId,
                        Username = x.User.Username,
                        Permission = x.Permission
                    }).ToList()
                }).ToListAsync();

            return (true, seasons, null);
        }

        public async Task<(bool IsSuccess, List<Driver>? Drivers, string? ErrorMessage)> GetDriversBySeasonId(Guid seasonId)
        {
            if (_carRacingTournamentDbContext.Seasons.Where(x => x.Id == seasonId).FirstOrDefaultAsync().Result == null)
                return (false, null, SEASON_NOT_FOUND);

            var drivers = await _carRacingTournamentDbContext.Drivers
                .Where(x => x.SeasonId == seasonId)
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

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddDriver(Guid seasonId, DriverDto driverDto)
        {
            if (_carRacingTournamentDbContext.Seasons.Where(x => x.Id == seasonId).FirstOrDefaultAsync().Result == null)
                return (false, SEASON_NOT_FOUND);

            if (driverDto == null)
                return (false, "Please provide the driver data!");

            if (driverDto.Name.Length == 0)
                return (false, "Driver name cannot be empty!");

            if (driverDto.Number <= 0 || driverDto.Number >= 100)
                return (false, "Driver number must be between 1 and 99!");

            if (driverDto.ActualTeamId != null)
            {
                Team? teamObj = await _carRacingTournamentDbContext.Teams.Where(e => e.Id == driverDto!.ActualTeamId).FirstOrDefaultAsync();
                if (seasonId != teamObj!.SeasonId)
                    return (false, "Driver and team aren't in the same season");
            }

            var driver = _mapper.Map<Driver>(driverDto);
            driver.Id = Guid.NewGuid();
            driver.SeasonId = seasonId;
            await _carRacingTournamentDbContext.Drivers.AddAsync(driver);
            _carRacingTournamentDbContext.SaveChanges();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, List<Team>? Teams, string? ErrorMessage)> GetTeamsBySeasonId(Guid seasonId)
        {
            if (_carRacingTournamentDbContext.Seasons.Where(x => x.Id == seasonId).FirstOrDefaultAsync().Result == null)
                return (false, null, SEASON_NOT_FOUND);

            var teams = await _carRacingTournamentDbContext.Teams
                .Where(x => x.SeasonId == seasonId)
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

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddTeam(Guid seasonId, TeamDto teamDto)
        {
            if (teamDto == null)
                return (false, "Please provide the team data!");

            if (string.IsNullOrEmpty(teamDto.Name))
                return (false, "Team name cannot be empty!");

            Team teamObj = new Team
            {
                Id = Guid.NewGuid(),
                Name = teamDto.Name,
                Season = await _carRacingTournamentDbContext.Seasons.Where(e => e.Id == seasonId).FirstAsync(),
            };
            try
            {
                if (teamDto.Color.Substring(0, 1) != "#") // it should be in ColorUtils
                    teamDto.Color = "#" + teamDto.Color;
                var color = ColorTranslator.FromHtml(teamDto.Color);

                teamObj.Color = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            }
            catch (Exception)
            {
                return (false, "Incorrect color code");
            }
            await _carRacingTournamentDbContext.Teams.AddAsync(teamObj);
            _carRacingTournamentDbContext.SaveChanges();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, List<Race>? Races, string? ErrorMessage)> GetRacesBySeasonId(Guid seasonId)
        {
            if (_carRacingTournamentDbContext.Seasons.Where(x => x.Id == seasonId).FirstOrDefaultAsync().Result == null)
                return (false, null, SEASON_NOT_FOUND);

            var races = await _carRacingTournamentDbContext.Races
                .Where(x => x.SeasonId == seasonId)
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

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddRace(Guid seasonId, RaceDto raceDto)
        {
            if (raceDto == null)
                return (false, "Please provide the race data");
            
            var race = _mapper.Map<Race>(raceDto);
            race.Id = Guid.NewGuid();
            race.SeasonId = seasonId;
            await _carRacingTournamentDbContext.Races.AddAsync(race);
            _carRacingTournamentDbContext.SaveChanges();
            
            return (true, null);
        }
    }
}
