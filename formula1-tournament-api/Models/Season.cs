namespace formula1_tournament_api.Models
{
    public class Season
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public List<Team> Teams { get; set; }
        public List<Driver> Drivers { get; set; }
        public List<Race> Races { get; set; }
    }
}
