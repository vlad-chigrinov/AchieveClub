using AchieveClub.Server;
using AchieveClub.Server.Auth;
using AchieveClub.Server.RepositoryItems;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Promo.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        JwtTokenCreator jwtCreator,
        ApplicationContext db,
        HashService hasher
        ) : ControllerBase
    {
        private readonly JwtTokenCreator _jwtCreator = jwtCreator;
        private readonly ApplicationContext _db = db;
        private readonly HashService _hasher = hasher;

        public record LoginModel([Required, EmailAddress] string Email, [Required, StringLength(100, MinimumLength = 6)] string Password);
        public record RegistrationModel(
            [Required, StringLength(100, MinimumLength = 2)] string FirstName,
            [Required, StringLength(100, MinimumLength = 5)] string LastName,
            [Required, Range(1, double.PositiveInfinity)] int ClubId,
            [Required, EmailAddress] string Email,
            [Required, MinLength(6)] string Password,
            [Required] string AvatarURL
        );


        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginModel model)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == model.Email);

            if (user == null) return BadRequest();
            
            if (_hasher.ValidPassword(model.Password, user.Password))
            {
                var secureCookieOption = new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict };
                Response.Cookies.Append("X-User-Id", user.Id.ToString(), secureCookieOption);

                user.RefreshToken = GenerateRefreshToken();

                _db.Users.Update(user);
                if (_db.SaveChanges() != 1)
                    return Unauthorized();

                var token = GenerateJwtByUser(user);
                SetTokerCookiesPair(token, user.RefreshToken, Response);
                return Ok();
            }
            else return BadRequest();
        }

        [HttpPost("registration")]
        public ActionResult Registration([FromBody] RegistrationModel model)
        {
            //Validate Club
            if (_db.Clubs.Any(c => c.Id == model.ClubId) == false)
                return BadRequest($"Club with ClubId: {model.ClubId} not found!");

            //Uniq Email
            if (_db.Users.Any(u => u.Email == model.Email))
                return BadRequest("Email must be unique!");

            //Uniq Name
            if (_db.Users.Any(u => u.FirstName == model.FirstName && u.LastName == model.LastName))
                return BadRequest("Name must be unique!");

            //Hash Password
            var passwordHash = _hasher.HashPassword(model.Password).ToString();

            //Create new user
            var newUser = new UserDbo
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Avatar = model.AvatarURL,
                Email = model.Email,
                ClubRefId = model.ClubId,
                Password = passwordHash,
                RefreshToken = GenerateRefreshToken()
            };

            //add to db
            _db.Users.Add(newUser);
            if (_db.SaveChanges() != 1)
                return BadRequest();
            Response.Cookies.Append("X-User-Id", newUser.Id.ToString(), new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });

            var token = GenerateJwtByUser(newUser);
            SetTokerCookiesPair(token, newUser.RefreshToken, Response);
            return Ok();
        }

        [HttpGet("refresh")]
        public IActionResult Refresh()
        {
            var refreshToker = Request.Cookies["X-Refresh-Token"];
            var userIdString = Request.Cookies["X-User-Id"];

            if (refreshToker == null || userIdString == null)
                return Unauthorized();

            if (int.TryParse(userIdString, out var userId) == false)
                return Unauthorized();

            var user = _db.Users.FirstOrDefault(u => u.Id == userId);

            if(user == null)
                return Unauthorized();

            if(user.RefreshToken == null || user.RefreshToken != refreshToker)
                return Unauthorized();

            user.RefreshToken = GenerateRefreshToken();

            _db.Users.Update(user);
            if (_db.SaveChanges() != 1)
                return Unauthorized();

            var token = GenerateJwtByUser(user);
            SetTokerCookiesPair(token, user.RefreshToken, Response);
            return Ok();
        }

        private string GenerateJwtByUser(UserDbo user)
        {
            return _jwtCreator.Generate(user.Email, user.Id.ToString());
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
    }
}