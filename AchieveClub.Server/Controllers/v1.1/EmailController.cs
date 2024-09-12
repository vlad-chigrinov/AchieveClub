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
    public class EmailController(
        IStringLocalizer<EmailController> localizer,
        ILogger<EmailController> logger,
        EmailProofService emailProof
        ) : ControllerBase
    {
        private readonly IStringLocalizer<EmailController> _localizer = localizer;
        private readonly ILogger<EmailController> _logger = logger;
        private readonly EmailProofService _emailProof = emailProof;

        public record ProofCodeModel([Required, EmailAddress] string EmailAddress, [Required, Range(1000, 9999)] int ProofCode);


        [HttpPost("SendProofCode")]
        public ActionResult<int> SendProofCode([FromBody] string emailAddress)
        {
            int proofCode = _emailProof.GenerateProofCode(emailAddress);

            return Ok(proofCode);
        }

        [HttpPost("ValidateProofCode")]
        public ActionResult ValidateProofCode([FromBody] ProofCodeModel model)
        {
            if (_emailProof.ValidateProofCode(model.EmailAddress, model.ProofCode))
                return Ok();
            else
                return Unauthorized();
        }
    }
}