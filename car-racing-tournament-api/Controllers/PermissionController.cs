using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace car_racing_tournament_api.Controllers
{
    [Route("api/permission")]
    [ApiController]
    public class PermissionController : Controller
    {
        private IPermission _permissionService;

        public PermissionController(IPermission permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpDelete("id"), Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var resultGet = await _permissionService.GetPermissionById(id);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            if (!await _permissionService.IsAdminModerator(new Guid(User.Identity!.Name!), resultGet.Permission!.SeasonId))
                return Forbid();

            var resultDelete = await _permissionService.RemovePermission(new Guid(User.Identity!.Name!), moderatorId, seasonId);
            if (!resultDelete.IsSuccess)
                return BadRequest(resultDelete.ErrorMessage);

            return NoContent();
        }
    }
}
