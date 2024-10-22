using System.ComponentModel.DataAnnotations;

namespace AchieveClub.Server.Contract.Request
{
    public record CreateAchievementRequest
    (
        [Required, StringLength(100, MinimumLength = 5)] string Title_en,
            [Required, StringLength(100, MinimumLength = 5)] string Title_ru,
            [Required, StringLength(100, MinimumLength = 5)] string Title_pl,
            [Required, StringLength(300, MinimumLength = 5)] string Description_en,
            [Required, StringLength(300, MinimumLength = 5)] string Description_ru,
            [Required, StringLength(300, MinimumLength = 5)] string Description_pl,
            [Required] string LogoURL,
            [Required, Range(0, double.MaxValue)] int Xp,
            [Required] bool IsMultiple
    );
}
