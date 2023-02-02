using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;
using car_racing_tournament_api.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace car_racing_tournament_api.Tests.Unit
{
    [TestFixture]
    public class UserTests
    {
        [SetUp]
        public void Init()
        {

        }

        [Test]
        public async Task Login()
        {
            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new CarRacingTournamentDbContext(options))
            {
                context.Users.Add(new User { Username = "username", Email = "test@test.com", Password = "$2a$10$/Mw2QNUGYbV1AIyQ8QxXC.IhNRrmjwAW9SBgUv8Vh9xX2goWsQwG." });
                context.SaveChanges();

                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

                var userService = new UserService(context, configuration);
                var result = await userService.Login(new LoginDto { UsernameEmail = "username", Password = "Password1" });
                Assert.IsTrue(result.IsSuccess);
            }
        }
    }
}
