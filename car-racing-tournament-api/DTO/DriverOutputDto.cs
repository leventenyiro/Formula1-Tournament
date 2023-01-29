using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.DTO
{
    public class DriverOutputDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? RealName { get; set; }
        public int Number { get; set; }
        public Guid? ActualTeamId { get; set; }
        public Guid SeasonId { get; set; }
        public List<Result>? Results { get; set; }
    }
}
