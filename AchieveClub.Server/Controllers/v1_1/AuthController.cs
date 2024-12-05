using AchieveClub.Server.Auth;
using AchieveClub.Server.Contract.Request;
using AchieveClub.Server.Contract.Responce;
using AchieveClub.Server.RepositoryItems;
using AchieveClub.Server.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AchieveClub.Server.Controllers.v1_1
{
    [Route("api/[controller]")]
    [ApiVersion("1.1")]
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
        public ActionResult<TokenPairResponce> Login([FromBody] LoginRequest model)
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

                return Ok(new TokenPairResponce(user.Id, token, user.RefreshToken));
            }
            else return BadRequest();
        }

        [HttpPost("registration")]
        public ActionResult<TokenPairResponce> Registration([FromBody] RegistrationRequest model)
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

            emailProof.DeleteProofCode(model.EmailAddress);
            return Ok(new TokenPairResponce(newUser.Id, token, newUser.RefreshToken));
        }

        [HttpPost("refresh")]
        public ActionResult<TokenPairResponce> Refresh([FromBody] RefreshRequest refreshModel)
        {
            var user = db.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == refreshModel.UserId);

            if (user == null)
                return Unauthorized();

            if (user.RefreshToken == null || user.RefreshToken != refreshModel.RefreshToken)
                return Unauthorized();

            user.RefreshToken = GenerateRefreshToken();

            db.Users.Update(user);
            if (db.SaveChanges() != 1)
                return Unauthorized();

            var token = GenerateJwtByUser(user);

            return Ok(new TokenPairResponce(user.Id, token, user.RefreshToken));
        }

        [HttpPatch("change_password")]
        public ActionResult ChangePassword(ChangePasswordRequest model)
        {
            if (emailProof.ValidateProofCode(model.EmailAddress, model.ProofCode) == false)
                return Unauthorized();

            var passwordHash = hasher.HashPassword(model.Password).ToString();

            var user = db.Users.FirstOrDefault(u => u.Email == model.EmailAddress);

            if (user == null)
                return BadRequest();

            user.Password = passwordHash;
            db.Update(user);
            if (db.SaveChanges() != 1)
            {
                return BadRequest();
            }
            else
            {
                emailProof.DeleteProofCode(model.EmailAddress);
                return Ok();
            }
        }

        private string GenerateJwtByUser(UserDbo user)
        {
            return jwtCreator.Generate(user.Id, user.Role.Title);
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}