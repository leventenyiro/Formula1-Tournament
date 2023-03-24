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
        private IPermission _permissionService;
        private ISeason _seasonService;
        private IConfiguration _configuration;

        public UserController(
            IUser userService, 
            IPermission permissionService, 
            ISeason seasonService,
            IConfiguration configuration)
        {
            _userService = userService;
            _permissionService = permissionService;
            _seasonService = seasonService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var resultGetUser = await _userService.GetUserByUsernameEmail(loginDto.UsernameEmail);
            if (!resultGetUser.IsSuccess)
                return BadRequest(_configuration["ErrorMessages:LoginDetails"]);

            var result = _userService.Login(resultGetUser.User!, loginDto.Password, true);
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

        [HttpDelete, Authorize]
        public async Task<IActionResult> Delete([FromForm] string password) {
            var resultGetUser = await _userService.GetUserById(new Guid(User.Identity!.Name!));
            if (!resultGetUser.IsSuccess)
                return NotFound(resultGetUser.ErrorMessage);        
            
            var resultGetPermissions = await _permissionService.GetPermissionsByUser(resultGetUser.User!, Models.PermissionType.Admin);
            if (!resultGetPermissions.IsSuccess)
                return NotFound(resultGetPermissions.ErrorMessage);

            foreach (var permission in resultGetPermissions.Permissions!) {
                var resultGetSeason = await _seasonService.GetSeasonById(permission.SeasonId);
                if (!resultGetSeason.IsSuccess)
                    return NotFound(resultGetSeason.ErrorMessage);

                var resultDeleteSeason = await _seasonService.DeleteSeason(resultGetSeason.Season!);
                if (!resultDeleteSeason.IsSuccess)
                    return NotFound(resultDeleteSeason.ErrorMessage);
            }

            var resultDeleteUser = await _userService.DeleteUser(resultGetUser.User!, password);
            if (!resultDeleteUser.IsSuccess)
                return NotFound(resultDeleteUser.ErrorMessage);


            return NoContent();
        }
    }
}
