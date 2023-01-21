using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace car_racing_tournament_api.Controllers
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
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            //return StatusCode(StatusCodes.Status202Accepted, result.Token);
            return Accepted(result.Token);
        }

        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromBody] RegistrationDto registrationDto)
        {
            var result = await _userService.Registration(registrationDto);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);
            
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> Get()
        {
            if (User.Identity?.Name == null)
                return Unauthorized();

            var result = await _userService.GetUser(User.Identity.Name);
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
            
            return Ok(result.User);
        }

        [HttpPut("update/user"), Authorize]
        public async Task<IActionResult> Put([FromBody] UpdateUserDto updateUserDto)
        {
            if (User.Identity?.Name == null)
                return Unauthorized();

            var result = await _userService.UpdateUser(new Guid(User.Identity.Name), updateUserDto);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);
            
            return NoContent();
        }

        [HttpPut("update/password"), Authorize]
        public async Task<IActionResult> Put([FromBody] UpdatePasswordDto updatePasswordDto)
        {
            if (User.Identity?.Name == null)
                return Unauthorized();

            var result = await _userService.UpdatePassword(new Guid(User.Identity.Name), updatePasswordDto);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);
            
            return NoContent();
        }
    }
}
