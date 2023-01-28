namespace car_racing_tournament_api.Models
{
    public class Driver
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? RealName { get; set; }
        public int Number { get; set; }

        public Guid ActualTeamId { get; set; }
        public Guid SeasonId { get; set; }
        public List<Result>? Results { get; set; }
    }
}
