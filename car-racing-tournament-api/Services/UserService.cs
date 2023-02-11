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

        public UserService(CarRacingTournamentDbContext carRacingTournamentDbContext, IConfiguration configuration)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
            _configuration = configuration;
        }

        public async Task<(bool IsSuccess, string? Token, string? ErrorMessage)> Login(LoginDto loginDto)
        {
            var actualUser = await _carRacingTournamentDbContext.Users.Where(x => x.Username == loginDto.UsernameEmail || x.Email == loginDto.UsernameEmail).FirstOrDefaultAsync();
            if (actualUser == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, actualUser.Password))
                return (false, null, _configuration["ErrorMessages:LoginDetails"]);

            string token = CreateToken(actualUser);

            return (true, token, _configuration["SuccessMessages:Login"]);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> Registration(RegistrationDto registrationDto)
        {
            if (registrationDto.Username.Length < int.Parse(_configuration["Validation:UserNameMinLength"]))
                return (false, String.Format(
                    _configuration["ErrorMessages:UserName"],
                    _configuration["Validation:UserNameMinLength"]
                ));

            if (!IsEmail(registrationDto.Email))
                return (false, _configuration["ErrorMessages:EmailFormat"]);

            if (!IsPassword(registrationDto.Password))
                return (false, _configuration["ErrorMessages:PasswordLength"]);

            if (registrationDto.Password != registrationDto.PasswordAgain)
                return (false, _configuration["ErrorMessages:PasswordsPass"]);

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
                return (false, null, _configuration["ErrorMessages:UserNotFound"]);
            
            return (true, result, null);
        }

        public async Task<(bool IsSuccess, User? User, string? ErrorMessage)> GetUserByUsernameEmail(string usernameEmail)
        {
            var actualUser = await _carRacingTournamentDbContext.Users.Where(x => x.Username == usernameEmail || x.Email == usernameEmail).FirstOrDefaultAsync();
            if (actualUser == null)
                return (false, null, _configuration["ErrorMessages:UserNotFound"]);

            return (true, actualUser, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateUser(Guid id, UpdateUserDto updateUserDto)
        {
            if (updateUserDto.Username.Length < int.Parse(_configuration["Validation:UserNameMinLength"]))
                return (false, String.Format(
                    _configuration["ErrorMessages:UserName"],
                    _configuration["Validation:UserNameMinLength"]
                ));

            if (!IsEmail(updateUserDto.Email))
                return (false, _configuration["ErrorMessages:EmailFormat"]);

            var userObj = await _carRacingTournamentDbContext.Users.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (userObj == null)
                return (false, _configuration["ErrorMessages:UserNotFound"]);

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
                return (false, _configuration["ErrorMessages:UserNotFound"]);

            if (updatePasswordDto.Password != updatePasswordDto.PasswordAgain)
                return (false, _configuration["ErrorMessages:PasswordsPass"]);

            // password validation?

            userObj.Password = HashPassword(updatePasswordDto.Password);
            _carRacingTournamentDbContext.Users.Update(userObj);
            _carRacingTournamentDbContext.SaveChanges();
            return (true, null);
        }

        private string HashPassword(string password)
        {
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

        private bool IsPassword(string password)
        {
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$");
        }
    }
}
