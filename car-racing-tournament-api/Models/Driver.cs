using System.Text.Json.Serialization;

namespace car_racing_tournament_api.Models
{
    public class Driver
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? RealName { get; set; }
        public int Number { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public virtual Team? ActualTeam { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public virtual Season Season { get; set; } = default!;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid? ActualTeamId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid SeasonId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Result>? Results { get; set; }
    }
}
