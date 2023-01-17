using api.Controllers;
using api.Data;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace api.Tests
{
    [TestFixture]
    public class SeasonTests
    {
    /*    private SeasonController _seasonController;
        private SeasonService _seasonService;
        private Mock<FormulaDbContext> _mockDbContext;

        private Mock<DbSet<Season>> AddMockSet(List<Season> seasons)
        {
            var mockSet = new Mock<DbSet<Season>>();
            var data = seasons.AsQueryable();

            mockSet.As<IQueryable<Season>>().Setup(x => x.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Season>>().Setup(x => x.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Season>>().Setup(x => x.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Season>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());

            return mockSet;
        }

        [SetUp]
        public void SetUp()
        {
            _mockDbContext = new Mock<FormulaDbContext>();
            _seasonService = new SeasonService(_mockDbContext.Object);
            _seasonController = new SeasonController(_seasonService);
        }

        [Test]
        public void GetAllSeason_ShouldBeEmpty()
        {
            _mockDbContext.Setup(x => x.Seasons).Returns(AddMockSet(new List<Season>()).Object);

            var response = _seasonController.Get();
            Assert.IsTrue(response.IsCompleted);
            var result = (OkObjectResult)response.Result;
            var actual = result.Value as IEnumerable<Season>;

            Assert.AreEqual(result.StatusCode, 200);
            Assert.IsEmpty(actual);
        }

        [Test]
        public void GetAllSeason_ShouldContainElement()
        {
            var season = new Season { Id = new Guid(), Name = "test", UserId = new Guid() };
            _mockDbContext.Setup(x => x.Seasons).Returns(AddMockSet(new List<Season> { season }).Object);

            var response = _seasonController.Get();
            var result = (OkObjectResult)response.Result;
            var actual = result.Value as IEnumerable<Season>;

            Assert.AreEqual(actual.Count(), 1);
            Assert.AreEqual(actual.ToList()[0], season);
        }

        [Test]
        public void PostSeason_ShouldBeOk()
        {
            _mockDbContext.Setup(x => x.Seasons).Returns(AddMockSet(new List<Season>()).Object);

            var season = new Season { Name = "string", UserId = new Guid() };
            var response = _seasonController.Post(season);
            Assert.IsTrue(response.IsCompleted);
            var result = (StatusCodeResult)response.Result;

            Assert.AreEqual(result.StatusCode, 201);
        }

        [Test]
        public void PutSeason_ShouldBeOk()
        {
            var id = new Guid();
            var season = new Season { Id = id, Name = "test", UserId = new Guid() };
            _mockDbContext.Setup(x => x.Seasons).Returns(AddMockSet(new List<Season> { season }).Object);

            season = new Season { Name = "string", UserId = new Guid() };
            var response = _seasonController.Put(id, season);
            Assert.IsTrue(response.IsCompleted);
            var result = (StatusCodeResult)response.Result;

            Assert.AreEqual(result.StatusCode, 204);
        }

        [Test]
        public void GetSeason_ShouldBeNull()
        {
            _mockDbContext.Setup(x => x.Seasons).Returns(AddMockSet(new List<Season>()).Object);

            var response = _seasonController.Get(new Guid());
            Assert.IsTrue(response.IsCompleted);
            var result = (NotFoundObjectResult)response.Result;
            var actual = result.Value as Season;

            Assert.AreEqual(result.StatusCode, 404);
            Assert.IsNull(actual);
        }

        [Test]
        public void GetSeason_ShouldBeElement()
        {
            var id = new Guid();
            var season = new Season { Id = id, Name = "test", UserId = new Guid() };
            _mockDbContext.Setup(x => x.Seasons).Returns(AddMockSet(new List<Season> { season }).Object);

            var response = _seasonController.Get(id);
            var result = (OkObjectResult)response.Result;
            var actual = result.Value as Season;

            Assert.IsNotNull(actual);
            Assert.AreEqual(actual, season);
        }

        [Test]
        public void DeleteSeason_ShouldBeOk()
        {
            var id = new Guid();
            var season = new Season { Id = id, Name = "test", UserId = new Guid() };
            _mockDbContext.Setup(x => x.Seasons).Returns(AddMockSet(new List<Season> { season }).Object);

            var response = _seasonController.Delete(id);
            Assert.IsTrue(response.IsCompleted);
            var result = (StatusCodeResult)response.Result;

            Assert.AreEqual(result.StatusCode, 204);
        }*/
    }
}
