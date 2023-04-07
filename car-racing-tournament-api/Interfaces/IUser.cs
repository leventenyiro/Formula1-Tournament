using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.Interfaces
{
    public interface IUser
    {
        (bool IsSuccess, string? Token, string? ErrorMessage) Login(User user, string password, bool needToken);
        Task<(bool IsSuccess, string? ErrorMessage)> Registration(RegistrationDto registrationDto);
        Task<(bool IsSuccess, User? User, string? ErrorMessage)> GetUserById(Guid id);
        Task<(bool IsSuccess, User? User, string? ErrorMessage)> GetUserByUsernameEmail(string usernameEmail);
        Task<(bool IsSuccess, string? ErrorMessage)> UpdateUser(User user, UpdateUserDto updateUserDto);
        Task<(bool IsSuccess, string? ErrorMessage)> UpdatePassword(User user, UpdatePasswordDto updatePasswordDto);
        Task<(bool IsSuccess, string? ErrorMessage)> DeleteUser(User user);
    }
}
