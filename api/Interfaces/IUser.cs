using api.DTO;
using api.Models;

namespace api.Interfaces
{
    public interface IUser
    {
        Task<(bool IsSuccess, string Token, string ErrorMessage)> Login(LoginDto loginDto);
        Task<(bool IsSuccess, string ErrorMessage)> Registration(RegistrationDto registrationDto);
        Task<(bool IsSuccess, User User, string ErrorMessage)> GetUser(string userId);
        Task<(bool IsSuccess, User User, string ErrorMessage)> GetUserByUsernameEmail(string usernameEmail);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateUser(Guid id, UpdateUserDto updateUserDto);
        Task<(bool IsSuccess, string ErrorMessage)> UpdatePassword(Guid id, UpdatePasswordDto updatePasswordDto);
    }
}
