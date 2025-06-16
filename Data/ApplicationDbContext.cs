using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Supabase.Gotrue;
using yummealAPI.Models.Entities;

namespace yummealAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public DbSet<UserPreferences> UserPreferences { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserProfile>(entity =>
            {
                // Klucz główny
                entity.HasIndex(e => e.UserId).IsUnique();

                entity.Property(e => e.CurrentWeight).HasPrecision(5, 2);

                entity.Property(e => e.TargetWeight).HasPrecision(5, 2);

                entity.Property(e => e.BMR).HasPrecision(7, 2);

                entity.Property(e => e.TDEE).HasPrecision(7, 2);

                entity.Property(e => e.DailyProteinGoal).HasPrecision(6, 2);

                entity.Property(e => e.DailyCarbsGoal).HasPrecision(6, 2);

                entity.Property(e => e.DailyFatGoal).HasPrecision(6, 2);
            });

            modelBuilder.Entity<UserPreferences>(entity =>
            {
                entity
                    .HasOne(e => e.UserProfile)
                    .WithOne(e => e.Preferences)
                    .HasForeignKey<UserPreferences>(e => e.UserProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.MelsPerDay).HasDefaultValue(3);

                entity.Property(e => e.PlanningDays).HasDefaultValue(7);

                entity.Property(e => e.MaxCookingTime).HasDefaultValue(60);
                entity.Property(e => e.MaxCookingTime).HasDefaultValue(60);
            });
        }
    }
}
