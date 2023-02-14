using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.Interfaces
{
    public interface IPermission
    {
        public Task<bool> IsAdmin(Guid userId, Guid seasonId);
        public Task<bool> IsAdminModerator(Guid userId, Guid seasonId);
        public Task<(bool IsSuccess, Permission? Permission, string? ErrorMessage)> GetPermissionById(Guid id);
        public Task<(bool IsSuccess, string? ErrorMessage)> AddPermission(PermissionDto permissionDto);
        public Task<(bool IsSuccess, string? ErrorMessage)> UpdatePermissionType(Permission permission, PermissionType permissionType);
        public Task<(bool IsSuccess, string? ErrorMessage)> RemovePermission(Permission permission);
    }
}
