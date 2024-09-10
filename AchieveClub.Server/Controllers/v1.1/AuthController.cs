using AchieveClub.Server.Auth;
using AchieveClub.Server.RepositoryItems;
using AchieveClub.Server.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace AchieveClub.Server.Controllers.v1_1
{
    [Route("api/[controller]")]
    [ApiVersion("1.1")]
    [ApiController]
    public class AuthController(
        IStringLocalizer<AuthController> localizer,
        ILogger<AuthController> logger,
        JwtTokenCreator jwtCreator,
        ApplicationContext db,
        HashService hasher,
        EmailProofService emailProof
        ) : ControllerBase
    {
        private readonly IStringLocalizer<AuthController> _localizer = localizer;
        private readonly ILogger<AuthController> _logger = logger;
        private readonly JwtTokenCreator _jwtCreator = jwtCreator;
        private readonly ApplicationContext _db = db;
        private readonly HashService _hasher = hasher;
        private readonly EmailProofService _emailProof = emailProof;

        public record TokenPair(int UserId, string AuthToken, string RefreshToken);
        public record LoginModel(
            [Required, EmailAddress] string Email,
            [Required, StringLength(100, MinimumLength = 6)] string Password
        );
        public record RefreshModel(
            [Required, Range(1, double.MaxValue)] int UserId,
            [Required, MinLength(30), MaxLength(100)] string RefreshToken
        );
        public record RegistrationModel(
            [Required, StringLength(100, MinimumLength = 2)] string FirstName,
            [Required, StringLength(100, MinimumLength = 5)] string LastName,
            [Required, Range(1, double.PositiveInfinity)] int ClubId,
            [Required, MinLength(6), MaxLength(100)] string Password,
            [Required] string AvatarURL,
            [Required] ProofCodeModel EmailAndProof
        );

        public record ProofCodeModel([Required, EmailAddress] string EmailAddress, [Required, Range(1000, 9999)] int ProofCode);

        public record ChangePasswordModel([Required] ProofCodeModel EmailAndProof, [Required, MinLength(6), MaxLength(100)] string Password);

        [HttpPost("login")]
        public ActionResult<TokenPair> Login([FromBody] LoginModel model)
        {
            var user = _db.Users.Include(u => u.Role).FirstOrDefault(u => u.Email == model.Email);

            if (user == null) return BadRequest();

            if (_hasher.ValidPassword(model.Password, user.Password))
            {
                user.RefreshToken = GenerateRefreshToken();

                _db.Users.Update(user);
                if (_db.SaveChanges() != 1)
                    return Unauthorized();

                var token = GenerateJwtByUser(user);

                return Ok(new TokenPair(user.Id, token, user.RefreshToken));
            }
            else return BadRequest();
        }

        [HttpPost("registration")]
        public ActionResult Registration([FromBody] RegistrationModel model)
        {
            //Validate Club
            if (_db.Clubs.Any(c => c.Id == model.ClubId) == false)
                return Conflict("clubId");

            //Uniq Email
            if (_db.Users.Any(u => u.Email == model.EmailAndProof.EmailAddress))
                return Conflict("email");

            //Proof Code
            if (_emailProof.ValidateProofCode(model.EmailAndProof.EmailAddress, model.EmailAndProof.ProofCode) == false)
                return Unauthorized();

            //Uniq Name
            if (_db.Users.Any(u => u.FirstName == model.FirstName && u.LastName == model.LastName))
                return Conflict("name");

            //Hash Password
            var passwordHash = _hasher.HashPassword(model.Password).ToString();

            //Create new user
            var newUser = new UserDbo
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Avatar = model.AvatarURL,
                Email = model.EmailAndProof.EmailAddress,
                ClubRefId = model.ClubId,
                Password = passwordHash,
                RefreshToken = GenerateRefreshToken(),
                RoleRefId = 1,
                Role = _db.Roles.First(r => r.Id == 1)
            };

            //add to db
            _db.Users.Add(newUser);
            if (_db.SaveChanges() != 1)
                return BadRequest();

            _db.Users.Include(u => u.Role);

            var token = GenerateJwtByUser(newUser);

            return Ok(new TokenPair(newUser.Id, token, newUser.RefreshToken));
        }

        [HttpPost("refresh")]
        public ActionResult<TokenPair> Refresh([FromBody] RefreshModel refreshModel)
        {
            var user = _db.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == refreshModel.UserId);

            if (user == null)
                return Unauthorized();

            if (user.RefreshToken == null || user.RefreshToken != refreshModel.RefreshToken)
                return Unauthorized();

            user.RefreshToken = GenerateRefreshToken();

            _db.Users.Update(user);
            if (_db.SaveChanges() != 1)
                return Unauthorized();

            var token = GenerateJwtByUser(user);

            return Ok(new TokenPair(user.Id, token, user.RefreshToken));
        }

        [HttpPatch("ChangePassword")]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (_emailProof.ValidateProofCode(model.EmailAndProof.EmailAddress, model.EmailAndProof.ProofCode) == false)
                return Unauthorized();

            var passwordHash = _hasher.HashPassword(model.Password).ToString();

            var user = _db.Users.FirstOrDefault(u => u.Email == model.EmailAndProof.EmailAddress);

            if (user == null)
                return BadRequest();

            user.Password = passwordHash;
            _db.Update(user);
            if (_db.SaveChanges() != 1)
                return BadRequest();
            else
                return Ok();
        }

        private string GenerateJwtByUser(UserDbo user)
        {
            return _jwtCreator.Generate(user.Id, user.Role.Title);
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}