using System.Text.Json.Serialization;

namespace car_racing_tournament_api.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Password { get; set; } = default!;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Permission>? Permissions { get; set; }
    }
}
