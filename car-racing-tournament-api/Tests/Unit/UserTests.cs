using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;
using car_racing_tournament_api.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Data.Entity.Infrastructure;

namespace car_racing_tournament_api.Tests.Unit
{
    [TestFixture]
    public class UserTests
    {
        [Test]
        public async Task Login()
        {
            var data = new List<User>
            {
                new User
                {
                    Username = "username",
                    Password = "password"
                }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IDbAsyncEnumerable<User>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<User>(data.GetEnumerator()));


            // ANDRIS
            //Context = new PriceBookDbContext(new DbContextOptionsBuilder<PriceBookDbContext>().UseInMemoryDatabase("InMemoryDb").Options);

            mockSet.As<IQueryable<User>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<User>(data.Provider));

            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<CarRacingTournamentDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var userService = new UserService(mockContext.Object, configuration);
            //var blogs = await service.GetAllBlogsAsync();
            var loginDto = new LoginDto { UsernameEmail = "username", Password = "password" };
            var result = await userService.Login(loginDto);
            Assert.IsTrue(result.IsSuccess);
        }
    }
}
