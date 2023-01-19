using car_racing_tournament_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace car_racing_tournament_api.Controllers
{
    [Route("api/userseason")]
    [ApiController]
    public class UserSeasonController : Controller
    {
        private IUserSeason _userSeasonService;
        private IUser _userService;

        public UserSeasonController(IUserSeason userSeasonService)
        {
            _userSeasonService = userSeasonService;
        }

        [HttpPost("{seasonId}")]
        [Authorize]
        public async Task<IActionResult> Post(Guid seasonId, [FromForm] string usernameEmail)
        {
            if (!_userSeasonService.HasPermission(new Guid(User.Identity.Name), seasonId))
                return StatusCode(StatusCodes.Status403Forbidden);
            var result = await _userService.GetUserByUsernameEmail(usernameEmail);
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
            var result2 = await _userSeasonService.AddModerator(new Guid(User.Identity.Name), result.User.Id, seasonId);
            if (result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status201Created);
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("{seasonId}/{moderatorId}")]
        public async Task<IActionResult> Delete(Guid seasonId, Guid moderatorId)
        {
            if (!_userSeasonService.HasPermission(new Guid(User.Identity.Name), seasonId))
                return StatusCode(StatusCodes.Status403Forbidden);
            var result = await _userSeasonService.RemovePermission(new Guid(User.Identity.Name), moderatorId, seasonId);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }
    }
}
