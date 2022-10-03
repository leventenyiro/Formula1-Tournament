namespace formula1_tournament_api.DTO
{
    public class ResultDto
    {
        public int Position { get; set; }
        public int Points { get; set; }
        public Guid DriverId { get; set; }
        public Guid TeamId { get; set; }
        public Guid RaceId { get; set; }
    }
}
