using AchieveClub.Server.RepositoryItems;
using AchieveClubServer.Data.DTO;
using Microsoft.EntityFrameworkCore;

namespace AchieveClub.Server;

public class ApplicationContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<UserDbo> Users { get; set; } = null!;
    public DbSet<ClubDbo> Clubs { get; set; } = null!;
    public DbSet<SupervisorDbo> Supervisors { get; set; } = null!;
    public DbSet<AchievementDbo> Achievements { get; set; } = null!;
    public DbSet<CompletedAchievementDbo> CompletedAchievements { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<UserDbo>()
            .HasOne(u => u.Club)
            .WithMany(c => c.Users)
            .HasForeignKey(u => u.ClubRefId)
            .HasPrincipalKey(c => c.Id);
    }
}