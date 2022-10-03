using formula1_tournament_api.DTO;
using formula1_tournament_api.Interfaces;
using formula1_tournament_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace formula1_tournament_api.Controllers
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

        [HttpGet("{seasonId}")]
        [EnableQuery]
        public async Task<IActionResult> Get(Guid seasonId)
        {
            var result = await _driverService.GetAllDriversBySeasonId(seasonId);
            if (result.IsSuccess)
            {
                return Ok(result.Drivers);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpGet("{seasonId}/{id}")]
        public async Task<IActionResult> Get(Guid seasonId, Guid id)
        {
            var result = await _driverService.GetDriverById(id);
            if (result.IsSuccess)
            {
                return Ok(result.Driver);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpPost("{seasonId}")]
        [Authorize]
        public async Task<IActionResult> Post(Guid seasonId, [FromBody] DriverDto driverDto)
        {
            if (!_userSeasonService.HasPermission(new Guid(User.Identity.Name), seasonId))
                return StatusCode(StatusCodes.Status403Forbidden);
            var result = await _driverService.AddDriver(driverDto);
            if (result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status201Created);
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("{seasonId}/{id}")]
        public async Task<IActionResult> Put(Guid seasonId, Guid id, [FromBody] DriverDto driverDto)
        {
            if (!_userSeasonService.HasPermission(new Guid(User.Identity.Name), seasonId))
                return StatusCode(StatusCodes.Status403Forbidden);
            var result = await _driverService.UpdateDriver(id, driverDto);
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
            var result = await _driverService.DeleteDriver(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }
    }
}
