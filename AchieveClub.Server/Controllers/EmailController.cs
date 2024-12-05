using AchieveClub.Server.Auth;
using AchieveClub.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SendGrid.Helpers.Mail;
using SendGrid;
using AchieveClub.Server.Contract.Request;

namespace AchieveClub.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController(
        ILogger<EmailController> logger,
        IStringLocalizer<EmailController> localizer,
        EmailProofService emailProof,
        EmailSettings emailSettings,
        ApplicationContext db
        ) : ControllerBase
    {

        [HttpPost("change_password")]
        public async Task<ActionResult> SendChangePasswordCode([FromBody] string emailAddress, CancellationToken ct)
        {
            if (emailProof.Contains(emailAddress))
                return Conflict("timeout");

            if (db.Users.Any(u => u.Email == emailAddress) == false)
                return Conflict("email");

            int proofCode = emailProof.GenerateProofCode(emailAddress);

            var apiKey = emailSettings.ApiKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(emailSettings.Email, emailSettings.Name);
            var subject = localizer["Confirm password change"];
            var to = new EmailAddress(emailAddress);
            var htmlContent = $"<h3>{localizer["Your code"]}: <code>{proofCode}</code></h3>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
            var response = await client.SendEmailAsync(msg, ct);

            if (response.IsSuccessStatusCode == false)
            {
                logger.LogInformation("Could not send proof code email");
                return BadRequest(response.StatusCode);
            }

            logger.LogInformation("Code sent successfully");
            return Ok();
        }

        [HttpPost("proof_email")]
        public async Task<ActionResult> SendEmailProofCode([FromBody] string emailAddress)
        {
            if (emailProof.Contains(emailAddress))
                return Conflict("timeout");

            if (db.Users.Any(u => u.Email == emailAddress))
                return Conflict("email");

            int proofCode = emailProof.GenerateProofCode(emailAddress);

            var apiKey = emailSettings.ApiKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(emailSettings.Email, emailSettings.Name);
            var subject = localizer["Registration confirmation"];
            var to = new EmailAddress(emailAddress);
            var htmlContent = $"<h3>{localizer["Your code"]}: <code>{proofCode}</code></h3>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
                return Ok();
            else
                return BadRequest(response.StatusCode);
        }

        [HttpPost("validate_code")]
        public ActionResult ValidateProofCode([FromBody] ProofCodeRequest model)
        {
            if (emailProof.ValidateProofCode(model.EmailAddress, model.ProofCode))
                return Ok();
            else
                return Unauthorized();
        }
    }
}