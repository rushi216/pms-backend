using Microsoft.EntityFrameworkCore;
using pms_backend.Models;

namespace pms_backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserManagerMapping> UserManagerMappings { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User-UserManagerMapping relationships
        modelBuilder.Entity<User>()
            .HasMany(u => u.DirectReports)
            .WithOne(m => m.Manager)
            .HasForeignKey(m => m.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.ManagerMappings)
            .WithOne(m => m.User)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure User-Review relationships
        modelBuilder.Entity<User>()
            .HasMany(u => u.ReviewsReceived)
            .WithOne(r => r.ForUser)
            .HasForeignKey(r => r.ForUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.ReviewsGiven)
            .WithOne(r => r.FromUser)
            .HasForeignKey(r => r.FromUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Add unique constraint on User.MicrosoftId
        modelBuilder.Entity<User>()
            .HasIndex(u => u.MicrosoftId)
            .IsUnique();

        // Add unique constraint on User.Email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}
