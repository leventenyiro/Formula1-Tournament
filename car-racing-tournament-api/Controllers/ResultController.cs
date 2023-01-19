using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace car_racing_tournament_api.Controllers
{
    [Route("api/result")]
    [ApiController]
    public class ResultController : Controller
    {
        private Interfaces.IResult _resultService;
        private IUserSeason _userSeasonService;
        private IDriver _driverService;

        public ResultController(Interfaces.IResult resultService, IUserSeason userSeasonService, IDriver driverService)
        {
            _resultService = resultService;
            _userSeasonService = userSeasonService;
            _driverService = driverService;
        }

        [HttpGet("{seasonId}")]
        [EnableQuery]
        public async Task<IActionResult> Get(Guid raceId)
        {
            var result = await _resultService.GetAllResultsByRaceId(raceId);
            if (result.IsSuccess)
            {
                return Ok(result.Results);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpGet("{seasonId}/{id}")]
        public async Task<IActionResult> Get(Guid seasonId, Guid id)
        {
            var result = await _resultService.GetResultById(id);
            if (result.IsSuccess)
            {
                return Ok(result.Result);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpPost("{seasonId}")]
        public async Task<IActionResult> Post(Guid seasonId, [FromBody] ResultDto resultDto)
        {
            if (!_userSeasonService.HasPermission(new Guid(User.Identity.Name), seasonId))
                return StatusCode(StatusCodes.Status403Forbidden);
            var result = await _resultService.AddResult(resultDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            var result2 = await _driverService.UpdateDriverTeam(resultDto.DriverId, resultDto.TeamId);
            if (!result2.IsSuccess)
            {
                return BadRequest(result2.ErrorMessage);
            }
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{seasonId}/{id}")]
        public async Task<IActionResult> Put(Guid seasonId, Guid id, [FromBody] ResultDto resultDto)
        {
            if (!_userSeasonService.HasPermission(new Guid(User.Identity.Name), seasonId))
                return StatusCode(StatusCodes.Status403Forbidden);
            var result = await _resultService.UpdateResult(id, resultDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            var result2 = await _driverService.UpdateDriverTeam(resultDto.DriverId, resultDto.TeamId);
            if (!result2.IsSuccess)
            {
                return BadRequest(result2.ErrorMessage);
            }
            return NoContent();
        }

        [HttpDelete("{seasonId}/{id}")]
        public async Task<IActionResult> Delete(Guid seasonId, Guid id)
        {
            if (!_userSeasonService.HasPermission(new Guid(User.Identity.Name), seasonId))
                return StatusCode(StatusCodes.Status403Forbidden);
            var result = await _resultService.DeleteResult(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }
    }
}
