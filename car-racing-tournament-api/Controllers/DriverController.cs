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

        public DriverController(IDriver driverService, IUserSeason userSeasonService)
        {
            _driverService = driverService;
            _userSeasonService = userSeasonService;
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
            var resultGet = await _driverService.GetDriverById(id);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            if (!await _userSeasonService.HasPermission(new Guid(User.Identity!.Name!), resultGet.Driver!.SeasonId))
                return Forbid();

            var resultUpdate = await _driverService.UpdateDriver(id, driverDto);
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

            if (!await _userSeasonService.HasPermission(new Guid(User.Identity!.Name!), resultGet.Driver!.SeasonId))
                return Forbid();

            var resultDelete = await _driverService.DeleteDriver(id);
            if (!resultDelete.IsSuccess)
                return BadRequest(resultDelete.ErrorMessage);

            return NoContent();
        }
    }
}
