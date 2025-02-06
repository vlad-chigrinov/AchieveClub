using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AchieveClub.Server.ApiContracts.Achievements.Response;

namespace AchieveClubServer.Data.DTO;

[Table("Achievements")]
public class AchievementDbo
{
    public int Id { get; set; }
    public int Xp { get; set; }
    [MaxLength(100)] public required string Title { get; set; }
    [MaxLength(1000)] public required string Description { get; set; }
    [MaxLength(4000)] public required string LogoURL { get; set; }
    public bool IsMultiple { get; set; }

    public AchievementResponse ToResponse() => new AchievementResponse(Id, Xp, Title, Description, LogoURL, IsMultiple);
}