using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.Interfaces
{
    public interface IPermission
    {
        Task<bool> IsAdmin(Guid userId, Guid seasonId);
        Task<bool> IsAdminModerator(Guid userId, Guid seasonId);
        Task<(bool IsSuccess, Permission? Permission, string? ErrorMessage)> GetPermissionBySeasonAndUser(Season season, Guid userId);
        Task<(bool IsSuccess, List<PermissionOutputDto>? Permissions, string? ErrorMessage)> GetPermissionsBySeason(Season season);
        Task<(bool IsSuccess, List<PermissionOutputDto>? Permissions, string? ErrorMessage)> GetPermissionsByUser(User user, PermissionType? permissionTypeFilter);
        Task<(bool IsSuccess, Permission? Permission, string? ErrorMessage)> GetPermissionById(Guid id);
        Task<(bool IsSuccess, string? ErrorMessage)> AddPermission(User user, Season season, PermissionType permissionType);
        Task<(bool IsSuccess, string? ErrorMessage)> UpdatePermissionType(Permission permission, PermissionType permissionType);
        Task<(bool IsSuccess, string? ErrorMessage)> RemovePermission(Permission permission);
    }
}
