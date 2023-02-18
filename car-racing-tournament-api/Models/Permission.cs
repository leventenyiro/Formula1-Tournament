using System.Text.Json.Serialization;

namespace car_racing_tournament_api.Models
{
    public class Permission
    {
        public Guid Id { get; set; }
        public DateTime Joined { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual User User { get; set; } = default!;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual Season Season { get; set; } = default!;
        public Guid UserId { get; set; }
        public Guid SeasonId { get; set; }
        public PermissionType Type { get; set; }
    }

    public enum PermissionType
    {
        Moderator,
        Admin
    }
}
