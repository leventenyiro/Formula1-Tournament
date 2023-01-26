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
        private readonly CarRacingDbContext _carRacingDbContext;

        public TeamService(CarRacingDbContext carRacingDbContext)
        {
            _carRacingDbContext = carRacingDbContext;
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteTeam(Guid id)
        {
            var team = await _carRacingDbContext.Teams.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (team == null)
                return (false, "Team not found");
            
            _carRacingDbContext.Teams.Remove(team);
            _carRacingDbContext.SaveChanges();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, Team? Team, string? ErrorMessage)> GetTeamById(Guid id)
        {
            var team = await _carRacingDbContext.Teams.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (team == null)
                return (false, null, "Team not found");
            
            return (true, team, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateTeam(Guid id, TeamDto team)
        {
            var teamObj = await _carRacingDbContext.Teams.Where(e => e.Id == id).FirstOrDefaultAsync();
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
            _carRacingDbContext.Teams.Update(teamObj);
            _carRacingDbContext.SaveChanges();
            
            return (true, null);
        }
    }
}
