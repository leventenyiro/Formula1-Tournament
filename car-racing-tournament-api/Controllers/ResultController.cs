using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _resultService.GetResultById(id);
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
            
            return Ok(result.Result);
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> Put(Guid id, [FromBody] ResultDto resultDto)
        {
            var resultGetResult = await _resultService.GetResultById(id);
            if (!resultGetResult.IsSuccess)
                return BadRequest(resultGetResult.ErrorMessage);

            var resultGetDriver = await _driverService.GetDriverById(resultGetResult.Result!.DriverId);
            if (!resultGetDriver.IsSuccess)
                return BadRequest(resultGetDriver.ErrorMessage);

            if (!await _userSeasonService.IsAdminModerator(new Guid(User.Identity!.Name!), resultGetDriver.Driver!.SeasonId))
                return Forbid();

            var resultUpdate = await _resultService.UpdateResult(id, resultDto);
            if (!resultUpdate.IsSuccess)
                return BadRequest(resultUpdate.ErrorMessage);

            if (resultGetDriver.Driver.ActualTeamId != resultDto.TeamId)
            {
                var resultUpdateTeam = await _driverService.UpdateDriverTeam(resultDto.DriverId, resultDto.TeamId);
                if (!resultUpdateTeam.IsSuccess)
                    return BadRequest(resultUpdateTeam.ErrorMessage);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var resultGetResult = await _resultService.GetResultById(id);
            if (!resultGetResult.IsSuccess)
                return BadRequest(resultGetResult.ErrorMessage);

            var resultGetDriver = await _driverService.GetDriverById(resultGetResult.Result!.DriverId);
            if (!resultGetDriver.IsSuccess)
                return BadRequest(resultGetDriver.ErrorMessage);

            if (!await _userSeasonService.IsAdminModerator(new Guid(User.Identity!.Name!), resultGetDriver.Driver!.SeasonId))
                return Forbid();

            var resultDelete = await _resultService.DeleteResult(id);
            if (!resultDelete.IsSuccess)
                return BadRequest(resultDelete.ErrorMessage);

            return NoContent();
        }
    }
}
