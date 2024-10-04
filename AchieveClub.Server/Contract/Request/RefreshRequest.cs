using System.ComponentModel.DataAnnotations;

namespace AchieveClub.Server.Contract.Request
{
    public record RefreshRequest(
            [Required, Range(1, double.MaxValue)] int UserId,
            [Required, MinLength(30), MaxLength(100)] string RefreshToken
        );
}
