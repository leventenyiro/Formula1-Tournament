using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace car_racing_tournament_api.Controllers
{
    [Route("api/permission")]
    [ApiController]
    public class PermissionController : Controller
    {
        private IPermission _permissionService;
        private ISeason _seasonService;

        public PermissionController(IPermission permissionService, ISeason seasonService)
        {
            _permissionService = permissionService;
            _seasonService = seasonService;
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> UpgradePermission(Guid id)
        {
            var resultGetPermission = await _permissionService.GetPermissionById(id);
            if (!resultGetPermission.IsSuccess)
                return NotFound(resultGetPermission.ErrorMessage);

            if (!await _permissionService.IsAdmin(new Guid(User.Identity!.Name!), resultGetPermission.Permission!.SeasonId))
                return Forbid();

            if (resultGetPermission.Permission.Type == PermissionType.Admin)
                return BadRequest("You cannot downgrade your permission in this way!");

            var resultGetAdmin = await _permissionService.GetPermissionByUserId(new Guid(User.Identity!.Name!));
            if (!resultGetAdmin.IsSuccess)
                return NotFound(resultGetAdmin.ErrorMessage);

            var resultDowngrade = await _permissionService.UpdatePermissionType(resultGetAdmin.Permission!, PermissionType.Moderator);
            if (!resultDowngrade.IsSuccess)
                return BadRequest(resultDowngrade.ErrorMessage);

            var resultUpdate = await _permissionService.UpdatePermissionType(resultGetPermission.Permission, PermissionType.Admin);
            if (!resultUpdate.IsSuccess)
                return BadRequest(resultUpdate.ErrorMessage);

            return NoContent();
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var resultGet = await _permissionService.GetPermissionById(id);
            if (!resultGet.IsSuccess)
                return NotFound(resultGet.ErrorMessage);

            if (!await _permissionService.IsAdmin(new Guid(User.Identity!.Name!), resultGet.Permission!.SeasonId) &&
                new Guid(User.Identity.Name!) != resultGet.Permission.UserId)
                return Forbid();

            if (resultGet.Permission.Type == PermissionType.Admin) {
                var resultGetSeason = await _seasonService.GetSeasonById(resultGet.Permission.SeasonId);
                if (!resultGetSeason.IsSuccess)
                    return NotFound(resultGetSeason.ErrorMessage);

                var resultGetPermissions = await _permissionService.GetPermissionsBySeason(resultGetSeason.Season!);
                if (!resultGetPermissions.IsSuccess)
                    return NotFound(resultGetPermissions.ErrorMessage);

                if (resultGetPermissions.Permissions!.Count > 1)
                    return BadRequest("You cannot downgrade your permission in this way!");
                else {
                    var resultDeleteSeason = await _seasonService.DeleteSeason(resultGetSeason.Season!);
                    if (!resultDeleteSeason.IsSuccess)
                        return BadRequest(resultDeleteSeason.ErrorMessage);
                }
            }
            else
            {
                var resultDelete = await _permissionService.RemovePermission(resultGet.Permission);
                if (!resultDelete.IsSuccess)
                    return BadRequest(resultDelete.ErrorMessage);
            }


            return NoContent();
        }
    }
}
