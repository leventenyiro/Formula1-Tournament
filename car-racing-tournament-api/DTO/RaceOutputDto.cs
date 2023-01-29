using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.DTO
{
    public class RaceOutputDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTime DateTime { get; set; }
        public Guid SeasonId { get; set; }
        public List<Result>? Results { get; set; }
    }
}
