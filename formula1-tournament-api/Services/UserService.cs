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
            var actualUser = _formulaDbContext.User.Where(x => x.Username == usernameEmail || x.Email == usernameEmail).FirstOrDefault();
            if (actualUser == null || !BCrypt.Net.BCrypt.Verify(password, actualUser.Password))
            {
                return (false, null, "Incorrect username or password!");
            }

            string token = CreateToken(actualUser);

            return (true, token, "Successful login");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> Registration(string username, string email, string password, string passwordAgain)
        {
            if (password != passwordAgain)
            {
                return (false, "Passwords aren't pass!");
            }

            _formulaDbContext.Add(new User { Id = new Guid(), Username = username, Email = email, Password = HashPassword(password) });
            _formulaDbContext.SaveChanges();
            return (true, null);
        }

        public async Task<(bool IsSuccess, User User, string ErrorMessage)> GetUser(string userId)
        {
            var result = _formulaDbContext.User.Where(x => x.Id == Guid.Parse(userId)).FirstOrDefault();
            if (result != null)
            {
                return (true, result, null);
            }
            return (false, null, "User not found");
        }

        private string HashPassword(string password)
        {
            if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$"))
            {
                throw new Exception("Password should be minimum eight characters, at least one uppercase letter and one number!");
            }

            return BCrypt.Net.BCrypt.HashPassword(password, 10);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Secret").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
