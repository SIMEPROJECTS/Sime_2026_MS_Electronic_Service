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
using static MicroservicesEcosystem.Models.DTO.OtpGenerator;

namespace MicroservicesEcosystem.Services
{
    public class TokenValidationService : ITokenValidationService
    {
        private readonly ITokenValidationRepository tokenValidationRepository;
        private readonly IConfiguration configuration;
        private readonly IMSMessagesClient mSMessagesClient;
        private readonly IJwtAuthenticationManager jwtAuthenticationManager;
        private readonly IMSBusinessClient mSBusinessClient;
        private readonly IMSMedicalRecordClient mMSMedicalRecordClient;
        public TokenValidationService(ITokenValidationRepository tokenValidationRepository, IConfiguration configuration, IMSMessagesClient mSMessagesClient,
            IJwtAuthenticationManager jwtAuthenticationManager, IMSBusinessClient mSBusinessClient, IMSMedicalRecordClient mMSMedicalRecordClient)
        {
            this.tokenValidationRepository = tokenValidationRepository;
            this.configuration = configuration;
            this.mSMessagesClient = mSMessagesClient;
            this.jwtAuthenticationManager = jwtAuthenticationManager;
            this.mSBusinessClient = mSBusinessClient;
            this.mMSMedicalRecordClient = mMSMedicalRecordClient;
        }
        public async Task<IActionResult> PostOTPPortal(OtpRequestEmailSmsMessage otpRequestMessage)
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
            TokenResponse response = jwtAuthenticationManager.AuthenticateOTP(tokenValidation.Id, otp);
            tokenValidation.ExpiresAt = response.ExpiresIn;
            tokenValidation.Token = response.AccessToken;
            tokenValidation = await tokenValidationRepository.Update(tokenValidation);
            tokenValidation.Status = TypeStatus.ENVIADO.ToString();
            tokenValidation.UpdatedAt = DateTime.Now;
            tokenValidation = await tokenValidationRepository.Update(tokenValidation);
            tokenValidation = new TokenValidation();
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
            tokenValidation.ExpiresAt = response.ExpiresIn;
            tokenValidation.Token = response.AccessToken;
            tokenValidation = await tokenValidationRepository.Update(tokenValidation);
            SendEmailMessageOTPPortalRequest sendSmsMessageRequestEmail = new SendEmailMessageOTPPortalRequest();
            sendSmsMessageRequestEmail.Dni = otpRequestMessage.Dni;
            sendSmsMessageRequestEmail.Name = otpRequestMessage.Name;
            sendSmsMessageRequestEmail.Email = otpRequestMessage.Email;
            sendSmsMessageRequestEmail.Phone = otpRequestMessage.Phone;
            sendSmsMessageRequestEmail.Otp = otp;
            await mSMessagesClient.postEnvioOTPEmailPortalMessages(sendSmsMessageRequestEmail);
            tokenValidation.Status = TypeStatus.ENVIADO.ToString();
            tokenValidation.UpdatedAt = DateTime.Now;
            tokenValidation = await tokenValidationRepository.Update(tokenValidation);
            return await Task.FromResult(new OkObjectResult(new { tokenOTP = response.AccessToken, expiresIn = response.ExpiresIn }));
        }
        public async Task<IActionResult> PostOTPInsurance(OtpRequestSmsEmailMessage otpRequestMessage)
        {
            OtpGenerator generator = new OtpGenerator();
            long count = generator.RandomNumber();
            string otp = generator.GenerateOTP(configuration["OTPKey"], count, 4);
            BusinessRequestInfo gcAseguradora = await mSBusinessClient.getBusinessInformation(otpRequestMessage.Broker);
            if (otpRequestMessage.Broker != Guid.Parse("8AB0CB42-357F-4144-8E7A-39FD7423EAC7"))
            {
                if (gcAseguradora == null) throw new ArgumentException(Errors.ErrorNoAseguradora.ToString());
            }
            //if (gcAseguradora.Id != Guid.Parse("ad056d97-02f1-4e6a-2e3a-08de470075f6") && gcAseguradora.Id != Guid.Parse("785c723c-6f37-41ce-3eb9-08de38d1a349")) throw new ArgumentException("Aseguradora no es AIG");
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
            TokenResponse response = jwtAuthenticationManager.AuthenticateOTP(tokenValidation.Id, otp);
            tokenValidation.ExpiresAt = response.ExpiresIn;
            tokenValidation.Token = response.AccessToken;
            tokenValidation = await tokenValidationRepository.Update(tokenValidation);
            SendSmsMessageRequest sendSmsMessageRequest = new SendSmsMessageRequest();
            sendSmsMessageRequest.TemplateId = Guid.Parse("8E40D2C4-6DFE-4A8B-B04B-E64073D44D60");
            sendSmsMessageRequest.Name =  otpRequestMessage.Name;
            sendSmsMessageRequest.Otp = otp + " - Orden: " + otpRequestMessage.OrderAttentionId;
            sendSmsMessageRequest.Dni = otpRequestMessage.Dni;
            sendSmsMessageRequest.Broker = otpRequestMessage.Broker == Guid.Parse("8AB0CB42-357F-4144-8E7A-39FD7423EAC7") ? "Ninguna" : gcAseguradora.Name;
            sendSmsMessageRequest.Phone = otpRequestMessage.Phone;
            sendSmsMessageRequest.Email = "";
            await mSMessagesClient.postEnvioOTPSmsMessage(sendSmsMessageRequest);
            sendSmsMessageRequest.TemplateId = Guid.Parse("0bb060b7-7c2a-486e-9805-59cdf8756137");
            sendSmsMessageRequest.Email = otpRequestMessage.Email;
            sendSmsMessageRequest.Phone = "";
            await mSMessagesClient.postEnvioOTPEmailMessage(sendSmsMessageRequest);
            tokenValidation.Status = TypeStatus.ENVIADO.ToString();
            tokenValidation.UpdatedAt = DateTime.Now;
            tokenValidation = await tokenValidationRepository.Update(tokenValidation);
            return await Task.FromResult(new OkObjectResult(new { valueOtp = otp, tokenOTP = response.AccessToken, expiresIn = response.ExpiresIn }));
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
            TokenResponse response = jwtAuthenticationManager.AuthenticateOTP(tokenValidation.Id, otp);
            tokenValidation.ExpiresAt = response.ExpiresIn;
            tokenValidation.Token = response.AccessToken;
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
            TokenResponse response = jwtAuthenticationManager.AuthenticateOTP(tokenValidation.Id, otp);
            tokenValidation.ExpiresAt = response.ExpiresIn;
            tokenValidation.Token = response.AccessToken;
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

        public async Task<IActionResult> ValidarOTPOrder(OtpGeneratorOrder otpGenerator)
        {
            TokenValidation tokenValidation = await tokenValidationRepository.GetTokenValidationByOrderAttentionId(otpGenerator.OrderCode);
            if (tokenValidation == null) throw new ArgumentException("No se encontro ningun OTP con el código de orden");
            if (tokenValidation.Token == null)
            {
                tokenValidation.Status = TypeStatus.EXPIRADO.ToString();
                tokenValidation.UpdatedAt = DateTime.Now;
                tokenValidation = await tokenValidationRepository.Update(tokenValidation);
                throw new ArgumentException("No se encontro ningun token");
            }
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(tokenValidation.Token) as JwtSecurityToken;
            if (jsonToken != null)
            {
                if (jsonToken.ValidTo < DateTime.UtcNow)
                {                   
                    tokenValidation.Status = TypeStatus.EXPIRADO.ToString();
                    tokenValidation.UpdatedAt = DateTime.Now;
                    tokenValidation = await tokenValidationRepository.Update(tokenValidation);
                    throw new ArgumentException(Errors.TokenExpirado.ToString());
                }
                else
                {
                    if(tokenValidation.Status == TypeStatus.USADO.ToString())
                    {  
                        tokenValidation.Status = TypeStatus.USADO.ToString();
                        tokenValidation.UpdatedAt = DateTime.Now;
                        tokenValidation = await tokenValidationRepository.Update(tokenValidation);
                    }
                    else
                    {
                        if (otpGenerator.Otp == null) throw new ArgumentException("Es neceseario ingresar el OTP.");
                        OtpGenerator generator = new OtpGenerator();
                        Boolean successLogin = otpGenerator.VerifyPassword(otpGenerator.Otp, tokenValidation.TokenValue);
                        if (!successLogin) throw new ArgumentException(Errors.OtpInvalido.ToString());
                        tokenValidation.Status = TypeStatus.USADO.ToString();
                        tokenValidation.UpdatedAt = DateTime.Now;
                        tokenValidation = await tokenValidationRepository.Update(tokenValidation);
                        await mMSMedicalRecordClient.PostTimeLogs(new Models.DTOs.TimeLogsRequest { OrderAttentionCode = tokenValidation.MsMedicalRecordOrderAttentionCode ?? 0, Type = TypeStatus.CONSULTA.ToString() });
                    }
          
                        return await Task.FromResult(new OkObjectResult(new { Status = TypeStatus.SUCCESS.ToString() }));
                   
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
