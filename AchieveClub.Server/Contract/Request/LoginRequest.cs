using System.ComponentModel.DataAnnotations;

namespace AchieveClub.Server.Contract.Request
{
    public record LoginRequest([Required, EmailAddress] string Email, [Required, StringLength(100, MinimumLength = 6)] string Password);
}
