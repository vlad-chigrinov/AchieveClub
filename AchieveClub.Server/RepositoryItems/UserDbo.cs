using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AchieveClub.Server.RepositoryItems;

[Table("Users")]
public class UserDbo
{
    public int Id { get; set; }
    [MaxLength(100)] public required string FirstName { get; set; }
    [MaxLength(100)] public required string LastName { get; set; }
    [MaxLength(500)] public required string Email { get; set; }
    [MaxLength(256)] public required string Password { get; set; }
    [MaxLength(4000)] public required string Avatar { get; set; }
    [MaxLength(256)] public string? RefreshToken { get; set; }
    public int ClubRefId { get; set; }
    [ForeignKey(nameof(ClubRefId))] public ClubDbo? Club { get; set; }
    public int RoleRefId { get; set; }
    [ForeignKey(nameof(RoleRefId))] public required RoleDbo Role { get; set; }

    public UserState ToUserState(int xpSum, string lang)
    {
        return new UserState(
            Id: this.Id,
            FirstName: this.FirstName,
            LastName: this.LastName,
            Avatar: this.Avatar,
            ClubId: this.ClubRefId,
            ClubName: this.Club?.ToTitleState(lang).Title ?? "null",
            ClubLogo: this.Club?.LogoURL ?? "null",
            XpSum: xpSum
        );
    }
}

public record UserState(
    int Id,
    string FirstName,
    string LastName,
    string Avatar,
    int ClubId,
    string ClubName,
    string ClubLogo,
    int XpSum);