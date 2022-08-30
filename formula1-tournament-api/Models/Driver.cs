namespace formula1_tournament_api.Models
{
    public class Driver
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Team Team { get; set; }
        public Season Season { get; set; }

        public Guid TeamId { get; set; }
        public Guid SeasonId { get; set; }
        public List<Race> Races { get; set; }
    }
}
