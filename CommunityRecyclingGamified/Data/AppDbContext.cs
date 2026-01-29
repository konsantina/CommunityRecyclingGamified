using CommunityRecyclingGamified.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityRecyclingGamified.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<Material> Materials => Set<Material>();
        public DbSet<Dropoff> Dropoffs => Set<Dropoff>();
        public DbSet<Badge> Badges => Set<Badge>();
        public DbSet<UserBadge> UserBadges => Set<UserBadge>();
        public DbSet<Reward> Rewards => Set<Reward>();
        public DbSet<Redemption> Redemptions => Set<Redemption>();
        public DbSet<UserPointLedger> UserPointLedgers => Set<UserPointLedger>();
        public DbSet<Neighborhood> Neighborhoods => Set<Neighborhood>();
        public DbSet<NeighborhoodScore> NeighborhoodScores => Set<NeighborhoodScore>();
        public DbSet<LevelRule> LevelRules => Set<LevelRule>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserProfile>(e =>
            {
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.DisplayName).HasMaxLength(60).IsRequired();
                e.Property(u => u.Email).HasMaxLength(120).IsRequired();
                e.HasOne(u => u.Neighborhood)
                    .WithMany(n => n.Users)
                    .HasForeignKey(u => u.NeighborhoodId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Material>(e =>
            {
                e.Property(m => m.PointFactor).HasPrecision(10, 3);
                e.HasIndex(m => m.Name).IsUnique();
                e.Property(m => m.Name).HasMaxLength(60).IsRequired();
            });

            modelBuilder.Entity<Dropoff>(e =>
            {
                e.Property(d => d.Quantity).HasPrecision(12, 3);
                e.Property(d => d.Location).HasMaxLength(150);
                e.HasOne(d => d.UserProfile)
                    .WithMany(u => u.Dropoffs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(d => d.Material)
                    .WithMany(m => m.Dropoffs)
                    .HasForeignKey(d => d.MaterialId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(d => d.Neighborhood)
                    .WithMany(n => n.Dropoffs)
                    .HasForeignKey(d => d.NeighborhoodId)
                    .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(d => d.VerifiedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.VerifiedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(d => d.CreatedAt);
                e.HasIndex(d => new { d.UserId, d.CreatedAt });
                e.HasIndex(d => new { d.NeighborhoodId, d.CreatedAt });
            });

            modelBuilder.Entity<Badge>(e =>
            {
                e.HasIndex(b => b.Name).IsUnique();
                e.Property(b => b.Name).HasMaxLength(80).IsRequired();
                e.Property(b => b.IconUrl).HasMaxLength(200);
            });

            modelBuilder.Entity<UserBadge>(e =>
            {
                e.HasIndex(ub => new { ub.UserId, ub.BadgeId }).IsUnique();
                e.HasOne(ub => ub.UserProfile)
                    .WithMany(u => u.UserBadges)
                    .HasForeignKey(ub => ub.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(ub => ub.Badge)
                    .WithMany(b => b.UserBadges)
                    .HasForeignKey(ub => ub.BadgeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Reward>(e =>
            {
                e.Property(r => r.Title).HasMaxLength(120).IsRequired();
                e.Property(r => r.Description).HasMaxLength(300);
                e.Property(r => r.TermsUrl).HasMaxLength(200);
                e.HasIndex(r => r.IsActive);
            });

            modelBuilder.Entity<Redemption>(e =>
            {
                e.HasOne(r => r.UserProfile)
                    .WithMany(u => u.Redemptions)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(r => r.Reward)
                    .WithMany(w => w.Redemptions)
                    .HasForeignKey(r => r.RewardId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(r => r.ApprovedByUser)
                    .WithMany()
                    .HasForeignKey(r => r.ApprovedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(r => r.Status);
                e.HasIndex(r => r.CreatedAt);
            });

            modelBuilder.Entity<UserPointLedger>(e =>
            {
                e.HasOne(l => l.UserProfile)
                    .WithMany(u => u.LedgerEntries)
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.Property(l => l.RefEntityType).HasMaxLength(50);
                e.HasIndex(l => new { l.UserId, l.CreatedAt });
            });

            modelBuilder.Entity<Neighborhood>(e =>
            {
                e.HasIndex(n => n.Code).IsUnique();
                e.Property(n => n.Name).HasMaxLength(120).IsRequired();
                e.Property(n => n.Code).HasMaxLength(60).IsRequired();
            });

            modelBuilder.Entity<NeighborhoodScore>(e =>
            {
                e.Property(s => s.PeriodKey).HasMaxLength(20).IsRequired();
                e.HasIndex(s => new { s.NeighborhoodId, s.Period, s.PeriodKey }).IsUnique();
                e.HasOne(s => s.Neighborhood)
                    .WithMany(n => n.Scores)
                    .HasForeignKey(s => s.NeighborhoodId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<LevelRule>(e =>
            {
                e.HasIndex(l => l.LevelName).IsUnique();
            });

        }
    }
}
