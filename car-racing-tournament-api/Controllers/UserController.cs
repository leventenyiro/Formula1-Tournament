using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace car_racing_tournament_api.Controllers
{
    [Route("api/user")]
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

            return StatusCode(StatusCodes.Status202Accepted, result.Token);
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
            var result = await _userService.GetUserById(new Guid(User.Identity!.Name!));
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
            
            return Ok(result.User);
        }

        [HttpPut, Authorize]
        public async Task<IActionResult> Put([FromBody] UpdateUserDto updateUserDto)
        {
            var resultGetUser = await _userService.GetUserById(new Guid(User.Identity!.Name!));
            if (!resultGetUser.IsSuccess)
                return NotFound(resultGetUser.ErrorMessage);

            var resultUpdate = await _userService.UpdateUser(resultGetUser.User!, updateUserDto);
            if (!resultUpdate.IsSuccess)
                return BadRequest(resultUpdate.ErrorMessage);
            
            return NoContent();
        }

        [HttpPut("password"), Authorize]
        public async Task<IActionResult> Put([FromBody] UpdatePasswordDto updatePasswordDto)
        {
            var resultGetUser = await _userService.GetUserById(new Guid(User.Identity!.Name!));
            if (!resultGetUser.IsSuccess)
                return NotFound(resultGetUser.ErrorMessage);

            var result = await _userService.UpdatePassword(resultGetUser.User!, updatePasswordDto);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);
            
            return NoContent();
        }
    }
}
