using System.Text.Json.Serialization;

namespace car_racing_tournament_api.Models
{
    public class Race
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTime? DateTime { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual Season Season { get; set; } = default!;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid SeasonId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Result>? Results { get; set; }
    }
}
