using System.ComponentModel.DataAnnotations;

namespace AchieveClub.Server.Contract.Request
{
    public record RegistrationRequest(
            [Required, StringLength(100, MinimumLength = 2)] string FirstName,
            [Required, StringLength(100, MinimumLength = 5)] string LastName,
            [Required, Range(1, double.PositiveInfinity)] int ClubId,
            [Required, MinLength(6), MaxLength(100)] string Password,
            [Required] string AvatarURL,
            [Required, EmailAddress] string EmailAddress,
            [Required, Range(1000, 9999)] int ProofCode
    );
}
