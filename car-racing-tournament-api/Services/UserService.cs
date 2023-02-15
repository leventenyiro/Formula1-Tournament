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

            if (!Regex.IsMatch(registrationDto.Email, _configuration["Validation:EmailRegex"]))
                return (false, _configuration["ErrorMessages:EmailFormat"]);

            if (!Regex.IsMatch(registrationDto.Password, _configuration["Validation:PasswordRegex"]))
                return (false, _configuration["ErrorMessages:PasswordFormat"]);

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

        public async Task<(bool IsSuccess, User? User, string? ErrorMessage)> GetUserById(Guid id)
        {
            var result = await _carRacingTournamentDbContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
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

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateUser(User user, UpdateUserDto updateUserDto)
        {
            if (updateUserDto.Username.Length < int.Parse(_configuration["Validation:UserNameMinLength"]))
                return (false, String.Format(
                    _configuration["ErrorMessages:UserName"],
                    _configuration["Validation:UserNameMinLength"]
                ));

            if (!Regex.IsMatch(updateUserDto.Email, _configuration["Validation:EmailRegex"]))
                return (false, _configuration["ErrorMessages:EmailFormat"]);

            user.Username = updateUserDto.Username;
            user.Email = updateUserDto.Email;
            _carRacingTournamentDbContext.Users.Update(user);
            await _carRacingTournamentDbContext.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdatePassword(User user, UpdatePasswordDto updatePasswordDto)
        {
            if (updatePasswordDto.Password != updatePasswordDto.PasswordAgain)
                return (false, _configuration["ErrorMessages:PasswordsPass"]);

            if (!Regex.IsMatch(updatePasswordDto.Password, _configuration["Validation:PasswordRegex"]))
                return (false, _configuration["ErrorMessages:PasswordFormat"]);

            user.Password = HashPassword(updatePasswordDto.Password);
            _carRacingTournamentDbContext.Users.Update(user);
            await _carRacingTournamentDbContext.SaveChangesAsync();

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
    }
}
