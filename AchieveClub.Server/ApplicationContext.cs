using AchieveClub.Server.RepositoryItems;
using AchieveClubServer.Data.DTO;
using Microsoft.EntityFrameworkCore;

namespace AchieveClub.Server;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<UserDbo> Users { get; set; } = null!;
    public DbSet<ClubDbo> Clubs { get; set; } = null!;
    public DbSet<SupervisorDbo> Supervisors { get; set; } = null!;
    public DbSet<AchievementDbo> Achievements { get; set; } = null!;
    public DbSet<CompletedAchievementDbo> CompletedAchievements { get; set; } = null!;
    public DbSet<RoleDbo> Roles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<UserDbo>()
            .HasOne(u => u.Club)
            .WithMany(c => c.Users)
            .HasForeignKey(u => u.ClubRefId)
            .HasPrincipalKey(c => c.Id);
        modelBuilder
            .Entity<UserDbo>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleRefId)
            .HasPrincipalKey(r => r.Id);

        modelBuilder
            .Entity<CompletedAchievementDbo>()
            .HasOne(ca => ca.User);
        modelBuilder
            .Entity<CompletedAchievementDbo>()
            .HasOne(ca => ca.Supervisor);
    }
}