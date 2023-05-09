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
        public virtual DbSet<Favorite> Favorites { get; set; } = default!;
        private IConfiguration _configuration;

        public CarRacingTournamentDbContext() {
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }

        public CarRacingTournamentDbContext(DbContextOptions<CarRacingTournamentDbContext> options) : base(options) {
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (bool.Parse(_configuration["Development"]))
                modelBuilder.UseCollation("utf8_bin");

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
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                entity.HasOne(e => e.Season)
                    .WithMany(e => e.Permissions)
                    .HasForeignKey(e => e.SeasonId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Favorites)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                entity.HasOne(e => e.Season)
                    .WithMany(e => e.Favorites)
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
                    .OnDelete(DeleteBehavior.NoAction);

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
                    .OnDelete(DeleteBehavior.NoAction)
                    .IsRequired();

                entity.HasOne(e => e.Race)
                    .WithMany(e => e.Results)
                    .HasForeignKey(e => e.RaceId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .IsRequired();
            });

            // Database seed
            if (bool.Parse(_configuration["DatabaseSeed"]))
            {
                modelBuilder.Entity<Season>().HasData(
                    new Season {
                        Id = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1"),
                        CreatedAt = DateTime.Parse("2021-03-28T14:00:00"),
                        Name = "Formula 1 2021",
                        Description = "This is the results of 2021 Formula 1 season",
                        IsArchived = true,
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
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1"),
                        Type = PermissionType.Admin
                    },
                    new Permission
                    {
                        Id = new Guid("d1ae948b-4b54-47db-9028-07fe9084b7ff"),
                        UserId = new Guid("08db26a9-9264-4fb6-88aa-4c547e6326dc"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1"),
                        Type = PermissionType.Moderator
                    }
                );

                modelBuilder.Entity<Favorite>().HasData(
                    new Favorite
                    {
                        Id = Guid.NewGuid(),
                        UserId = new Guid("08db26a9-840c-42ee-82c5-ceec14c2a104"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    }
                );

                modelBuilder.Entity<Race>().HasData(
                    new Race
                    {
                        Id = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd"),
                        Name = "Bahrein",
                        DateTime = DateTime.Parse("2021-03-28T14:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d"),
                        Name = "Emilia Romagna",
                        DateTime = DateTime.Parse("2021-04-18T12:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba"),
                        Name = "Portugál",
                        DateTime = DateTime.Parse("2021-05-02T14:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102"),
                        Name = "Spanyol",
                        DateTime = DateTime.Parse("2021-05-09T13:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("7d411456-e69c-4612-b372-a178d152f86d"),
                        Name = "Monaco",
                        DateTime = DateTime.Parse("2021-05-23T13:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271"),
                        Name = "Azerbajdzsán",
                        DateTime = DateTime.Parse("2021-06-06T12:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("09d4acad-8089-41b1-af8e-48565540acbc"),
                        Name = "Francia",
                        DateTime = DateTime.Parse("2021-06-20T13:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a"),
                        Name = "Stájer",
                        DateTime = DateTime.Parse("2021-06-27T13:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f"),
                        Name = "Ausztria",
                        DateTime = DateTime.Parse("2021-07-04T13:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13"),
                        Name = "Brit sprint",
                        DateTime = DateTime.Parse("2021-07-17T13:30:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94"),
                        Name = "Brit",
                        DateTime = DateTime.Parse("2021-07-18T14:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("72ce4177-715b-445a-af83-fd303ab00f41"),
                        Name = "Magyar",
                        DateTime = DateTime.Parse("2021-08-01T13:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021"),
                        Name = "Belgium",
                        DateTime = DateTime.Parse("2021-08-29T13:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e"),
                        Name = "Hollandia",
                        DateTime = DateTime.Parse("2021-09-05T13:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db"),
                        Name = "Olasz sprint",
                        DateTime = DateTime.Parse("2021-09-11T13:30:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed"),
                        Name = "Olasz",
                        DateTime = DateTime.Parse("2021-09-12T13:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156"),
                        Name = "Orosz",
                        DateTime = DateTime.Parse("2021-09-26T13:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080"),
                        Name = "Török",
                        DateTime = DateTime.Parse("2021-10-10T12:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9"),
                        Name = "USA",
                        DateTime = DateTime.Parse("2021-10-24T19:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528"),
                        Name = "Mexikó",
                        DateTime = DateTime.Parse("2021-11-07T19:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b"),
                        Name = "Brazil sprint",
                        DateTime = DateTime.Parse("2021-11-13T19:30:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4"),
                        Name = "Brazil",
                        DateTime = DateTime.Parse("2021-11-14T17:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f"),
                        Name = "Katar",
                        DateTime = DateTime.Parse("2021-11-21T15:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a"),
                        Name = "Szaúd Arábia",
                        DateTime = DateTime.Parse("2021-12-05T17:30:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Race
                    {
                        Id = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d"),
                        Name = "Abu Dhabi",
                        DateTime = DateTime.Parse("2021-12-12T13:00:00"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    }
                );

                modelBuilder.Entity<Team>().HasData(
                    new Team
                    {
                        Id = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        Name = "Mercedes",
                        Color = "#000000",
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Team
                    {
                        Id = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        Name = "Alpine",
                        Color = "#021BD4",
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Team
                    {
                        Id = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        Name = "Aston Martin",
                        Color = "#28865D",
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Team
                    {
                        Id = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        Name = "Haas",
                        Color = "#FFFFFF",
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Team
                    {
                        Id = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        Name = "Red Bull",
                        Color = "#563D7C",
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Team
                    {
                        Id = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        Name = "Ferrari",
                        Color = "#B20101",
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Team
                    {
                        Id = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        Name = "Williams",
                        Color = "#5C67FF",
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Team
                    {
                        Id = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        Name = "AlphaTauri",
                        Color = "#2E0E5D",
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Team
                    {
                        Id = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        Name = "Mclaren",
                        Color = "#D48908",
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Team
                    {
                        Id = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        Name = "Alfa Romeo",
                        Color = "#860404",
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    }
                );

                modelBuilder.Entity<Driver>().HasData(
                    new Driver
                    {
                        Id = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        Name = "Mazepin",
                        RealName = "Nikita Mazepin",
                        Number = 9,
                        ActualTeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        Name = "Giovinazzi",
                        RealName = "Antonio Giovinazzi",
                        Number = 99,
                        ActualTeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        Name = "Raikkonen",
                        RealName = "Kimi Raikkonen",
                        Number = 7,
                        ActualTeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        Name = "Hamilton",
                        RealName = "Lewis Hamilton",
                        Number = 44,
                        ActualTeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        Name = "Tsunoda",
                        RealName = "Yuki Tsunoda",
                        Number = 22,
                        ActualTeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        Name = "Alonso",
                        RealName = "Fernando Alonso",
                        Number = 14,
                        ActualTeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        Name = "Vettel",
                        RealName = "Sebastian Vettel",
                        Number = 5,
                        ActualTeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        Name = "Ocon",
                        RealName = "Esteban Ocon",
                        Number = 31,
                        ActualTeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        Name = "Russell",
                        RealName = "George Russell",
                        Number = 63,
                        ActualTeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        Name = "Sainz",
                        RealName = "Carlos Sainz",
                        Number = 55,
                        ActualTeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        Name = "Latifi",
                        RealName = "Nicholas Latifi",
                        Number = 6,
                        ActualTeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        Name = "Schumacher",
                        RealName = "Mick Schumacher",
                        Number = 47,
                        ActualTeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        Name = "Leclerc",
                        RealName = "Charles Leclerc",
                        Number = 16,
                        ActualTeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        Name = "Ricciardo",
                        RealName = "Daniel Ricciardo",
                        Number = 3,
                        ActualTeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        Name = "Bottas",
                        RealName = "Valtteri Bottas",
                        Number = 77,
                        ActualTeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        Name = "Verstappen",
                        RealName = "Max Verstappen",
                        Number = 33,
                        ActualTeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        Name = "Norris",
                        RealName = "Lando Norris",
                        Number = 4,
                        ActualTeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        Name = "Gasly",
                        RealName = "Pierre Gasly",
                        Number = 10,
                        ActualTeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        Name = "Pérez",
                        RealName = "Sergio Pérez",
                        Number = 11,
                        ActualTeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        Name = "Stroll",
                        RealName = "Lance Stroll",
                        Number = 18,
                        ActualTeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("8ea53266-9f3b-4d74-99c4-ee1e4c68596e"),
                        Name = "Kubica",
                        RealName = "Robert Kubica",
                        Number = 88,
                        ActualTeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    }
                );

                modelBuilder.Entity<Result>().HasData(
                    new Result
                    {
                        Id = new Guid("653fcfc4-73de-43c8-ae2f-26c953840f10"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd")
                    },
                    new Result
                    {
                        Id = new Guid("7c5adb5c-0e14-415e-ab83-3132ce1c04ab"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd")
                    },
                    new Result
                    {
                        Id = new Guid("0af3fd35-7f88-41cb-b925-362aa70fe166"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd")
                    },
                    new Result
                    {
                        Id = new Guid("044710c8-7f65-4a8e-abf5-36aa5c0ab64a"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd")
                    },
                    new Result
                    {
                        Id = new Guid("f3b0014d-f5f8-4d60-b178-4307f5f04812"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd")
                    },
                    new Result
                    {
                        Id = new Guid("f00dc026-df57-43a4-8c1a-5bdc84d1083b"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd")
                    },
                    new Result
                    {
                        Id = new Guid("0f922386-1c97-4eee-acf3-78cc061fa86f"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd")
                    },
                    new Result
                    {
                        Id = new Guid("a01c22ab-e9d0-4b3f-91d5-7a6558eb425b"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 16,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd")
                    },
                    new Result
                    {
                        Id = new Guid("54315da2-701e-43e7-862f-805e5e8e7af2"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd")
                    },
                    new Result
                    {
                        Id = new Guid("01fb1a30-e09f-429a-82db-94cc9f7db77f"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd")
                    },
                    new Result
                    {
                        Id = new Guid("7703fb6b-cce7-4651-941a-a112b2e50062"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd")
                    },
                    new Result
                    {
                        Id = new Guid("b1e679da-fa3c-4ca9-a64a-a2ce41340166"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd")
                    },
                    new Result
                    {
                        Id = new Guid("051e3ce8-e1ee-4efd-aa1a-af9e14142b8b"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd")
                    },
                    new Result
                    {
                        Id = new Guid("0e79541e-9f81-4f95-ba11-b5441ab84760"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd")
                    },
                    new Result
                    {
                        Id = new Guid("0780dc5b-31b6-4b95-b7ee-c0935d4d9694"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd")
                    },
                    new Result
                    {
                        Id = new Guid("0944b941-1f2a-4dec-beb8-db36b745df24"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd")
                    },
                    new Result
                    {
                        Id = new Guid("9f6cf70e-f391-4238-b14b-def7eef33f51"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd")
                    },
                    new Result
                    {
                        Id = new Guid("978581b5-7f0c-4639-b8cb-e16102773aee"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd")
                    },
                    new Result
                    {
                        Id = new Guid("7aa3d4ca-053d-4b3a-8759-f193de12242f"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd")
                    },
                    new Result
                    {
                        Id = new Guid("9309933c-9b6d-4169-bb45-fe17cc14f6ee"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("a6f714f2-3a37-40a9-a0cf-598866fa02cd")
                    },
                    new Result
                    {
                        Id = new Guid("5bf2201a-39e7-4bf8-a0ab-028dc95ea766"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d")
                    },
                    new Result
                    {
                        Id = new Guid("f8ce30c6-4275-4271-a67b-085717cd264b"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d")
                    },
                    new Result
                    {
                        Id = new Guid("d79b9af0-218f-420a-9e30-15c9b9c9a919"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d")
                    },
                    new Result
                    {
                        Id = new Guid("de4f8176-cd0d-4110-a77b-18804b3e61a1"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d")
                    },
                    new Result
                    {
                        Id = new Guid("f564e613-5657-4ad1-98ce-1b868497a3ce"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d")
                    },
                    new Result
                    {
                        Id = new Guid("24955a59-b57b-416f-8d41-1d6b1ea72572"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d")
                    },
                    new Result
                    {
                        Id = new Guid("f1c3d7c0-63a2-47d3-ae76-4252da5fda67"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d")
                    },
                    new Result
                    {
                        Id = new Guid("f1c3b7b4-2316-432b-a670-47c24f9e25d3"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d")
                    },
                    new Result
                    {
                        Id = new Guid("ee8370b5-cb53-4384-8d4f-4c8bed788220"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 19,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d")
                    },
                    new Result
                    {
                        Id = new Guid("eb92cb82-2813-4c84-bb04-57f79278b59d"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d")
                    },
                    new Result
                    {
                        Id = new Guid("e7454c4c-adb6-4756-a764-597f9cc15e69"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d")
                    },
                    new Result
                    {
                        Id = new Guid("e9258ad8-ef42-4a55-a39b-5ce8e0a5b95d"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d")
                    },
                    new Result
                    {
                        Id = new Guid("0fc5b549-3d00-414c-bf3c-629ddbe1a24d"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d")
                    },
                    new Result
                    {
                        Id = new Guid("386f832d-e847-42f0-85ee-6eafe18e39d0"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d")
                    },
                    new Result
                    {
                        Id = new Guid("6dc110f7-e403-45c0-979c-93f395bfae04"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d")
                    },
                    new Result
                    {
                        Id = new Guid("b1bc6ac8-85d9-47b3-98b6-966e3b276931"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d")
                    },
                    new Result
                    {
                        Id = new Guid("3e845617-7cc5-4154-9870-9e9b3fe8b390"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d")
                    },
                    new Result
                    {
                        Id = new Guid("cc6a6993-7832-4094-b555-a325426c98ce"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d")
                    },
                    new Result
                    {
                        Id = new Guid("0e27d53e-38c1-4903-bcfe-bca979e60f1d"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d")
                    },
                    new Result
                    {
                        Id = new Guid("083fdbe3-efb0-446a-8241-f8ac1e1499d3"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("99d2b84f-af1a-423b-88a1-5387e5d4436d")
                    },
                    new Result
                    {
                        Id = new Guid("8fb5e2a0-3a0c-4ea7-a87d-03e0454b0c87"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba")
                    },
                    new Result
                    {
                        Id = new Guid("48748eb4-9ee9-48e0-b56c-08b5aa463dfd"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba")
                    },
                    new Result
                    {
                        Id = new Guid("3a4429c0-f12f-437d-9112-184e5ba77585"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba")
                    },
                    new Result
                    {
                        Id = new Guid("57a3ff51-3f6e-467f-9655-1a22fe395866"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba")
                    },
                    new Result
                    {
                        Id = new Guid("68e0f7ec-c519-4362-b11a-1b645e0bc9b3"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba")
                    },
                    new Result
                    {
                        Id = new Guid("b8fb858a-62d4-4569-abda-29fc1a5fc157"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba")
                    },
                    new Result
                    {
                        Id = new Guid("7582ddac-c7ed-4c65-922d-304ad0729232"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba")
                    },
                    new Result
                    {
                        Id = new Guid("c3e75cc6-6ff6-46e8-8334-31c0237c6f44"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba")
                    },
                    new Result
                    {
                        Id = new Guid("4b19cd8b-2fe0-44c1-ae86-68e4426322dc"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 16,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba")
                    },
                    new Result
                    {
                        Id = new Guid("f04f357a-72c7-4eb9-be7d-74bb25b01beb"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba")
                    },
                    new Result
                    {
                        Id = new Guid("0654970d-c5f5-40f7-b20c-8a7882f90891"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba")
                    },
                    new Result
                    {
                        Id = new Guid("ee944410-2eae-4e36-bdf9-8f1c4d4991cd"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba")
                    },
                    new Result
                    {
                        Id = new Guid("2037b437-789f-436e-a812-91309e0222a9"),
                        Type = ResultType.Finished,
                        Position = 19,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba")
                    },
                    new Result
                    {
                        Id = new Guid("334c7e41-5229-4ae5-a9d5-9e0316ef27f0"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba")
                    },
                    new Result
                    {
                        Id = new Guid("6a065243-abbc-4b5e-9d78-ba0436d40725"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba")
                    },
                    new Result
                    {
                        Id = new Guid("06e105fc-aaaa-413d-9fb7-cdec0ff66837"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba")
                    },
                    new Result
                    {
                        Id = new Guid("4a5a0b88-73cf-4ac0-ab36-d4bbfab7e6e5"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba")
                    },
                    new Result
                    {
                        Id = new Guid("41e04c8d-8809-4b77-9766-e41cb1c613b2"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba")
                    },
                    new Result
                    {
                        Id = new Guid("c741cd23-b59e-499c-b95b-f31407eb3a70"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba")
                    },
                    new Result
                    {
                        Id = new Guid("56082ea7-5212-4a33-8e8d-fac8ea2f7d62"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("0d28c650-1a99-4a0a-b19f-57ad614c40ba")
                    },
                    new Result
                    {
                        Id = new Guid("a8a0c678-9e49-4972-8a75-30a16e0def17"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102")
                    },
                    new Result
                    {
                        Id = new Guid("6a8d2350-9bba-4785-a3fb-344ea8ac0f17"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102")
                    },
                    new Result
                    {
                        Id = new Guid("305234e5-3284-4165-8264-39c5de8838f4"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102")
                    },
                    new Result
                    {
                        Id = new Guid("df1b6467-149c-4de0-8618-447d4a7a7223"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102")
                    },
                    new Result
                    {
                        Id = new Guid("62b9e69e-b07b-4765-9ec3-4743c48a2e71"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102")
                    },
                    new Result
                    {
                        Id = new Guid("6b84d75a-74ce-4952-b5a3-586e31aa84f0"),
                        Type = ResultType.Finished,
                        Position = 19,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102")
                    },
                    new Result
                    {
                        Id = new Guid("91eeafa5-0419-459e-85ec-5a5842bacc98"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102")
                    },
                    new Result
                    {
                        Id = new Guid("062b7a9c-ad01-4f03-b3cb-6721cbd100e2"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102")
                    },
                    new Result
                    {
                        Id = new Guid("bed8c878-5d09-4d1a-84d0-6cfba76af766"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102")
                    },
                    new Result
                    {
                        Id = new Guid("535d3372-301f-4ce2-a24c-71d90178fda8"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102")
                    },
                    new Result
                    {
                        Id = new Guid("0130f07b-2e6f-4a20-ae7d-78d771cea220"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102")
                    },
                    new Result
                    {
                        Id = new Guid("334bb2f2-d6a8-409c-bb3d-79b4ba08e9dd"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102")
                    },
                    new Result
                    {
                        Id = new Guid("0a354f10-92ec-422d-8f0d-7d15ff8ef146"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102")
                    },
                    new Result
                    {
                        Id = new Guid("ccada1c2-378a-426b-8f90-98f4e1ee56db"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 19,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102")
                    },
                    new Result
                    {
                        Id = new Guid("25079df7-3287-46bb-bbd8-a7624b2ddc9f"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102")
                    },
                    new Result
                    {
                        Id = new Guid("8aeb1da4-5817-45e2-aa2d-c66559b3e3f1"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102")
                    },
                    new Result
                    {
                        Id = new Guid("681af775-accc-4e69-884b-d3a76fa58259"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102")
                    },
                    new Result
                    {
                        Id = new Guid("57c5611c-2fa5-468e-8d4d-d95c29c7de64"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102")
                    },
                    new Result
                    {
                        Id = new Guid("929e73de-ffa3-4444-8002-f2d06fc09b99"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102")
                    },
                    new Result
                    {
                        Id = new Guid("fb8fb22a-e97a-417c-b9d5-fbe841081f7d"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("f5663ea5-12ed-4310-94b4-84cb09dc5102")
                    },
                    new Result
                    {
                        Id = new Guid("19266a84-9692-4135-8eb3-4215e45b1d29"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("7d411456-e69c-4612-b372-a178d152f86d")
                    },
                    new Result
                    {
                        Id = new Guid("2e032461-81db-425a-8cd9-457e7860b17f"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("7d411456-e69c-4612-b372-a178d152f86d")
                    },
                    new Result
                    {
                        Id = new Guid("cedb1b40-e52d-4093-a34e-4a55ed45703a"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("7d411456-e69c-4612-b372-a178d152f86d")
                    },
                    new Result
                    {
                        Id = new Guid("86828463-eb03-4077-bf42-4d299153f2e4"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("7d411456-e69c-4612-b372-a178d152f86d")
                    },
                    new Result
                    {
                        Id = new Guid("4240ae30-da65-466a-b31e-59f82da8f748"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("7d411456-e69c-4612-b372-a178d152f86d")
                    },
                    new Result
                    {
                        Id = new Guid("471df83c-9872-4ef6-8467-5a811aaf8a1b"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("7d411456-e69c-4612-b372-a178d152f86d")
                    },
                    new Result
                    {
                        Id = new Guid("8060e357-8c38-4efb-b558-651af4d4b297"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("7d411456-e69c-4612-b372-a178d152f86d")
                    },
                    new Result
                    {
                        Id = new Guid("732de025-fabe-453e-a386-726f62fc35b7"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("7d411456-e69c-4612-b372-a178d152f86d")
                    },
                    new Result
                    {
                        Id = new Guid("2caf3eed-edd5-4d24-bf49-778c3671946b"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("7d411456-e69c-4612-b372-a178d152f86d")
                    },
                    new Result
                    {
                        Id = new Guid("14f6accd-631d-4b66-a1e2-870c870234bc"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("7d411456-e69c-4612-b372-a178d152f86d")
                    },
                    new Result
                    {
                        Id = new Guid("fb5059c7-764e-4d6c-9cf8-a1c5946744f7"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 7,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("7d411456-e69c-4612-b372-a178d152f86d")
                    },
                    new Result
                    {
                        Id = new Guid("275e2d95-78b8-48e2-aeae-ac34e14ad395"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("7d411456-e69c-4612-b372-a178d152f86d")
                    },
                    new Result
                    {
                        Id = new Guid("247c0dba-a66d-488d-be56-afce4ee55d7d"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("7d411456-e69c-4612-b372-a178d152f86d")
                    },
                    new Result
                    {
                        Id = new Guid("b881abfb-0bb0-424b-9aef-b4fe0f437a42"),
                        Type = ResultType.DNS,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("7d411456-e69c-4612-b372-a178d152f86d")
                    },
                    new Result
                    {
                        Id = new Guid("881ed6de-518c-41b0-885f-b614cec81371"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("7d411456-e69c-4612-b372-a178d152f86d")
                    },
                    new Result
                    {
                        Id = new Guid("571f6907-4bea-40d8-9ca4-d07e1da7753e"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("7d411456-e69c-4612-b372-a178d152f86d")
                    },
                    new Result
                    {
                        Id = new Guid("02617b18-f4d5-4b3b-8778-f1b6b991eb1a"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("7d411456-e69c-4612-b372-a178d152f86d")
                    },
                    new Result
                    {
                        Id = new Guid("7874e603-117c-49e3-ab80-f3a653e0aad1"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("7d411456-e69c-4612-b372-a178d152f86d")
                    },
                    new Result
                    {
                        Id = new Guid("9948c0ce-caae-4176-8c09-f6fd450633f4"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("7d411456-e69c-4612-b372-a178d152f86d")
                    },
                    new Result
                    {
                        Id = new Guid("66613e5a-d8b6-4530-b17b-f9461f0dd82a"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("7d411456-e69c-4612-b372-a178d152f86d")
                    },
                    new Result
                    {
                        Id = new Guid("0d838d15-293f-457b-b599-079a106ab125"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271")
                    },
                    new Result
                    {
                        Id = new Guid("fd816b41-ce53-46ba-bd62-18ba3af3fd77"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271")
                    },
                    new Result
                    {
                        Id = new Guid("a0a97f09-0caf-4ee6-9bc9-3fa333b6147b"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271")
                    },
                    new Result
                    {
                        Id = new Guid("d89cf4de-bbaf-4df1-8da4-411311a5aa5c"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271")
                    },
                    new Result
                    {
                        Id = new Guid("ab400fc2-2dd5-4ed0-a2ae-7c73c7c488f6"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271")
                    },
                    new Result
                    {
                        Id = new Guid("6d39629d-78eb-461b-9808-81dd86261c61"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271")
                    },
                    new Result
                    {
                        Id = new Guid("52299646-2ca9-40cc-aa19-8be1bf4ba546"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271")
                    },
                    new Result
                    {
                        Id = new Guid("7699a9a4-3c7a-4fc0-86a3-8ccc5a41fc71"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271")
                    },
                    new Result
                    {
                        Id = new Guid("36c67a53-0f1b-41f2-8564-8cf6a11e2ff8"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271")
                    },
                    new Result
                    {
                        Id = new Guid("bd4522b3-3746-4d93-a967-914f1d21fd19"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271")
                    },
                    new Result
                    {
                        Id = new Guid("77d521e1-09ce-4a38-8b71-928255a90d0d"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271")
                    },
                    new Result
                    {
                        Id = new Guid("4d20aaf5-20a3-4746-aa3b-95ea11edaff7"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271")
                    },
                    new Result
                    {
                        Id = new Guid("71cca50a-1303-4c39-badf-bd3a2289eb03"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271")
                    },
                    new Result
                    {
                        Id = new Guid("c9917d94-2cad-4f27-ba91-c1e1a89ad88e"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271")
                    },
                    new Result
                    {
                        Id = new Guid("9500a3bb-c999-482f-9ba8-cd9e35d37354"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271")
                    },
                    new Result
                    {
                        Id = new Guid("9ede4e5a-9634-4502-96a8-ddf554b25d17"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271")
                    },
                    new Result
                    {
                        Id = new Guid("04d07217-a0e3-4fe2-b082-e002fa4eab93"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271")
                    },
                    new Result
                    {
                        Id = new Guid("9b912dd1-c1e3-4e26-87ab-e731fe49f5d2"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271")
                    },
                    new Result
                    {
                        Id = new Guid("8530a5d1-5395-45bb-a795-fbcc4f891cd1"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271")
                    },
                    new Result
                    {
                        Id = new Guid("46afdafe-6789-4ced-b57a-fc9670c6020d"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("1bb25a8e-bbed-424e-93cc-c7682b159271")
                    },
                    new Result
                    {
                        Id = new Guid("a6016a61-7b6f-4dc1-81fa-01ec47bd0324"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("09d4acad-8089-41b1-af8e-48565540acbc")
                    },
                    new Result
                    {
                        Id = new Guid("f11a3a48-b56f-42e8-80b7-14ee6216faad"),
                        Type = ResultType.Finished,
                        Position = 19,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("09d4acad-8089-41b1-af8e-48565540acbc")
                    },
                    new Result
                    {
                        Id = new Guid("d1af600d-7d33-4b78-a1ae-18d27a57cd59"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("09d4acad-8089-41b1-af8e-48565540acbc")
                    },
                    new Result
                    {
                        Id = new Guid("6a997be4-6de9-4841-992a-1ffc0267b3a1"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("09d4acad-8089-41b1-af8e-48565540acbc")
                    },
                    new Result
                    {
                        Id = new Guid("5cabccf3-1aba-4c85-bfeb-28a74f0279cf"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 26,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("09d4acad-8089-41b1-af8e-48565540acbc")
                    },
                    new Result
                    {
                        Id = new Guid("4f281cbe-05b2-4074-aa99-2e81160da7a6"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("09d4acad-8089-41b1-af8e-48565540acbc")
                    },
                    new Result
                    {
                        Id = new Guid("278c4098-2ee5-4954-b0a8-3ddc3f2686ae"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("09d4acad-8089-41b1-af8e-48565540acbc")
                    },
                    new Result
                    {
                        Id = new Guid("ebf6dfb5-24e3-4ab2-b3a6-4ea2e81fbb66"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("09d4acad-8089-41b1-af8e-48565540acbc")
                    },
                    new Result
                    {
                        Id = new Guid("0fa7ec05-219e-431f-8979-501baa9e5be1"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("09d4acad-8089-41b1-af8e-48565540acbc")
                    },
                    new Result
                    {
                        Id = new Guid("d5ac2d0a-70d6-41f3-8dcf-5b248bff845f"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("09d4acad-8089-41b1-af8e-48565540acbc")
                    },
                    new Result
                    {
                        Id = new Guid("b8e28634-9bd5-4951-8027-5ccf22360927"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("09d4acad-8089-41b1-af8e-48565540acbc")
                    },
                    new Result
                    {
                        Id = new Guid("d717e6ca-38ea-4131-9dc4-61d15be76e24"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("09d4acad-8089-41b1-af8e-48565540acbc")
                    },
                    new Result
                    {
                        Id = new Guid("1d264548-b0ad-4037-8e53-840b549693d3"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("09d4acad-8089-41b1-af8e-48565540acbc")
                    },
                    new Result
                    {
                        Id = new Guid("db44553d-6487-4910-8b5c-adb35fce2474"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("09d4acad-8089-41b1-af8e-48565540acbc")
                    },
                    new Result
                    {
                        Id = new Guid("b8d728ff-fb78-43b6-9fcc-c3702e9336b7"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("09d4acad-8089-41b1-af8e-48565540acbc")
                    },
                    new Result
                    {
                        Id = new Guid("d03e2ebf-33b9-483d-a244-ccb42468ef2e"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("09d4acad-8089-41b1-af8e-48565540acbc")
                    },
                    new Result
                    {
                        Id = new Guid("397e51fe-fdfc-4e0e-ba7d-cd94db9f3c95"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("09d4acad-8089-41b1-af8e-48565540acbc")
                    },
                    new Result
                    {
                        Id = new Guid("d625c0ba-fd87-49ee-b95c-d5fd011e2362"),
                        Type = ResultType.Finished,
                        Position = 20,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("09d4acad-8089-41b1-af8e-48565540acbc")
                    },
                    new Result
                    {
                        Id = new Guid("27fd9a53-3800-4b13-9ce6-f2dbb30d5218"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("09d4acad-8089-41b1-af8e-48565540acbc")
                    },
                    new Result
                    {
                        Id = new Guid("f550cb37-5be2-4ee9-afa0-fc7c0a3c805b"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("09d4acad-8089-41b1-af8e-48565540acbc")
                    },
                    new Result
                    {
                        Id = new Guid("f14297af-436c-4066-bddb-07af41b2b3d0"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a")
                    },
                    new Result
                    {
                        Id = new Guid("ca1ecc3b-1c76-477c-9102-2550e73d5fa5"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a")
                    },
                    new Result
                    {
                        Id = new Guid("e4f2c6f6-9505-4d9b-ba48-3e85aaf1761e"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a")
                    },
                    new Result
                    {
                        Id = new Guid("53817b3b-2c4f-4290-a57a-496f0328ce61"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a")
                    },
                    new Result
                    {
                        Id = new Guid("861bfba8-7fa9-45b9-ae4b-50d53c37586d"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a")
                    },
                    new Result
                    {
                        Id = new Guid("610ceef4-0a36-4dae-818f-535b443ac69b"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a")
                    },
                    new Result
                    {
                        Id = new Guid("44992ea0-4723-4297-9759-65838d8d2958"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a")
                    },
                    new Result
                    {
                        Id = new Guid("1f9240d4-cd84-4f52-a133-6c884a50251e"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a")
                    },
                    new Result
                    {
                        Id = new Guid("b3daa9d5-7725-4426-8229-6e912fd72eaf"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a")
                    },
                    new Result
                    {
                        Id = new Guid("f56d673a-04bb-4c80-baf3-7b49882754bd"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a")
                    },
                    new Result
                    {
                        Id = new Guid("1912d1c8-0d5d-4742-8241-81e6d1a9b217"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a")
                    },
                    new Result
                    {
                        Id = new Guid("7a6017ad-2500-4f88-8f88-8671661f4508"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a")
                    },
                    new Result
                    {
                        Id = new Guid("734fc7fa-cac3-41ac-b19c-8a01f565aad2"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 19,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a")
                    },
                    new Result
                    {
                        Id = new Guid("da596f38-4562-40a2-9fa3-94a3c88aab95"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a")
                    },
                    new Result
                    {
                        Id = new Guid("dabf3bb4-5665-4fb0-90d1-9f21aa517ab2"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a")
                    },
                    new Result
                    {
                        Id = new Guid("19b965cd-b28f-4d8f-b2d2-9f5792f68036"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a")
                    },
                    new Result
                    {
                        Id = new Guid("9cc5823c-fe9a-49c8-97a1-ba6277ab6d7f"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a")
                    },
                    new Result
                    {
                        Id = new Guid("87f5fbd0-ed1f-4315-9bf4-d886416cc1ed"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a")
                    },
                    new Result
                    {
                        Id = new Guid("7fe500e0-fc35-492d-b99f-dea732be3c12"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a")
                    },
                    new Result
                    {
                        Id = new Guid("1cc21b5c-9020-4768-8fae-fe4f1b324b74"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("b9d62668-0f3a-4bd2-a0fe-dade214e928a")
                    },
                    new Result
                    {
                        Id = new Guid("f1a4f476-2cec-422b-80d8-0c182e5ca9f3"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f")
                    },
                    new Result
                    {
                        Id = new Guid("1fa8fd1a-657c-4381-aad6-0d9e4c744c4b"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f")
                    },
                    new Result
                    {
                        Id = new Guid("5acbb744-b9dc-4c2d-beb5-1e5c5ce05b92"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f")
                    },
                    new Result
                    {
                        Id = new Guid("e72a70ef-f245-4511-8049-2281516000f1"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f")
                    },
                    new Result
                    {
                        Id = new Guid("f05f2c9e-e548-44b8-bf47-2aa68ba42d43"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f")
                    },
                    new Result
                    {
                        Id = new Guid("23171499-db0e-4b16-9499-364ba011494d"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f")
                    },
                    new Result
                    {
                        Id = new Guid("33f89cb4-e82c-4b80-ac83-37443667737b"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f")
                    },
                    new Result
                    {
                        Id = new Guid("6601f4c8-0a3a-4007-85f9-52c6eff4a88f"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f")
                    },
                    new Result
                    {
                        Id = new Guid("c6cd8548-b484-4cc3-b205-530b445633a1"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f")
                    },
                    new Result
                    {
                        Id = new Guid("e8fcafa2-da62-4d9d-b7ce-593bca8ce823"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f")
                    },
                    new Result
                    {
                        Id = new Guid("0e888378-dd27-4955-8ae0-5d76f43b66b2"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f")
                    },
                    new Result
                    {
                        Id = new Guid("8c8c6e18-a17a-40a5-9c46-6215cfcb469c"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f")
                    },
                    new Result
                    {
                        Id = new Guid("04920571-639c-4464-9c6b-6cd399597483"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f")
                    },
                    new Result
                    {
                        Id = new Guid("5fbfadec-4b1a-462d-bdab-76e08582c9e1"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f")
                    },
                    new Result
                    {
                        Id = new Guid("e5a7658a-bfe9-43df-ba5b-8adeda76c311"),
                        Type = ResultType.Finished,
                        Position = 19,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f")
                    },
                    new Result
                    {
                        Id = new Guid("39adaf21-664d-4945-b986-8afe846e2e34"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f")
                    },
                    new Result
                    {
                        Id = new Guid("be88e7a9-ea29-48db-ab96-acf3ccfb621e"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 26,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f")
                    },
                    new Result
                    {
                        Id = new Guid("0b1daba7-1c6e-4101-a9d4-cc0e8b1fb154"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f")
                    },
                    new Result
                    {
                        Id = new Guid("bcbf7e9c-584b-44d3-8193-de7db1dc5a73"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f")
                    },
                    new Result
                    {
                        Id = new Guid("c3a90d8b-3ecb-4d79-aa63-fa6c966e2d5b"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("7f8afbba-84eb-4c4a-9b27-7f95b74dbc0f")
                    },
                    new Result
                    {
                        Id = new Guid("93106dc0-7d50-4bc1-952b-0255a11c15c9"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 1,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13")
                    },
                    new Result
                    {
                        Id = new Guid("35e51e7f-1c4b-47d9-8ff6-236f41604a25"),
                        Type = ResultType.Finished,
                        Position = 19,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13")
                    },
                    new Result
                    {
                        Id = new Guid("6add97a4-69da-4a02-b620-25ef12803ec6"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 0,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13")
                    },
                    new Result
                    {
                        Id = new Guid("39bcc83a-1f28-4fe8-8935-26baef9b3b19"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13")
                    },
                    new Result
                    {
                        Id = new Guid("98745dbd-eb83-450f-8bf5-2f8de5e6df43"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 3,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13")
                    },
                    new Result
                    {
                        Id = new Guid("ca3a8197-0d78-4adc-9400-3535c50a82f9"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 0,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13")
                    },
                    new Result
                    {
                        Id = new Guid("0da133a4-752c-4280-a0e0-390ef96c8364"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13")
                    },
                    new Result
                    {
                        Id = new Guid("204af14b-da4c-46ef-9379-4477026ccc99"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13")
                    },
                    new Result
                    {
                        Id = new Guid("7e8dad89-50f8-4722-ba6f-5bbf17cc6168"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 2,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13")
                    },
                    new Result
                    {
                        Id = new Guid("631e11af-ed6e-4196-85ec-7e52fb5a0605"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13")
                    },
                    new Result
                    {
                        Id = new Guid("e34ba29d-fa78-45c7-acac-847077f8dab1"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13")
                    },
                    new Result
                    {
                        Id = new Guid("7944cac5-24c6-4b3f-b621-859b0ebf054b"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 0,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13")
                    },
                    new Result
                    {
                        Id = new Guid("3d30a0e7-9a6c-493a-b309-8a483e038a12"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13")
                    },
                    new Result
                    {
                        Id = new Guid("66343af1-37c1-4486-b446-8ca2278c7fdb"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 0,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13")
                    },
                    new Result
                    {
                        Id = new Guid("2ebc52b2-df41-441d-b9c8-a589bce343e5"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 0,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13")
                    },
                    new Result
                    {
                        Id = new Guid("a323d70b-0d01-4ac7-87e1-bd0327b66d8f"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13")
                    },
                    new Result
                    {
                        Id = new Guid("a2dd6482-cddd-4020-a5f1-cb5d30e0e54b"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13")
                    },
                    new Result
                    {
                        Id = new Guid("ef75935c-4dae-4e9f-9786-e070e3602161"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 0,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13")
                    },
                    new Result
                    {
                        Id = new Guid("059ee6dc-6e2d-490a-bde1-e676299adc28"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13")
                    },
                    new Result
                    {
                        Id = new Guid("b77f95b2-f592-4c71-a253-fa93e447e12a"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("e91b6450-caf4-4def-8a9c-2717bfd0ca13")
                    },
                    new Result
                    {
                        Id = new Guid("66dd7caa-bf6c-47e2-a12c-10613969a02b"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94")
                    },
                    new Result
                    {
                        Id = new Guid("1be7cf88-87b6-4a56-b71e-1b3b9642c481"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94")
                    },
                    new Result
                    {
                        Id = new Guid("fcfa60d9-a3bf-47f2-a4e4-30751f83a645"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94")
                    },
                    new Result
                    {
                        Id = new Guid("0dd15f74-c6e2-4328-aecb-347a0206ed0d"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94")
                    },
                    new Result
                    {
                        Id = new Guid("abec1513-a8da-41b3-ab1c-384f876502de"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94")
                    },
                    new Result
                    {
                        Id = new Guid("31e1e130-a950-4a6c-b49a-51d2a3b2bf64"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94")
                    },
                    new Result
                    {
                        Id = new Guid("63a59a14-4a53-4121-b5f8-8c27343a846a"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94")
                    },
                    new Result
                    {
                        Id = new Guid("29840767-0355-4ed3-9edd-9068b701d93d"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94")
                    },
                    new Result
                    {
                        Id = new Guid("fa30c06f-a31e-4de5-b5a4-95547ac2382f"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94")
                    },
                    new Result
                    {
                        Id = new Guid("ba250c48-b818-4d97-844c-a80ff7aabbdc"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94")
                    },
                    new Result
                    {
                        Id = new Guid("b78dd1ac-e516-4a85-8769-b5360d488675"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94")
                    },
                    new Result
                    {
                        Id = new Guid("260b6ce4-d49b-4a09-9d23-cfb787b4bc09"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94")
                    },
                    new Result
                    {
                        Id = new Guid("2968a4cc-3053-4701-8fdf-dd6089c5c88d"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94")
                    },
                    new Result
                    {
                        Id = new Guid("267c18d9-6d7b-45bf-916c-e1f6e2ee46a2"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94")
                    },
                    new Result
                    {
                        Id = new Guid("da01c1c5-0ec4-4b9c-b6a0-e24114f01af7"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94")
                    },
                    new Result
                    {
                        Id = new Guid("22d0fd7d-b413-4a4f-8060-e5058c6606f2"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94")
                    },
                    new Result
                    {
                        Id = new Guid("aa0280db-8894-4cd5-ae5f-e71510cebdb4"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94")
                    },
                    new Result
                    {
                        Id = new Guid("14a6e5c0-c0da-4143-b9ac-eda6f894c3e8"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94")
                    },
                    new Result
                    {
                        Id = new Guid("e263681f-aedd-4185-a25a-f55fd55c542f"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94")
                    },
                    new Result
                    {
                        Id = new Guid("69d993f3-040e-4e9a-99fb-ffa380fb1616"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("a782ee44-a767-4eab-be24-4ae2af719c94")
                    },
                    new Result
                    {
                        Id = new Guid("0a0a2dc2-6460-46d0-ba73-04faa7e7b42d"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("72ce4177-715b-445a-af83-fd303ab00f41")
                    },
                    new Result
                    {
                        Id = new Guid("b8a070a8-eb33-4f39-8757-17f7da75b356"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("72ce4177-715b-445a-af83-fd303ab00f41")
                    },
                    new Result
                    {
                        Id = new Guid("92824883-9cb1-4c1c-9430-220fc75cdc2a"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("72ce4177-715b-445a-af83-fd303ab00f41")
                    },
                    new Result
                    {
                        Id = new Guid("cc9d5e10-8c1b-496e-82da-235c4e339b71"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("72ce4177-715b-445a-af83-fd303ab00f41")
                    },
                    new Result
                    {
                        Id = new Guid("dd95cab5-19e7-44b8-9e0f-368d64172aa2"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("72ce4177-715b-445a-af83-fd303ab00f41")
                    },
                    new Result
                    {
                        Id = new Guid("bb27a501-5591-4288-867c-47c1f5939610"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("72ce4177-715b-445a-af83-fd303ab00f41")
                    },
                    new Result
                    {
                        Id = new Guid("c277959c-cc56-4418-9713-4db7a6a5e364"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("72ce4177-715b-445a-af83-fd303ab00f41")
                    },
                    new Result
                    {
                        Id = new Guid("eda9470d-6d5e-42e6-b4ca-4ec71c9f1510"),
                        Type = ResultType.DSQ,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("72ce4177-715b-445a-af83-fd303ab00f41")
                    },
                    new Result
                    {
                        Id = new Guid("84644fa9-7d98-4716-a462-514d6481ca7f"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("72ce4177-715b-445a-af83-fd303ab00f41")
                    },
                    new Result
                    {
                        Id = new Guid("294f23bd-a927-4c3a-ae6f-5aaaa5e53bef"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("72ce4177-715b-445a-af83-fd303ab00f41")
                    },
                    new Result
                    {
                        Id = new Guid("8f00745b-347a-4722-97d4-656a781ce235"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("72ce4177-715b-445a-af83-fd303ab00f41")
                    },
                    new Result
                    {
                        Id = new Guid("81788f14-3071-4241-8154-6b0292fc9ef4"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("72ce4177-715b-445a-af83-fd303ab00f41")
                    },
                    new Result
                    {
                        Id = new Guid("2d9dee51-e970-45d7-a919-816836406051"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("72ce4177-715b-445a-af83-fd303ab00f41")
                    },
                    new Result
                    {
                        Id = new Guid("c44ab0d9-80f0-462d-97db-82dd5035d145"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("72ce4177-715b-445a-af83-fd303ab00f41")
                    },
                    new Result
                    {
                        Id = new Guid("eecf76f0-0e3a-400d-882c-901e5cf5eda5"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("72ce4177-715b-445a-af83-fd303ab00f41")
                    },
                    new Result
                    {
                        Id = new Guid("70d178c5-91bd-4b57-95c4-9e691b0ea41d"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("72ce4177-715b-445a-af83-fd303ab00f41")
                    },
                    new Result
                    {
                        Id = new Guid("b40d1716-e027-4460-aa3e-b5a9b86e8d5d"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("72ce4177-715b-445a-af83-fd303ab00f41")
                    },
                    new Result
                    {
                        Id = new Guid("78eea7bc-721b-4c72-9b25-e9359d0dcd54"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("72ce4177-715b-445a-af83-fd303ab00f41")
                    },
                    new Result
                    {
                        Id = new Guid("d5f7bc58-db5d-4ab3-ae8e-eb4e5e1d69ea"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 11,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("72ce4177-715b-445a-af83-fd303ab00f41")
                    },
                    new Result
                    {
                        Id = new Guid("8f0b46d1-0e1e-4172-8f32-f80ac25d1c5c"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("72ce4177-715b-445a-af83-fd303ab00f41")
                    },
                    new Result
                    {
                        Id = new Guid("22a2d92b-1c55-42d9-b958-11315216b4a3"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 1,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021")
                    },
                    new Result
                    {
                        Id = new Guid("f4183d15-0473-424d-bbef-175c3f51da8a"),
                        Type = ResultType.Finished,
                        Position = 20,
                        Point = 0,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021")
                    },
                    new Result
                    {
                        Id = new Guid("ce469dd1-3969-4c45-95a6-1948e3e931dc"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 5,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021")
                    },
                    new Result
                    {
                        Id = new Guid("8367def6-7622-4d7f-978e-1b684e80d239"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021")
                    },
                    new Result
                    {
                        Id = new Guid("811cdbd0-1c2d-4fcc-a52e-298a2c3db6d4"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021")
                    },
                    new Result
                    {
                        Id = new Guid("997e1d44-6d14-4b29-aba6-2a5e83471204"),
                        Type = ResultType.Finished,
                        Position = 19,
                        Point = 0,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021")
                    },
                    new Result
                    {
                        Id = new Guid("4186f4e3-6114-4463-a406-2e7427d203ea"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 6,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021")
                    },
                    new Result
                    {
                        Id = new Guid("0472f3d5-a947-41cd-9e69-2eedbf1f996f"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 9,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021")
                    },
                    new Result
                    {
                        Id = new Guid("186845e8-1bf5-46e3-9f24-3290bb6dc348"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 12.5,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021")
                    },
                    new Result
                    {
                        Id = new Guid("06906a70-8098-4182-8265-5446b016dfb2"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021")
                    },
                    new Result
                    {
                        Id = new Guid("2f0ec403-6d13-437e-9afb-6265039b2ad1"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021")
                    },
                    new Result
                    {
                        Id = new Guid("c77126f5-1760-41c7-889a-675ec7afe0a4"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021")
                    },
                    new Result
                    {
                        Id = new Guid("8afcac85-ab56-4503-8897-8709a45d5158"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021")
                    },
                    new Result
                    {
                        Id = new Guid("86eba943-6212-4543-8d4f-9c2ee0b82245"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 4,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021")
                    },
                    new Result
                    {
                        Id = new Guid("39d315af-6ffd-4b99-bb06-b6329d2294a0"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021")
                    },
                    new Result
                    {
                        Id = new Guid("bf618445-56d6-4161-be39-b8d9cc1f994c"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 7.5,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021")
                    },
                    new Result
                    {
                        Id = new Guid("d4cbe44a-ad15-4f57-bee4-b98b511fb62d"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 3,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021")
                    },
                    new Result
                    {
                        Id = new Guid("5fda1763-b59e-49c2-b810-bb97e5196b10"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 0.5,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021")
                    },
                    new Result
                    {
                        Id = new Guid("37f84997-1f61-4efc-be82-bf9ebaa76fd6"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021")
                    },
                    new Result
                    {
                        Id = new Guid("8f88f760-6eaa-4e2b-b030-e72465434a52"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 2,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("dda38f1a-97f5-4ab4-b153-78622638e021")
                    },
                    new Result
                    {
                        Id = new Guid("3e7e064a-2b32-440f-aadf-173e81426e67"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e")
                    },
                    new Result
                    {
                        Id = new Guid("e2e35685-d192-4c07-9838-1d2de6aaf280"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e")
                    },
                    new Result
                    {
                        Id = new Guid("47fca973-922f-4d10-9839-1d361ad5c1a0"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e")
                    },
                    new Result
                    {
                        Id = new Guid("60b92625-bd25-4ccf-8aa5-3a8e17d75c58"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e")
                    },
                    new Result
                    {
                        Id = new Guid("6e5a9c04-7b18-4d69-81ee-5328ed6db07a"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e")
                    },
                    new Result
                    {
                        Id = new Guid("e40d1954-bbed-48a9-9a68-54d9e5560688"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("8ea53266-9f3b-4d74-99c4-ee1e4c68596e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e")
                    },
                    new Result
                    {
                        Id = new Guid("14df73a2-2785-4b73-ada7-5c4550814f1e"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e")
                    },
                    new Result
                    {
                        Id = new Guid("fabdeda0-9498-4d49-ada0-67380c6d63fc"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e")
                    },
                    new Result
                    {
                        Id = new Guid("7bf2af82-1413-4979-bb32-6a0f3282ccc0"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e")
                    },
                    new Result
                    {
                        Id = new Guid("4fc5c007-c7bd-4a6c-9cad-6eff1d7e276c"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e")
                    },
                    new Result
                    {
                        Id = new Guid("a55ccf79-791a-4577-9c5b-6f03679baf93"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e")
                    },
                    new Result
                    {
                        Id = new Guid("7ccbe7fa-cbde-46a2-930c-87d7393f8b27"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e")
                    },
                    new Result
                    {
                        Id = new Guid("01ac96d3-3663-4104-afe3-936b7ba704c7"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e")
                    },
                    new Result
                    {
                        Id = new Guid("f3a8f71d-66b4-42a2-a02b-a1e8d07ec7fa"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e")
                    },
                    new Result
                    {
                        Id = new Guid("1c4b3dcd-be99-4696-8943-c5529eb183c9"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e")
                    },
                    new Result
                    {
                        Id = new Guid("560bc1b7-259b-4188-a5d0-dbd49cfcdbf5"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e")
                    },
                    new Result
                    {
                        Id = new Guid("29dc54ba-8c8c-4029-8c09-df277082d733"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e")
                    },
                    new Result
                    {
                        Id = new Guid("7e2ffc8e-449f-4c0c-8443-e204ddba8a41"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 19,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e")
                    },
                    new Result
                    {
                        Id = new Guid("cf2ee9f0-79dc-4a05-8a61-e598dc89dcc0"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e")
                    },
                    new Result
                    {
                        Id = new Guid("45868224-e647-4a07-a2ee-f80fc6086ae1"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("49d014d9-ab60-4878-9899-18ecbaa4653e")
                    },
                    new Result
                    {
                        Id = new Guid("10425511-0c7e-4910-ad3c-023d1278a20c"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db")
                    },
                    new Result
                    {
                        Id = new Guid("88c1a00a-a5ed-468a-88ef-0bad3899e25a"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 1,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db")
                    },
                    new Result
                    {
                        Id = new Guid("17b5401f-d574-4a56-87f4-141149af991e"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 0,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db")
                    },
                    new Result
                    {
                        Id = new Guid("54d60ac6-7a9c-4c0d-ba77-14298de3ba8b"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 0,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db")
                    },
                    new Result
                    {
                        Id = new Guid("004f4b0f-1b66-4b18-b970-172e9f47bb8d"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("8ea53266-9f3b-4d74-99c4-ee1e4c68596e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db")
                    },
                    new Result
                    {
                        Id = new Guid("b127df35-5d18-44ae-8b7e-1b7aeced15fa"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 0,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db")
                    },
                    new Result
                    {
                        Id = new Guid("2739cce4-04d0-44ed-be8a-2b20647807e5"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 0,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db")
                    },
                    new Result
                    {
                        Id = new Guid("1c0ebd72-1440-4bf4-8d7e-2d538e6780c0"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db")
                    },
                    new Result
                    {
                        Id = new Guid("9fd82eb3-c3e7-47d7-8a01-367933744675"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db")
                    },
                    new Result
                    {
                        Id = new Guid("55adf3f6-5246-469f-8e73-3f74279d5a84"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db")
                    },
                    new Result
                    {
                        Id = new Guid("9dfa7b56-c392-4c75-822b-5aa2b57bb58e"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 0,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db")
                    },
                    new Result
                    {
                        Id = new Guid("c4716276-8198-4ae3-91a0-62c328dce55d"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db")
                    },
                    new Result
                    {
                        Id = new Guid("b1f8f3d3-6a6c-4450-b11f-6c287ba3037f"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db")
                    },
                    new Result
                    {
                        Id = new Guid("011a8c86-d254-4c40-b5bd-6ddf306ad162"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 3,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db")
                    },
                    new Result
                    {
                        Id = new Guid("37481fd9-11df-465f-841b-7224df489c11"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db")
                    },
                    new Result
                    {
                        Id = new Guid("6aeae0de-4fe1-4df0-970a-b90d7f2bbda1"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 0,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db")
                    },
                    new Result
                    {
                        Id = new Guid("8737eeeb-7972-4c4c-805b-d0e8c697db51"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 2,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db")
                    },
                    new Result
                    {
                        Id = new Guid("b7316876-1387-48c1-a182-e1dbb8f56ee0"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db")
                    },
                    new Result
                    {
                        Id = new Guid("b4cdf5bf-14ec-4e66-8872-e9de40817d13"),
                        Type = ResultType.Finished,
                        Position = 19,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db")
                    },
                    new Result
                    {
                        Id = new Guid("afd31dff-53dd-4fb3-ae77-ef0206ac5ba5"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("c24ee9b0-ecba-45d8-8cba-e64b3615c9db")
                    },
                    new Result
                    {
                        Id = new Guid("65f86db1-20af-4feb-b490-04384a89af02"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed")
                    },
                    new Result
                    {
                        Id = new Guid("4d0dbc1d-e5b0-44fd-9ff1-1d6d515ad1bb"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed")
                    },
                    new Result
                    {
                        Id = new Guid("91a0c54b-655f-41cd-a019-23758e1ac842"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("8ea53266-9f3b-4d74-99c4-ee1e4c68596e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed")
                    },
                    new Result
                    {
                        Id = new Guid("183a8a10-d249-48fd-8354-2a448508eca0"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed")
                    },
                    new Result
                    {
                        Id = new Guid("2a460cf8-053c-42e2-92e0-34c91786e5e6"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed")
                    },
                    new Result
                    {
                        Id = new Guid("32203836-b87b-4eb1-8459-3c22bb6e22f8"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed")
                    },
                    new Result
                    {
                        Id = new Guid("9df82d48-b7f0-4c7c-a224-4f4346a967f6"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed")
                    },
                    new Result
                    {
                        Id = new Guid("a7b9c863-ecaf-473a-bb28-62af3e3995fa"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed")
                    },
                    new Result
                    {
                        Id = new Guid("50f671f3-e2a1-407e-b89c-8158545b2e61"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed")
                    },
                    new Result
                    {
                        Id = new Guid("75bd7e19-e725-48b9-876e-8c040d4e88a9"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed")
                    },
                    new Result
                    {
                        Id = new Guid("d2392be9-3696-466d-bc49-9d2fc1a765f0"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed")
                    },
                    new Result
                    {
                        Id = new Guid("78fc5a40-42e9-4b25-ad6e-a173282dc8b5"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed")
                    },
                    new Result
                    {
                        Id = new Guid("97a12563-1cba-4963-bae1-ad29ea9e162b"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 26,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed")
                    },
                    new Result
                    {
                        Id = new Guid("0a27e6a2-b511-4d5d-bb1b-bf838086eb2d"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed")
                    },
                    new Result
                    {
                        Id = new Guid("1ce28612-03df-4827-a555-c0235c80ed7b"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed")
                    },
                    new Result
                    {
                        Id = new Guid("d10cde40-6b3d-478e-a819-cb0a100192f6"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed")
                    },
                    new Result
                    {
                        Id = new Guid("d7daea41-7d60-4cf7-a37f-cb397c12703d"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed")
                    },
                    new Result
                    {
                        Id = new Guid("68b77f43-5d88-461d-bc2a-de615604e2ab"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed")
                    },
                    new Result
                    {
                        Id = new Guid("2c2b0fa5-64e4-4086-bad8-e47be295e1d9"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed")
                    },
                    new Result
                    {
                        Id = new Guid("933d4060-7ffb-492d-9d08-fbcda372f099"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("937a8e3a-0d17-442c-95b7-0c5aa753feed")
                    },
                    new Result
                    {
                        Id = new Guid("0acc2c23-8c97-49e7-945f-0e62787b03a0"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156")
                    },
                    new Result
                    {
                        Id = new Guid("25ca8651-2dc2-44ed-a79d-10f9ba2d70c5"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156")
                    },
                    new Result
                    {
                        Id = new Guid("22bf8437-08e2-4e02-841e-1843fe095364"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156")
                    },
                    new Result
                    {
                        Id = new Guid("e6fc1dda-7c1a-427a-b774-1bd9d411b47a"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 7,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156")
                    },
                    new Result
                    {
                        Id = new Guid("f9ee6bbe-85b5-4fb8-bde5-1f8f6c952227"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156")
                    },
                    new Result
                    {
                        Id = new Guid("ec23b0fa-3327-4278-ae27-2e63ff4ff125"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156")
                    },
                    new Result
                    {
                        Id = new Guid("c6443ce5-e07d-4b02-8f95-3fb5e7f6e861"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156")
                    },
                    new Result
                    {
                        Id = new Guid("519d167d-a9fc-42fb-ae5f-48e83ac10072"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156")
                    },
                    new Result
                    {
                        Id = new Guid("885c0a65-4d2d-4f7a-b4db-494d1d7c3d45"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156")
                    },
                    new Result
                    {
                        Id = new Guid("70274458-4446-4541-890d-4d9c5dfbc277"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156")
                    },
                    new Result
                    {
                        Id = new Guid("9c93e709-2f8c-4dab-9047-554c5477a1c9"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156")
                    },
                    new Result
                    {
                        Id = new Guid("f8a3235a-d651-48f4-b320-6ed8fec39807"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156")
                    },
                    new Result
                    {
                        Id = new Guid("1bf1f2bf-49ff-4685-ba33-7c147d774f42"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156")
                    },
                    new Result
                    {
                        Id = new Guid("cbbef958-ba2e-41c3-866b-8967894996fe"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156")
                    },
                    new Result
                    {
                        Id = new Guid("7a5b1b2e-da04-4034-a887-91663b5506d3"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156")
                    },
                    new Result
                    {
                        Id = new Guid("66f37790-3155-43a3-bb72-9769c6d1e146"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156")
                    },
                    new Result
                    {
                        Id = new Guid("4df09b1e-dc69-46f0-ba2a-b11c922c41ff"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156")
                    },
                    new Result
                    {
                        Id = new Guid("399e34bb-62a9-4f61-a729-bf8d665005c2"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156")
                    },
                    new Result
                    {
                        Id = new Guid("0fefce2e-3b46-4342-928f-c1ee3e536c30"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156")
                    },
                    new Result
                    {
                        Id = new Guid("cf8e486f-fe1a-4386-bca0-fc53185db6ed"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("94a8c3b5-a442-462e-8342-c8929dfc0156")
                    },
                    new Result
                    {
                        Id = new Guid("a93a1695-979f-43b1-834a-026b962968f3"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080")
                    },
                    new Result
                    {
                        Id = new Guid("31093261-30b1-4e35-b1d1-169fde78d43b"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080")
                    },
                    new Result
                    {
                        Id = new Guid("14472da3-0bee-467c-a0b0-2e176716d058"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 26,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080")
                    },
                    new Result
                    {
                        Id = new Guid("277a0c05-7cb9-40c3-b290-460382abf7b3"),
                        Type = ResultType.Finished,
                        Position = 19,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080")
                    },
                    new Result
                    {
                        Id = new Guid("3c99dc1d-2434-4b14-b39d-4f36c6a91e9a"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080")
                    },
                    new Result
                    {
                        Id = new Guid("996f4bca-58d6-45c8-8948-51f6f326945a"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080")
                    },
                    new Result
                    {
                        Id = new Guid("c5118563-b248-4bda-9675-550e3e046a85"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080")
                    },
                    new Result
                    {
                        Id = new Guid("11cba34c-1186-4a35-9800-7bb7fa604b73"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080")
                    },
                    new Result
                    {
                        Id = new Guid("d8ceb28e-1317-4a00-8344-8ca3dcea1044"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080")
                    },
                    new Result
                    {
                        Id = new Guid("506eb2a6-692e-43f0-b7a1-9143d1d3d204"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080")
                    },
                    new Result
                    {
                        Id = new Guid("35b5d1b0-d50f-43e6-a795-961891f37a17"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080")
                    },
                    new Result
                    {
                        Id = new Guid("1deec533-ac45-4148-8440-989a7398d298"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080")
                    },
                    new Result
                    {
                        Id = new Guid("bd7e8afb-90c8-4eb1-8de4-a27f7924bf3f"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080")
                    },
                    new Result
                    {
                        Id = new Guid("bbf75293-9778-475a-a06e-a8a19b7dc85f"),
                        Type = ResultType.Finished,
                        Position = 20,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080")
                    },
                    new Result
                    {
                        Id = new Guid("96e41f1c-4b19-41e7-9f34-a8d20788249e"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080")
                    },
                    new Result
                    {
                        Id = new Guid("e2b68a33-4366-4817-8c15-b13da02ac28f"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080")
                    },
                    new Result
                    {
                        Id = new Guid("39c90cb1-abad-4e98-a8b1-b3dfc282c4db"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080")
                    },
                    new Result
                    {
                        Id = new Guid("302b7a97-1227-44e5-b4ee-cc1deb9ad7c7"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080")
                    },
                    new Result
                    {
                        Id = new Guid("8be6e3d0-2a11-493c-8650-de9e4ced3944"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080")
                    },
                    new Result
                    {
                        Id = new Guid("ef43b738-081a-4f09-92d0-fff79683a8da"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("0cebdc58-3dbb-47b7-a6af-8a3f928d2080")
                    },
                    new Result
                    {
                        Id = new Guid("2776b176-86d3-400f-92c1-17703a72e289"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9")
                    },
                    new Result
                    {
                        Id = new Guid("3410fa3a-9863-4f31-aac3-17b382973f8e"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9")
                    },
                    new Result
                    {
                        Id = new Guid("0fc38f0b-e693-4ce5-a0b8-2488c9883659"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9")
                    },
                    new Result
                    {
                        Id = new Guid("45aeb4d9-afa9-4832-8bf4-25b801ff08b1"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 19,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9")
                    },
                    new Result
                    {
                        Id = new Guid("ab2495cb-a7f7-4ec5-821f-26681e4032c3"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9")
                    },
                    new Result
                    {
                        Id = new Guid("b31e1c64-c78f-4168-b64c-3649c2c939c4"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9")
                    },
                    new Result
                    {
                        Id = new Guid("7d2c425a-fe5a-4f41-baf8-4820562c48e8"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9")
                    },
                    new Result
                    {
                        Id = new Guid("f4e2b15e-1e37-4906-8b50-5b45f9bfbd1c"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9")
                    },
                    new Result
                    {
                        Id = new Guid("244a7e28-d41c-4f78-842b-6044b242fa59"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9")
                    },
                    new Result
                    {
                        Id = new Guid("11367fda-8876-4b4d-b3c6-67889d8a2707"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9")
                    },
                    new Result
                    {
                        Id = new Guid("b87b7713-ab01-47e4-9cab-78ec9b7607b9"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9")
                    },
                    new Result
                    {
                        Id = new Guid("eeb6b02a-bd01-4804-8e4d-8a50814705f1"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9")
                    },
                    new Result
                    {
                        Id = new Guid("dd10da00-887b-4a88-9679-951c4fa07038"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9")
                    },
                    new Result
                    {
                        Id = new Guid("bc14cc2e-d572-49cc-9e43-a4ebb167dd87"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9")
                    },
                    new Result
                    {
                        Id = new Guid("e721f435-3a86-4059-a178-c560fc560062"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9")
                    },
                    new Result
                    {
                        Id = new Guid("5184c1ea-61fe-495b-9add-cd47aad51c68"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9")
                    },
                    new Result
                    {
                        Id = new Guid("be55146d-7412-4050-be19-d690017fbac9"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9")
                    },
                    new Result
                    {
                        Id = new Guid("7b9309b1-95c4-44d1-99ca-defa36b12152"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9")
                    },
                    new Result
                    {
                        Id = new Guid("983cb132-3e52-49fc-b5d2-f0b7851dfe02"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9")
                    },
                    new Result
                    {
                        Id = new Guid("01f0019a-c246-4cbf-b323-f626443df22f"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("bb7dce6b-7853-4f6c-a21d-9deb643215b9")
                    },
                    new Result
                    {
                        Id = new Guid("7248f913-6b83-40d0-909b-1f46c52a66c7"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528")
                    },
                    new Result
                    {
                        Id = new Guid("1035fb2c-7a1e-48db-b1fc-2a3d529cf1f2"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528")
                    },
                    new Result
                    {
                        Id = new Guid("f050a841-dfc7-4470-9b54-2e5dcbf92c14"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528")
                    },
                    new Result
                    {
                        Id = new Guid("569588ce-c812-48cf-adf6-3de5b308931d"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528")
                    },
                    new Result
                    {
                        Id = new Guid("ace5da0c-76af-4551-a188-54daa2ef5ff3"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528")
                    },
                    new Result
                    {
                        Id = new Guid("9c7006b3-1793-4192-b451-644e23b43a92"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528")
                    },
                    new Result
                    {
                        Id = new Guid("dd6f32a7-0884-4f97-84a4-6605adcf2297"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528")
                    },
                    new Result
                    {
                        Id = new Guid("1496085e-09d0-4e89-a405-684e14df05e5"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528")
                    },
                    new Result
                    {
                        Id = new Guid("d864f5f3-f331-4e72-9715-82740bfd0a4b"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528")
                    },
                    new Result
                    {
                        Id = new Guid("7a27f680-721c-4014-b214-915dcd509d23"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528")
                    },
                    new Result
                    {
                        Id = new Guid("124a7f46-a230-4020-991d-95abbbc886b5"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528")
                    },
                    new Result
                    {
                        Id = new Guid("328067bb-3a90-4cc7-a8d0-a54a14bf29da"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528")
                    },
                    new Result
                    {
                        Id = new Guid("972d1b5a-ad3d-49f1-af0a-b781bd53a4e2"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528")
                    },
                    new Result
                    {
                        Id = new Guid("1e907761-01a4-4cb1-9102-bcad6d039e07"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528")
                    },
                    new Result
                    {
                        Id = new Guid("997600e4-76ac-40d7-baa9-c55b13507e82"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528")
                    },
                    new Result
                    {
                        Id = new Guid("6c64f149-0d77-46ee-9a89-c949648dd765"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528")
                    },
                    new Result
                    {
                        Id = new Guid("83f3cecd-fa3a-4451-b8fa-cce2b91f0deb"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528")
                    },
                    new Result
                    {
                        Id = new Guid("7f23211d-ce27-4ce8-ac1c-dc87d109f542"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528")
                    },
                    new Result
                    {
                        Id = new Guid("6fee47c5-2028-49dc-8d3b-ef5c8ecbbeb8"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528")
                    },
                    new Result
                    {
                        Id = new Guid("e9ce70de-ba81-4038-b06b-fa7019832508"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("f366a05b-e15e-4adc-a7ae-88bcf929a528")
                    },
                    new Result
                    {
                        Id = new Guid("73a1a84d-3668-4099-8b82-08fc47574fa9"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 0,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b")
                    },
                    new Result
                    {
                        Id = new Guid("c22e3ef6-3467-4792-9e6f-0f8ee7d3c0bb"),
                        Type = ResultType.Finished,
                        Position = 19,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b")
                    },
                    new Result
                    {
                        Id = new Guid("a41ebdca-cc82-4dfd-9b79-1f506e79926e"),
                        Type = ResultType.Finished,
                        Position = 20,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b")
                    },
                    new Result
                    {
                        Id = new Guid("281f79c4-71b7-4bfe-a4b4-39b42f630c69"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b")
                    },
                    new Result
                    {
                        Id = new Guid("a9f52867-ae28-4c67-8fa6-4e96ee3eef4a"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 0,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b")
                    },
                    new Result
                    {
                        Id = new Guid("7f084c98-467b-4334-a841-4ef3d5fb119b"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 0,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b")
                    },
                    new Result
                    {
                        Id = new Guid("786bcc4d-19d4-4787-94ce-72c3dfe678b1"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 0,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b")
                    },
                    new Result
                    {
                        Id = new Guid("c1316c4c-d704-404b-9dae-86f160858965"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b")
                    },
                    new Result
                    {
                        Id = new Guid("4529d699-bfbf-4c24-8f1c-883ed2f5f5a0"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b")
                    },
                    new Result
                    {
                        Id = new Guid("de0eab12-23a1-44a1-90e3-93cf5386f7b7"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b")
                    },
                    new Result
                    {
                        Id = new Guid("0e26644d-e28a-4c2f-8b76-990e9fcc1abe"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b")
                    },
                    new Result
                    {
                        Id = new Guid("a2c23a62-f834-4cce-927e-99c5872c8971"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 3,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b")
                    },
                    new Result
                    {
                        Id = new Guid("a0b47b59-dd06-4d75-b97c-a2d30cbdba5d"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b")
                    },
                    new Result
                    {
                        Id = new Guid("26fd05db-ef51-4a1b-aba3-a30e16c21383"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b")
                    },
                    new Result
                    {
                        Id = new Guid("32305598-f964-4d10-b74f-b4f9643a5838"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 0,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b")
                    },
                    new Result
                    {
                        Id = new Guid("0068a0bc-f3bd-42a6-a352-c07ac65a8328"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b")
                    },
                    new Result
                    {
                        Id = new Guid("ce6a3dde-7dec-4622-b986-cb711a01d976"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 0,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b")
                    },
                    new Result
                    {
                        Id = new Guid("880ab134-3693-4195-afac-ebfbc66bab4c"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 1,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b")
                    },
                    new Result
                    {
                        Id = new Guid("500ce3f6-4c3b-4ad3-8da5-ec0f4fcf5917"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 0,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b")
                    },
                    new Result
                    {
                        Id = new Guid("034aeefe-21ab-4479-9bb7-f7b95bc6d87a"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 2,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("39dc5054-514b-4c60-95b6-7037d9c3955b")
                    },
                    new Result
                    {
                        Id = new Guid("0566dad6-1a5a-4c08-89e1-17631eade733"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4")
                    },
                    new Result
                    {
                        Id = new Guid("311371c7-f058-489d-bc16-1f4ae9154c3e"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4")
                    },
                    new Result
                    {
                        Id = new Guid("1bf51ec8-0015-4633-99f7-2da2f98d33d5"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4")
                    },
                    new Result
                    {
                        Id = new Guid("70606929-311d-4c8c-ae4b-393625a3d167"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4")
                    },
                    new Result
                    {
                        Id = new Guid("627d3654-8e4e-4811-a598-3e5c1d885174"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4")
                    },
                    new Result
                    {
                        Id = new Guid("3ea52cd7-2743-469e-be27-48fe0469f90f"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4")
                    },
                    new Result
                    {
                        Id = new Guid("df35e5f8-a222-4254-8e38-58467a021408"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4")
                    },
                    new Result
                    {
                        Id = new Guid("e521be98-67d9-4358-8bad-5884cf97fc64"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4")
                    },
                    new Result
                    {
                        Id = new Guid("be94a431-d44c-4711-9f8e-629f3cc4d319"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4")
                    },
                    new Result
                    {
                        Id = new Guid("48fd1943-1ee4-40a7-9fa3-847fc77c7855"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4")
                    },
                    new Result
                    {
                        Id = new Guid("da315571-83e0-49b7-8f1e-8f636d5667f8"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 13,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4")
                    },
                    new Result
                    {
                        Id = new Guid("02517dfa-fb3e-4d2d-87b7-a7d3077e111c"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4")
                    },
                    new Result
                    {
                        Id = new Guid("86cdd7a9-8a84-4023-9994-a81b84db32a3"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4")
                    },
                    new Result
                    {
                        Id = new Guid("1f152d4b-44ac-470c-9562-b2f480c12856"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4")
                    },
                    new Result
                    {
                        Id = new Guid("be4b4f3b-e43a-4fb6-9f29-bd46ee5eaf01"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4")
                    },
                    new Result
                    {
                        Id = new Guid("a534a5a6-5e1d-4d81-8f3f-d207866f2fac"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4")
                    },
                    new Result
                    {
                        Id = new Guid("73e5886c-45c2-410e-944e-e2c8098d3b7f"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4")
                    },
                    new Result
                    {
                        Id = new Guid("58c2d4bf-205d-4e6f-8430-e4461945907f"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4")
                    },
                    new Result
                    {
                        Id = new Guid("5673543e-ebfd-4ef7-942d-ed2bad90ba7f"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4")
                    },
                    new Result
                    {
                        Id = new Guid("ee479c9a-e838-411d-8fdb-f5fc9b70a943"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("9aa321ab-cb0f-423e-bcf1-28c7ee4e68e4")
                    },
                    new Result
                    {
                        Id = new Guid("a24c1e4f-708b-4d3c-84e3-050353a063ab"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f")
                    },
                    new Result
                    {
                        Id = new Guid("6b2219cf-f391-4eab-b86a-17ad5a83d28d"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f")
                    },
                    new Result
                    {
                        Id = new Guid("2eaffc4b-ebc3-4b04-acd4-17dd94ac43b5"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f")
                    },
                    new Result
                    {
                        Id = new Guid("de58bcd9-11d7-489a-9efb-2857992c2de8"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f")
                    },
                    new Result
                    {
                        Id = new Guid("300c80b9-fa00-41d5-a930-29bbf13fafa4"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f")
                    },
                    new Result
                    {
                        Id = new Guid("996f78c2-fdec-4f1c-916e-29e8956f0a65"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f")
                    },
                    new Result
                    {
                        Id = new Guid("c524aaad-078b-4bcc-b67c-30bfcbd1ede5"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 19,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f")
                    },
                    new Result
                    {
                        Id = new Guid("ae0f0afa-289d-4ee0-b149-352977f830a2"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f")
                    },
                    new Result
                    {
                        Id = new Guid("2811d570-52cc-4ade-bc53-382f3ec335fd"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f")
                    },
                    new Result
                    {
                        Id = new Guid("2f5a75d0-aff6-4cf3-aaee-4d4f958a1c09"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f")
                    },
                    new Result
                    {
                        Id = new Guid("eef8eaa3-4c9d-4ea3-82f9-64a5079fe1ac"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f")
                    },
                    new Result
                    {
                        Id = new Guid("f76e15a6-957f-4878-a489-7be2b43195d4"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f")
                    },
                    new Result
                    {
                        Id = new Guid("4d2c92ca-b49a-4943-89cc-7cf22433b4ce"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f")
                    },
                    new Result
                    {
                        Id = new Guid("281abb17-8cdc-44b3-a239-8549b8747936"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f")
                    },
                    new Result
                    {
                        Id = new Guid("f2e0f4cd-7f13-47d7-b6f9-90b3d62ff37c"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f")
                    },
                    new Result
                    {
                        Id = new Guid("003bb06f-0725-4b9a-8c52-a0f8290367e8"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f")
                    },
                    new Result
                    {
                        Id = new Guid("c349b76e-1d72-4517-af18-c2067adf022e"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f")
                    },
                    new Result
                    {
                        Id = new Guid("2e2fcfb5-3705-4975-a7f4-d826a08d4c33"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f")
                    },
                    new Result
                    {
                        Id = new Guid("dcfa3f81-d319-4a25-88cb-fcb035301519"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f")
                    },
                    new Result
                    {
                        Id = new Guid("9fd71a0e-d17c-4f8a-8377-fcc249893bf1"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("cc3d6c1f-ca88-4c47-8de9-def9ea4d7d9f")
                    },
                    new Result
                    {
                        Id = new Guid("b838579d-dcd7-4c44-826c-02d1ebb0079c"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a")
                    },
                    new Result
                    {
                        Id = new Guid("f41faff8-cbce-4acc-baef-0c5c80921df4"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a")
                    },
                    new Result
                    {
                        Id = new Guid("07d2666d-5925-4fe0-aaee-215ce5d5fce1"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a")
                    },
                    new Result
                    {
                        Id = new Guid("acea5147-6cce-4bbd-aa5c-24fcf0309ed6"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a")
                    },
                    new Result
                    {
                        Id = new Guid("ffd2c8b7-1ea9-4263-b5fd-2989254f3efd"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a")
                    },
                    new Result
                    {
                        Id = new Guid("a3d80acf-8dfe-4163-9a8a-2b902bea7aba"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a")
                    },
                    new Result
                    {
                        Id = new Guid("7e39cb94-011f-4a9a-92cb-3d3ee56cca79"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a")
                    },
                    new Result
                    {
                        Id = new Guid("8a61ac82-d537-475d-90b9-46a6d6ec5d23"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a")
                    },
                    new Result
                    {
                        Id = new Guid("27db789a-8623-4575-a625-4eb186a4682c"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a")
                    },
                    new Result
                    {
                        Id = new Guid("3b1eb17f-0451-4659-863c-8242cf441ded"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a")
                    },
                    new Result
                    {
                        Id = new Guid("4d38e725-e714-47a8-8f58-88affc66eb5d"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a")
                    },
                    new Result
                    {
                        Id = new Guid("b42ad413-dd14-4bae-967d-90965b84ce18"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a")
                    },
                    new Result
                    {
                        Id = new Guid("4f6b9104-1188-4735-8c62-a0bdd9e6150f"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a")
                    },
                    new Result
                    {
                        Id = new Guid("8c8318bf-1359-4c19-a7ae-a52de10557bb"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a")
                    },
                    new Result
                    {
                        Id = new Guid("c4f9f810-1dbf-4fa9-929f-a64aa0bfb1ad"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a")
                    },
                    new Result
                    {
                        Id = new Guid("a22d1fc6-bad2-4b53-b44f-af10237e3a14"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a")
                    },
                    new Result
                    {
                        Id = new Guid("31030637-1496-421f-9af6-b54966326173"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a")
                    },
                    new Result
                    {
                        Id = new Guid("39fe4e74-ae0c-47e1-b4a0-d328aae4d812"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a")
                    },
                    new Result
                    {
                        Id = new Guid("55fb5aaf-2286-46e7-b4c1-d44ac0e821a0"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a")
                    },
                    new Result
                    {
                        Id = new Guid("46f72533-db5c-4198-b377-ffd296078a0b"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 26,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("8821398a-2b8d-4aed-bc34-86dfa165443a")
                    },
                    new Result
                    {
                        Id = new Guid("f49d76a0-35e9-4edd-8a50-34eb4032c371"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d")
                    },
                    new Result
                    {
                        Id = new Guid("7d403bf9-49ed-4f40-9ac0-3aaf1abeaad9"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d")
                    },
                    new Result
                    {
                        Id = new Guid("c310f6c0-b3a3-4982-b43a-41a90c915544"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d")
                    },
                    new Result
                    {
                        Id = new Guid("963d9037-dab4-42b2-a93f-42060bf4a8b3"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d")
                    },
                    new Result
                    {
                        Id = new Guid("5ed5d0be-9f33-4b12-a5d4-43d45059e860"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d")
                    },
                    new Result
                    {
                        Id = new Guid("9b7af2db-49f3-42d6-86d6-4eb62bb495b6"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d")
                    },
                    new Result
                    {
                        Id = new Guid("470df61c-ac69-407f-85d5-512b77503b28"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        TeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        RaceId = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d")
                    },
                    new Result
                    {
                        Id = new Guid("ec011e3c-8cb5-44c7-83fa-531484396414"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        TeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        RaceId = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d")
                    },
                    new Result
                    {
                        Id = new Guid("67e648bf-a13b-4639-96e5-59337abc9c1f"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d")
                    },
                    new Result
                    {
                        Id = new Guid("67ccde8e-cca0-4248-a852-7259b4ef11c2"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        TeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        RaceId = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d")
                    },
                    new Result
                    {
                        Id = new Guid("fea93be8-4d86-451d-ac01-74b2715964aa"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d")
                    },
                    new Result
                    {
                        Id = new Guid("03bb98bb-fc4d-49db-a711-7f4bda7bddc4"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d")
                    },
                    new Result
                    {
                        Id = new Guid("f8523541-d59e-4ccf-9a0f-83a404a43456"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d")
                    },
                    new Result
                    {
                        Id = new Guid("a8012663-1985-4dee-87cb-865d4f64203a"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        TeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        RaceId = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d")
                    },
                    new Result
                    {
                        Id = new Guid("c4e83074-bcd2-47f3-8bcd-8a5ed43c37d3"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        TeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        RaceId = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d")
                    },
                    new Result
                    {
                        Id = new Guid("c39b57e7-aa5d-45e6-9a6c-d367794fc674"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        TeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        RaceId = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d")
                    },
                    new Result
                    {
                        Id = new Guid("babeda00-c7e1-4784-9d9f-d6c68208524e"),
                        Type = ResultType.DNS,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        TeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        RaceId = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d")
                    },
                    new Result
                    {
                        Id = new Guid("7b10111d-f68a-42a0-ae76-dd86be3309b7"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        TeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        RaceId = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d")
                    },
                    new Result
                    {
                        Id = new Guid("28fffdd9-edb7-4847-a310-e5c3de14f2ee"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        TeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        RaceId = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d")
                    },
                    new Result
                    {
                        Id = new Guid("21d744f3-9192-4eab-9e4e-f7cb69e11771"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 26,
                        DriverId = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        TeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        RaceId = new Guid("a3648fb1-2144-4cbb-af55-65cc6a42020d")
                    }
                );
            }
        }
    }
}
