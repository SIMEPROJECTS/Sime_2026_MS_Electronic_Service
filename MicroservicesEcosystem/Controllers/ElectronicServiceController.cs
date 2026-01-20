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
        private readonly ISignService signService;

        public ElectronicServiceController(ITokenValidationService tokenValidationService,
            ISignService signService)
        {
            this.tokenValidationService  = tokenValidationService;
            this.signService = signService;
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
        [HttpPost("otp/business")]
        public async Task<IActionResult> PostOTPInsurance([FromBody] OtpRequestSmsEmailMessage otpRequestMessage)
        {
            return await tokenValidationService.PostOTPInsurance(otpRequestMessage);
        }

        [HttpPost("otp/validate")]
        public async Task<IActionResult> ValidarOTP([FromBody] OtpGenerator otpGenerator)
        {
            return await tokenValidationService.ValidarOTP(otpGenerator);
        }

        [HttpPost("otp/validate/order-attention")]
        public async Task<IActionResult> ValidarOTPOrder([FromBody] OtpGeneratorOrder otpGenerator)
        {
            return await tokenValidationService.ValidarOTPOrder(otpGenerator);
        }

        [HttpPost("form/document")]
        public async Task<IActionResult> CreateFormDocument([FromBody] DocumentRequest documentRequest)
        {
            return await signService.CreateFormDocument(documentRequest);
        }

        [HttpPost("form/sign/document")]
        public async Task<IActionResult> SignFormDocument([FromBody] SignRequest signRequest)
        {
            return await signService.SignPatientForm(signRequest);
        }
    }
}
