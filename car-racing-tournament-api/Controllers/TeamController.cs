using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace car_racing_tournament_api.Controllers
{
    [Route("api/team")]
    [ApiController]
    public class TeamController : Controller
    {
        private ITeam _teamService;
        private IUserSeason _userSeasonService;

        public TeamController(ITeam teamService, IUserSeason userSeasonService)
        {
            _teamService = teamService;
            _userSeasonService = userSeasonService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var resultGet = await _teamService.GetTeamById(id);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            return Ok(resultGet.Team);
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> Put(Guid id, [FromBody] TeamDto teamDto)
        {
            var resultGet = await _teamService.GetTeamById(id);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            if (!await _userSeasonService.IsAdminModerator(new Guid(User.Identity!.Name!), resultGet.Team!.SeasonId))
                return Forbid();

            var resultUpdate = await _teamService.UpdateTeam(resultGet.Team, teamDto);
            if (!resultUpdate.IsSuccess)
                return BadRequest(resultUpdate.ErrorMessage);

            return NoContent();
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var resultGet = await _teamService.GetTeamById(id);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            if (!await _userSeasonService.IsAdminModerator(new Guid(User.Identity!.Name!), resultGet.Team!.SeasonId))
                return Forbid();

            var resultDelete = await _teamService.DeleteTeam(resultGet.Team);
            if (!resultDelete.IsSuccess)
                return BadRequest(resultDelete.ErrorMessage);

            return NoContent();
        }
    }
}
