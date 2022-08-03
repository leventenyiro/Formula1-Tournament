using System.Text.RegularExpressions;
using formula1_tournament_api.Data;
using formula1_tournament_api.Interfaces;
using formula1_tournament_api.Models;

namespace formula1_tournament_api.Services
{
    public class UserService : IUser
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
            if (verifyPassword(actualUser.Password, user.Password))
            {
                // then generate token
                string token = "";
                return (true, token, null);
            }
            // ...
        }

        public static string hashPassword(string password)
        {
            if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$"))
                throw new Exception("Password should be minimum eight characters, at least one uppercase letter and one number");

            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool verifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
