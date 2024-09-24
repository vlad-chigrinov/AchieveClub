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
using static AchieveClub.Server.Controllers.v1_1.AuthController;

namespace AchieveClub.Server.Controllers.v1_1
{
    [Route("api/[controller]")]
    [ApiVersion("1.1")]
    [ApiController]
    public class EmailController(
        IStringLocalizer<EmailController> localizer,
        ILogger<EmailController> logger,
        EmailProofService emailProof,
        EmailSettings emailSettings
        ) : ControllerBase
    {
        private readonly IStringLocalizer<EmailController> _localizer = localizer;
        private readonly ILogger<EmailController> _logger = logger;
        private readonly EmailProofService _emailProof = emailProof;
        private readonly EmailSettings _emailSettings = emailSettings;

        [HttpPost("send")]
        public async Task<ActionResult> SendProofCode([FromBody] string emailAddress)
        {
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

        [HttpPost("validate")]
        public ActionResult ValidateProofCode([FromBody] ProofCodeModel model)
        {
            if (_emailProof.ValidateProofCode(model.EmailAddress, model.ProofCode))
                return Ok();
            else
                return Unauthorized();
        }
    }
}