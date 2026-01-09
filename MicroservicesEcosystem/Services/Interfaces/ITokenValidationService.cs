using MicroservicesEcosystem.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesEcosystem.Services.Interfaces
{
    public interface ITokenValidationService
    {
        Task<IActionResult> GetOTPPhone(OtpRequestSmsMessage otpRequestMessage);
        Task<IActionResult> ValidarOTP(OtpGenerator otpGenerator);
        Task<IActionResult> GetOTPEmail(OtpRequestMessage otpRequestMessage);
    }
}
