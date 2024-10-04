using AchieveClub.Server.Auth;
using AchieveClub.Server.RepositoryItems;
using AchieveClub.Server.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using AchieveClub.Server.Contract.Request;

namespace AchieveClub.Server.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthController(
        IStringLocalizer<AuthController> localizer,
        ILogger<AuthController> logger,
        JwtTokenCreator jwtCreator,
        ApplicationContext db,
        HashService hasher,
        EmailProofService emailProof,
        EmailSettings emailSettings
        ) : ControllerBase
    {
        private readonly IStringLocalizer<AuthController> _localizer = localizer;
        private readonly ILogger<AuthController> _logger = logger;
        private readonly JwtTokenCreator _jwtCreator = jwtCreator;
        private readonly ApplicationContext _db = db;
        private readonly HashService _hasher = hasher;
        private readonly EmailProofService _emailProof = emailProof;
        private readonly EmailSettings _emailSettings = emailSettings;

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginRequest model)
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
                SetTokerCookiesPair(token, user.RefreshToken, Response);
                SetUserIdCookie(user.Id, Response);

                return Ok();
            }
            else return BadRequest();
        }

        [HttpPost("registration")]
        public ActionResult Registration([FromBody] RegistrationRequest model)
        {
            //Validate Club
            if (_db.Clubs.Any(c => c.Id == model.ClubId) == false)
                return Conflict("clubId");

            //Uniq Email
            if (_db.Users.Any(u => u.Email == model.EmailAddress))
                return Conflict("email");

            //Proof Code
            if (_emailProof.ValidateProofCode(model.EmailAddress, model.ProofCode) == false)
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
                Email = model.EmailAddress,
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
            SetTokerCookiesPair(token, newUser.RefreshToken, Response);
            SetUserIdCookie(newUser.Id, Response);

            _emailProof.DeleteProofCode(model.EmailAddress);
            return Ok();
        }

        [HttpPatch("change_password")]
        public ActionResult ChangePassword(ChangePasswordRequest model)
        {
            if (_emailProof.ValidateProofCode(model.EmailAddress, model.ProofCode) == false)
                return Unauthorized();

            var passwordHash = _hasher.HashPassword(model.Password).ToString();

            var user = _db.Users.FirstOrDefault(u => u.Email == model.EmailAddress);

            if (user == null)
                return BadRequest();

            user.Password = passwordHash;
            _db.Update(user);
            if (_db.SaveChanges() != 1)
            {
                return BadRequest();
            }
            else
            {
                _emailProof.DeleteProofCode(model.EmailAddress);
                return Ok();
            }
        }

        [HttpGet("refresh")]
        public ActionResult Refresh()
        {
            var refreshToker = Request.Cookies["X-Refresh-Token"];
            var userIdString = Request.Cookies["X-User-Id"];

            if (refreshToker == null || userIdString == null)
                return Unauthorized();

            if (int.TryParse(userIdString, out var userId) == false)
                return Unauthorized();

            var user = _db.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return Unauthorized();

            if (user.RefreshToken == null || user.RefreshToken != refreshToker)
                return Unauthorized();

            user.RefreshToken = GenerateRefreshToken();

            _db.Users.Update(user);
            if (_db.SaveChanges() != 1)
                return Unauthorized();

            var token = GenerateJwtByUser(user);
            SetTokerCookiesPair(token, user.RefreshToken, Response);
            SetUserIdCookie(user.Id, Response);

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

        private void SetTokerCookiesPair(string jwtToken, string refreshToken, HttpResponse response)
        {
            var secureCookieOption = new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict };
            Response.Cookies.Append("X-Access-Token", jwtToken, secureCookieOption);
            Response.Cookies.Append("X-Refresh-Token", refreshToken, secureCookieOption);
        }

        private void SetUserIdCookie(int userId, HttpResponse response)
        {
            var secureCookieOption = new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict };
            response.Cookies.Append("X-User-Id", userId.ToString(), secureCookieOption);
        }
    }
}