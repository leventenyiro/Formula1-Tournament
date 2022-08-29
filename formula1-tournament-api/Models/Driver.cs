namespace formula1_tournament_api.Models
{
    public class Driver
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid TeamId { get; set; }
        public Guid SeasonId { get; set; }
    }
}
