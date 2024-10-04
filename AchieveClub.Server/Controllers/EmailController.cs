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
        IStringLocalizer<EmailController> localizer,
        ILogger<EmailController> logger,
        EmailProofService emailProof,
        EmailSettings emailSettings,
        ApplicationContext db
        ) : ControllerBase
    {
        private readonly ApplicationContext _db = db;
        private readonly IStringLocalizer<EmailController> _localizer = localizer;
        private readonly ILogger<EmailController> _logger = logger;
        private readonly EmailProofService _emailProof = emailProof;
        private readonly EmailSettings _emailSettings = emailSettings;

        [HttpPost("change_password")]
        public async Task<ActionResult> SendChangePasswordCode([FromBody] string emailAddress)
        {
            if (_emailProof.Contains(emailAddress))
                return Forbid("timeout");

            if (_db.Users.Any(u => u.Email == emailAddress) == false)
                return Forbid("email");

            int proofCode = _emailProof.GenerateProofCode(emailAddress);

            var apiKey = _emailSettings.ApiKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_emailSettings.Email, _emailSettings.Name);
            var subject = _localizer["Confirm password change"];
            var to = new EmailAddress(emailAddress);
            var htmlContent = $"<h3>{_localizer["Your code"]}: <code>{proofCode}</code></h3>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
                return Ok();
            else
                return BadRequest(response.StatusCode);
        }

        [HttpPost("proof_email")]
        public async Task<ActionResult> SendEmailProofCode([FromBody] string emailAddress)
        {
            if (_emailProof.Contains(emailAddress))
                return Forbid("timeout");

            if (_db.Users.Any(u => u.Email == emailAddress))
                return Conflict("email");

            int proofCode = _emailProof.GenerateProofCode(emailAddress);

            var apiKey = _emailSettings.ApiKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_emailSettings.Email, _emailSettings.Name);
            var subject = _localizer["Registration confirmation"];
            var to = new EmailAddress(emailAddress);
            var htmlContent = $"<h3>{_localizer["Your code"]}: <code>{proofCode}</code></h3>";
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
            if (_emailProof.ValidateProofCode(model.EmailAddress, model.ProofCode))
                return Ok();
            else
                return Unauthorized();
        }
    }
}