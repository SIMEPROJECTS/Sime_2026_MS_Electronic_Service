using MicroservicesEcosystem.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesEcosystem.Services.Interfaces
{
    public interface ITokenValidationService
    {
        Task<IActionResult> PostOTPInsurance(OtpRequestSmsEmailMessage otpRequestMessage);
        Task<IActionResult> GetOTPPhone(OtpRequestSmsMessage otpRequestMessage);
        Task<IActionResult> ValidarOTP(OtpGenerator otpGenerator);
        Task<IActionResult> GetOTPEmail(OtpRequestMessage otpRequestMessage);
        Task<IActionResult> ValidarOTPOrder(OtpGeneratorOrder otpGenerator);
    }
}
