namespace car_racing_tournament_api.DTO
{
    public class RegistrationDto
    {
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string PasswordAgain { get; set; } = default!;
    }
}
