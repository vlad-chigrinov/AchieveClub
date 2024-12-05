using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AchieveClubServer.Data.DTO;

[Table("Achievements")]
public class AchievementDbo
{
    public int Id { get; set; }
    public int Xp { get; set; }
    [MaxLength(100)] public required string Title_en { get; set; }
    [MaxLength(100)] public required string Title_ru { get; set; }
    [MaxLength(100)] public required string Title_pl { get; set; }
    [MaxLength(1000)] public required string Description_en { get; set; }
    [MaxLength(1000)] public required string Description_ru { get; set; }
    [MaxLength(1000)] public required string Description_pl { get; set; }
    [MaxLength(4000)] public required string LogoURL { get; set; }
    public bool IsMultiple { get; set; }

    public AchievementState ToState(int ratio, string lang)
    {
        switch (lang)
        {
            case "en":
                return new AchievementState(Id, Xp, Title_en, Description_en, LogoURL, ratio, IsMultiple);
            case "ru":
                return new AchievementState(Id, Xp, Title_ru, Description_ru, LogoURL, ratio, IsMultiple);
            case "pl":
                return new AchievementState(Id, Xp, Title_pl, Description_pl, LogoURL, ratio, IsMultiple);
            default:
                throw new NotImplementedException("Lang not supported!");
        }
    }
}

public record AchievementState(
    int Id,
    int Xp,
    string Title,
    string Description,
    string LogoUrl,
    int CompletionRatio,
    bool IsMultiple
);