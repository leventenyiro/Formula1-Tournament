using api.DTO;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
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
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _userService.Login(loginDto);
            if (result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status202Accepted, result.Token);
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromBody] RegistrationDto registrationDto)
        {
            var result = await _userService.Registration(registrationDto);
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

        [HttpPut("update/user"), Authorize]
        public async Task<IActionResult> Put([FromBody] UpdateUserDto updateUserDto)
        {
            var result = await _userService.UpdateUser(new Guid(User.Identity.Name), updateUserDto);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("update/password"), Authorize]
        public async Task<IActionResult> Put([FromBody] UpdatePasswordDto updatePasswordDto)
        {
            var result = await _userService.UpdatePassword(new Guid(User.Identity.Name), updatePasswordDto);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }
    }
}
