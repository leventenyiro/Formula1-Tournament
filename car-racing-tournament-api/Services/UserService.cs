using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace car_racing_tournament_api.Services
{
    public class UserService : IUser
    {
        private readonly CarRacingTournamentDbContext _carRacingTournamentDbContext;
        private readonly IConfiguration _configuration;

        private const string INCORRECT_EMAIL_FORMAT = "It is not a valid email";
        private const string USER_NOT_FOUND = "User not found";
        private const string PASSWORD_NOT_PASS = "Passwords aren't pass!";

        public UserService(CarRacingTournamentDbContext carRacingTournamentDbContext, IConfiguration configuration)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
            _configuration = configuration;
        }

        public async Task<(bool IsSuccess, string? Token, string? ErrorMessage)> Login(LoginDto loginDto)
        {
            var actualUser = await _carRacingTournamentDbContext.Users.Where(x => x.Username == loginDto.UsernameEmail || x.Email == loginDto.UsernameEmail).FirstOrDefaultAsync();
            if (actualUser == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, actualUser.Password))
                return (false, null, "Incorrect username or password!");

            string token = CreateToken(actualUser);

            return (true, token, "Successful login");
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> Registration(RegistrationDto registrationDto)
        {
            if (!IsEmail(registrationDto.Email))
                return (false, INCORRECT_EMAIL_FORMAT);

            if (registrationDto.Password != registrationDto.PasswordAgain)
                return (false, PASSWORD_NOT_PASS);

            await _carRacingTournamentDbContext.AddAsync(new User { 
                Id = new Guid(), 
                Username = registrationDto.Username,
                Email = registrationDto.Email,
                Password = HashPassword(registrationDto.Password) 
            });
            _carRacingTournamentDbContext.SaveChanges();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, User? User, string? ErrorMessage)> GetUser(string userId)
        {
            var result = await _carRacingTournamentDbContext.Users.Where(x => x.Id == Guid.Parse(userId)).FirstOrDefaultAsync();
            if (result == null)
                return (false, null, USER_NOT_FOUND);
            
            return (true, result, null);
        }

        public async Task<(bool IsSuccess, User? User, string? ErrorMessage)> GetUserByUsernameEmail(string usernameEmail)
        {
            var actualUser = await _carRacingTournamentDbContext.Users.Where(x => x.Username == usernameEmail || x.Email == usernameEmail).FirstOrDefaultAsync();
            if (actualUser == null)
                return (false, null, USER_NOT_FOUND);

            return (true, actualUser, null);
        }

        private string HashPassword(string password)
        {
            if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$"))
                throw new Exception("Password should be minimum eight characters, at least one uppercase letter and one number!");

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

        private bool IsEmail(string email)
        {
            return Regex.IsMatch(email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateUser(Guid id, UpdateUserDto updateUserDto)
        {
            if (!IsEmail(updateUserDto.Email))
                return (false, INCORRECT_EMAIL_FORMAT);

            var userObj = await _carRacingTournamentDbContext.Users.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (userObj == null)
                return (false, USER_NOT_FOUND);
            
            userObj.Username = updateUserDto.Username;
            userObj.Email = updateUserDto.Email;
            _carRacingTournamentDbContext.Users.Update(userObj);
            _carRacingTournamentDbContext.SaveChanges();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdatePassword(Guid id, UpdatePasswordDto updatePasswordDto)
        {
            var userObj = await _carRacingTournamentDbContext.Users.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (userObj == null)
                return (false, USER_NOT_FOUND);

            if (updatePasswordDto.Password != updatePasswordDto.PasswordAgain)
                return (false, PASSWORD_NOT_PASS);
            
            userObj.Password = HashPassword(updatePasswordDto.Password);
            _carRacingTournamentDbContext.Users.Update(userObj);
            _carRacingTournamentDbContext.SaveChanges();
            return (true, null);
        }
    }
}
