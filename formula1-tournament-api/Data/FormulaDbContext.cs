using formula1_tournament_api.Models;
using Microsoft.EntityFrameworkCore;

namespace formula1_tournament_api.Data
{
    public class FormulaDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Season> Seasons { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<Driver> Drivers { get; set; }
        public virtual DbSet<Race> Races { get; set; }
        public virtual DbSet<Result> Results { get; set; }
        public virtual DbSet<UserSeason> UserSeasons { get; set; }

        public FormulaDbContext() { }

        public FormulaDbContext(DbContextOptions<FormulaDbContext> options) : base(options)
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
            });

            modelBuilder.Entity<UserSeason>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Permission)
                    .IsRequired();
                entity.HasOne(e => e.User)
                    .WithMany(e => e.UserSeasons)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .IsRequired();

                entity.HasOne(e => e.Season)
                    .WithMany(e => e.UserSeasons)
                    .HasForeignKey(e => e.SeasonId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .IsRequired();
            });

            modelBuilder.Entity<Season>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
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
                    .OnDelete(DeleteBehavior.ClientCascade)
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
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .IsRequired();
                entity.HasOne(e => e.Season)
                    .WithMany(e => e.Drivers)
                    .HasForeignKey(e => e.SeasonId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .IsRequired();
            });

            modelBuilder.Entity<Race>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired();
                entity.Property(e => e.DateTime)
                    .IsRequired();
                entity.HasOne(e => e.Season)
                    .WithMany(e => e.Races)
                    .HasForeignKey(e => e.SeasonId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .IsRequired();
            });

            modelBuilder.Entity<Result>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Position)
                    .IsRequired();
                entity.Property(e => e.Points)
                    .IsRequired();
                entity.HasOne(e => e.Driver)
                    .WithMany(e => e.Results)
                    .HasForeignKey(e => e.DriverId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .IsRequired();
                entity.HasOne(e => e.Team)
                    .WithMany(e => e.Results)
                    .HasForeignKey(e => e.TeamId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .IsRequired();
                entity.HasOne(e => e.Race)
                    .WithMany(e => e.Results)
                    .HasForeignKey(e => e.RaceId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .IsRequired();
            });
        }
    }
}
