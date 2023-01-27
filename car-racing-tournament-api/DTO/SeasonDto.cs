using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.DTO
{
    public class SeasonDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}
