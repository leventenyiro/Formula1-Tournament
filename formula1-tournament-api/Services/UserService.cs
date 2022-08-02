using formula1_tournament_api.Data;
using formula1_tournament_api.Models;

namespace formula1_tournament_api.Services
{
    public class UserService
    {
        private readonly FormulaDbContext _formulaDbContext;

        public UserService(FormulaDbContext formulaDbContext)
        {
            _formulaDbContext = formulaDbContext;
        }

        public async Task<(bool IsSuccess, string Token, string ErrorMessage)> Login(User user)
        {
            // find by name
            var actualUser = _formulaDbContext.User.Where(x => x.Username == user.Username).FirstOrDefault();
            if (actualUser == null)
            {
                return (false, null, "Incorrect username or password");
            }
            //return (false, null, "No seasons found");

            // check password
            if (actualUser.Password != actualUser.Password) // needs of encryption
            {
                
            } 
        }
    }
}
