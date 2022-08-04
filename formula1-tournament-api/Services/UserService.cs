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

        public async Task<(bool IsSuccess, User User, string ErrorMessage)> Login(string usernameEmail, string password)
        {
            // find by name
            var actualUser = _formulaDbContext.User.Where(x => x.Username == usernameEmail).FirstOrDefault();
            if (actualUser == null)
            {
                return (false, null, "Incorrect username or password!");
            }
            //return (false, null, "No seasons found");

            // check password
            if (!BCrypt.Net.BCrypt.Verify(actualUser.Password, password))
            {
                return (false, null, "Incorrect username or password");
            }
            return (true, actualUser, "Successful login");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> Registration(string username, string password, string passwordAgain)
        {
            if (password != passwordAgain)
            {
                return (false, "Passwords aren't pass!");
            }
            // insert
            _formulaDbContext.Add(new User { Id = new Guid(), Username = username, Password = HashPassword(password) });
            _formulaDbContext.SaveChanges();
            return (true, null);
        }

        public static string HashPassword(string password)
        {
            if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$"))
            {
                throw new Exception("Password should be minimum eight characters, at least one uppercase letter and one number!");
            }

            return BCrypt.Net.BCrypt.HashPassword(password, 10);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
