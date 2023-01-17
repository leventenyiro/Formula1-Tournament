using api.Models;

namespace api.DTO
{
    public class SeasonDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<UserSeasonDto> UserSeasons { get; set; }
    }

    public class UserSeasonDto
    {
        public string Username { get; set; }
        public UserSeasonPermission Permission { get; set; }
    }
}
