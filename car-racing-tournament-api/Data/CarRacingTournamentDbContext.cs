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

                entity.Property(e => e.NationalityAlpha2);

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
                        IsArchived = true
                    },
                    new Season {
                        Id = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee"),
                        CreatedAt = DateTime.Parse("2022-03-20T15:00:00"),
                        Name = "Formula 1 2022",
                        Description = "This is the results of 2022 Formula 1 season",
                        IsArchived = false
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
                        Id = Guid.NewGuid(),
                        UserId = new Guid("08db26a9-840c-42ee-82c5-ceec14c2a104"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1"),
                        Type = PermissionType.Admin
                    },
                    new Permission
                    {
                        Id = Guid.NewGuid(),
                        UserId = new Guid("08db26a9-9264-4fb6-88aa-4c547e6326dc"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1"),
                        Type = PermissionType.Moderator
                    },
                    new Permission
                    {
                        Id = Guid.NewGuid(),
                        UserId = new Guid("08db26a9-840c-42ee-82c5-ceec14c2a104"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee"),
                        Type = PermissionType.Admin
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
                    },
                    new Race
                    {
                        Id = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083"),
                        Name = "Bahrein",
                        DateTime = DateTime.Parse("2022-03-20T15:00:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712"),
                        Name = "Szaúd Arábia",
                        DateTime = DateTime.Parse("2022-03-27T16:30:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58"),
                        Name = "Ausztrália",
                        DateTime = DateTime.Parse("2022-04-10T05:00:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82"),
                        Name = "Emilia -Romagna sprint",
                        DateTime = DateTime.Parse("2022-04-22T13:30:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa"),
                        Name = "Emilia-Romagna",
                        DateTime = DateTime.Parse("2022-04-23T12:00:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822"),
                        Name = "Miami",
                        DateTime = DateTime.Parse("2022-05-08T19:30:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450"),
                        Name = "Spanyol",
                        DateTime = DateTime.Parse("2022-05-22T13:00:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1"),
                        Name = "Monaco",
                        DateTime = DateTime.Parse("2022-05-29T13:00:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("e144d809-4fe8-46c0-89de-3ac6f907c506"),
                        Name = "Azerbajdzsán",
                        DateTime = DateTime.Parse("2022-06-12T11:00:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543"),
                        Name = "Kanada",
                        DateTime = DateTime.Parse("2022-06-19T18:00:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872"),
                        Name = "Brit",
                        DateTime = DateTime.Parse("2022-07-03T14:00:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("49172116-db53-45e1-b43c-de99be1148c6"),
                        Name = "Ausztria sprint",
                        DateTime = DateTime.Parse("2022-07-09T14:30:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b"),
                        Name = "Ausztria",
                        DateTime = DateTime.Parse("2022-07-10T13:00:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b"),
                        Name = "Francia",
                        DateTime = DateTime.Parse("2022-07-24T13:00:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e"),
                        Name = "Magyar",
                        DateTime = DateTime.Parse("2022-07-31T13:00:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("32b35aff-f243-4b23-b096-cc51938bc523"),
                        Name = "Belgium",
                        DateTime = DateTime.Parse("2022-08-28T13:00:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9"),
                        Name = "Hollandia",
                        DateTime = DateTime.Parse("2022-09-04T13:00:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc"),
                        Name = "Olasz",
                        DateTime = DateTime.Parse("2022-09-11T13:00:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb"),
                        Name = "Szingapúr",
                        DateTime = DateTime.Parse("2022-10-02T13:00:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc"),
                        Name = "Japán",
                        DateTime = DateTime.Parse("2022-10-09T05:00:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a"),
                        Name = "USA",
                        DateTime = DateTime.Parse("2022-10-23T19:00:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43"),
                        Name = "Mexikó",
                        DateTime = DateTime.Parse("2022-10-30T19:00:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("e39be72c-720c-4679-a260-7346d05fce99"),
                        Name = "Brazil sprint",
                        DateTime = DateTime.Parse("2022-11-12T19:30:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2"),
                        Name = "Brazil",
                        DateTime = DateTime.Parse("2022-11-13T18:00:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Race
                    {
                        Id = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae"),
                        Name = "Abu Dhabi",
                        DateTime = DateTime.Parse("2022-11-20T13:00:00"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
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
                    },
                    new Team
                    {
                        Id = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        Name = "AlphaTauri",
                        Color = "#481692",
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Team
                    {
                        Id = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        Name = "Aston Martin",
                        Color = "#22D337",
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Team
                    {
                        Id = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        Name = "Ferrari",
                        Color = "#E10909",
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Team
                    {
                        Id = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        Name = "Williams",
                        Color = "#636CEE",
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Team
                    {
                        Id = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        Name = "Mercedes",
                        Color = "#CBC8D0",
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Team
                    {
                        Id = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        Name = "Red Bull",
                        Color = "#563D7C",
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Team
                    {
                        Id = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        Name = "Haas",
                        Color = "#FFFFFF",
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Team
                    {
                        Id = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        Name = "Alpine",
                        Color = "#3E4BF9",
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Team
                    {
                        Id = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        Name = "Alfa Romeo",
                        Color = "#750000",
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Team
                    {
                        Id = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        Name = "Mclaren",
                        Color = "#C1A806",
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    }
                );

                modelBuilder.Entity<Driver>().HasData(
                    new Driver
                    {
                        Id = new Guid("73fcfbba-4c96-4b37-be28-031cea37fedf"),
                        Name = "Mazepin",
                        RealName = "Nikita Mazepin",
                        NationalityAlpha2 = "ru",
                        Number = 9,
                        ActualTeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("95956026-8b8e-422f-9d9c-0c612fd65286"),
                        Name = "Giovinazzi",
                        RealName = "Antonio Giovinazzi",
                        NationalityAlpha2 = "it",
                        Number = 99,
                        ActualTeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("4abe77d1-5c5c-45de-98b5-0ce706c2a92e"),
                        Name = "Raikkonen",
                        RealName = "Kimi Raikkonen",
                        NationalityAlpha2 = "fi",
                        Number = 7,
                        ActualTeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("66158497-e437-4574-bb13-0ffbb32f5275"),
                        Name = "Hamilton",
                        RealName = "Lewis Hamilton",
                        NationalityAlpha2 = "gb",
                        Number = 44,
                        ActualTeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("d367fa9f-6d62-465b-a79c-1a4d301d4d45"),
                        Name = "Tsunoda",
                        RealName = "Yuki Tsunoda",
                        NationalityAlpha2 = "jp",
                        Number = 22,
                        ActualTeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("ca473f80-146f-4581-95bf-1c725a37771d"),
                        Name = "Alonso",
                        RealName = "Fernando Alonso",
                        NationalityAlpha2 = "es",
                        Number = 14,
                        ActualTeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("492ede9f-9565-49c3-987e-1fa4d6688280"),
                        Name = "Vettel",
                        RealName = "Sebastian Vettel",
                        NationalityAlpha2 = "de",
                        Number = 5,
                        ActualTeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("4bc5f5a6-5d6b-4135-a05f-22f0dc07869a"),
                        Name = "Ocon",
                        RealName = "Esteban Ocon",
                        NationalityAlpha2 = "fr",
                        Number = 31,
                        ActualTeamId = new Guid("ef09fdca-d18a-4b98-b779-02d92e707039"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("06a7cd9f-b418-493e-9639-2e3ab68d05b9"),
                        Name = "Russell",
                        RealName = "George Russell",
                        NationalityAlpha2 = "gb",
                        Number = 63,
                        ActualTeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("5429d0d4-ab87-40a3-98d8-3af65e7af665"),
                        Name = "Sainz",
                        RealName = "Carlos Sainz",
                        NationalityAlpha2 = "es",
                        Number = 55,
                        ActualTeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("6082ad8c-6a85-4d4b-9515-4494c70c095a"),
                        Name = "Latifi",
                        RealName = "Nicholas Latifi",
                        NationalityAlpha2 = "ca",
                        Number = 6,
                        ActualTeamId = new Guid("2323203f-67e8-4565-abad-a06ebe4678ee"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("a0b49580-e960-49bc-8495-46f923a1c648"),
                        Name = "Schumacher",
                        RealName = "Mick Schumacher",
                        NationalityAlpha2 = "de",
                        Number = 47,
                        ActualTeamId = new Guid("e8ef00d1-6e72-4002-9ac5-4047cca447d3"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("5e485bf5-dfba-4adf-b355-7cae28a45b1e"),
                        Name = "Leclerc",
                        RealName = "Charles Leclerc",
                        NationalityAlpha2 = "mc",
                        Number = 16,
                        ActualTeamId = new Guid("fb8316fd-290a-4e5b-8790-8caa0237e633"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("7015da16-b0e1-49c6-a2e7-9f501d936859"),
                        Name = "Ricciardo",
                        RealName = "Daniel Ricciardo",
                        NationalityAlpha2 = "au",
                        Number = 3,
                        ActualTeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("50ac6d2d-55e4-4fd1-8a14-d57d59d5dada"),
                        Name = "Bottas",
                        RealName = "Valtteri Bottas",
                        NationalityAlpha2 = "fi",
                        Number = 77,
                        ActualTeamId = new Guid("1b742c78-01fb-48fd-bc3c-020787206ecc"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("d9f51506-2c85-40a5-b82b-d99ca48fe87f"),
                        Name = "Verstappen",
                        RealName = "Max Verstappen",
                        NationalityAlpha2 = "nl",
                        Number = 33,
                        ActualTeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("08317333-17a8-44bd-8d3d-db80fa4e4ac9"),
                        Name = "Norris",
                        RealName = "Lando Norris",
                        NationalityAlpha2 = "gb",
                        Number = 4,
                        ActualTeamId = new Guid("7c8aeddd-30e8-4ca9-b6fe-cb91ca725bfd"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("14079124-1050-4474-ba27-db9432f6a7c1"),
                        Name = "Gasly",
                        RealName = "Pierre Gasly",
                        NationalityAlpha2 = "fr",
                        Number = 10,
                        ActualTeamId = new Guid("4f1af95f-6cce-41b3-9677-cb411348bd13"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("4b1b6e60-6a50-4d2f-a3d5-dc929a1aa8d1"),
                        Name = "Pérez",
                        RealName = "Sergio Pérez",
                        NationalityAlpha2 = "mx",
                        Number = 11,
                        ActualTeamId = new Guid("25211f1c-b53e-4776-83c1-503ea35764d2"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("2ebc893a-914a-44b9-b420-ea6e3d9bee85"),
                        Name = "Stroll",
                        RealName = "Lance Stroll",
                        NationalityAlpha2 = "ca",
                        Number = 18,
                        ActualTeamId = new Guid("0d7dccbf-0982-4b46-912c-074fa8c0465f"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("8ea53266-9f3b-4d74-99c4-ee1e4c68596e"),
                        Name = "Kubica",
                        RealName = "Robert Kubica",
                        NationalityAlpha2 = "pl",
                        Number = 88,
                        ActualTeamId = new Guid("4e5bf09f-8c78-4fd7-a78e-d851e2e93b35"),
                        SeasonId = new Guid("ef87fc1a-aad7-4835-a80d-25178f418cc1")
                    },
                    new Driver
                    {
                        Id = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        Name = "Schumacher",
                        RealName = "Mick Schumacher",
                        NationalityAlpha2 = "de",
                        Number = 47,
                        ActualTeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        Name = "Vettel",
                        RealName = "Sebastian Vettel",
                        NationalityAlpha2 = "de",
                        Number = 5,
                        ActualTeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        Name = "Latifi",
                        RealName = "Nicholas Latifi",
                        NationalityAlpha2 = "ca",
                        Number = 6,
                        ActualTeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        Name = "Norris",
                        RealName = "Lando Norris",
                        NationalityAlpha2 = "gb",
                        Number = 4,
                        ActualTeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("1a24df46-4adb-4fe4-8685-3e15b87c5706"),
                        Name = "Hülkenberg",
                        RealName = "Nico Hülkenberg",
                        NationalityAlpha2 = "de",
                        Number = 27,
                        ActualTeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        Name = "Gasly",
                        RealName = "Pierre Gasly",
                        NationalityAlpha2 = "fr",
                        Number = 10,
                        ActualTeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        Name = "Ocon",
                        RealName = "Esteban Ocon",
                        NationalityAlpha2 = "fr",
                        Number = 31,
                        ActualTeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        Name = "Verstappen",
                        RealName = "Max Verstappen",
                        NationalityAlpha2 = "nl",
                        Number = 1,
                        ActualTeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        Name = "Bottas",
                        RealName = "Valtteri Bottas",
                        NationalityAlpha2 = "fi",
                        Number = 77,
                        ActualTeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        Name = "Sainz",
                        RealName = "Carlos Sainz",
                        NationalityAlpha2 = "es",
                        Number = 55,
                        ActualTeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        Name = "Stroll",
                        RealName = "Lance Stroll",
                        NationalityAlpha2 = "ca",
                        Number = 18,
                        ActualTeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        Name = "Hamilton",
                        RealName = "Lewis Hamilton",
                        NationalityAlpha2 = "gb",
                        Number = 44,
                        ActualTeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        Name = "Russell",
                        RealName = "George Russell",
                        NationalityAlpha2 = "gb",
                        Number = 63,
                        ActualTeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        Name = "Tsunoda",
                        RealName = "Yuki Tsunoda",
                        NationalityAlpha2 = "jp",
                        Number = 22,
                        ActualTeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("b74e2b6d-5516-4132-b50b-a9ca7ed83502"),
                        Name = "De",
                        RealName = "Nyck De Vries",
                        NationalityAlpha2 = "nl",
                        Number = 45,
                        ActualTeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        Name = "Pérez",
                        RealName = "Sergio Pérez",
                        NationalityAlpha2 = "mx",
                        Number = 11,
                        ActualTeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        Name = "Zhou",
                        RealName = "Guanyu Zhou",
                        NationalityAlpha2 = "cn",
                        Number = 24,
                        ActualTeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        Name = "Alonso",
                        RealName = "Fernando Alonso",
                        NationalityAlpha2 = "es",
                        Number = 14,
                        ActualTeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        Name = "Ricciardo",
                        RealName = "Daniel Ricciardo",
                        NationalityAlpha2 = "au",
                        Number = 3,
                        ActualTeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        Name = "Leclerc",
                        RealName = "Charles Leclerc",
                        NationalityAlpha2 = "mc",
                        Number = 16,
                        ActualTeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        Name = "Albon",
                        RealName = "Alexander Albon",
                        NationalityAlpha2 = "th",
                        Number = 23,
                        ActualTeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
                    },
                    new Driver
                    {
                        Id = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        Name = "Magnussen",
                        RealName = "Kevin Magnussen",
                        NationalityAlpha2 = "dk",
                        Number = 20,
                        ActualTeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        SeasonId = new Guid("a68066e1-ca8a-4e31-9edc-5b7f5687a9ee")
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
                    },
                    new Result
                    {
                        Id = new Guid("5c10305b-aa67-4122-9cb9-00992ebdb950"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083")
                    },
                    new Result
                    {
                        Id = new Guid("a6997dd9-5ccc-4daa-9f77-1afae5c5c534"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 26,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083")
                    },
                    new Result
                    {
                        Id = new Guid("8e205363-db51-493a-81a1-2522820a22a8"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083")
                    },
                    new Result
                    {
                        Id = new Guid("4de09fb2-b489-432f-bb2b-3662bc09b206"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083")
                    },
                    new Result
                    {
                        Id = new Guid("5cc02d1f-fc36-45f5-b100-4d46b9c491f8"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083")
                    },
                    new Result
                    {
                        Id = new Guid("83eea44a-ab0c-4dd6-9e17-50e5997617e4"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083")
                    },
                    new Result
                    {
                        Id = new Guid("0e090908-17cf-4f72-b67d-5ed256dd056d"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083")
                    },
                    new Result
                    {
                        Id = new Guid("963c1ea1-7931-4e66-84be-5ef2fc0d46fa"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083")
                    },
                    new Result
                    {
                        Id = new Guid("43c66ad4-b2bb-4992-865a-64db1c7c5695"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("1a24df46-4adb-4fe4-8685-3e15b87c5706"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083")
                    },
                    new Result
                    {
                        Id = new Guid("8c2bca4a-4cbb-475a-871c-7c6644de3696"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083")
                    },
                    new Result
                    {
                        Id = new Guid("65a03032-e840-436a-a4d3-8bc03c70a634"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083")
                    },
                    new Result
                    {
                        Id = new Guid("3dd6be93-0b93-40e3-9bb7-9bda2c39767a"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083")
                    },
                    new Result
                    {
                        Id = new Guid("bff8dc6e-cf24-42fa-b961-ae655ade6313"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083")
                    },
                    new Result
                    {
                        Id = new Guid("409ffb20-95e3-4abf-85b3-af00cf4d5507"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083")
                    },
                    new Result
                    {
                        Id = new Guid("3577046e-9cee-4b35-90d0-af60f7123cd2"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083")
                    },
                    new Result
                    {
                        Id = new Guid("ec40fd9a-207b-44bd-a24e-b26d86dab320"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083")
                    },
                    new Result
                    {
                        Id = new Guid("b54f566b-2921-467f-b818-d0d0d0bd464a"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083")
                    },
                    new Result
                    {
                        Id = new Guid("3964d147-a436-40e6-a313-d4f95d4fc802"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083")
                    },
                    new Result
                    {
                        Id = new Guid("fe75abda-cd3c-4906-bfb8-e49e8d8c01a8"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083")
                    },
                    new Result
                    {
                        Id = new Guid("bd8435ab-fa95-4733-9a56-e8067b00071b"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("12b4ae87-8798-4d47-b89f-8265a8003083")
                    },
                    new Result
                    {
                        Id = new Guid("d5f3882e-95e7-42f9-97bf-050c69efe444"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712")
                    },
                    new Result
                    {
                        Id = new Guid("250684ab-a9bf-4b3e-8c17-0f97ff6ff3a0"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712")
                    },
                    new Result
                    {
                        Id = new Guid("11e5a706-11c8-44ae-89da-1344abbe1bce"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 19,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712")
                    },
                    new Result
                    {
                        Id = new Guid("b0d81e8c-414a-46cb-8f78-14cef635c16e"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712")
                    },
                    new Result
                    {
                        Id = new Guid("66d6b8ba-894a-4114-aaa8-2aada84d959b"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("1a24df46-4adb-4fe4-8685-3e15b87c5706"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712")
                    },
                    new Result
                    {
                        Id = new Guid("5b128787-1b51-4997-bd28-32e6db9e8ba7"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712")
                    },
                    new Result
                    {
                        Id = new Guid("786de524-7fb0-4038-b57f-379803ad8ab0"),
                        Type = ResultType.DNS,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712")
                    },
                    new Result
                    {
                        Id = new Guid("cc106bbf-00da-4839-b3d8-66285e0950e9"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712")
                    },
                    new Result
                    {
                        Id = new Guid("a43dafcd-9578-4ea3-aa1c-760c5d3c0ad0"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712")
                    },
                    new Result
                    {
                        Id = new Guid("a7f2c379-d1bc-48b7-9d9b-77a422b8744f"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712")
                    },
                    new Result
                    {
                        Id = new Guid("4c5d6931-8eac-4e7e-b2ef-80d9e3bd70f9"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 1,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712")
                    },
                    new Result
                    {
                        Id = new Guid("811dcaee-9f7d-47b7-a46f-891fd6ba7368"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712")
                    },
                    new Result
                    {
                        Id = new Guid("00474a55-2eed-4b8a-a8f5-97edf098daa5"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712")
                    },
                    new Result
                    {
                        Id = new Guid("23afe2b0-fc8b-41f1-98bd-a469f63a056d"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712")
                    },
                    new Result
                    {
                        Id = new Guid("be7e7895-cd39-4fb8-bad0-bdad6dcf6800"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712")
                    },
                    new Result
                    {
                        Id = new Guid("612ae09a-826f-4917-8c8c-be4cbdd82f01"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712")
                    },
                    new Result
                    {
                        Id = new Guid("c9b71f12-5fe0-4d4f-a7fa-cf4982450621"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712")
                    },
                    new Result
                    {
                        Id = new Guid("0927c8a1-a91b-40a8-a175-e357f8a79a85"),
                        Type = ResultType.DNS,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712")
                    },
                    new Result
                    {
                        Id = new Guid("34c958dc-d919-467e-b988-e55ac5438d47"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712")
                    },
                    new Result
                    {
                        Id = new Guid("70c2cce2-8e79-42be-85c9-f10a61c53410"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("3090a6c6-aa9d-468f-acb9-b571c5440712")
                    },
                    new Result
                    {
                        Id = new Guid("23d88ff8-d77f-4dbe-8431-1f07657f775b"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58")
                    },
                    new Result
                    {
                        Id = new Guid("9017ad25-ce81-4960-81ca-2182ecdfaab5"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58")
                    },
                    new Result
                    {
                        Id = new Guid("fb18836f-9170-4ff4-8f7c-23810aa1e369"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58")
                    },
                    new Result
                    {
                        Id = new Guid("1c141c87-2d62-4aed-82e7-23bcb18f6c37"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58")
                    },
                    new Result
                    {
                        Id = new Guid("cb3b576c-8f17-41d0-9f6e-426588452d9e"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58")
                    },
                    new Result
                    {
                        Id = new Guid("934d8394-fdbe-4754-bac3-5424256dbb11"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58")
                    },
                    new Result
                    {
                        Id = new Guid("68055d89-de14-46d6-bd39-5fbb440eb707"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58")
                    },
                    new Result
                    {
                        Id = new Guid("cc9a5aac-3fe9-445a-887f-665e97a37c6d"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58")
                    },
                    new Result
                    {
                        Id = new Guid("38e4cc88-0356-4804-9bf2-6a568552c056"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58")
                    },
                    new Result
                    {
                        Id = new Guid("dc3bd5d5-2fa9-4555-b5e8-70267306aa6c"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 26,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58")
                    },
                    new Result
                    {
                        Id = new Guid("3b96083f-edd7-448d-a14c-723ee94f7d61"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58")
                    },
                    new Result
                    {
                        Id = new Guid("4211c831-b03e-42c6-9a6d-7591efed4efd"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58")
                    },
                    new Result
                    {
                        Id = new Guid("6f1235bf-7bac-4c33-a4ec-7f398d956a3f"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58")
                    },
                    new Result
                    {
                        Id = new Guid("243627d1-5a01-43bf-8c3d-affc94e3dec4"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58")
                    },
                    new Result
                    {
                        Id = new Guid("d978f1d9-d2c5-485d-ac93-b31e43e38e21"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58")
                    },
                    new Result
                    {
                        Id = new Guid("b04e5883-814d-4199-9629-bda4f6ebf470"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58")
                    },
                    new Result
                    {
                        Id = new Guid("aa75fc0e-ee2b-458d-9a74-c5c57872125c"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58")
                    },
                    new Result
                    {
                        Id = new Guid("5dd1ee60-dcf9-4535-94c6-c6a0f78b7360"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58")
                    },
                    new Result
                    {
                        Id = new Guid("a6186ad5-134e-47c9-aa91-d4765c7e4448"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58")
                    },
                    new Result
                    {
                        Id = new Guid("8d83ded8-2ac3-41bc-9f0b-e297dedada95"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("d51d6b37-dcd2-4ee9-b292-d8e0d0588c58")
                    },
                    new Result
                    {
                        Id = new Guid("c92a0d58-8c54-4ead-a91d-06a411c6e1bf"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82")
                    },
                    new Result
                    {
                        Id = new Guid("7dedcc53-03e3-4ffd-9e7c-123d106ceb98"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 6,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82")
                    },
                    new Result
                    {
                        Id = new Guid("f60fcd47-3e69-4a6e-a9f1-1b1d11af8d5f"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 2,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82")
                    },
                    new Result
                    {
                        Id = new Guid("4542b93a-9483-440e-9525-2177b7a9799d"),
                        Type = ResultType.Finished,
                        Position = 19,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82")
                    },
                    new Result
                    {
                        Id = new Guid("11f57fec-2ef0-4327-ab70-2221d2203b2a"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 3,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82")
                    },
                    new Result
                    {
                        Id = new Guid("b410d948-bccc-4632-85de-276d93de3351"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82")
                    },
                    new Result
                    {
                        Id = new Guid("31f29e98-6fc5-4125-ae61-4d830d0f988b"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 4,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82")
                    },
                    new Result
                    {
                        Id = new Guid("5883f213-79b6-4b20-b2b8-60a9cfa6a4ad"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 0,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82")
                    },
                    new Result
                    {
                        Id = new Guid("b3214e8a-c747-4645-827b-65fca84223c7"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 5,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82")
                    },
                    new Result
                    {
                        Id = new Guid("a92c2896-63bd-4d5f-8245-92552a53d42c"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 8,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82")
                    },
                    new Result
                    {
                        Id = new Guid("c5021b4b-6572-4304-b423-9b3f17ca274f"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82")
                    },
                    new Result
                    {
                        Id = new Guid("8c2a517c-000e-4ec4-a63a-a1fbe0e4f12e"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82")
                    },
                    new Result
                    {
                        Id = new Guid("bde9456a-b0ac-4453-8b6f-ae59c49bc8df"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82")
                    },
                    new Result
                    {
                        Id = new Guid("328d7fbb-f49a-4a18-a86f-aee8e67c8344"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 1,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82")
                    },
                    new Result
                    {
                        Id = new Guid("9e8174c0-8ef4-4712-9bf3-b1f5a4081c26"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82")
                    },
                    new Result
                    {
                        Id = new Guid("bfeacab2-7f2f-4e03-8a7e-ba58175ad847"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 7,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82")
                    },
                    new Result
                    {
                        Id = new Guid("d8a1bfd6-36c9-4421-bccd-c83f91d1714c"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82")
                    },
                    new Result
                    {
                        Id = new Guid("b4372790-d58b-4894-a726-d08cfeb2c111"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82")
                    },
                    new Result
                    {
                        Id = new Guid("860037d5-4d3e-48c2-a726-d1cfcf138d70"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82")
                    },
                    new Result
                    {
                        Id = new Guid("4b67e598-3857-4deb-b284-db19b8a967f2"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("0ea38874-b69d-4cc9-8f91-29267d27ec82")
                    },
                    new Result
                    {
                        Id = new Guid("be0a651f-0393-45b7-8a16-3430579a2b07"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa")
                    },
                    new Result
                    {
                        Id = new Guid("ec9293f1-61a5-4156-b9be-3ef3f93c777e"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa")
                    },
                    new Result
                    {
                        Id = new Guid("d729b688-66d2-4905-8bc2-3f021522f1a9"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa")
                    },
                    new Result
                    {
                        Id = new Guid("739c9a98-bb39-4d37-90aa-4de83052c4cc"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa")
                    },
                    new Result
                    {
                        Id = new Guid("c1309824-31e0-4fbd-9218-68e8be02bda3"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa")
                    },
                    new Result
                    {
                        Id = new Guid("ec00ae37-5005-4d0a-bb4e-6fbf67cfa974"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa")
                    },
                    new Result
                    {
                        Id = new Guid("19f7d124-6e6e-442f-9b2d-78a52200602c"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa")
                    },
                    new Result
                    {
                        Id = new Guid("75338704-6d13-47fc-9359-7f293a2781e2"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa")
                    },
                    new Result
                    {
                        Id = new Guid("6e310e71-ebf3-4b86-baa6-98ab387a229d"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa")
                    },
                    new Result
                    {
                        Id = new Guid("b700baf0-adc8-42f4-bd46-b9824ec9c559"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa")
                    },
                    new Result
                    {
                        Id = new Guid("16538db5-4019-4211-b5a8-c35cf3b5adf1"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa")
                    },
                    new Result
                    {
                        Id = new Guid("d501154a-a607-4f95-a733-c3869dfb3669"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa")
                    },
                    new Result
                    {
                        Id = new Guid("015d376e-876f-4886-8c93-c447a250e06a"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa")
                    },
                    new Result
                    {
                        Id = new Guid("ee0ee0d4-e889-4998-a825-c669aaeaed03"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 26,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa")
                    },
                    new Result
                    {
                        Id = new Guid("1df0afea-7152-428e-a802-d073aff1cb4c"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa")
                    },
                    new Result
                    {
                        Id = new Guid("630f2dcb-eb3c-4162-b05a-e05805bc649d"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa")
                    },
                    new Result
                    {
                        Id = new Guid("f97fb9f9-f9fa-4402-b934-e2a463858329"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa")
                    },
                    new Result
                    {
                        Id = new Guid("93a71eb3-7725-4c32-a8d9-e7998c3b0122"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa")
                    },
                    new Result
                    {
                        Id = new Guid("bd92d232-51ec-4214-a5eb-ebbe9b277663"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa")
                    },
                    new Result
                    {
                        Id = new Guid("91c3212a-9401-421f-92d3-f0d0e6bbf4ec"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("ecf46e84-3520-41bf-883e-5b31d68052fa")
                    },
                    new Result
                    {
                        Id = new Guid("ee405307-d009-42e4-941c-070d23b94905"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822")
                    },
                    new Result
                    {
                        Id = new Guid("99d74194-1ef5-42e6-a09a-0ddcc7d739ca"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822")
                    },
                    new Result
                    {
                        Id = new Guid("67390e65-9fe6-4077-9a8f-10e321331e73"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822")
                    },
                    new Result
                    {
                        Id = new Guid("13ec3cb4-6f1b-498d-9549-151a7260b4e5"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 26,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822")
                    },
                    new Result
                    {
                        Id = new Guid("09ad514d-3ff0-4545-8299-182bef14ed04"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822")
                    },
                    new Result
                    {
                        Id = new Guid("58c6f550-1d9a-42b5-8643-48f16486806d"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822")
                    },
                    new Result
                    {
                        Id = new Guid("a999787b-7e1a-495a-a616-65ba29e0b906"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822")
                    },
                    new Result
                    {
                        Id = new Guid("21a9e26b-15c1-4567-8764-7d7bb0ab86cb"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822")
                    },
                    new Result
                    {
                        Id = new Guid("509b7c0a-4e68-4984-8e20-844e3681ff23"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822")
                    },
                    new Result
                    {
                        Id = new Guid("c9e01c36-bf7a-44db-ad85-849c9b7fa91c"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822")
                    },
                    new Result
                    {
                        Id = new Guid("a525d228-0105-40a4-b15b-95c4e47f9a5a"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822")
                    },
                    new Result
                    {
                        Id = new Guid("4142e8fb-57ee-40b1-9483-98f4a64634f3"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822")
                    },
                    new Result
                    {
                        Id = new Guid("5aaa2042-f535-40b5-9ca3-a8411f01d307"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822")
                    },
                    new Result
                    {
                        Id = new Guid("a927a31b-f31a-4b6c-b17f-ad077b77dd56"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822")
                    },
                    new Result
                    {
                        Id = new Guid("6f7c721e-05f5-4815-83ea-b29a231f46ef"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822")
                    },
                    new Result
                    {
                        Id = new Guid("72689a17-23fd-43dd-ad26-d42cac119601"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822")
                    },
                    new Result
                    {
                        Id = new Guid("7180e4e1-266f-4b0a-939c-d8827ac3ac06"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822")
                    },
                    new Result
                    {
                        Id = new Guid("48a23500-badd-4805-a100-e57ae51b9ca4"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822")
                    },
                    new Result
                    {
                        Id = new Guid("aa6fc468-4bd6-4678-8516-f58839d038da"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822")
                    },
                    new Result
                    {
                        Id = new Guid("da9834ce-fa07-4c87-a458-f75ed9398021"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("2c74f398-7d0c-475b-b994-c62f98ac0822")
                    },
                    new Result
                    {
                        Id = new Guid("f5f63226-a817-4dcd-ae61-1703742cd71f"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 19,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450")
                    },
                    new Result
                    {
                        Id = new Guid("5391170f-b1c2-4958-9098-18563759ee2e"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450")
                    },
                    new Result
                    {
                        Id = new Guid("d66b41be-3f11-44f0-ad3e-28dd37611f8d"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450")
                    },
                    new Result
                    {
                        Id = new Guid("9a33e877-8fc4-4ef9-aa12-39cf9a03ae0b"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450")
                    },
                    new Result
                    {
                        Id = new Guid("383049b3-7c5a-46cb-be79-451fe3fafde4"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450")
                    },
                    new Result
                    {
                        Id = new Guid("49ee8c5e-efbc-4993-a6c5-4d24aca87241"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450")
                    },
                    new Result
                    {
                        Id = new Guid("c28e82d6-ddab-42eb-97e1-4d340cedb021"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450")
                    },
                    new Result
                    {
                        Id = new Guid("ea382546-df61-43b3-9c21-572faf282456"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450")
                    },
                    new Result
                    {
                        Id = new Guid("3bdc566f-dd3f-4027-a713-5f4a5f05c8c1"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450")
                    },
                    new Result
                    {
                        Id = new Guid("aa08d360-77f2-423a-8c57-6b94cf5a79e5"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450")
                    },
                    new Result
                    {
                        Id = new Guid("0072683f-7aec-406c-8f62-6f466ab0ca4a"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450")
                    },
                    new Result
                    {
                        Id = new Guid("05877fe2-230a-42df-bcda-71b43cfc03e0"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450")
                    },
                    new Result
                    {
                        Id = new Guid("98fc1bd6-766f-4b1f-8ff7-79f5f2b0e20c"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450")
                    },
                    new Result
                    {
                        Id = new Guid("98dc8530-07ea-482c-9eb8-9e973b7344c8"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450")
                    },
                    new Result
                    {
                        Id = new Guid("8ccda4a2-2dea-4de4-a2a7-a6facf88f290"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450")
                    },
                    new Result
                    {
                        Id = new Guid("3ceee99c-5f9b-4552-b94b-cfe707e9fd39"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450")
                    },
                    new Result
                    {
                        Id = new Guid("db00a445-ff47-4cc3-afb4-d728195ce69b"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450")
                    },
                    new Result
                    {
                        Id = new Guid("ba4035cf-9504-4586-8ca0-d758d7e543a1"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450")
                    },
                    new Result
                    {
                        Id = new Guid("18f87ab0-b0f0-4f3f-a7c6-d7e6427d1d4a"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450")
                    },
                    new Result
                    {
                        Id = new Guid("f27f659b-e716-4327-a28c-dc1c655eb81e"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("fcbc5f86-18e4-4fbf-8720-1c655138d450")
                    },
                    new Result
                    {
                        Id = new Guid("30c19b29-2e11-4302-82be-13c59216a2c7"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1")
                    },
                    new Result
                    {
                        Id = new Guid("70648bc5-e23b-430e-a053-2cce30ef7df6"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1")
                    },
                    new Result
                    {
                        Id = new Guid("ca1c6dac-940b-475b-bac9-449f11e97d04"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1")
                    },
                    new Result
                    {
                        Id = new Guid("9710dbf2-1d19-478f-9a61-59aafccb869b"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1")
                    },
                    new Result
                    {
                        Id = new Guid("a3499f8c-787b-41ea-966d-647ab62ea01a"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1")
                    },
                    new Result
                    {
                        Id = new Guid("d26af6df-e4d4-474b-9a05-654ffca9a03a"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1")
                    },
                    new Result
                    {
                        Id = new Guid("490e4718-905b-4ff5-a02c-6b88e47139cc"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 9,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1")
                    },
                    new Result
                    {
                        Id = new Guid("d2bd759b-d10e-47a9-92d3-7638ad9570e6"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1")
                    },
                    new Result
                    {
                        Id = new Guid("dfbc5df8-a1d9-4977-a363-7e88e12158e7"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1")
                    },
                    new Result
                    {
                        Id = new Guid("30ec562f-27b8-4ec7-8519-904604245bbc"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1")
                    },
                    new Result
                    {
                        Id = new Guid("6080132f-5840-440d-854f-9556f2bef748"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1")
                    },
                    new Result
                    {
                        Id = new Guid("236d7885-a818-487d-b3cc-a4c1a45a96ee"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1")
                    },
                    new Result
                    {
                        Id = new Guid("86f1ac30-b787-4b92-8360-a7a749ca6313"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1")
                    },
                    new Result
                    {
                        Id = new Guid("54db36e6-468c-4577-a50c-c16161af84b0"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1")
                    },
                    new Result
                    {
                        Id = new Guid("5565c84f-3393-43a1-87e2-c3b90e4f3435"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1")
                    },
                    new Result
                    {
                        Id = new Guid("654330f1-9b5b-4626-9c8f-d0a1f9e1f977"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1")
                    },
                    new Result
                    {
                        Id = new Guid("bf984a63-563d-4dc3-9a8a-d5cecb502cec"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1")
                    },
                    new Result
                    {
                        Id = new Guid("832bb117-e939-4a4c-9737-fade4a889a40"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1")
                    },
                    new Result
                    {
                        Id = new Guid("4914afdb-c060-47fd-94c8-fd220fcc01b0"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1")
                    },
                    new Result
                    {
                        Id = new Guid("e77a88ae-1c7e-4230-91ce-ff8d438e02b1"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("f0a50f67-d85e-4525-9b99-5b4aa2ed4fd1")
                    },
                    new Result
                    {
                        Id = new Guid("634c774a-86b0-479e-998d-0a1992350ae8"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("e144d809-4fe8-46c0-89de-3ac6f907c506")
                    },
                    new Result
                    {
                        Id = new Guid("cfe57662-e80e-4be6-abb1-100b48f29d5a"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("e144d809-4fe8-46c0-89de-3ac6f907c506")
                    },
                    new Result
                    {
                        Id = new Guid("457cde65-98e2-4443-804a-13a1019f433a"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("e144d809-4fe8-46c0-89de-3ac6f907c506")
                    },
                    new Result
                    {
                        Id = new Guid("dfb31996-8c96-4795-927c-15f52b7e63f1"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("e144d809-4fe8-46c0-89de-3ac6f907c506")
                    },
                    new Result
                    {
                        Id = new Guid("b63b5810-20f9-4a52-ab78-16b4cb468108"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("e144d809-4fe8-46c0-89de-3ac6f907c506")
                    },
                    new Result
                    {
                        Id = new Guid("a8b45083-2175-4f84-9e04-1bb44703a218"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("e144d809-4fe8-46c0-89de-3ac6f907c506")
                    },
                    new Result
                    {
                        Id = new Guid("35600175-70b6-4bf3-bdbe-1cb9c820ad98"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("e144d809-4fe8-46c0-89de-3ac6f907c506")
                    },
                    new Result
                    {
                        Id = new Guid("5f6d19ec-9c1b-44c5-b5cc-2317f0da5bae"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("e144d809-4fe8-46c0-89de-3ac6f907c506")
                    },
                    new Result
                    {
                        Id = new Guid("a804d630-5706-410a-954e-2a31aa313b02"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("e144d809-4fe8-46c0-89de-3ac6f907c506")
                    },
                    new Result
                    {
                        Id = new Guid("93616780-922d-4974-ba6c-89c6c997303d"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("e144d809-4fe8-46c0-89de-3ac6f907c506")
                    },
                    new Result
                    {
                        Id = new Guid("b0749070-4275-4a43-8b2e-b9bd1c4eb0ae"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("e144d809-4fe8-46c0-89de-3ac6f907c506")
                    },
                    new Result
                    {
                        Id = new Guid("011b8e3d-fba4-4eab-b4b0-bdbb7d6807ce"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("e144d809-4fe8-46c0-89de-3ac6f907c506")
                    },
                    new Result
                    {
                        Id = new Guid("f4c707c8-cf4b-421b-922d-cb5d74aa4740"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("e144d809-4fe8-46c0-89de-3ac6f907c506")
                    },
                    new Result
                    {
                        Id = new Guid("a8ef8ee7-d978-4f45-9299-d684a6779b4d"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("e144d809-4fe8-46c0-89de-3ac6f907c506")
                    },
                    new Result
                    {
                        Id = new Guid("c747765d-347e-4ee0-a2bc-dfe2d4411a95"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("e144d809-4fe8-46c0-89de-3ac6f907c506")
                    },
                    new Result
                    {
                        Id = new Guid("92229a4a-b897-4ded-8504-e395091a4a8b"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("e144d809-4fe8-46c0-89de-3ac6f907c506")
                    },
                    new Result
                    {
                        Id = new Guid("d6cd7f8e-67a2-4953-a6a5-e741da7846f6"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 19,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("e144d809-4fe8-46c0-89de-3ac6f907c506")
                    },
                    new Result
                    {
                        Id = new Guid("5c117094-c08d-48f5-95e5-f391433c4ee9"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("e144d809-4fe8-46c0-89de-3ac6f907c506")
                    },
                    new Result
                    {
                        Id = new Guid("69bc3f4e-3db5-4439-82d8-f57e261b7db5"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("e144d809-4fe8-46c0-89de-3ac6f907c506")
                    },
                    new Result
                    {
                        Id = new Guid("2dfb1fd2-15ad-47a5-8655-1c012cd82b01"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543")
                    },
                    new Result
                    {
                        Id = new Guid("c9f9e295-d3bd-4fb6-b978-1c3fc144abd7"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543")
                    },
                    new Result
                    {
                        Id = new Guid("a46497ff-6bc6-4017-812e-273b9b3cfca5"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543")
                    },
                    new Result
                    {
                        Id = new Guid("ad242320-7c5f-4d88-bb86-4027a9fab984"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543")
                    },
                    new Result
                    {
                        Id = new Guid("96f60e58-4441-4ba1-a7b6-45122b5b178e"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543")
                    },
                    new Result
                    {
                        Id = new Guid("dbc4e82c-65ea-43af-8f24-46ac0b57db21"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543")
                    },
                    new Result
                    {
                        Id = new Guid("7bba75a4-0bae-4db5-be8a-48cda0323fb3"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543")
                    },
                    new Result
                    {
                        Id = new Guid("f498bf87-d241-4932-a5b5-4b6405c5274d"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543")
                    },
                    new Result
                    {
                        Id = new Guid("95de1f54-3610-472c-856f-5be0cecd1d37"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543")
                    },
                    new Result
                    {
                        Id = new Guid("a9ac1ea8-6f30-41ad-a854-77619b28c075"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543")
                    },
                    new Result
                    {
                        Id = new Guid("9005c7a8-a0d6-4fc2-826a-7b2bede97059"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543")
                    },
                    new Result
                    {
                        Id = new Guid("b96806da-c754-42a5-9ffe-997745810344"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 19,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543")
                    },
                    new Result
                    {
                        Id = new Guid("e22f5d21-a52e-465a-909a-a7f28740ca00"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543")
                    },
                    new Result
                    {
                        Id = new Guid("5f53a56e-8ae8-4877-a42d-af07ef36cbb8"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543")
                    },
                    new Result
                    {
                        Id = new Guid("6303816b-f3ad-45b7-8c01-b41081fe326b"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543")
                    },
                    new Result
                    {
                        Id = new Guid("2731e886-212e-4e4d-bf8f-c325a56138e3"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543")
                    },
                    new Result
                    {
                        Id = new Guid("3bbb25d7-7757-4bbb-8424-d7670fac56d0"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543")
                    },
                    new Result
                    {
                        Id = new Guid("be1c8b77-934d-4483-b28e-d8bc2f442f84"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543")
                    },
                    new Result
                    {
                        Id = new Guid("6cf3d9f4-96d7-4e55-99e7-da33190c4514"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543")
                    },
                    new Result
                    {
                        Id = new Guid("a56bdb71-7925-476b-b8f8-f8d3a93b4580"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("333c7116-cc70-43be-9024-5c96f9d8e543")
                    },
                    new Result
                    {
                        Id = new Guid("c812dad0-a6c0-4a2d-92f6-07704739decd"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872")
                    },
                    new Result
                    {
                        Id = new Guid("5a22141c-80f3-44c3-afd5-16f217310b5f"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872")
                    },
                    new Result
                    {
                        Id = new Guid("045a26cb-c52f-4f14-8bfa-19e819da85b2"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872")
                    },
                    new Result
                    {
                        Id = new Guid("a0d25378-b11c-40da-9dbe-1eba21a05a3d"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872")
                    },
                    new Result
                    {
                        Id = new Guid("be8c1837-8f12-4358-b6f7-45806dbf595c"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872")
                    },
                    new Result
                    {
                        Id = new Guid("795a0a23-d6a5-41de-8215-4f035f401a3f"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872")
                    },
                    new Result
                    {
                        Id = new Guid("d3e2a782-81a8-4e23-8736-58e2e57693a7"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872")
                    },
                    new Result
                    {
                        Id = new Guid("a12b1f93-2024-4f7f-a0ad-5e24d408f9e3"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872")
                    },
                    new Result
                    {
                        Id = new Guid("21262cd8-a4c3-4579-bdf1-62270edb02e2"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872")
                    },
                    new Result
                    {
                        Id = new Guid("0821efe1-6291-490e-bcab-6333dc55cae8"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872")
                    },
                    new Result
                    {
                        Id = new Guid("b4942c14-bcff-4e11-b782-71b4e0d505ad"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872")
                    },
                    new Result
                    {
                        Id = new Guid("28cdd403-dc6e-4211-bed6-786f87b84050"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 16,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872")
                    },
                    new Result
                    {
                        Id = new Guid("6398f59c-1c31-4b16-b786-7a3c8d86af41"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872")
                    },
                    new Result
                    {
                        Id = new Guid("b2a0caf6-7784-4968-94ff-83d80193e943"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872")
                    },
                    new Result
                    {
                        Id = new Guid("9e3967c2-1621-49c3-b12b-8564d15dad11"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872")
                    },
                    new Result
                    {
                        Id = new Guid("72ddd297-5220-4919-abfc-a865460e9071"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872")
                    },
                    new Result
                    {
                        Id = new Guid("96c0c344-bf42-4278-b55d-b447e6660efe"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872")
                    },
                    new Result
                    {
                        Id = new Guid("e781c66e-a26c-40da-a7ab-bd059e3cf5b2"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872")
                    },
                    new Result
                    {
                        Id = new Guid("d24a43cc-2567-40bb-9ab9-cbeec76aae47"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872")
                    },
                    new Result
                    {
                        Id = new Guid("2337e26b-907d-435a-85d2-ff2cf85db762"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("6b9a1f8c-3427-4315-aa22-86e17c987872")
                    },
                    new Result
                    {
                        Id = new Guid("1adb1569-3f2a-4676-9157-0454575fb2b3"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("49172116-db53-45e1-b43c-de99be1148c6")
                    },
                    new Result
                    {
                        Id = new Guid("053c0946-c8ba-4737-a980-19868697fe5c"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 4,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("49172116-db53-45e1-b43c-de99be1148c6")
                    },
                    new Result
                    {
                        Id = new Guid("cd903c26-988b-4028-99d8-259613769bb8"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 5,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("49172116-db53-45e1-b43c-de99be1148c6")
                    },
                    new Result
                    {
                        Id = new Guid("f4f7092c-00d1-460b-a435-2c9e533eb95e"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("49172116-db53-45e1-b43c-de99be1148c6")
                    },
                    new Result
                    {
                        Id = new Guid("6382bf39-ceef-4189-971a-2deaf3ac21a2"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 7,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("49172116-db53-45e1-b43c-de99be1148c6")
                    },
                    new Result
                    {
                        Id = new Guid("93b35d76-0897-4ffb-8df9-5b046dd7575b"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("49172116-db53-45e1-b43c-de99be1148c6")
                    },
                    new Result
                    {
                        Id = new Guid("b2ecb94e-ec3f-413a-bd39-660d1df23672"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("49172116-db53-45e1-b43c-de99be1148c6")
                    },
                    new Result
                    {
                        Id = new Guid("54f7efc7-765d-4aa4-83ef-6750893570b9"),
                        Type = ResultType.DNS,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("49172116-db53-45e1-b43c-de99be1148c6")
                    },
                    new Result
                    {
                        Id = new Guid("246d42f4-29aa-420d-955d-6d6f87e92d0f"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("49172116-db53-45e1-b43c-de99be1148c6")
                    },
                    new Result
                    {
                        Id = new Guid("40cd3e3d-1c66-4928-8589-70ed68568205"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("49172116-db53-45e1-b43c-de99be1148c6")
                    },
                    new Result
                    {
                        Id = new Guid("decd9e48-0014-41c3-a8b8-770f9693363a"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 1,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("49172116-db53-45e1-b43c-de99be1148c6")
                    },
                    new Result
                    {
                        Id = new Guid("c5206461-a136-474e-9b56-9b3915d239fd"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("49172116-db53-45e1-b43c-de99be1148c6")
                    },
                    new Result
                    {
                        Id = new Guid("0d108fa3-7739-4942-b677-9f92ab566aa5"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 0,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("49172116-db53-45e1-b43c-de99be1148c6")
                    },
                    new Result
                    {
                        Id = new Guid("5c693ca9-874e-42f0-a245-a72e8b1e5bbf"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 2,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("49172116-db53-45e1-b43c-de99be1148c6")
                    },
                    new Result
                    {
                        Id = new Guid("d0cc57ce-4507-4c9b-b68d-ada32ae5f1ca"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 6,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("49172116-db53-45e1-b43c-de99be1148c6")
                    },
                    new Result
                    {
                        Id = new Guid("c6aa2b46-0825-49b6-9520-b9164d4b9c5c"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 8,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("49172116-db53-45e1-b43c-de99be1148c6")
                    },
                    new Result
                    {
                        Id = new Guid("789e58a4-161c-42f8-83a8-c2d194356ebc"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 3,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("49172116-db53-45e1-b43c-de99be1148c6")
                    },
                    new Result
                    {
                        Id = new Guid("ecb0ea51-2375-41b1-8c86-c6e1aca3be19"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("49172116-db53-45e1-b43c-de99be1148c6")
                    },
                    new Result
                    {
                        Id = new Guid("84f3f2a5-55bf-473a-b0f8-ca367a88bf14"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("49172116-db53-45e1-b43c-de99be1148c6")
                    },
                    new Result
                    {
                        Id = new Guid("e0badbf4-61a1-4dea-bc96-e2f3877450a5"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("49172116-db53-45e1-b43c-de99be1148c6")
                    },
                    new Result
                    {
                        Id = new Guid("c1df4cc8-247c-4158-a291-04035967d848"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b")
                    },
                    new Result
                    {
                        Id = new Guid("c72b2e25-757c-4c53-ac50-0c86f238e171"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b")
                    },
                    new Result
                    {
                        Id = new Guid("a6d21714-3aa7-41d5-8a66-169fa6547688"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b")
                    },
                    new Result
                    {
                        Id = new Guid("38b02f68-6863-40e4-b90d-3cf267cace40"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b")
                    },
                    new Result
                    {
                        Id = new Guid("5701a9d0-3115-428e-9f0b-456ef5707d59"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b")
                    },
                    new Result
                    {
                        Id = new Guid("2a5fd9e1-30ad-4905-a44b-457c51983693"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b")
                    },
                    new Result
                    {
                        Id = new Guid("17e5c9d7-dd8d-4846-b33a-51c2c751c9bd"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b")
                    },
                    new Result
                    {
                        Id = new Guid("5a39fa71-f573-4299-a49e-538124f5e984"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 19,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b")
                    },
                    new Result
                    {
                        Id = new Guid("1cb3b63e-c066-4701-8891-7531c3ad2b98"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b")
                    },
                    new Result
                    {
                        Id = new Guid("051d97a7-f93f-4645-889f-764d5f34fb44"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b")
                    },
                    new Result
                    {
                        Id = new Guid("6c674a60-8b19-4d4b-9cdc-77bce5dd15b9"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b")
                    },
                    new Result
                    {
                        Id = new Guid("fb44fe1c-e809-4ac1-8cac-94b7f2b952c3"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b")
                    },
                    new Result
                    {
                        Id = new Guid("f4bb4b07-ecf5-443f-87a9-9ccc838e1550"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b")
                    },
                    new Result
                    {
                        Id = new Guid("9f686dcd-5628-4739-bb7a-a3fe6b3d5439"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b")
                    },
                    new Result
                    {
                        Id = new Guid("1b05dc93-bac7-4927-aee7-a427ea11a106"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b")
                    },
                    new Result
                    {
                        Id = new Guid("5d6341ec-75f1-4193-a157-eea9262ebf87"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b")
                    },
                    new Result
                    {
                        Id = new Guid("fdf22823-aba2-4d66-b06e-f7dfed45d8f7"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b")
                    },
                    new Result
                    {
                        Id = new Guid("f6e840e5-f19b-4c53-b607-f94456cb5478"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b")
                    },
                    new Result
                    {
                        Id = new Guid("e1c898a6-0d67-4de1-9fe7-f982f396f71b"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b")
                    },
                    new Result
                    {
                        Id = new Guid("a6686b42-cd3d-4290-b958-fe4f24eae74f"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("29a7707c-cb45-4dbd-9379-ca82b8a9fd5b")
                    },
                    new Result
                    {
                        Id = new Guid("e0dd3ac1-b8af-4459-885f-13c1913d09e2"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b")
                    },
                    new Result
                    {
                        Id = new Guid("b45c4a4d-6a71-4900-9130-193bdfafb79b"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b")
                    },
                    new Result
                    {
                        Id = new Guid("6d6bc5c5-7785-4d91-abe0-1e99f693d527"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b")
                    },
                    new Result
                    {
                        Id = new Guid("bba187eb-8266-4886-b583-24e248969079"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b")
                    },
                    new Result
                    {
                        Id = new Guid("99746520-4323-4f99-b093-277f6054c2a5"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b")
                    },
                    new Result
                    {
                        Id = new Guid("9fa355b4-363f-4d65-b326-4473a28a10a1"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b")
                    },
                    new Result
                    {
                        Id = new Guid("21f7cf92-e8e2-4701-8128-655cd6228359"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b")
                    },
                    new Result
                    {
                        Id = new Guid("21f90833-e39b-4383-9b95-6f17683066d6"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b")
                    },
                    new Result
                    {
                        Id = new Guid("dafd3fa5-636e-412b-9b5d-6f69712d9d95"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b")
                    },
                    new Result
                    {
                        Id = new Guid("bdaffc95-8f01-4534-9c49-7731a3a12ba0"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 11,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b")
                    },
                    new Result
                    {
                        Id = new Guid("aa73bc95-1efa-4795-9c84-81750259af76"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b")
                    },
                    new Result
                    {
                        Id = new Guid("9e8b495e-b2a3-4201-b819-9f7e635501a5"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b")
                    },
                    new Result
                    {
                        Id = new Guid("99cde804-3de4-4c26-aed4-bcbf2ca030ce"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b")
                    },
                    new Result
                    {
                        Id = new Guid("78ce52ee-fad6-420e-8d7b-bed5038ef739"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b")
                    },
                    new Result
                    {
                        Id = new Guid("98a77688-a415-4042-acd0-cf2d86a1a6d1"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b")
                    },
                    new Result
                    {
                        Id = new Guid("312cd730-bc79-4abb-8892-d5736f1966c0"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b")
                    },
                    new Result
                    {
                        Id = new Guid("fed20079-fc49-464a-807a-da86e58a58dd"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b")
                    },
                    new Result
                    {
                        Id = new Guid("c825f06e-a63e-45a9-9dc3-ddfad5dd0145"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b")
                    },
                    new Result
                    {
                        Id = new Guid("be665f1b-2ff2-468f-b84b-ec869996f05f"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b")
                    },
                    new Result
                    {
                        Id = new Guid("5584a5e4-980a-44a1-9cc0-f538c5fdbc92"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("a604a7ce-09ae-44ba-acfd-5ef86115549b")
                    },
                    new Result
                    {
                        Id = new Guid("df70b590-397d-45e0-b9a0-0590181c2629"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e")
                    },
                    new Result
                    {
                        Id = new Guid("c598f7f0-80c3-4a30-840f-172c75789606"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e")
                    },
                    new Result
                    {
                        Id = new Guid("a13fff0b-ffc5-4f43-b9ab-1c52c1eeb8c2"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 12,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e")
                    },
                    new Result
                    {
                        Id = new Guid("9726f2ac-5fdf-4938-a5b0-1c8834c0aa25"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e")
                    },
                    new Result
                    {
                        Id = new Guid("b84bd646-9fa5-4a35-8041-2b55fe4c1b23"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e")
                    },
                    new Result
                    {
                        Id = new Guid("1ab2e4fc-8b8b-4f64-8589-3ae9a5eef98b"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e")
                    },
                    new Result
                    {
                        Id = new Guid("48c7d61a-0230-49ed-b665-3bb346cbc163"),
                        Type = ResultType.Finished,
                        Position = 19,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e")
                    },
                    new Result
                    {
                        Id = new Guid("30d4afdb-e73b-4881-a28e-5e5bd16e4bef"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 19,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e")
                    },
                    new Result
                    {
                        Id = new Guid("8e9e1e03-12ba-4f60-a37f-672eca9799c6"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e")
                    },
                    new Result
                    {
                        Id = new Guid("56e9f247-027d-4264-892e-7a475debe087"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e")
                    },
                    new Result
                    {
                        Id = new Guid("753ac925-c777-4fc3-90a5-7e90d97d52b7"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e")
                    },
                    new Result
                    {
                        Id = new Guid("c192d088-2bcf-4cee-83cc-9003d28078ee"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e")
                    },
                    new Result
                    {
                        Id = new Guid("ea6dcdb8-1646-4bd4-919f-92d74250d218"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e")
                    },
                    new Result
                    {
                        Id = new Guid("89b0ee98-0ea7-4576-9565-9c55fc002fc4"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e")
                    },
                    new Result
                    {
                        Id = new Guid("6997cede-414c-44c1-8694-bc92a8c239f2"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e")
                    },
                    new Result
                    {
                        Id = new Guid("a3faa5d8-c7c0-4914-a6a0-cde8d2355cbb"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e")
                    },
                    new Result
                    {
                        Id = new Guid("b2fa681e-e7d4-40ce-a8f7-dc818ae5369e"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e")
                    },
                    new Result
                    {
                        Id = new Guid("d7590fba-846d-4f3f-b103-e361a1abee5a"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e")
                    },
                    new Result
                    {
                        Id = new Guid("a775ad39-aa32-44a7-966d-eb968bc946a9"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e")
                    },
                    new Result
                    {
                        Id = new Guid("23cb4636-3f6d-44cc-b5ef-f736dcb58f5e"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("5aefd64b-0202-4c8b-a086-068458c4c39e")
                    },
                    new Result
                    {
                        Id = new Guid("f8e7b1b8-be82-470e-86cf-184b9b6d84af"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("32b35aff-f243-4b23-b096-cc51938bc523")
                    },
                    new Result
                    {
                        Id = new Guid("36653b67-b726-4149-9462-1e7dfaaa36c8"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("32b35aff-f243-4b23-b096-cc51938bc523")
                    },
                    new Result
                    {
                        Id = new Guid("e51802cf-fbfa-4270-b0e1-2498b3f70d85"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("32b35aff-f243-4b23-b096-cc51938bc523")
                    },
                    new Result
                    {
                        Id = new Guid("0c307429-5750-484d-a788-43794f4d1b46"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("32b35aff-f243-4b23-b096-cc51938bc523")
                    },
                    new Result
                    {
                        Id = new Guid("6dead600-1a1c-4eb6-be26-5bd6648e9d47"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("32b35aff-f243-4b23-b096-cc51938bc523")
                    },
                    new Result
                    {
                        Id = new Guid("d27ea380-548d-4fbc-a8c8-5d46559ecd54"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("32b35aff-f243-4b23-b096-cc51938bc523")
                    },
                    new Result
                    {
                        Id = new Guid("91c3f43d-1d43-44b9-a751-6313af41eee0"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("32b35aff-f243-4b23-b096-cc51938bc523")
                    },
                    new Result
                    {
                        Id = new Guid("2f0ec39a-4177-4a56-9dbe-675e42b3b0e4"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("32b35aff-f243-4b23-b096-cc51938bc523")
                    },
                    new Result
                    {
                        Id = new Guid("951f97ea-4108-4cd0-af8d-6fb5e053c197"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("32b35aff-f243-4b23-b096-cc51938bc523")
                    },
                    new Result
                    {
                        Id = new Guid("f79d589c-8c2a-4a3f-bc36-734f2860778a"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 26,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("32b35aff-f243-4b23-b096-cc51938bc523")
                    },
                    new Result
                    {
                        Id = new Guid("8c9d71a8-d7f8-4cf5-a0ca-971e5f483957"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("32b35aff-f243-4b23-b096-cc51938bc523")
                    },
                    new Result
                    {
                        Id = new Guid("0cfe34be-a0e4-411f-8588-a65866d8c6c8"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("32b35aff-f243-4b23-b096-cc51938bc523")
                    },
                    new Result
                    {
                        Id = new Guid("f715bc99-1254-496c-ae4e-a94b86a05ce1"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("32b35aff-f243-4b23-b096-cc51938bc523")
                    },
                    new Result
                    {
                        Id = new Guid("02ef580e-7825-43ba-91dd-c5e23c4d1e73"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("32b35aff-f243-4b23-b096-cc51938bc523")
                    },
                    new Result
                    {
                        Id = new Guid("c7c64a35-03b1-4ecb-b201-c8db0fec8516"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("32b35aff-f243-4b23-b096-cc51938bc523")
                    },
                    new Result
                    {
                        Id = new Guid("475e32a8-81f8-4df8-80b8-cad96b86dcde"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("32b35aff-f243-4b23-b096-cc51938bc523")
                    },
                    new Result
                    {
                        Id = new Guid("0dceadc8-a6a3-47b3-adab-d23e26bdb7b8"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("32b35aff-f243-4b23-b096-cc51938bc523")
                    },
                    new Result
                    {
                        Id = new Guid("5b7a457d-862d-4dde-af20-e6db6868ae1e"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("32b35aff-f243-4b23-b096-cc51938bc523")
                    },
                    new Result
                    {
                        Id = new Guid("78a70a8f-3cfe-4175-abeb-e8191a06389d"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("32b35aff-f243-4b23-b096-cc51938bc523")
                    },
                    new Result
                    {
                        Id = new Guid("54f77b68-30ce-4abb-885c-f63bd3f99c76"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("32b35aff-f243-4b23-b096-cc51938bc523")
                    },
                    new Result
                    {
                        Id = new Guid("4ca76071-e866-4a89-b7bb-12da9cfbf461"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9")
                    },
                    new Result
                    {
                        Id = new Guid("f7a159d3-0eb1-4e2e-8fcf-27bd4c6252b6"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9")
                    },
                    new Result
                    {
                        Id = new Guid("d4483b7d-7f3e-4ebb-b7eb-2a4069b5c0c0"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9")
                    },
                    new Result
                    {
                        Id = new Guid("a5fe0359-1236-4f1e-adbc-40afe3b27844"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9")
                    },
                    new Result
                    {
                        Id = new Guid("8c589553-860e-4636-87fa-56b2235936eb"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9")
                    },
                    new Result
                    {
                        Id = new Guid("00b44565-93b2-43ea-8594-5c4f82ea8371"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9")
                    },
                    new Result
                    {
                        Id = new Guid("0fe3c79b-f75f-44c9-92a6-6f1b38135e3b"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9")
                    },
                    new Result
                    {
                        Id = new Guid("016a60e6-43c1-488e-9ed1-703dd91c2595"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9")
                    },
                    new Result
                    {
                        Id = new Guid("45a1ce41-f969-483c-b5ce-81bc7e874c9e"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9")
                    },
                    new Result
                    {
                        Id = new Guid("603f67d1-2c9a-4892-9100-8b7955bae6a2"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9")
                    },
                    new Result
                    {
                        Id = new Guid("9a78823d-6486-4bd6-944c-9523e09c24b0"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9")
                    },
                    new Result
                    {
                        Id = new Guid("fa4cd0f0-c461-412b-b3a4-a4c584074877"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9")
                    },
                    new Result
                    {
                        Id = new Guid("3b04f1e1-b5ae-41f6-afe3-aad340817b3a"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9")
                    },
                    new Result
                    {
                        Id = new Guid("e4d62b43-836b-4e9c-b083-ad25a8d014e4"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9")
                    },
                    new Result
                    {
                        Id = new Guid("34f55f4f-0cf6-46a6-8ef3-adc078216e4e"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9")
                    },
                    new Result
                    {
                        Id = new Guid("3a437b0a-8f17-4c99-b960-b16c19b0a937"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9")
                    },
                    new Result
                    {
                        Id = new Guid("a14a8c2d-6771-4619-839f-ba9a62c1da48"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9")
                    },
                    new Result
                    {
                        Id = new Guid("e6ee68a9-6ba2-4c4d-b013-cfd60c67a695"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9")
                    },
                    new Result
                    {
                        Id = new Guid("6bc2e91e-f2af-4dd5-a49d-df3b646d31a3"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9")
                    },
                    new Result
                    {
                        Id = new Guid("9448badc-8aeb-4cce-b8fa-e37edeecbf64"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 26,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("e5cbd8b6-9e6f-4884-8db8-f900385647a9")
                    },
                    new Result
                    {
                        Id = new Guid("7233cb2b-5273-4e6f-986c-06bbfab96c11"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc")
                    },
                    new Result
                    {
                        Id = new Guid("e3c7fd6e-edc9-4d9e-a2f4-2f80848aefc7"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc")
                    },
                    new Result
                    {
                        Id = new Guid("a7b09b81-8e98-465f-b12e-4178155aba80"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc")
                    },
                    new Result
                    {
                        Id = new Guid("bea1efc8-a898-44ff-bd46-4dc4b1db929b"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc")
                    },
                    new Result
                    {
                        Id = new Guid("07f44224-87c6-47c6-a307-4e261fd8026a"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc")
                    },
                    new Result
                    {
                        Id = new Guid("86fe96de-f8ca-4aa7-9c50-676de2a1e20c"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc")
                    },
                    new Result
                    {
                        Id = new Guid("de7d7464-a450-4236-9e48-7343206e0d2f"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 9,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc")
                    },
                    new Result
                    {
                        Id = new Guid("6c58ef9a-f586-4e42-ac25-8130d355989e"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc")
                    },
                    new Result
                    {
                        Id = new Guid("88d803b9-0d89-4436-b098-8b064ac0791b"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("b74e2b6d-5516-4132-b50b-a9ca7ed83502"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc")
                    },
                    new Result
                    {
                        Id = new Guid("f803643e-a685-4c68-a95d-936a75ac0bda"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc")
                    },
                    new Result
                    {
                        Id = new Guid("8e055e43-61c8-458a-82b4-9971a8207991"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc")
                    },
                    new Result
                    {
                        Id = new Guid("55d80a74-7058-4705-94c9-9b5c10ed51ca"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc")
                    },
                    new Result
                    {
                        Id = new Guid("4c4f43ea-4532-42f9-9db9-a25a6970a9f4"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc")
                    },
                    new Result
                    {
                        Id = new Guid("5fe61abc-f01c-4232-8d8a-a27f043c400d"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc")
                    },
                    new Result
                    {
                        Id = new Guid("8f2a484f-fc4e-4614-a1f4-aa7c134468ae"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc")
                    },
                    new Result
                    {
                        Id = new Guid("7f28656d-d43d-4631-a8bf-bfd3d38b4d65"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc")
                    },
                    new Result
                    {
                        Id = new Guid("ecee8888-1bc5-4bc3-8ec9-c18f38027a65"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc")
                    },
                    new Result
                    {
                        Id = new Guid("da8c2a32-065d-4aee-8968-dacc54249091"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc")
                    },
                    new Result
                    {
                        Id = new Guid("cab8cad0-d40a-4163-8b40-e13e85ac39b7"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc")
                    },
                    new Result
                    {
                        Id = new Guid("c25d71e6-56dc-400a-ba65-e44e809e82c6"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("bc27d6df-c975-48c5-be1e-e61b86708cdc")
                    },
                    new Result
                    {
                        Id = new Guid("23db0c2c-24c0-4328-846b-00788ad64311"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb")
                    },
                    new Result
                    {
                        Id = new Guid("dd05a086-9db5-4505-b37d-0336117c801c"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb")
                    },
                    new Result
                    {
                        Id = new Guid("91eae04b-88d5-4e08-aef7-14773cdd1a6a"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb")
                    },
                    new Result
                    {
                        Id = new Guid("927d4412-3276-45af-bdd7-22786477ea5d"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb")
                    },
                    new Result
                    {
                        Id = new Guid("5d0c76eb-4266-4b1a-a448-25c460571a7d"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb")
                    },
                    new Result
                    {
                        Id = new Guid("0da13af6-a567-4a04-90a6-2c8952af183d"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb")
                    },
                    new Result
                    {
                        Id = new Guid("c5487a9b-69f7-4de9-9254-3d902e5189b0"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb")
                    },
                    new Result
                    {
                        Id = new Guid("ccf5151e-12c2-4fbc-807c-3e107feeb4eb"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb")
                    },
                    new Result
                    {
                        Id = new Guid("1c8260f1-be5c-4987-9d24-407dae040871"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb")
                    },
                    new Result
                    {
                        Id = new Guid("2661e5e0-3da8-4522-8e09-51408c89fd3f"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb")
                    },
                    new Result
                    {
                        Id = new Guid("748e0599-3fc6-4be8-afe6-5de47ebc9456"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb")
                    },
                    new Result
                    {
                        Id = new Guid("65b49d7e-0ac0-4363-b53d-5e05e7fad550"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb")
                    },
                    new Result
                    {
                        Id = new Guid("ebef87e4-7b95-4422-8042-60462577be1d"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb")
                    },
                    new Result
                    {
                        Id = new Guid("0d900b18-e64f-4cfe-af7c-6ec87652b2dc"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb")
                    },
                    new Result
                    {
                        Id = new Guid("6d476af4-44fc-4b6d-9af8-840109608134"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb")
                    },
                    new Result
                    {
                        Id = new Guid("3ca6fbcd-34b0-4247-9956-a49411d82edc"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb")
                    },
                    new Result
                    {
                        Id = new Guid("90957e52-6ec0-4bf8-ac41-e3f291a2933a"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb")
                    },
                    new Result
                    {
                        Id = new Guid("c3a3c7ba-8205-41e7-bead-e416cfa2e13e"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb")
                    },
                    new Result
                    {
                        Id = new Guid("efff76f7-3fe4-4888-b128-e6442e62fabd"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb")
                    },
                    new Result
                    {
                        Id = new Guid("f21deb10-ed52-4449-a0df-ec535e42165c"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("62a81cbb-0c53-4125-b786-55a75d9b0ebb")
                    },
                    new Result
                    {
                        Id = new Guid("97af83b7-c548-4c59-a575-01d4e8c249b0"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc")
                    },
                    new Result
                    {
                        Id = new Guid("79ecf959-7ec9-4014-ba9a-05aabdc0f77f"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc")
                    },
                    new Result
                    {
                        Id = new Guid("1d8ecbc9-358d-4fa1-94ca-08ab4b6c016e"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc")
                    },
                    new Result
                    {
                        Id = new Guid("202a1def-cf20-40ed-8c14-0a2aef1daf34"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc")
                    },
                    new Result
                    {
                        Id = new Guid("6ce94e16-60f4-4a70-b5db-17e158dbc06b"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc")
                    },
                    new Result
                    {
                        Id = new Guid("a072709a-c195-4d8e-89e2-3d48f09671a5"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc")
                    },
                    new Result
                    {
                        Id = new Guid("387359b6-5266-4426-b7dd-4ef000a1ea67"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc")
                    },
                    new Result
                    {
                        Id = new Guid("e30a05b9-3a8d-4016-a7b5-63f9a49c401a"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc")
                    },
                    new Result
                    {
                        Id = new Guid("03662ca6-a2e5-4882-b4d7-66052a254019"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc")
                    },
                    new Result
                    {
                        Id = new Guid("5a2b09ff-3cf0-40e7-bdc4-6a608783dbb9"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc")
                    },
                    new Result
                    {
                        Id = new Guid("663ec593-6868-4eb0-bef2-74cd81f9938a"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc")
                    },
                    new Result
                    {
                        Id = new Guid("fc838207-96f0-4bac-a04e-7f0279d45e5c"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc")
                    },
                    new Result
                    {
                        Id = new Guid("18fbdf97-93b0-4ac8-9cf5-a9b1679a2a19"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc")
                    },
                    new Result
                    {
                        Id = new Guid("e98ffdd7-3770-4a77-b594-aabf5223c444"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc")
                    },
                    new Result
                    {
                        Id = new Guid("a0a6dbf2-62e6-4a03-8614-b650c2a5c0ed"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc")
                    },
                    new Result
                    {
                        Id = new Guid("3924cd8c-de53-4024-b4d2-cc241b49b904"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc")
                    },
                    new Result
                    {
                        Id = new Guid("de89b7d5-cfa9-4c99-9053-ce18e6d395be"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc")
                    },
                    new Result
                    {
                        Id = new Guid("f63304bd-b758-4acf-ac60-f737c985f339"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc")
                    },
                    new Result
                    {
                        Id = new Guid("5a619599-5c73-4de7-8a71-fb1191dc8b64"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc")
                    },
                    new Result
                    {
                        Id = new Guid("6be3570c-625a-4a87-be2f-fe4ab3d52f51"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("6f746a32-4ce6-44df-b449-942dcb28cfbc")
                    },
                    new Result
                    {
                        Id = new Guid("a0614544-2e9d-4099-b299-1c320d9e423c"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a")
                    },
                    new Result
                    {
                        Id = new Guid("0f7dfd4c-a3b4-4f6b-971f-2783a1db23df"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a")
                    },
                    new Result
                    {
                        Id = new Guid("829c5baa-f8ce-4f28-bbe7-2884be028743"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a")
                    },
                    new Result
                    {
                        Id = new Guid("97b72df9-500c-4643-abc5-29dcd7913f73"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a")
                    },
                    new Result
                    {
                        Id = new Guid("d7edecdb-c6be-4074-a2be-2d5d288b0778"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a")
                    },
                    new Result
                    {
                        Id = new Guid("66468a0d-b3b6-4474-8170-300068a6df0c"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a")
                    },
                    new Result
                    {
                        Id = new Guid("d5dfe3a6-3763-4fde-a253-3479d4c75f9a"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a")
                    },
                    new Result
                    {
                        Id = new Guid("b3b747cc-c51c-4214-9d6a-3a902c6653a1"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a")
                    },
                    new Result
                    {
                        Id = new Guid("6c5edfff-b032-43e5-93cd-4c0eca658cb8"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a")
                    },
                    new Result
                    {
                        Id = new Guid("43b6da6a-00fb-437e-a935-511b6165b6e2"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a")
                    },
                    new Result
                    {
                        Id = new Guid("5d798945-f10d-4764-8ef6-662cb79f7d1c"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a")
                    },
                    new Result
                    {
                        Id = new Guid("f40aa013-301d-4d4b-b098-959fd3c3648f"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a")
                    },
                    new Result
                    {
                        Id = new Guid("a7e16bd5-2340-4cf2-909f-988ae3264336"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 11,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a")
                    },
                    new Result
                    {
                        Id = new Guid("35808847-7f70-4d59-aa4f-abe00119d7ff"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a")
                    },
                    new Result
                    {
                        Id = new Guid("10590e7e-8842-450f-b681-b6950690283d"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a")
                    },
                    new Result
                    {
                        Id = new Guid("d4220e6f-29ce-4c84-8e76-c22fa0b52a18"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a")
                    },
                    new Result
                    {
                        Id = new Guid("b9b83910-449b-4609-9aa6-eaf1b86f8bdb"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a")
                    },
                    new Result
                    {
                        Id = new Guid("fc33bcc8-3a87-4fab-9621-f8c2f37f42d2"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a")
                    },
                    new Result
                    {
                        Id = new Guid("db549f1c-38d1-4c6f-b15e-fd4cdfd06eb0"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a")
                    },
                    new Result
                    {
                        Id = new Guid("0a083915-e2db-48a9-a6be-ff87a0309f70"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("d52bd8f4-9e77-4102-890d-a6b1e858913a")
                    },
                    new Result
                    {
                        Id = new Guid("2b067744-8334-4373-9548-0b4f018135ce"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43")
                    },
                    new Result
                    {
                        Id = new Guid("4a78ee0b-191e-4dc2-a9cd-12f66882db59"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43")
                    },
                    new Result
                    {
                        Id = new Guid("50417925-7b0e-4594-860e-1ea2490eb35a"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43")
                    },
                    new Result
                    {
                        Id = new Guid("8348aea3-6143-496a-aa38-3bd78355e6fa"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43")
                    },
                    new Result
                    {
                        Id = new Guid("17e575fd-d8e7-45c2-8c29-47e5805542da"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43")
                    },
                    new Result
                    {
                        Id = new Guid("676a5ea3-8692-45c2-b977-546834e4158e"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43")
                    },
                    new Result
                    {
                        Id = new Guid("b498fca5-9891-4366-b95d-57af0a0aa478"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43")
                    },
                    new Result
                    {
                        Id = new Guid("29545309-23f9-485f-93f4-5c2e682e470c"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43")
                    },
                    new Result
                    {
                        Id = new Guid("d31de712-a5a6-43a7-9549-6c7be9f4c644"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43")
                    },
                    new Result
                    {
                        Id = new Guid("9c18b249-5e20-411f-aac4-79cbcb62ba98"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43")
                    },
                    new Result
                    {
                        Id = new Guid("576b4ed9-aa2e-451f-861f-95f694dc0df3"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43")
                    },
                    new Result
                    {
                        Id = new Guid("f60b295a-e451-402e-8567-97661dbf3b87"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 13,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43")
                    },
                    new Result
                    {
                        Id = new Guid("a0f2951f-f662-438c-91dc-aa9c8fe8ae85"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43")
                    },
                    new Result
                    {
                        Id = new Guid("e31ab9b1-bed9-446d-932f-bb23fe64dbf4"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43")
                    },
                    new Result
                    {
                        Id = new Guid("99fc3818-7f36-4441-bd81-c3e49c89f7a7"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43")
                    },
                    new Result
                    {
                        Id = new Guid("d7553215-e939-4213-af94-ccacd45a8a49"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43")
                    },
                    new Result
                    {
                        Id = new Guid("ca662168-1da6-4261-beea-ddb665533de3"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43")
                    },
                    new Result
                    {
                        Id = new Guid("e938d11c-8b11-4330-aa3e-e01b2d5a8ec0"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43")
                    },
                    new Result
                    {
                        Id = new Guid("3eac33fc-e920-4497-b593-edb35be5d767"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43")
                    },
                    new Result
                    {
                        Id = new Guid("727b575c-dabc-4600-a92a-f38c8cfc6b40"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("224244b5-bec3-480e-bb0a-be2ee050de43")
                    },
                    new Result
                    {
                        Id = new Guid("0d2d0f72-620a-4332-baef-05ecf7b590a2"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 8,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("e39be72c-720c-4679-a260-7346d05fce99")
                    },
                    new Result
                    {
                        Id = new Guid("2ecc8f9d-c8ab-4df0-9e2d-0c906367355c"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 1,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("e39be72c-720c-4679-a260-7346d05fce99")
                    },
                    new Result
                    {
                        Id = new Guid("614f2084-4000-43fa-a5d5-0ce5dd05d8e2"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("e39be72c-720c-4679-a260-7346d05fce99")
                    },
                    new Result
                    {
                        Id = new Guid("82bd751d-577a-46b5-9682-2ffcc668079c"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("e39be72c-720c-4679-a260-7346d05fce99")
                    },
                    new Result
                    {
                        Id = new Guid("01b19aee-0263-41f3-af2e-4c58052eca02"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 7,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("e39be72c-720c-4679-a260-7346d05fce99")
                    },
                    new Result
                    {
                        Id = new Guid("269f9c13-4191-4a1f-bd9e-4d140c011dac"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("e39be72c-720c-4679-a260-7346d05fce99")
                    },
                    new Result
                    {
                        Id = new Guid("147624de-4e47-4e1a-86fb-532eae77480b"),
                        Type = ResultType.Finished,
                        Position = 19,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("e39be72c-720c-4679-a260-7346d05fce99")
                    },
                    new Result
                    {
                        Id = new Guid("2863ebdb-e30d-4eb9-8908-6a239398ec50"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("e39be72c-720c-4679-a260-7346d05fce99")
                    },
                    new Result
                    {
                        Id = new Guid("893cf2e7-f3ba-4197-8aa5-75e0c19a3919"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("e39be72c-720c-4679-a260-7346d05fce99")
                    },
                    new Result
                    {
                        Id = new Guid("09155b4d-af02-44ca-8189-7ac79d4ef3b4"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 5,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("e39be72c-720c-4679-a260-7346d05fce99")
                    },
                    new Result
                    {
                        Id = new Guid("7b0c5369-25d8-4e37-8bfe-81215254a98f"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 2,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("e39be72c-720c-4679-a260-7346d05fce99")
                    },
                    new Result
                    {
                        Id = new Guid("e2bbe7ec-89d5-42de-b76a-962f03f15256"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 4,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("e39be72c-720c-4679-a260-7346d05fce99")
                    },
                    new Result
                    {
                        Id = new Guid("425ae2ed-bebb-4d4f-81b4-99580ca63a63"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 0,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("e39be72c-720c-4679-a260-7346d05fce99")
                    },
                    new Result
                    {
                        Id = new Guid("a022aea6-d422-4a5b-84ae-9a9ee7441bc7"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 6,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("e39be72c-720c-4679-a260-7346d05fce99")
                    },
                    new Result
                    {
                        Id = new Guid("4b6c2473-81d3-4fd6-846c-9ca53af69b71"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("e39be72c-720c-4679-a260-7346d05fce99")
                    },
                    new Result
                    {
                        Id = new Guid("041e89e2-26f4-4fe8-b70c-ace8c4177c01"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("e39be72c-720c-4679-a260-7346d05fce99")
                    },
                    new Result
                    {
                        Id = new Guid("ec44a83b-1572-49f8-a8e5-b6d8a07b4b08"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 3,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("e39be72c-720c-4679-a260-7346d05fce99")
                    },
                    new Result
                    {
                        Id = new Guid("1fb51952-2560-4f1d-82cf-c26311a975ed"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("e39be72c-720c-4679-a260-7346d05fce99")
                    },
                    new Result
                    {
                        Id = new Guid("14c34abc-fc58-4b30-a5f5-e2add1c79ec5"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("e39be72c-720c-4679-a260-7346d05fce99")
                    },
                    new Result
                    {
                        Id = new Guid("3a0a2650-b16c-4a72-b009-f0ad13527a3d"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 0,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("e39be72c-720c-4679-a260-7346d05fce99")
                    },
                    new Result
                    {
                        Id = new Guid("e7b73f2e-d39c-4502-87f5-0188375b87cd"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2")
                    },
                    new Result
                    {
                        Id = new Guid("72bc06d1-08e8-4e92-b833-0b9829a75724"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2")
                    },
                    new Result
                    {
                        Id = new Guid("cc7239be-2e55-4297-831a-4a336795a72c"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2")
                    },
                    new Result
                    {
                        Id = new Guid("66ff9645-743d-4883-bbb5-4aae82401b1e"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2")
                    },
                    new Result
                    {
                        Id = new Guid("9ffce53d-0154-46da-a854-4c6fc843a25d"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2")
                    },
                    new Result
                    {
                        Id = new Guid("515a6ec4-28de-41c7-9657-543d21f007d8"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2")
                    },
                    new Result
                    {
                        Id = new Guid("265530ba-70d1-4ac9-8f1e-60d8595225f5"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2")
                    },
                    new Result
                    {
                        Id = new Guid("0c3334ab-06fa-4987-907a-625330ea8818"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2")
                    },
                    new Result
                    {
                        Id = new Guid("b477039b-745e-4f4e-8c5b-63741d603769"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 26,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2")
                    },
                    new Result
                    {
                        Id = new Guid("02097401-85c4-4e7d-9f7c-64e08b5b4c09"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2")
                    },
                    new Result
                    {
                        Id = new Guid("23d0f740-7284-4d58-aba8-72890250737e"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2")
                    },
                    new Result
                    {
                        Id = new Guid("987fc4ab-ccc5-4efd-88d3-72aa9b174f46"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2")
                    },
                    new Result
                    {
                        Id = new Guid("ec9fd1ef-37c6-4553-955c-778542c4c89f"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2")
                    },
                    new Result
                    {
                        Id = new Guid("25b21c24-a7a6-4152-b239-787cbda11206"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 8,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2")
                    },
                    new Result
                    {
                        Id = new Guid("7e9eb98c-5ad0-449d-9aed-8d8fa07477a6"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2")
                    },
                    new Result
                    {
                        Id = new Guid("be98eb4a-bcef-4abe-9a6a-8ff815e81fb5"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2")
                    },
                    new Result
                    {
                        Id = new Guid("c3f987b5-084c-43df-87cf-a867c3bf8098"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2")
                    },
                    new Result
                    {
                        Id = new Guid("c6c3cdfe-1daf-4cb2-bb1a-c848e5d0809d"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2")
                    },
                    new Result
                    {
                        Id = new Guid("cb45bfbe-3b26-4233-8bd9-d3c3410402a9"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2")
                    },
                    new Result
                    {
                        Id = new Guid("854cd533-04be-4ff6-aae9-e135d333f6f9"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("4f180f36-6694-45d8-a9ea-798ea87c65e2")
                    },
                    new Result
                    {
                        Id = new Guid("45915a6d-2bfc-4553-b2b0-06b28677117d"),
                        Type = ResultType.Finished,
                        Position = 9,
                        Point = 2,
                        DriverId = new Guid("e09b47a3-1d81-498e-ae41-e6a68ab6aa6b"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae")
                    },
                    new Result
                    {
                        Id = new Guid("c7c8314c-bd95-4f1c-ba29-16bc18fd5e37"),
                        Type = ResultType.Finished,
                        Position = 7,
                        Point = 6,
                        DriverId = new Guid("791a7994-dce3-4684-a431-5676863ae6ce"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae")
                    },
                    new Result
                    {
                        Id = new Guid("0fb9c144-8e37-4f9f-8799-25efd354dcda"),
                        Type = ResultType.Finished,
                        Position = 12,
                        Point = 0,
                        DriverId = new Guid("fcb20019-28da-4ac6-b48c-ce0483228173"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae")
                    },
                    new Result
                    {
                        Id = new Guid("1537d152-3d53-47f9-8b13-3dd416b9d70c"),
                        Type = ResultType.Finished,
                        Position = 11,
                        Point = 0,
                        DriverId = new Guid("94bd0be9-8ca1-4bf7-9488-98f87e16ab9a"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae")
                    },
                    new Result
                    {
                        Id = new Guid("6e940d11-000d-4705-9917-4b8eb9341e99"),
                        Type = ResultType.Finished,
                        Position = 6,
                        Point = 9,
                        DriverId = new Guid("8fc76328-637e-4406-8e39-3db570657514"),
                        TeamId = new Guid("94c5e0fb-96c2-4098-b702-e4c8a02367bc"),
                        RaceId = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae")
                    },
                    new Result
                    {
                        Id = new Guid("cb36ee54-bff1-4243-8101-4c95a1128697"),
                        Type = ResultType.Finished,
                        Position = 13,
                        Point = 0,
                        DriverId = new Guid("d7f48c1f-5173-4db3-bda7-ee43c0a4ceb5"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae")
                    },
                    new Result
                    {
                        Id = new Guid("9ba6dc30-83a8-4b64-a2e7-577182b34ca6"),
                        Type = ResultType.Finished,
                        Position = 8,
                        Point = 4,
                        DriverId = new Guid("0f781624-51a2-41e4-9ad1-6f1032d1c225"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae")
                    },
                    new Result
                    {
                        Id = new Guid("71046775-8dc0-4625-a29a-60a1f20574a2"),
                        Type = ResultType.DNF,
                        Position = null,
                        Point = 0,
                        DriverId = new Guid("63d1195e-0f4f-4ef4-aaa5-de97ade2bc8e"),
                        TeamId = new Guid("f4db90ae-5e32-4b56-8fc5-d82f0c13c5a4"),
                        RaceId = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae")
                    },
                    new Result
                    {
                        Id = new Guid("0331261e-86d7-47cb-a9a9-62733ea6280a"),
                        Type = ResultType.Finished,
                        Position = 5,
                        Point = 10,
                        DriverId = new Guid("6d8620df-975d-43dd-a1be-9309b4ebb01d"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae")
                    },
                    new Result
                    {
                        Id = new Guid("cc373cbf-9d6b-4dbf-ba24-7c92ba1af2eb"),
                        Type = ResultType.Finished,
                        Position = 15,
                        Point = 0,
                        DriverId = new Guid("b427561f-7035-4b63-a2e1-5830837c8df7"),
                        TeamId = new Guid("f0e181bc-b2a7-4637-808e-dc8463ec6859"),
                        RaceId = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae")
                    },
                    new Result
                    {
                        Id = new Guid("7a8d9b2e-6926-4103-b98d-842f5b5d73fb"),
                        Type = ResultType.Finished,
                        Position = 4,
                        Point = 12,
                        DriverId = new Guid("015e876d-49e1-4c32-82a9-6ebc72931562"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae")
                    },
                    new Result
                    {
                        Id = new Guid("9478e450-5849-491c-b91b-9eedadb875b6"),
                        Type = ResultType.Finished,
                        Position = 18,
                        Point = 0,
                        DriverId = new Guid("1e26e2e4-3b8b-4b9e-a73f-741689d981fa"),
                        TeamId = new Guid("76ca6f7d-3d6c-43f9-b68d-524c96611ff4"),
                        RaceId = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae")
                    },
                    new Result
                    {
                        Id = new Guid("62687b4b-cbdc-41f0-b54d-a63d297fea8c"),
                        Type = ResultType.Finished,
                        Position = 14,
                        Point = 0,
                        DriverId = new Guid("844281ac-4a62-49e5-a4e6-4e74bb00eba4"),
                        TeamId = new Guid("c70636d0-61e1-4642-981f-0d840ea47dfe"),
                        RaceId = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae")
                    },
                    new Result
                    {
                        Id = new Guid("1a21ce11-8cd2-487c-8ea0-a9dfc6f6713d"),
                        Type = ResultType.Finished,
                        Position = 16,
                        Point = 0,
                        DriverId = new Guid("c6e111b8-41fa-4073-bbae-10d40e063ee7"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae")
                    },
                    new Result
                    {
                        Id = new Guid("966fe32c-56d1-44a8-8e7e-c68d3d787924"),
                        Type = ResultType.Finished,
                        Position = 3,
                        Point = 15,
                        DriverId = new Guid("e1bcc2a7-9d6b-4261-a324-c2ec78250b3d"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae")
                    },
                    new Result
                    {
                        Id = new Guid("274e1de1-39ef-4c9f-a4a3-d039fb965213"),
                        Type = ResultType.Finished,
                        Position = 1,
                        Point = 25,
                        DriverId = new Guid("d5b558b5-2b9e-4264-86ee-57cef4534ddf"),
                        TeamId = new Guid("792d6d4c-cc3b-491b-9535-55757aa1ac5f"),
                        RaceId = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae")
                    },
                    new Result
                    {
                        Id = new Guid("ae916e88-1043-45da-96ea-d077c95426e6"),
                        Type = ResultType.Finished,
                        Position = 10,
                        Point = 1,
                        DriverId = new Guid("dbc2b7ba-4e2c-4892-b817-1c74a31f41a1"),
                        TeamId = new Guid("d31332b6-903a-410c-9f95-0fb2e6f3a9b7"),
                        RaceId = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae")
                    },
                    new Result
                    {
                        Id = new Guid("ca07c125-6f14-462a-98e3-d6037f25e036"),
                        Type = ResultType.Finished,
                        Position = 2,
                        Point = 18,
                        DriverId = new Guid("3f59af4b-3b68-4334-9989-ebb56b0d4dfc"),
                        TeamId = new Guid("6c678063-11c6-4a23-9db0-2705a1691731"),
                        RaceId = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae")
                    },
                    new Result
                    {
                        Id = new Guid("bd106f46-1c6b-49c4-a9f7-ee11a1e91dcb"),
                        Type = ResultType.Finished,
                        Position = 19,
                        Point = 0,
                        DriverId = new Guid("26d745a0-4db1-418e-8919-3ae89b8a1b5c"),
                        TeamId = new Guid("0d96385c-b302-4ad8-b220-2f3265139032"),
                        RaceId = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae")
                    },
                    new Result
                    {
                        Id = new Guid("a3623d72-9b8d-4cd7-9b72-fc2a141e2b4f"),
                        Type = ResultType.Finished,
                        Position = 17,
                        Point = 0,
                        DriverId = new Guid("8a1831a5-7cb5-4acd-aaf4-f3ca7d6bdbb2"),
                        TeamId = new Guid("d44544d8-b4a9-4ca8-9dd5-b6e92f25ed88"),
                        RaceId = new Guid("a839b7a9-8bc6-4aad-aa79-c8e1b17778ae")
                    }
                );
            }
        }
    }
}
