using AchieveClub.Server.Auth;
using AchieveClub.Server.RepositoryItems;
using AchieveClub.Server.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AchieveClub.Server.Contract.Request;

namespace AchieveClub.Server.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthController(
        ILogger<AuthController> logger,
        JwtTokenCreator jwtCreator,
        ApplicationContext db,
        HashService hasher,
        EmailProofService emailProof
        ) : ControllerBase
    {
        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginRequest model)
        {
            var user = db.Users.Include(u => u.Role).FirstOrDefault(u => u.Email == model.Email);

            if (user == null) return BadRequest();

            if (hasher.ValidPassword(model.Password, user.Password))
            {
                user.RefreshToken = GenerateRefreshToken();

                db.Users.Update(user);
                if (db.SaveChanges() != 1)
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
            if (db.Clubs.Any(c => c.Id == model.ClubId) == false)
                return Conflict("clubId");

            //Uniq Email
            if (db.Users.Any(u => u.Email == model.EmailAddress))
                return Conflict("email");

            //Proof Code
            if (emailProof.ValidateProofCode(model.EmailAddress, model.ProofCode) == false)
                return Unauthorized();

            //Uniq Name
            if (db.Users.Any(u => u.FirstName == model.FirstName && u.LastName == model.LastName))
                return Conflict("name");

            //Hash Password
            var passwordHash = hasher.HashPassword(model.Password).ToString();

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
                Role = db.Roles.First(r => r.Id == 1)
            };

            //add to db
            db.Users.Add(newUser);
            if (db.SaveChanges() != 1)
                return BadRequest();

            db.Users.Include(u => u.Role);

            var token = GenerateJwtByUser(newUser);
            SetTokerCookiesPair(token, newUser.RefreshToken, Response);
            SetUserIdCookie(newUser.Id, Response);

            emailProof.DeleteProofCode(model.EmailAddress);
            return Ok();
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

            var user = db.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return Unauthorized();

            if (user.RefreshToken == null || user.RefreshToken != refreshToker)
                return Unauthorized();

            user.RefreshToken = GenerateRefreshToken();

            db.Users.Update(user);
            if (db.SaveChanges() != 1)
                return Unauthorized();

            var token = GenerateJwtByUser(user);
            SetTokerCookiesPair(token, user.RefreshToken, Response);
            SetUserIdCookie(user.Id, Response);

            return Ok();
        }

        private string GenerateJwtByUser(UserDbo user)
        {
            return jwtCreator.Generate(user.Id, user.Role.Title);
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        private void SetTokerCookiesPair(string jwtToken, string refreshToken, HttpResponse response)
        {
            var secureCookieOption = new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict };
            response.Cookies.Append("X-Access-Token", jwtToken, secureCookieOption);
            response.Cookies.Append("X-Refresh-Token", refreshToken, secureCookieOption);
        }

        private void SetUserIdCookie(int userId, HttpResponse response)
        {
            var secureCookieOption = new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict };
            response.Cookies.Append("X-User-Id", userId.ToString(), secureCookieOption);
        }
    }
}