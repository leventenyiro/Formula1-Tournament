using formula1_tournament_api.Interfaces;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] string usernameEmail, [FromForm] string password)
        {
            var result = await _userService.Login(usernameEmail, password);
            if (result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status202Accepted, result.Token);
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromForm] string username, [FromForm] string email, [FromForm] string password, [FromForm] string passwordAgain)
        {
            var result = await _userService.Registration(username, email, password, passwordAgain);
            if (result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status201Created);
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> Get()
        {
            var userId = User?.Identity?.Name;
            var result = await _userService.GetUser(userId);
            if (result.IsSuccess)
            {
                return Ok(result.User);
            }
            return NotFound(result.ErrorMessage);
        }
    }
}
