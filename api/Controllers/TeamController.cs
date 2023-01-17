using api.DTO;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace api.Controllers
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

        [HttpGet("{seasonId}")]
        [EnableQuery]
        public async Task<IActionResult> Get(Guid seasonId)
        {
            var result = await _teamService.GetAllTeamsBySeasonId(seasonId);
            if (result.IsSuccess)
            {
                return Ok(result.Teams);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpGet("{seasonId}/{id}")]
        public async Task<IActionResult> Get(Guid seasonId, Guid id)
        {
            var result = await _teamService.GetTeamById(id);
            if (result.IsSuccess)
            {
                return Ok(result.Team);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpPost("{seasonId}")]
        public async Task<IActionResult> Post(Guid seasonId, [FromBody] TeamDto team)
        {
            if (!_userSeasonService.HasPermission(new Guid(User.Identity.Name), seasonId))
                return StatusCode(StatusCodes.Status403Forbidden);
            var result = await _teamService.AddTeam(team);
            if (result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status201Created);
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("{seasonId}/{id}")]
        public async Task<IActionResult> Put(Guid seasonId, Guid id, [FromBody] TeamDto team)
        {
            if (!_userSeasonService.HasPermission(new Guid(User.Identity.Name), seasonId))
                return StatusCode(StatusCodes.Status403Forbidden);
            var result = await _teamService.UpdateTeam(id, team);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("{seasonId}/{id}")]
        public async Task<IActionResult> Delete(Guid seasonId, Guid id)
        {
            if (!_userSeasonService.HasPermission(new Guid(User.Identity.Name), seasonId))
                return StatusCode(StatusCodes.Status403Forbidden);
            var result = await _teamService.DeleteTeam(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }
    }
}
