namespace car_racing_tournament_api.DTO
{
    public class RaceDto
    {
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public Guid SeasonId { get; set; }
    }
}
