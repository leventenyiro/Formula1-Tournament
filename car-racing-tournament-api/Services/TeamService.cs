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

        private const string TEAM_NOT_FOUND = "Team not found";

        public TeamService(CarRacingTournamentDbContext carRacingTournamentDbContext)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteTeam(Guid id)
        {
            var team = await _carRacingTournamentDbContext.Teams.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (team == null)
                return (false, TEAM_NOT_FOUND);
            
            _carRacingTournamentDbContext.Teams.Remove(team);
            _carRacingTournamentDbContext.SaveChanges();
            
            return (true, null);
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
                }).FirstOrDefaultAsync();
            if (team == null)
                return (false, null, TEAM_NOT_FOUND);
            
            return (true, team, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateTeam(Guid id, TeamDto team)
        {
            var teamObj = await _carRacingTournamentDbContext.Teams.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (teamObj == null)
                return (false, TEAM_NOT_FOUND);
            
            teamObj.Name = team.Name;
            try
            {
                ColorTranslator.FromHtml(team.Color);
                teamObj.Color = team.Color;
            }
            catch (Exception)
            {
                return (false, "Incorrect color code");
            }
            _carRacingTournamentDbContext.Teams.Update(teamObj);
            _carRacingTournamentDbContext.SaveChanges();
            
            return (true, null);
        }
    }
}
