using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace car_racing_tournament_api.Controllers
{
    [Route("api/race")]
    [ApiController]
    public class RaceController : Controller
    {
        private IRace _raceService;
        private IUserSeason _userSeasonService;
        private IDriver _driverService;

        public RaceController(IRace raceService, IUserSeason userSeasonService, IDriver driverService)
        {
            _raceService = raceService;
            _userSeasonService = userSeasonService;
            _driverService = driverService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var resultGet = await _raceService.GetRaceById(id);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);
            
            return Ok(resultGet.Race);
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> Put(Guid id, [FromBody] RaceDto raceDto)
        {
            var resultGet = await _raceService.GetRaceById(id);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            if (!_userSeasonService.HasPermission(new Guid(User.Identity!.Name!), resultGet.Race!.SeasonId))
                return Forbid();
            
            var resultUpdate = await _raceService.UpdateRace(id, raceDto);
            if (!resultUpdate.IsSuccess)
                return BadRequest(resultUpdate.ErrorMessage);
            
            return NoContent();
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var resultGet = await _raceService.GetRaceById(id);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            if (!_userSeasonService.HasPermission(new Guid(User.Identity!.Name!), resultGet.Race!.SeasonId))
                return Forbid();

            var result = await _raceService.DeleteRace(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);
            
            return NoContent();
        }

        [HttpGet("{raceId}/result")]
        public async Task<IActionResult> GetResultsByRaceId(Guid raceId)
        {
            var result = await _raceService.GetResultsByRaceId(raceId);
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
            
            return Ok(result.Results);
        }

        [HttpPost("{raceId}/result"), Authorize]
        public async Task<IActionResult> PostResult(Guid raceId, [FromBody] ResultDto resultDto)
        {
            var resultGet = await _raceService.GetRaceById(raceId);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            if (!_userSeasonService.HasPermission(new Guid(User.Identity!.Name!), resultGet.Race!.SeasonId))
                return Forbid();

            var resultAdd = await _raceService.AddResult(raceId, resultDto);
            if (!resultAdd.IsSuccess)
                return BadRequest(resultAdd.ErrorMessage);

            var resultUpdate = await _driverService.UpdateDriverTeam(resultDto.DriverId, resultDto.TeamId);
            if (!resultUpdate.IsSuccess)
                return BadRequest(resultUpdate.ErrorMessage);
            
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
