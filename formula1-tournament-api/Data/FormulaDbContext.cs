using formula1_tournament_api.Models;
using Microsoft.EntityFrameworkCore;

namespace formula1_tournament_api.Data
{
    public class FormulaDbContext : DbContext
    {
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Season> Season { get; set; }
        public virtual DbSet<Team> Team { get; set; }
        public virtual DbSet<Racer> Racer { get; set; }
        public virtual DbSet<Race> Race { get; set; }

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
                entity.Property(e => e.Password)
                    .IsRequired();
            });

            modelBuilder.Entity<Season>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired();
                entity.Property(e => e.UserId)
                    .IsRequired();
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired();
                entity.Property(e => e.Color)
                    .IsRequired();
                entity.Property(e => e.SeasonId)
                    .IsRequired();
            });

            modelBuilder.Entity<Racer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired();
                entity.Property(e => e.TeamId)
                    .IsRequired();
                entity.Property(e => e.SeasonId)
                    .IsRequired();
            });

            modelBuilder.Entity<Race>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Position)
                    .IsRequired();
                entity.Property(e => e.Points)
                    .IsRequired();
                entity.Property(e => e.RacerId)
                    .IsRequired();
                entity.Property(e => e.RacerId)
                    .IsRequired();
            });
        }
    }
}
