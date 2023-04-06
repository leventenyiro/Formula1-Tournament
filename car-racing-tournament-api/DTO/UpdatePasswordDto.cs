namespace car_racing_tournament_api.DTO
{
    public class UpdatePasswordDto
    {
        public string PasswordOld { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string PasswordAgain { get; set; } = default!;
    }
}
