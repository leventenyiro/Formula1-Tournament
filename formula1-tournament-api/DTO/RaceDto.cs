namespace formula1_tournament_api.DTO
{
    public class RaceDto
    {
        public string Name { get; set; }
        public int Position { get; set; }
        public int Points { get; set; }
        public Guid DriverId { get; set; }
        public Guid SeasonId { get; set; }
    }
}
