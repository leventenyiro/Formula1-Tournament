﻿using car_racing_tournament_api.Data;
using car_racing_tournament_api.Models;
using car_racing_tournament_api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace car_racing_tournament_api.Tests.Unit
{
    public class PermissionTests
    {
        private CarRacingTournamentDbContext? _context;
        private PermissionService? _permissionService;
        private Models.User? _user1;
        private Models.User? _user2;
        private Season? _season1;
        private Season? _season2;
        private Permission? _permissionAdmin;
        private Permission? _permissionModerator;
        private IConfiguration? _configuration;

        [SetUp]
        public void Init()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseSqlite(connection)
                .Options;

            _context = new CarRacingTournamentDbContext(options);
            _context.Database.EnsureCreatedAsync();

            _season1 = new Season
            {
                Id = Guid.NewGuid(),
                Name = "Test Season",
                IsArchived = false
            };
            _context.Seasons.AddAsync(_season1);

            _user1 = new Models.User
            {
                Id = Guid.NewGuid(),
                Username = "FirstUser",
                Email = "first@user.com",
                Password = "test"
            };
            _context.Users.AddAsync(_user1);

            _season2 = new Season
            {
                Id = Guid.NewGuid(),
                Name = "Test Season 2",
                IsArchived = false
            };
            _context.Seasons.AddAsync(_season2);

            _user2 = new Models.User
            {
                Id = Guid.NewGuid(),
                Username = "SecondUser",
                Email = "second@user.com",
                Password = "test"
            };
            _context.Users.AddAsync(_user2);

            _permissionAdmin = new Permission
            {
                Id = Guid.NewGuid(),
                Season = _season1,
                User = _user1,
                Type = PermissionType.Admin
            };
            _context.Permissions.AddAsync(_permissionAdmin);

            _permissionModerator = new Permission
            {
                Id = Guid.NewGuid(),
                Season = _season1,
                User = _user2,
                Type = PermissionType.Moderator
            };
            _context.Permissions.AddAsync(_permissionModerator);

            _context.SaveChanges();

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _permissionService = new PermissionService(_context, _configuration);
        }

        // permission not found - need to be implemented

        [Test]
        public async Task IsAdminSuccess()
        {
            var result = await _permissionService!.IsAdmin(_user1!.Id, _season1!.Id);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task IsAdminFailed()
        {
            var result = await _permissionService!.IsAdmin(_user2!.Id, _season1!.Id);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task IsAdminModeratorSuccess()
        {
            var result = await _permissionService!.IsAdminModerator(_user1!.Id, _season1!.Id);
            Assert.IsTrue(result);

            result = await _permissionService!.IsAdminModerator(_user2!.Id, _season1!.Id);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task IsAdminModeratorFailed()
        {
            var result = await _permissionService!.IsAdmin(_user1!.Id, _season2!.Id);
            Assert.IsFalse(result);

            result = await _permissionService!.IsAdmin(_user2!.Id, _season2!.Id);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task AddAdminSuccess()
        {
            var result = await _permissionService!.AddPermission(_user1!, _season2!, PermissionType.Admin);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(_context!.Permissions.CountAsync().Result, 3);

            result = await _permissionService!.AddPermission(_user2!, _season2!, PermissionType.Moderator);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(_context!.Permissions.CountAsync().Result, 4);
        }

        // PERMISSION ALREADZ EXISTS

        [Test]
        public async Task UpdatePermissionTypeSuccess()
        {
            var season = new Season
            {
                Id = Guid.NewGuid(),
                Name = "New Season",
                IsArchived = false
            };
            await _context!.Seasons.AddAsync(season);

            var permission = new Permission
            {
                Id = Guid.NewGuid(),
                User = _user1!,
                Season = season,
                Type = PermissionType.Moderator
            };
            await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync();

            var result = await _permissionService!.UpdatePermissionType(permission, PermissionType.Admin);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(season.Permissions.Count(), 1);
            Assert.AreEqual(permission.Type, PermissionType.Admin);
        }

        [Test]
        public async Task UpdatePermissionTypeAdminExists()
        {
            var season = new Season
            {
                Id = Guid.NewGuid(),
                Name = "New Season",
                IsArchived = false,
                Permissions = new List<Permission>()
                {
                    new Permission
                    {
                        Id = Guid.NewGuid(),
                        UserId = Guid.NewGuid(),
                        SeasonId = Guid.NewGuid(),
                        Type = PermissionType.Admin
                    }
                }
            };
            await _context!.Seasons.AddAsync(season);

            var permission = new Permission
            {
                Id = Guid.NewGuid(),
                User = _user1!,
                Season = season,
                Type = PermissionType.Moderator
            };
            await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync();

            var result = await _permissionService!.UpdatePermissionType(permission, PermissionType.Admin);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:SeasonHasAdmin"]);
            Assert.AreEqual(season.Permissions.Count(), 2);
            Assert.AreEqual(permission.Type, PermissionType.Moderator);
        }
    }
}
