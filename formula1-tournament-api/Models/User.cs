namespace formula1_tournament_api.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Season> Seasons { get; set; }
        public List<UserSeason> UserSeasons { get; set; }
    }
}
