namespace car_racing_tournament_api.Models
{
    public class Race
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTime DateTime { get; set; }
        public Season Season { get; set; } = default!;
        public Guid SeasonId { get; set; }
        public List<Result>? Results { get; set; }
    }
}
