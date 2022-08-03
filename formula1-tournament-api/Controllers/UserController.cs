using formula1_tournament_api.Interfaces;
using formula1_tournament_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace formula1_tournament_api.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class UserController : Controller
    {
        private IUser _userService;

        public UserController(IUser userService)
        {
            _userService = userService;
        }

        /*[HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var result = await _seasonService.GetAllSeasons();
            if (result.IsSuccess)
            {
                return Ok(result.Season);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _seasonService.GetSeasonById(id);
            if (result.IsSuccess)
            {
                return Ok(result.Season);
            }
            return NotFound(result.ErrorMessage);
        }*/

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var result = await _userService.Login(user);
            if (result.IsSuccess)
            {
                return Ok(result.Token);
            }
            return BadRequest(result.ErrorMessage);
        }

        /*[HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Season season)
        {
            var result = await _seasonService.UpdateSeason(id, season);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _seasonService.DeleteSeason(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }*/
    }
}
