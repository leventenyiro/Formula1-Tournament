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

        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var result = await _driverService.GetAllDrivers();
            if (result.IsSuccess)
            {
                return Ok(result.Drivers);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _driverService.GetDriverById(id);
            if (result.IsSuccess)
            {
                return Ok(result.Driver);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] Driver driver)
        {
            if (_userSeasonService.HasPermission(new Guid(User.Identity.Name), driver.Season))
                return StatusCode(StatusCodes.Status403Forbidden);
            var result = await _driverService.AddDriver(driver);
            if (result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status201Created);
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Driver driver)
        {
            Driver driverObj = // what should I do here?
            if (_userSeasonService.HasPermission(new Guid(User.Identity.Name), driver.Season))
                return StatusCode(StatusCodes.Status403Forbidden);
            var result = await _driverService.UpdateDriver(id, driver);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (_userSeasonService.HasPermission(new Guid(User.Identity.Name), driver.Season))
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
