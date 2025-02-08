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
    public int Balance { get; set; }
    [MaxLength(256)] public string? RefreshToken { get; set; }
    public int RoleRefId { get; set; }
    [ForeignKey(nameof(RoleRefId))] public required RoleDbo Role { get; set; }

    public UserState ToUserState(int xpSum) => new UserState(Id, FirstName, LastName, Avatar, xpSum);
}

public record UserState(
    int Id,
    string FirstName,
    string LastName,
    string Avatar,
    int XpSum);