using MicroservicesEcosystem.Authentication;
using MicroservicesEcosystem.Authentication.Models;
using MicroservicesEcosystem.Clients.Internal.Interface;
using MicroservicesEcosystem.Clients.Models;
using MicroservicesEcosystem.Exceptions;
using MicroservicesEcosystem.Models;
using MicroservicesEcosystem.Models.DTO;
using MicroservicesEcosystem.Repositories.Interfaces;
using MicroservicesEcosystem.Services.Interfaces;
using MicroservicesEcosystem.Types;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;

namespace MicroservicesEcosystem.Services
{
    public class TokenValidationService : ITokenValidationService
    {
        private readonly ITokenValidationRepository tokenValidationRepository;
        private readonly IConfiguration configuration;
        private readonly IMSMessagesClient mSMessagesClient;
        private readonly IJwtAuthenticationManager jwtAuthenticationManager;
        public TokenValidationService(ITokenValidationRepository tokenValidationRepository, IConfiguration configuration, IMSMessagesClient mSMessagesClient, IJwtAuthenticationManager jwtAuthenticationManager)
        {
            this.tokenValidationRepository = tokenValidationRepository;
            this.configuration = configuration;
            this.mSMessagesClient = mSMessagesClient;
            this.jwtAuthenticationManager = jwtAuthenticationManager; 
        }
        public async Task<IActionResult> GetOTPPhone(OtpRequestSmsMessage otpRequestMessage)
        {
            OtpGenerator generator = new OtpGenerator();
            long count = generator.RandomNumber();
            string otp = generator.GenerateOTP(configuration["OTPKey"], count, 4);
            TokenValidation tokenValidation = new TokenValidation();
            tokenValidation.Type = otpRequestMessage.Type != null ? otpRequestMessage.Type : "N/A";
            tokenValidation.TokenValue = BCrypt.Net.BCrypt.HashPassword(otp);
            tokenValidation.Status = TypeStatus.NOENVIADO.ToString();
            tokenValidation.CreatedAt = DateTime.Now;
            tokenValidation.ExpiresAt = DateTime.Now;
            tokenValidation.MsMedicalRecordOrderAttentionCode = otpRequestMessage.OrderAttentionId != null ? otpRequestMessage.OrderAttentionId : null;
            tokenValidation.Name = otpRequestMessage.Name;
            tokenValidation.Dni = otpRequestMessage.Dni;
            tokenValidation.Phone = otpRequestMessage.Phone;
            tokenValidation.Email = "N/A";
            tokenValidation = await tokenValidationRepository.Add(tokenValidation);
            TokenResponse response = jwtAuthenticationManager.AuthenticateOTP(tokenValidation.Id);
            tokenValidation.ExpiresAt = response.ExpiresIn;
            tokenValidation = await tokenValidationRepository.Update(tokenValidation);
            SendSmsMessageRequest sendSmsMessageRequest = new SendSmsMessageRequest(otpRequestMessage, otp, Guid.Parse("d67e35f6-72f6-40bc-b3ea-b83de3bf87e3"));
            await mSMessagesClient.postEnvioOTPSmsMessage(sendSmsMessageRequest);
            tokenValidation.Status = TypeStatus.ENVIADO.ToString();
            tokenValidation.UpdatedAt = DateTime.Now;
            tokenValidation = await tokenValidationRepository.Update(tokenValidation);
            return await Task.FromResult(new OkObjectResult(new { tokenOTP = response.AccessToken, expiresIn = response.ExpiresIn }));
        }
        public async Task<IActionResult> GetOTPEmail(OtpRequestMessage otpRequestMessage)
        {
            OtpGenerator generator = new OtpGenerator();
            long count = generator.RandomNumber();
            string otp = generator.GenerateOTP(configuration["OTPKey"], count, 4);
            TokenValidation tokenValidation = new TokenValidation();
            tokenValidation.Type = otpRequestMessage.Type != null ? otpRequestMessage.Type : "N/A";            
            tokenValidation.TokenValue = BCrypt.Net.BCrypt.HashPassword(otp);
            tokenValidation.Status = TypeStatus.NOENVIADO.ToString();
            tokenValidation.CreatedAt = DateTime.Now;
            tokenValidation.ExpiresAt = DateTime.Now;
            tokenValidation.MsMedicalRecordOrderAttentionCode = otpRequestMessage.OrderAttentionId != null ? otpRequestMessage.OrderAttentionId : null;
            tokenValidation.Name = otpRequestMessage.Name;
            tokenValidation.Dni = otpRequestMessage.Dni;
            tokenValidation.Email = otpRequestMessage.Email;
            tokenValidation.Phone = "N/A"; 
            tokenValidation = await tokenValidationRepository.Add(tokenValidation);
            TokenResponse response = jwtAuthenticationManager.AuthenticateOTP(tokenValidation.Id);
            tokenValidation.ExpiresAt = response.ExpiresIn;
            tokenValidation = await tokenValidationRepository.Update(tokenValidation);
            SendEmailMessageOTPRequest sendSmsMessageRequest = new SendEmailMessageOTPRequest(otpRequestMessage, otp);
            await mSMessagesClient.postEnvioOTPEmailPortalMessage(sendSmsMessageRequest);
            tokenValidation.Status = TypeStatus.ENVIADO.ToString();
            tokenValidation.UpdatedAt = DateTime.Now;
            tokenValidation = await tokenValidationRepository.Update(tokenValidation);
            return await Task.FromResult(new OkObjectResult(new { tokenOTP = response.AccessToken, expiresIn = response.ExpiresIn }));
        }

        public async Task<IActionResult> ValidarOTP(OtpGenerator otpGenerator)
        {
            OtpGenerator generator = new OtpGenerator();
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(otpGenerator.token) as JwtSecurityToken;
            if (jsonToken != null)
            {
                if (jsonToken.ValidTo < DateTime.UtcNow)
                {
                    var otpClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "id");
                    TokenValidation tokenValidation = await tokenValidationRepository.Get(Guid.Parse(otpClaim.Value.ToString()));
                    if (tokenValidation == null) throw new ArgumentException(Errors.OtpInvalido.ToString());
                    tokenValidation.Status = TypeStatus.EXPIRADO.ToString();
                    tokenValidation.UpdatedAt = DateTime.Now;
                    tokenValidation = await tokenValidationRepository.Update(tokenValidation);
                    throw new ArgumentException(Errors.TokenExpirado.ToString());
                }
                else
                {
                    var otpClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "id");
                    if (otpClaim != null)
                    {                      
                        TokenValidation tokenValidation = await tokenValidationRepository.Get(Guid.Parse(otpClaim.Value.ToString()));
                        if (tokenValidation == null) throw new ArgumentException(Errors.OtpInvalido.ToString());
                        Boolean successLogin = otpGenerator.VerifyPassword(otpGenerator.Otp,tokenValidation.TokenValue);
                        if (!successLogin) throw new ArgumentException(Errors.OtpInvalido.ToString());
                        tokenValidation.Status = TypeStatus.USADO.ToString();
                        tokenValidation.UpdatedAt = DateTime.Now;
                        tokenValidation = await tokenValidationRepository.Update(tokenValidation);
                        return await Task.FromResult(new OkObjectResult(new { Status = TypeStatus.SUCCESS.ToString() }));
                    }
                    else
                    {
                        throw new ArgumentException(Errors.TokenInvalido.ToString());
                    }
                }

            }
            else
            {
                throw new ArgumentException(Errors.TokenInvalido.ToString());
            }
            throw new ArgumentException(Errors.TokenInvalido.ToString());

        }


    }
}
