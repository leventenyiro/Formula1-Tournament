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

        public (bool IsSuccess, string? Token, string? ErrorMessage) Login(User user, string password, bool needToken)
        {
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                return (false, null, _configuration["ErrorMessages:LoginDetails"]);

            if (!needToken)
                return (true, null, _configuration["SuccessMessages:Login"]);
                
            return (true, CreateToken(user), _configuration["SuccessMessages:Login"]);
            
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> Registration(RegistrationDto registrationDto)
        {
            registrationDto.Username = registrationDto.Username.Trim();
            if (!Regex.IsMatch(registrationDto.Username, _configuration["Validation:NameRegexWithoutWhiteSpace"]))
                return (false, _configuration["ErrorMessages:UserName"]);

            registrationDto.Email = registrationDto.Email.Trim().ToLower();
            if (!Regex.IsMatch(registrationDto.Email, _configuration["Validation:EmailRegex"]))
                return (false, _configuration["ErrorMessages:EmailFormat"]);

            if (!Regex.IsMatch(registrationDto.Password, _configuration["Validation:PasswordRegex"]))
                return (false, _configuration["ErrorMessages:PasswordFormat"]);

            if (registrationDto.Password != registrationDto.PasswordAgain)
                return (false, _configuration["ErrorMessages:PasswordsPass"]);

            if (await _carRacingTournamentDbContext.Users.CountAsync(x => x.Username == registrationDto.Username) != 0)
                return (false, _configuration["ErrorMessages:UserNameExists"]);

            if (await _carRacingTournamentDbContext.Users.CountAsync(x => x.Email == registrationDto.Email) != 0)
                return (false, _configuration["ErrorMessages:EmailExists"]);

            await _carRacingTournamentDbContext.Users.AddAsync(new User { 
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
            var result = await _carRacingTournamentDbContext.Users
            .Where(x => x.Id == id)
            .Select(x => new User {
                Id = x.Id,
                Username = x.Username,
                Email = x.Email
            }).FirstOrDefaultAsync();
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
            updateUserDto.Username = updateUserDto.Username.Trim();
            if (!Regex.IsMatch(updateUserDto.Username, _configuration["Validation:NameRegexWithoutWhiteSpace"]))
                return (false, _configuration["ErrorMessages:UserName"]);

            updateUserDto.Email = updateUserDto.Email.Trim().ToLower();
            if (!Regex.IsMatch(updateUserDto.Email, _configuration["Validation:EmailRegex"]))
                return (false, _configuration["ErrorMessages:EmailFormat"]);

            if (user.Username != updateUserDto.Username && 
                await _carRacingTournamentDbContext.Users.Where(x => x.Username == updateUserDto.Username).CountAsync() != 0)
                return (false, _configuration["ErrorMessages:UserNameExists"]);

            if (user.Email != updateUserDto.Email && 
                await _carRacingTournamentDbContext.Users.Where(x => x.Email == updateUserDto.Email).CountAsync() != 0)
                return (false, _configuration["ErrorMessages:EmailExists"]);

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

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteUser(User user, string password)
        {
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                return (false, _configuration["ErrorMessages:LoginDetails"]);

            _carRacingTournamentDbContext.Users.Remove(user);
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
