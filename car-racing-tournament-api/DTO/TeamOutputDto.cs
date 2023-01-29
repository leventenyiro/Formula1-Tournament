using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.DTO
{
    public class TeamOutputDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Color { get; set; } = default!;
        public Guid SeasonId { get; set; }
        public List<Driver>? Drivers { get; set; }
        public List<Result>? Results { get; set; }
    }
}
