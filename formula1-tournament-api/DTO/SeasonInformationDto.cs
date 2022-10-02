using formula1_tournament_api.Models;

namespace formula1_tournament_api.DTO
{
    public class SeasonInformationDto
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
