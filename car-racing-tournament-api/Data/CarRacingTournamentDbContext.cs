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
        public virtual DbSet<UserSeason> UserSeasons { get; set; } = default!;

        public CarRacingTournamentDbContext() { }

        public CarRacingTournamentDbContext(DbContextOptions<CarRacingTournamentDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username)
                    .IsRequired();
                entity.Property(e => e.Email)
                    .IsRequired();
                entity.Property(e => e.Password)
                    .IsRequired();
                entity.HasMany(e => e.UserSeasons)
                    .WithOne(e => e.User);
            });

            modelBuilder.Entity<UserSeason>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Permission)
                    .IsRequired();
            });

            modelBuilder.Entity<Season>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired();
                entity.Property(e => e.Description);

                entity.HasMany(e => e.UserSeasons)
                    .WithOne(e => e.Season)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Teams)
                .WithOne(e => e.Season)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Drivers)
                    .WithOne(e => e.Season)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Races)
                    .WithOne(e => e.Season)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired();
                entity.Property(e => e.Color)
                    .IsRequired();

                entity.HasMany(e => e.Drivers)
                    .WithOne(e => e.ActualTeam)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(e => e.Results)
                    .WithOne(e => e.Team)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Driver>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired();
                entity.Property(e => e.RealName);
                entity.Property(e => e.Number)
                    .IsRequired();

                entity.HasMany(e => e.Results)
                    .WithOne(e => e.Driver)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Race>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired();
                entity.Property(e => e.DateTime);

                entity.HasMany(e => e.Results)
                    .WithOne(e => e.Race)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Result>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Position)
                    .IsRequired();
                entity.Property(e => e.Points)
                    .IsRequired();
            });
        }
    }
}
