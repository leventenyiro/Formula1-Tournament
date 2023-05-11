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
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public List<Result>? Results { get; set; }
    }

    public class Nationality
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;
        [JsonPropertyName("alpha2")]
        public string Alpha2 { get; set; } = default!;
        [JsonPropertyName("alpha3")]
        public string Alpha3 { get; set; } = default!;
    }
}
