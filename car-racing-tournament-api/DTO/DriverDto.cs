namespace car_racing_tournament_api.DTO
{
    public class DriverDto
    {
        public string Name { get; set; }
        public string RealName { get; set; }
        public int Number { get; set; }
        public Guid ActualTeamId { get; set; }
    }
}
