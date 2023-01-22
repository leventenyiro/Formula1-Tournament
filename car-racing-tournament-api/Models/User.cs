namespace car_racing_tournament_api.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public List<UserSeason>? UserSeasons { get; set; }
    }
}
