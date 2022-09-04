using formula1_tournament_api.Interfaces;
using formula1_tournament_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace formula1_tournament_api.Controllers
{
    [Route("api/race")]
    [ApiController]
    public class RaceController : Controller
    {
        private IRace _raceService;
        private IUserSeason _userSeasonService;

        public RaceController(IRace raceService, IUserSeason userSeasonService)
        {
            _raceService = raceService;
            _userSeasonService = userSeasonService;
        }

        [HttpGet("{seasonId}")]
        [EnableQuery]
        public async Task<IActionResult> Get(Guid seasonId)
        {
            var result = await _raceService.GetAllRacesBySeasonId(seasonId);
            if (result.IsSuccess)
            {
                return Ok(result.Races);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpGet("{seasonId}/{id}")]
        public async Task<IActionResult> Get(Guid seasonId, Guid id)
        {
            var result = await _raceService.GetRaceById(id);
            if (result.IsSuccess)
            {
                return Ok(result.Race);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpPost("{seasonId}")]
        public async Task<IActionResult> Post(Guid seasonId, [FromBody] Race race)
        {
            if (!_userSeasonService.HasPermission(new Guid(User.Identity.Name), seasonId))
                return StatusCode(StatusCodes.Status403Forbidden);
            var result = await _raceService.AddRace(race);
            if (result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status201Created);
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("{seasonId}/{id}")]
        public async Task<IActionResult> Put(Guid seasonId, Guid id, [FromBody] Race race)
        {
            if (!_userSeasonService.HasPermission(new Guid(User.Identity.Name), seasonId))
                return StatusCode(StatusCodes.Status403Forbidden);
            var result = await _raceService.UpdateRace(id, race);
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
            var result = await _raceService.DeleteRace(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }
    }
}
