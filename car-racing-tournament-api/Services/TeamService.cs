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

        public TeamService(CarRacingTournamentDbContext carRacingTournamentDbContext)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteTeam(Guid id)
        {
            var team = await _carRacingTournamentDbContext.Teams.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (team == null)
                return (false, "Team not found");
            
            _carRacingTournamentDbContext.Teams.Remove(team);
            _carRacingTournamentDbContext.SaveChanges();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, Team? Team, string? ErrorMessage)> GetTeamById(Guid id)
        {
            var team = await _carRacingTournamentDbContext.Teams.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (team == null)
                return (false, null, "Team not found");
            
            return (true, team, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateTeam(Guid id, TeamDto team)
        {
            var teamObj = await _carRacingTournamentDbContext.Teams.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (teamObj == null)
                return (false, "Team not found");
            
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
