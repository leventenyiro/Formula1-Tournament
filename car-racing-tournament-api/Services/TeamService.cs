using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using System.Drawing;

namespace car_racing_tournament_api.Services
{
    public class TeamService : ITeam
    {
        private readonly FormulaDbContext _formulaDbContext;

        public TeamService(FormulaDbContext formulaDbContext)
        {
            _formulaDbContext = formulaDbContext;
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteTeam(Guid id)
        {
            var team = _formulaDbContext.Teams.Where(e => e.Id == id).FirstOrDefault();
            if (team != null)
            {
                _formulaDbContext.Teams.Remove(team);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Team not found");
        }

        public async Task<(bool IsSuccess, Team Team, string ErrorMessage)> GetTeamById(Guid id)
        {
            var team = _formulaDbContext.Teams.Where(e => e.Id == id).FirstOrDefault();
            if (team != null)
            {
                return (true, team, null);
            }
            return (false, null, "Team not found");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateTeam(Guid id, TeamDto team)
        {
            var teamObj = _formulaDbContext.Teams.Where(e => e.Id == id).FirstOrDefault();
            if (teamObj != null)
            {
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
                _formulaDbContext.Teams.Update(teamObj);
                _formulaDbContext.SaveChanges();
                return (true, null);
            }
            return (false, "Team not found");
        }
    }
}
