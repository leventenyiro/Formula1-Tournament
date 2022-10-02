using formula1_tournament_api.DTO;
using formula1_tournament_api.Models;

namespace formula1_tournament_api.Interfaces
{
    public interface IUser
    {
        Task<(bool IsSuccess, string Token, string ErrorMessage)> Login(LoginDto loginDto);
        Task<(bool IsSuccess, string ErrorMessage)> Registration(RegistrationDto registrationDto);
        Task<(bool IsSuccess, User User, string ErrorMessage)> GetUser(string userId);
        Task<(bool IsSuccess, User User, string ErrorMessage)> GetUserByUsernameEmail(string usernameEmail);
    }
}
