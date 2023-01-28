namespace car_racing_tournament_api.Models
{
    public class UserSeason
    {
        public Guid Id { get; set; }
        public virtual User User { get; set; } = default!;
        public virtual Season Season { get; set; } = default!;
        public Guid UserId { get; set; }
        public Guid SeasonId { get; set; }
        public UserSeasonPermission Permission { get; set; }
    }

    public enum UserSeasonPermission
    {
        Moderator,
        Admin
    }
}
