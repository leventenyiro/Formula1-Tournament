using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using formula1_tournament_api.Data;
using formula1_tournament_api.Interfaces;
using formula1_tournament_api.Models;
using Microsoft.IdentityModel.Tokens;

namespace formula1_tournament_api.Services
{
    public class UserService : IUser
    {
        private readonly FormulaDbContext _formulaDbContext;
        private readonly IConfiguration _configuration;

        public UserService(FormulaDbContext formulaDbContext, IConfiguration configuration)
        {
            _formulaDbContext = formulaDbContext;
            _configuration = configuration;
        }

        public async Task<(bool IsSuccess, string Token, string ErrorMessage)> Login(string usernameEmail, string password)
        {
            // find by name
            var actualUser = _formulaDbContext.User.Where(x => x.Username == usernameEmail).FirstOrDefault();
            if (actualUser == null || !BCrypt.Net.BCrypt.Verify(password, actualUser.Password))
            {
                return (false, null, "Incorrect username or password!");
            }

            // token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("Secret").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, actualUser.Id.ToString())
                    //new Claim(ClaimTypes.GivenName, actualUser.Username),
                    //new Claim(ClaimTypes.Email, actualUser.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // needs here cookie?

            return (true, tokenString, "Successful login");
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

        public async Task<(bool IsSuccess, User User, string ErrorMessage)> GetUser()
        {
            var result = _formulaDbContext.User.Where(x => x.Id == new Guid(ClaimTypes.NameIdentifier)).FirstOrDefault();
            if (result != null)
            {
                return (true, result, null);
            }
            return (false, null, "User not found");
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
