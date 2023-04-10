using car_racing_tournament_api.Models;
using System.Text.Json.Serialization;

namespace car_racing_tournament_api.DTO
{
    public class SeasonOutputDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public bool IsArchived { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Favorite { get; set; }
        public List<PermissionOutputDto> Permissions { get; set; } = default!;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Team>? Teams { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Driver>? Drivers { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Race>? Races { get; set; }
    }
}
