namespace api.Models
{
    public class Result
    {
        public Guid Id { get; set; }
        public int Position { get; set; }
        public int Points { get; set; }
        public Driver Driver { get; set; }
        public Team Team { get; set; }
        public Race Race { get; set; }
        public Guid DriverId { get; set; }
        public Guid TeamId { get; set; }
        public Guid RaceId { get; set; }
    }
}
