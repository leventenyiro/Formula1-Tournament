namespace car_racing_tournament_api.Models
{
    public class Season
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<UserSeason> UserSeasons { get; set; }
        public List<Team> Teams { get; set; }
        public List<Driver> Drivers { get; set; }
        public List<Race> Races { get; set; }
    }
}
