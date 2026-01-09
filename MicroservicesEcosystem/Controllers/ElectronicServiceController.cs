using MicroservicesEcosystem.Models.DTO;
using MicroservicesEcosystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Diagnostics;


namespace MicroservicesEcosystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElectronicServiceController : ControllerBase
    {

        private readonly ITokenValidationService tokenValidationService;

        public ElectronicServiceController(ITokenValidationService tokenValidationService)
        {
            this.tokenValidationService  = tokenValidationService;
        }

        [HttpPost("otp/phone")]
        public async Task<IActionResult> GetOTPPhone([FromBody]  OtpRequestSmsMessage otpRequestMessage)
        {
            return await tokenValidationService.GetOTPPhone(otpRequestMessage);
        }

        [HttpPost("otp/email")]
        public async Task<IActionResult> PostOTPEmail([FromBody] OtpRequestMessage otpRequestMessage)
        {
            return await tokenValidationService.GetOTPEmail(otpRequestMessage);
        }

        [HttpPost("otp/validate")]
        public async Task<IActionResult> ValidarOTP([FromBody] OtpGenerator otpGenerator)
        {
            return await tokenValidationService.ValidarOTP(otpGenerator);
        }       

    }
}
