using formula1_tournament_api.Models;

namespace formula1_tournament_api.Interfaces
{
    public class IUser
    {
        Task<(bool IsSuccess, User user, string ErrorMessage)> Login(User user);

        Task<(bool IsSuccess, string ErrorMessage)> Registration(User user);
    }
}
