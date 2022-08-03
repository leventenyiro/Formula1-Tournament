using formula1_tournament_api.Models;

namespace formula1_tournament_api.Interfaces
{
    public interface IUser
    {
        Task<(bool IsSuccess, User User, string ErrorMessage)> Login(User user);
        Task<(bool IsSuccess, string ErrorMessage)> Registration(string username, string password, string passwordAgain);
    }
}
