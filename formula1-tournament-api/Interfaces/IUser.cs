using formula1_tournament_api.Models;

namespace formula1_tournament_api.Interfaces
{
    public interface IUser
    {
        Task<(bool IsSuccess, User User, string ErrorMessage)> Login(string usernameEmail, string password);
        Task<(bool IsSuccess, string ErrorMessage)> Registration(string username, string password, string passwordAgain);
    }
}
