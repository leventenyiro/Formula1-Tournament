namespace car_racing_tournament_api.DTO
{
    public class ResultDto
    {
        public int Position { get; set; }
        public int Point { get; set; }
        public Guid DriverId { get; set; }
        public Guid TeamId { get; set; }
    }
}
