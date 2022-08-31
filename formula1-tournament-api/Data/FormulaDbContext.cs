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
                entity.HasMany(e => e.Seasons)
                    .WithMany(e => e.Users)
                    .UsingEntity<UserSeason>(
                        e => e.HasOne(us => us.Season)
                            .WithMany(s => s.UserSeasons)
                            .HasForeignKey(us => us.SeasonId),
                        e => e.HasOne(us => us.User)
                            .WithMany(u => u.UserSeasons)
                            .HasForeignKey(us => us.UserId),
                        e => e.Property(e => e.Permission)
                            .IsRequired()
                    );
            });

            modelBuilder.Entity<Season>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired();
                /*entity.HasOne(e => e.User)
                    .WithMany(e => e.Seasons)
                    .HasForeignKey(e => e.UserId)
                    .IsRequired();*/
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
                    .IsRequired();
            });

            modelBuilder.Entity<Driver>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired();
                entity.HasOne(e => e.Team)
                    .WithMany(e => e.Drivers)
                    .HasForeignKey(e => e.TeamId)
                    .IsRequired();
                entity.HasOne(e => e.Season)
                    .WithMany(e => e.Drivers)
                    .HasForeignKey(e => e.SeasonId)
                    .IsRequired();
            });

            modelBuilder.Entity<Race>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Position)
                    .IsRequired();
                entity.Property(e => e.Points)
                    .IsRequired();
                entity.HasOne(e => e.Driver)
                    .WithMany(e => e.Races)
                    .HasForeignKey(e => e.DriverId)
                    .IsRequired();
                entity.HasOne(e => e.Season)
                    .WithMany(e => e.Races)
                    .HasForeignKey(e => e.SeasonId)
                    .IsRequired();
            });
        }
    }
}
