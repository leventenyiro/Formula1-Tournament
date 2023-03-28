using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace car_racing_tournament_api.Services
{
    public class TeamService : ITeam
    {
        private readonly CarRacingTournamentDbContext _carRacingTournamentDbContext;
        private readonly IConfiguration _configuration;

        public TeamService(CarRacingTournamentDbContext carRacingTournamentDbContext)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();;
        }

        public async Task<(bool IsSuccess, Team? Team, string? ErrorMessage)> GetTeamById(Guid id)
        {
            var team = await _carRacingTournamentDbContext.Teams
                .Where(e => e.Id == id)
                .Include(x => x.Drivers)
                .Include(x => x.Results!).ThenInclude(x => x.Race)
                .Include(x => x.Results!).ThenInclude(x => x.Driver)
                .Select(x => new Team
                {
                    Id = x.Id,
                    Name = x.Name,
                    Color = x.Color,
                    SeasonId = x.SeasonId,
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
                        Type = x.Type,
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
                    }).ToList(),
                }).FirstOrDefaultAsync();
            if (team == null)
                return (false, null, _configuration["ErrorMessages:TeamNotFound"]);
            
            return (true, team, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddTeam(Season season, TeamDto teamDto)
        {
            teamDto.Name = teamDto.Name.Trim();
            if (string.IsNullOrEmpty(teamDto.Name))
                return (false, _configuration["ErrorMessages:TeamName"]);

            if (await _carRacingTournamentDbContext.Teams.CountAsync(
                x => x.Name == teamDto.Name && x.SeasonId == season.Id) != 0)
                return (false, _configuration["ErrorMessages:TeamExists"]);

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

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateTeam(Team team, TeamDto teamDto)
        {
            teamDto.Name = teamDto.Name.Trim();
            if (string.IsNullOrEmpty(teamDto.Name))
                return (false, _configuration["ErrorMessages:TeamName"]);

            if (team.Name != teamDto.Name &&
                await _carRacingTournamentDbContext.Teams.CountAsync(
                    x => x.Name == teamDto.Name && x.SeasonId == team.SeasonId) != 0)
                return (false, _configuration["ErrorMessages:TeamExists"]);

            team.Name = teamDto.Name;
            try
            {
                teamDto.Color = teamDto.Color.Trim();
                if (teamDto.Color.Substring(0, 1) != "#")
                    teamDto.Color = "#" + teamDto.Color;
                var color = ColorTranslator.FromHtml(teamDto.Color);

                team.Color = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            }
            catch (Exception)
            {
                return (false, _configuration["ErrorMessages:TeamColor"]);
            }
            
            _carRacingTournamentDbContext.Entry(team).State = EntityState.Modified;
            await _carRacingTournamentDbContext.SaveChangesAsync();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteTeam(Team team)
        {
            _carRacingTournamentDbContext.Teams.Remove(team);
            await _carRacingTournamentDbContext.SaveChangesAsync();

            return (true, null);
        }
    }
}
