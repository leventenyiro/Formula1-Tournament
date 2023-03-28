using car_racing_tournament_api.Models;
using Microsoft.EntityFrameworkCore;

namespace car_racing_tournament_api.Data
{
    public class CarRacingTournamentDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; } = default!;
        public virtual DbSet<Season> Seasons { get; set; } = default!;
        public virtual DbSet<Team> Teams { get; set; } = default!;
        public virtual DbSet<Driver> Drivers { get; set; } = default!;
        public virtual DbSet<Race> Races { get; set; } = default!;
        public virtual DbSet<Result> Results { get; set; } = default!;
        public virtual DbSet<Permission> Permissions { get; set; } = default!;

        public CarRacingTournamentDbContext() { }

        public CarRacingTournamentDbContext(DbContextOptions<CarRacingTournamentDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.Username)
                    .IsUnique();

                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.Property(e => e.Password)
                    .IsRequired();
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Type)
                    .IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Permissions)
                    .HasForeignKey(e => e.UserId)
                    .IsRequired();

                entity.HasOne(e => e.Season)
                    .WithMany(e => e.Permissions)
                    .HasForeignKey(e => e.SeasonId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<Season>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.Description);

                entity.Property(e => e.IsArchived)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.Color)
                    .IsRequired();

                entity.HasOne(e => e.Season)
                    .WithMany(e => e.Teams)
                    .HasForeignKey(e => e.SeasonId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<Driver>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.RealName);

                entity.Property(e => e.Number)
                    .IsRequired();

                entity.HasOne(e => e.ActualTeam)
                    .WithMany(e => e.Drivers)
                    .HasForeignKey(e => e.ActualTeamId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Season)
                    .WithMany(e => e.Drivers)
                    .HasForeignKey(e => e.SeasonId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<Race>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.DateTime);

                entity.HasOne(e => e.Season)
                    .WithMany(e => e.Races)
                    .HasForeignKey(e => e.SeasonId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<Result>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Type)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(e => e.Position);

                entity.Property(e => e.Point)
                    .IsRequired();

                entity.HasOne(e => e.Driver)
                    .WithMany(e => e.Results)
                    .HasForeignKey(e => e.DriverId)
                    .IsRequired();

                entity.HasOne(e => e.Team)
                    .WithMany(e => e.Results)
                    .HasForeignKey(e => e.TeamId)
                    .IsRequired();

                entity.HasOne(e => e.Race)
                    .WithMany(e => e.Results)
                    .HasForeignKey(e => e.RaceId)
                    .IsRequired();
            });

            // Database seed
            modelBuilder.Entity<Season>().HasData(
                new Season {
                    Id = new Guid("bd8cc085-2e18-4a7d-84e1-be5de33a52de"),
                    CreatedAt = DateTime.Now,
                    Name = "F1 League",
                    Description = "This is a test season",
                    IsArchived = false,
                }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = new Guid("08db26a9-840c-42ee-82c5-ceec14c2a104"),
                    Username = "leventenyiro",
                    Email = "nyiro.levente@gmail.com",
                    Password = "$2a$10$HcK9moclQc2FZ7mu9lPsJumxY1rKrkD1hGqrGXSRwKWuGpwAtAOgC"
                },
                new User
                {
                    Id = new Guid("08db26a9-9264-4fb6-88aa-4c547e6326dc"),
                    Username = "test1",
                    Email = "test1@gmail.com",
                    Password = "$2a$10$6twuIy5Y5IGmi6D8loXutu/d4MixZLvT2DJ7n2SLcVGczbIJokH6O"
                }
            );

            modelBuilder.Entity<Permission>().HasData(
                new Permission
                {
                    Id = new Guid("7779214e-8f1b-4181-8ab7-c1fad97f4765"),
                    UserId = new Guid("08db26a9-840c-42ee-82c5-ceec14c2a104"),
                    SeasonId = new Guid("bd8cc085-2e18-4a7d-84e1-be5de33a52de"),
                    Type = PermissionType.Admin
                },
                new Permission
                {
                    Id = new Guid("d1ae948b-4b54-47db-9028-07fe9084b7ff"),
                    UserId = new Guid("08db26a9-9264-4fb6-88aa-4c547e6326dc"),
                    SeasonId = new Guid("bd8cc085-2e18-4a7d-84e1-be5de33a52de"),
                    Type = PermissionType.Moderator
                }
            );

            modelBuilder.Entity<Race>().HasData(
                new Race
                {
                    Id = new Guid("085075c1-afb1-425f-babc-7a12a0bcfb3f"),
                    Name = "Bahrein",
                    DateTime = DateTime.Now.AddDays(-8),
                    SeasonId = new Guid("bd8cc085-2e18-4a7d-84e1-be5de33a52de")
                },
                new Race
                {
                    Id = new Guid("cd04e3bd-f0bb-4376-878b-7cd07b53f342"),
                    Name = "Australia",
                    DateTime = DateTime.Now.AddDays(-1),
                    SeasonId = new Guid("bd8cc085-2e18-4a7d-84e1-be5de33a52de")
                }
            );

            modelBuilder.Entity<Team>().HasData(
                new Team
                {
                    Id = new Guid("960a1b8c-2504-4e0e-99c4-cf978c0b856c"),
                    Name = "Mercedes",
                    Color = "#000000",
                    SeasonId = new Guid("bd8cc085-2e18-4a7d-84e1-be5de33a52de")
                },
                new Team
                {
                    Id = new Guid("bb0f78a2-c96e-4995-b84c-a68c9c5105dc"),
                    Name = "Ferrari",
                    Color = "#FF0000",
                    SeasonId = new Guid("bd8cc085-2e18-4a7d-84e1-be5de33a52de")
                }
            );

            modelBuilder.Entity<Driver>().HasData(
                new Driver
                {
                    Id = new Guid("02a46c7f-e8db-4bee-9594-75a6bb4311e7"),
                    Name = "Leclerc",
                    RealName = "Charles Leclerc",
                    Number = 16,
                    ActualTeamId = new Guid("bb0f78a2-c96e-4995-b84c-a68c9c5105dc"),
                    SeasonId = new Guid("bd8cc085-2e18-4a7d-84e1-be5de33a52de")
                },
                new Driver
                {
                    Id = new Guid("8c3d9760-861a-4697-bb35-64c299acdae6"),
                    Name = "Sainz",
                    RealName = "Carlos Sainz",
                    Number = 55,
                    ActualTeamId = new Guid("bb0f78a2-c96e-4995-b84c-a68c9c5105dc"),
                    SeasonId = new Guid("bd8cc085-2e18-4a7d-84e1-be5de33a52de")
                },
                new Driver
                {
                    Id = new Guid("ab201ce7-573c-450b-8514-b4eebee83655"),
                    Name = "Hamilton",
                    RealName = "Lewis Hamilton",
                    Number = 44,
                    ActualTeamId = new Guid("960a1b8c-2504-4e0e-99c4-cf978c0b856c"),
                    SeasonId = new Guid("bd8cc085-2e18-4a7d-84e1-be5de33a52de")
                }
            );

            modelBuilder.Entity<Result>().HasData(
                // BAH
                new Result
                {
                    Id = Guid.NewGuid(),
                    Type = ResultType.Finished,
                    Position = 2,
                    Point = 18,
                    DriverId = new Guid("ab201ce7-573c-450b-8514-b4eebee83655"),
                    TeamId = new Guid("960a1b8c-2504-4e0e-99c4-cf978c0b856c"),
                    RaceId = new Guid("085075c1-afb1-425f-babc-7a12a0bcfb3f")
                },
                new Result
                {
                    Id = Guid.NewGuid(),
                    Type = ResultType.DSQ,
                    Position = null,
                    Point = 0,
                    DriverId = new Guid("8c3d9760-861a-4697-bb35-64c299acdae6"),
                    TeamId = new Guid("bb0f78a2-c96e-4995-b84c-a68c9c5105dc"),
                    RaceId = new Guid("085075c1-afb1-425f-babc-7a12a0bcfb3f")
                },
                new Result
                {
                    Id = Guid.NewGuid(),
                    Type = ResultType.Finished,
                    Position = 1,
                    Point = 25,
                    DriverId = new Guid("02a46c7f-e8db-4bee-9594-75a6bb4311e7"),
                    TeamId = new Guid("bb0f78a2-c96e-4995-b84c-a68c9c5105dc"),
                    RaceId = new Guid("085075c1-afb1-425f-babc-7a12a0bcfb3f")
                },
                // AUS
                new Result
                {
                    Id = Guid.NewGuid(),
                    Type = ResultType.Finished,
                    Position = 1,
                    Point = 25,
                    DriverId = new Guid("ab201ce7-573c-450b-8514-b4eebee83655"),
                    TeamId = new Guid("960a1b8c-2504-4e0e-99c4-cf978c0b856c"),
                    RaceId = new Guid("cd04e3bd-f0bb-4376-878b-7cd07b53f342")
                },
                new Result
                {
                    Id = Guid.NewGuid(),
                    Type = ResultType.Finished,
                    Position = 2,
                    Point = 18,
                    DriverId = new Guid("8c3d9760-861a-4697-bb35-64c299acdae6"),
                    TeamId = new Guid("bb0f78a2-c96e-4995-b84c-a68c9c5105dc"),
                    RaceId = new Guid("cd04e3bd-f0bb-4376-878b-7cd07b53f342")
                },
                new Result
                {
                    Id = Guid.NewGuid(),
                    Type = ResultType.DNF,
                    Position = null,
                    Point = 0,
                    DriverId = new Guid("02a46c7f-e8db-4bee-9594-75a6bb4311e7"),
                    TeamId = new Guid("960a1b8c-2504-4e0e-99c4-cf978c0b856c"),
                    RaceId = new Guid("cd04e3bd-f0bb-4376-878b-7cd07b53f342")
                }
            );
        }
    }
}