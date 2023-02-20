using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.DTO
{
    public class PermissionOutputDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = default!;
        public PermissionType Type { get; set; }
    }
}
