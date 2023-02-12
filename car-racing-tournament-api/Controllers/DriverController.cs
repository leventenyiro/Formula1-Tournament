using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace car_racing_tournament_api.Controllers
{
    [Route("api/driver")]
    [ApiController]
    public class DriverController : Controller
    {
        private IDriver _driverService;
        private IUserSeason _userSeasonService;
        private ITeam _teamService;

        public DriverController(IDriver driverService, IUserSeason userSeasonService, ITeam teamService)
        {
            _driverService = driverService;
            _userSeasonService = userSeasonService;
            _teamService = teamService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var resultGet = await _driverService.GetDriverById(id);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);
            
            return Ok(resultGet.Driver);
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> Put(Guid id, [FromBody] DriverDto driverDto)
        {
            var resultGetDriver = await _driverService.GetDriverById(id);
            if (!resultGetDriver.IsSuccess)
                return NotFound(resultGetDriver.ErrorMessage);

            if (!await _userSeasonService.IsAdminModerator(new Guid(User.Identity!.Name!), resultGetDriver.Driver!.SeasonId))
                return Forbid();

            var resultGetTeam = await _teamService.GetTeamById(id);
            if (!resultGetTeam.IsSuccess)
                return NotFound(resultGetTeam.ErrorMessage);

            var resultUpdate = await _driverService.UpdateDriver(resultGetDriver.Driver, driverDto, resultGetTeam.Team!);
            if (!resultUpdate.IsSuccess)
                return BadRequest(resultUpdate.ErrorMessage);

            return NoContent();
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var resultGet = await _driverService.GetDriverById(id);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            if (!await _userSeasonService.IsAdminModerator(new Guid(User.Identity!.Name!), resultGet.Driver!.SeasonId))
                return Forbid();

            var resultDelete = await _driverService.DeleteDriver(resultGet.Driver);
            if (!resultDelete.IsSuccess)
                return BadRequest(resultDelete.ErrorMessage);

            return NoContent();
        }
    }
}
