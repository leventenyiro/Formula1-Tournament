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
        private readonly CarRacingDbContext _carRacingDbContext;
        private IMapper _mapper;

        private const string SEASON_NOT_FOUND = "Season not found";

        public SeasonService(CarRacingDbContext carRacingDbContext, IMapper mapper)
        {
            _carRacingDbContext = carRacingDbContext;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, List<SeasonDto>? Seasons, string? ErrorMessage)> GetSeasons()
        {
            List<SeasonDto> seasons = await _carRacingDbContext.Seasons.Include(x => x.UserSeasons).Select(x => new SeasonDto
            {
                Id = x.Id,
                Name = x.Name,
                UserSeasons = x.UserSeasons.Select(x => new UserSeasonDto
                {
                    Username = x.User.Username,
                    Permission = x.Permission
                }).ToList()
            }).ToListAsync();

            if (seasons == null)
                return (false, null, SEASON_NOT_FOUND);
            
            return (true, seasons, null);
        }

        public async Task<(bool IsSuccess, SeasonDto? Season, string? ErrorMessage)> GetSeasonById(Guid id)
        {
            SeasonDto season = await _carRacingDbContext.Seasons
                .Include(x => x.UserSeasons)
                .Where(x => x.Id == id)
                .Select(x => new SeasonDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UserSeasons = x.UserSeasons.Select(x => new UserSeasonDto
                    {
                        Username = x.User.Username,
                        Permission = x.Permission
                    }).ToList()
                }).FirstAsync();

            if (season == null)
                return (false, null, SEASON_NOT_FOUND);
            
            return (true, season, null);
        }

        public async Task<(bool IsSuccess, Guid SeasonId, string? ErrorMessage)> AddSeason(string name, Guid userId)
        {
            if (string.IsNullOrEmpty(name))
                return (false, Guid.Empty, "Please provide the season data");
            
            UserSeason userSeason = new UserSeason { Id = new Guid(), Permission = UserSeasonPermission.Admin, UserId = userId };
            List<UserSeason> userSeasons = new List<UserSeason> { userSeason }; 

            var seasonObj = new Season { Id = Guid.NewGuid(), Name = name, UserSeasons = userSeasons };
            await _carRacingDbContext.Seasons.AddAsync(seasonObj);
            _carRacingDbContext.SaveChanges();
            
            return (true, seasonObj.Id, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateSeason(Guid id, string name)
        {
            var seasonObj = await _carRacingDbContext.Seasons.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (seasonObj == null)
                return (false, SEASON_NOT_FOUND);
            
            seasonObj.Name = name;
            _carRacingDbContext.Seasons.Update(seasonObj);
            _carRacingDbContext.SaveChanges();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteSeason(Guid id)
        {
            var season = await _carRacingDbContext.Seasons.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (season == null)
                return (false, SEASON_NOT_FOUND);
            
            _carRacingDbContext.Seasons.Remove(season);
            _carRacingDbContext.SaveChanges();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, List<SeasonDto>? Seasons, string? ErrorMessage)> GetSeasonsByUserSeasonList(List<Guid> userSeasons)
        {
            List<SeasonDto> seasons = await _carRacingDbContext.Seasons
                .Include(x => x.UserSeasons)
                .Where(x => userSeasons.Contains(x.Id))
                .Select(x => new SeasonDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UserSeasons = x.UserSeasons.Select(x => new UserSeasonDto
                    {
                        Username = x.User.Username,
                        Permission = x.Permission
                    }).ToList()
                }).ToListAsync();

            if (seasons == null)
                return (false, null, SEASON_NOT_FOUND);
            
            return (true, seasons, null);
        }

        public async Task<(bool IsSuccess, List<Driver>? Drivers, string? ErrorMessage)> GetDriversBySeasonId(Guid seasonId)
        {
            var drivers = await _carRacingDbContext.Drivers.Where(x => x.SeasonId == seasonId).ToListAsync();
            if (drivers == null)
                return (false, null, "Drivers not found");
            
            return (true, drivers, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddDriver(Guid seasonId, DriverDto driverDto)
        {
            if (driverDto == null)
                return (false, "Please provide the driver data");
            
            var driver = _mapper.Map<Driver>(driverDto);
            driver.Id = Guid.NewGuid();
            await _carRacingDbContext.AddAsync(driver);
            _carRacingDbContext.SaveChanges();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, List<Team>? Teams, string? ErrorMessage)> GetTeamsBySeasonId(Guid seasonId)
        {
            var teams = await _carRacingDbContext.Teams.Where(x => x.SeasonId == seasonId).ToListAsync();
            if (teams == null)
                return (false, null, "Teams not found");
            
            return (true, teams, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddTeam(Guid seasonId, TeamDto team)
        {
            if (team == null)
                return (false, "Please provide the team data");
            
            Team teamObj = new Team
            {
                Id = Guid.NewGuid(),
                Name = team.Name,
                Season = await _carRacingDbContext.Seasons.Where(e => e.Id == seasonId).FirstAsync(),
            };
            try
            {
                ColorTranslator.FromHtml(team.Color);
                teamObj.Color = team.Color;
            }
            catch (Exception)
            {
                return (false, "Incorrect color code");
            }
            await _carRacingDbContext.AddAsync(teamObj);
            _carRacingDbContext.SaveChanges();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, List<Race>? Races, string? ErrorMessage)> GetRacesBySeasonId(Guid seasonId)
        {
            var races = await _carRacingDbContext.Races.Where(x => x.SeasonId == seasonId).ToListAsync();
            if (races == null)
                return (false, null, "Races not found");
            
            return (true, races, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddRace(Guid seasonId, RaceDto raceDto)
        {
            if (raceDto == null)
                return (false, "Please provide the race data");
            
            var race = _mapper.Map<Race>(raceDto);
            race.Id = Guid.NewGuid();
            await _carRacingDbContext.AddAsync(race);
            _carRacingDbContext.SaveChanges();
            
            return (true, null);
        }
    }
}
